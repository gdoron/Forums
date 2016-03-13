using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Primitives;

namespace Forums
{
    public class CompressionMiddleware
    {
        private readonly RequestDelegate _next;

        public CompressionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            StringValues acceptEncoding = httpContext.Request.Headers["Accept-Encoding"];
            if (acceptEncoding.Count > 0)
            {
                if (acceptEncoding.ToString().IndexOf
                ("gzip", StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var stream = httpContext.Response.Body;
                        httpContext.Response.Body = memoryStream;
                        await _next(httpContext);
                        if (httpContext.Response.Headers.ContainsKey("X-Content-Encoding"))
                        {
                            httpContext.Response.Headers.Remove("X-Content-Encoding");
                            httpContext.Response.Headers.Add("Content-Encoding", new[] {"gzip"});
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            return;
                        }

                        using (var compressedStream = new GZipStream(stream, CompressionLevel.Optimal))
                        {
                            httpContext.Response.Headers.Add("Content-Encoding", new[] { "gzip" });
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            await memoryStream.CopyToAsync(compressedStream);
                        }
                    }
                }
            }
        }
    }

    public static class CompressionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCompression(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CompressionMiddleware>();
        }
    }
}