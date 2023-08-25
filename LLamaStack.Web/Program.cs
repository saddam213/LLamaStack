using LLamaStack.Core.Config;
using LLamaStack.Core.Services;
using LLamaStack.Web.Hubs;
using LLamaStack.Core;

namespace LLamaStack.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();

            // Add LLamaStack
            builder.Services.AddLLamaStack<string>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapHub<SessionConnectionHub>(nameof(SessionConnectionHub));

            app.Run();
        }
    }
}