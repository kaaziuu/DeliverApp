using System.Text.Json;
using Deliver.Middleware;
using Deliver.Setup;
using DeployApp;

namespace Deliver
{
    public class Startup
    {
        const string useSqlArg = "localSql";
        const string onlySqlArg = "onlySql";
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            _configuration = configuration;
            _environment = hostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Deploy.RunSqlScript(_configuration.GetConnectionString("DefaultConnection"));

            services.SetupDb(_configuration);

            services.RegisterSettings(_configuration);

            services.RegisterRepository();

            services.RegisterUtils();

            services.RegisterService();

            services.RegisterIntegrations();

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            if (_environment.IsDevelopment())
            {
                var baseInsertTask = services.BaseInsert();
                baseInsertTask.Wait();
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseCors(x => x
                .SetIsOriginAllowed(o => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseMiddleware<CatchAppExceptionMiddleware>();

            app.UseMiddleware<CastToBaseResponseMiddleware>();

            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}