﻿using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Entities;
using Forums.Filters;
using Forums.log4net;
using Forums.Models;
using Forums.Services;
using Microsoft.AspNet.Authentication.OAuth;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.WebSockets.Server;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.WebEncoders;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Forums
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);
            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            env.ConfigureLog4Net("log4net.xml");

            //builder.AddEnvironmentVariables();


            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var connection = @"Server=(localdb)\mssqllocaldb;Database=EFGetStarted.AspNet5.NewDb;Trusted_Connection=True;";

            // Add framework services.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"])
                        //options.UseSqlServer(connection)
                        //options.UseSqlServer(Configuration["ConnectionString"])
                        .CommandTimeout(30));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
            });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<EntityFrameworkFilter>();
            services.AddTransient<DbSeeder>();

            services.Configure<AuthMessageSenderOptions>(Configuration);


            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Post, PostListViewModel>()
                    .ForMember(d => d.UserName, options => options.MapFrom(x => x.User.UserName));

                cfg.CreateMap<HierarchyPost, HierarchyPostModel>();
                cfg.CreateMap<CreateUserReviewModel, UserReview>().ReverseMap();
                cfg.CreateMap<UserReview, UserReviewListModel>()
                    .ForMember(d => d.FromUserName, options => options.MapFrom(s => s.FromUser.UserName));

                cfg.CreateMap<Message, IncomingMessageModel>()
                    .ForMember(d => d.SenderUserName, options => options.MapFrom(s => s.Sender.UserName));

                cfg.CreateMap<Message, OutgoingMessageModel>()
                    .ForMember(d => d.RecipientUserName, options => options.MapFrom(s => s.Recipient.UserName));

            });
            services.AddInstance(config);

            var redisConnectionString = Configuration["Data:RedisConnection"];
            var redis = ConnectionMultiplexer.Connect(redisConnectionString);

            services.AddInstance(redis);
            var messageHub = new RedisMessagesHub(redis);
            services.AddInstance(messageHub);
            services.AddTransient<PostsCacher>();


            services.AddCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, DbSeeder dbSeeder, RedisMessagesHub messagesHub)
        {
            loggerFactory.AddProvider(new Log4NetProvider());


            // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                serviceScope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseStaticFiles();

            app.UseIdentity();
            await dbSeeder.EnsureSeedData();
            app.UseFacebookAuthentication(options =>
            {
                options.AppId = Configuration["Authentication:Facebook:AppId"];
                options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                options.Events = new OAuthEvents
                                     {
                                         OnRemoteError = ctx =>

                                         {
                                             ctx.Response.Redirect("/error?FailureMessage=" + UrlEncoder.Default.UrlEncode(ctx.Error.Message));
                                             ctx.HandleResponse();
                                             return Task.FromResult(0);
                                         }
                                     };
            });

            app.ConfigureForumsSockets(messagesHub, "/sockets");

            //app.UseOAuthAuthentication(new OAuthOptions
            //                               {
            //                                   AuthenticationScheme = "Google-AccessToken",
            //                                   DisplayName = "Google-AccessToken(oauth{1})",
            //                                   ClientId = Configuration["google:clientid"],
            //                                   ClientSecret = Configuration["google:clientsecret"],
            //                                   CallbackPath = new PathString("/signin-google-token"),
            //                                   AuthorizationEndpoint = GoogleDefaults.AuthorizationEndpoint,
            //                                   TokenEndpoint = GoogleDefaults.TokenEndpoint,
            //                                   Scope = {"openid", "profile", "email"},
            //                                   SaveTokensAsClaims = true
            //                               });

            //// See config.json
            //// https://console.developers.google.com/project
            //app.UseGoogleAuthentication(new GoogleOptions
            //                                {
            //                                    ClientId = Configuration["google:clientid"],
            //                                    ClientSecret = Configuration["google:clientsecret"],
            //                                    DisplayName = "Second google",
            //                                    Events = new OAuthEvents()
            //                                                 {
            //                                                     OnRemoteError = ctx =>

            //                                                     {
            //                                                         ctx.Response.Redirect("/error?FailureMessage=" + UrlEncoder.Default.UrlEncode(ctx.Error.Message));
            //                                                         ctx.HandleResponse();
            //                                                         return Task.FromResult(0);
            //                                                     }
            //                                                 }
            //                                });

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

            //app.UseCompression();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
                //routes.MapRoute("DefaultApiPost", "Api/{controller}/{action}");
            });


        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        static readonly HashSet<WebSocket> Set = new HashSet<WebSocket>();
    }
}