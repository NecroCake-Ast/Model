using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Model.Models.Statistics.Testing;
using Model.Models.Statistics.Work;
using Model.Models.Testing.Load;
using Model.Models.User;
using Model.Models.Work;
using Model.Models.Works;

namespace Model
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IUserFinder>(s => new CFileUserFinder(Directory.GetCurrentDirectory() + "\\wwwroot\\Users"));
            services.AddSingleton<IAuthenticator>(s => new CFileAuthenticator(Directory.GetCurrentDirectory() + "\\wwwroot\\Users"));
            services.AddSingleton<IUserRegistrator>(s => new CFileRegistrator(Directory.GetCurrentDirectory() + "\\wwwroot\\Users",
                Directory.GetCurrentDirectory() + "\\wwwroot\\Data\\__NextID.dat"));
            services.AddSingleton<IWorkFinder>(s => new CFileWorkFinder(Directory.GetCurrentDirectory() + "\\wwwroot\\Works"));
            services.AddSingleton<ITestMaker>(s => new CFileTestMaker(Directory.GetCurrentDirectory() + "\\wwwroot\\Tests"));
            services.AddSingleton<ISettingReader>(s => new CFileSettingReader(Directory.GetCurrentDirectory() + "\\wwwroot\\WorkSettings"));
            services.AddSingleton<ITestResultManager>(s => new CFileTestResultManager(Directory.GetCurrentDirectory() + "\\wwwroot\\TestResults"));
            services.AddSingleton<IWorkResultManager>(s => new CFileWorkResultManager(Directory.GetCurrentDirectory() + "\\wwwroot\\WorkResults"));

            services.AddMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            FileExtensionContentTypeProvider provider = new FileExtensionContentTypeProvider();
            provider.Mappings.Add(".unityweb", "application/octet-stream");


            app.UseStaticFiles (new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider (
                Path.Combine (Directory.GetCurrentDirectory(), "wwwroot")),
                ContentTypeProvider = provider
            });
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Login}/{id?}");
            });
        }
    }
}
