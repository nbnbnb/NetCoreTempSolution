using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using WebAppCore.Ext;
using WebAppCore.TagHelpers;

namespace WebAppCore
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
            services
                    .AddMvc(options =>
                    {
                        options.EnableEndpointRouting = false;
                        // 添加自定义 BinderProvider
                        options.ModelBinderProviders.Insert(0, new AuthorEntityBinderProvider());
                    })
                    .ConfigureApplicationPartManager(apm =>
                    {
                        apm.FeatureProviders.Add(new GenericControllerFeatureProvider());
                    })
                    .AddSessionStateTempDataProvider() // SessionStateTempDataProvider or CookieTempDataProvider
                    .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Latest)  // 使用最新版本 MVC
                    .AddXmlSerializerFormatters();  // 添加 ModelBind XML 格式支持

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            // 注册自定义的 TagHelperComponent
            services.AddTransient<ITagHelperComponent, AddressTagHelperComponent>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName.Equals(Environments.Development))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            // In the Configure method, insert middleware to expose the generated Swagger as JSON endpoint(s)
            // At this point, you can spin up your application and view the generated Swagger JSON at "/swagger/v1/swagger.json."
            app.UseSwagger();

            // Optionally, insert the swagger - ui middleware if you want to expose interactive documentation
            // specifying the Swagger JSON endpoint(s) to power it from.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            

            app.UseCookiePolicy();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
