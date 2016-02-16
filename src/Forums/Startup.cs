using Entities;
using Forums.Filters;
using Forums.log4net;
using Forums.Models;
using Forums.Services;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = @"Server=(localdb)\mssqllocaldb;Database=EFGetStarted.AspNet5.NewDb;Trusted_Connection=True;";

            // Add framework services.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    //options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"])
                    //options.UseSqlServer(connection)
                    options.UseSqlServer(Configuration["ConnectionString"])
                        .CommandTimeout(30)
                        .MaxBatchSize(300));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<EntityFrameworkFilter>();

            services.Configure<AuthMessageSenderOptions>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new Log4NetProvider());


            // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                serviceScope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetService<ApplicationDbContext>().EnsureSeedData();
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

            app.UseFacebookAuthentication(options =>
            {
                options.AppId = Configuration["Authentication:Facebook:AppId"];
                options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });


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

            app.UseMvc(routes => { routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"); });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}