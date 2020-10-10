using AttackDefenseRunner.Hubs;
using AttackDefenseRunner.Model;
using AttackDefenseRunner.Util;
using AttackDefenseRunner.Util.Docker;
using AttackDefenseRunner.Util.Flag;
using AttackDefenseRunner.Util.Parsing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AttackDefenseRunner
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
            // Host
            services.AddRazorPages();
            services.AddControllers();
            
            // Db Contexts
            services.AddDbContext<ADRContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("ADRContext")));
            
            // Scoped
            services.AddScoped<SettingsHelper>();
            services.AddScoped<MonitorHub>();
            services.AddScoped<ServiceManager>();
            services.AddScoped<DockerTagManager>();
            services.AddScoped<IDockerImageManager, LocalDockerImageManager>();
            
            // Singletons
            services.AddSingleton<RunningSingleton>();
            services.AddSingleton<IFlagFinder, DockerFlagFinder>();
            services.AddSingleton<IFlagSubmitter, LogFlagSubmitter>();
            services.AddSingleton<IDockerContainerObserver, SimpleDockerContainerObserver>();
            services.AddSingleton<DockerImageJsonParser>();
            
            // SignalR
            services.AddSignalR();
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
                endpoints.MapControllers();
                endpoints.MapHub<MonitorHub>("/monitor");
            });
        }
    }
}