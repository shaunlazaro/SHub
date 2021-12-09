using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Tutorial.Managers;

namespace Tutorial
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages().AddRazorPagesOptions(options => 
                {
                    options.Conventions.AddPageRoute("/MainMenu/Index", "");
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            InitWindow();
        }

        // Bootstrap based
        public async void InitWindow()
        {
            await UserInfoManager.Instance.Init();

            BrowserWindowOptions options = new BrowserWindowOptions
            {
                Show = false,
                Frame = false,
            };
            BrowserWindow window = await Electron.WindowManager.CreateWindowAsync(options);
            await window.WebContents.Session.ClearCacheAsync();

            // We set up the window controls here because it's a bit fucky.
            window.OnMaximize += TitlebarManager.ToggleMaxRestoreButtons;
            window.OnUnmaximize += TitlebarManager.ToggleMaxRestoreButtons;
            TitlebarManager.Restore += window.Restore;
            TitlebarManager.Maximize += window.Maximize;
            TitlebarManager.Minimize += window.Minimize;
            TitlebarManager.Close += window.Close;
            Electron.App.WindowAllClosed += () => Electron.App.Exit();
            window.OnReadyToShow += window.Show;
        }
    }
}
