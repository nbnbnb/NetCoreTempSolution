using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Filtering;
using App.Metrics.Formatters.InfluxDB;
using App.Metrics.Health;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            // 基础 Web Metrics  配置
            var basicMetrics = AppMetrics.CreateDefaultBuilder()
                .Report.ToInfluxDb(
                    options =>
                    {
                        options.InfluxDb.BaseUri = new Uri("http://tcloud.zhangjin.tk:8086");
                        options.InfluxDb.Database = "AppMetricsDB";
                        options.InfluxDb.UserName = "appmetrics";
                        options.InfluxDb.Password = "appmetrics";
                        options.InfluxDb.CreateDataBaseIfNotExists = true;
                        options.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(30);
                        options.HttpPolicy.FailuresBeforeBackoff = 5;
                        options.HttpPolicy.Timeout = TimeSpan.FromSeconds(10);
                        options.MetricsOutputFormatter = new MetricsInfluxDbLineProtocolOutputFormatter();
                        options.FlushInterval = TimeSpan.FromSeconds(20);
                    })
                .Build();

            // Health Metrics 配置
            var healthMetrics = AppMetricsHealth.CreateDefaultBuilder()
                    .HealthChecks.RegisterFromAssembly(services)
                    .BuildAndAddTo(services);

            services.AddMetrics(basicMetrics);

            // 定期推送监控信息
            services.AddMetricsReportScheduler();

            // 启用 App.Metrics.AspNetCore.Tracking 包中的中间件
            services.AddMetricsTrackingMiddleware();

            // 启用 App.Metrics.AspNetCore.Endpoints 包中的中间件
            services.AddMetricsEndpoints();

            // 启用 App.Metrics.AspNetCore.Health 包中的中间件
            services.AddHealth(healthMetrics);

            services
                    .AddMvc(options =>
                    {
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
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "My API", Version = "v1" });
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // 打开所有的 Web Tracking 中间件功能
            // app.UseMetricsActiveRequestMiddleware();
            // app.UseMetricsErrorTrackingMiddleware();
            // app.UseMetricsPostAndPutSizeTrackingMiddleware();
            // app.UseMetricsRequestTrackingMiddleware();
            // app.UseMetricsOAuth2TrackingMiddleware();
            // app.UseMetricsApdexTrackingMiddleware();
            app.UseMetricsAllMiddleware();

            // 暴露所有的 HTTP 节点
            // 这样可以被其他应用集成
            // app.UseMetricsEndpoint();
            // app.UseMetricsTextEndpoint();
            // app.UseEnvInfoEndpoint();
            app.UseMetricsAllEndpoints();

            // 暴露 Health HTTP 节点
            // app.UseHealthEndpoint();
            // app.UsePingEndpoint();
            app.UseHealthAllEndpoints();

            app.UseStaticFiles();
            app.UseSwagger();
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
