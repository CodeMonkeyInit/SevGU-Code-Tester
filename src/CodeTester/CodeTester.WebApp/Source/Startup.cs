using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CodeTester.WebApp.Source
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) { }

        public void Configure(IApplicationBuilder builder, IHostingEnvironment environment)
        {
            if (environment.IsDevelopment())
                builder.UseDeveloperExceptionPage();

            builder.Run(Handle);
        }

        private static async Task Handle(HttpContext context) =>
            await GetHttpResponse(context).WriteAsync("code-tester");

        private static HttpResponse GetHttpResponse(HttpContext context) =>
            context.Response;
    }
}