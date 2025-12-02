using InterviewApp.API.Data;
using InterviewApp.API.Infrastructure.Middlewares;
using InterviewApp.API.Infrastructure.Middlewares.Extensions;
using InterviewApp.Data;
using InterviewApp.Data.Base;
using InterviewApp.Services;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace InterviewApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment WebHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DataProvider, CoreDataProvider>();
            services.AddSingleton<NhInitFactory>();
            services.AddSingleton<IConnectionSettingsProvider, ConnectionSettingsProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<ICustomerRepository, CustomerRepository>();

            services.AddScoped<CustomerService>();

            services.AddScoped<ErrorHandlingMiddleware>();

            services
                .AddControllers(options =>
                {
                    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
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
                //Strict-Transport-Security
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
#if DEBUG
                config.SwaggerEndpoint(@"/swagger/v1/swagger.json", "Test App API");
#endif
                config.RoutePrefix = "help";

                //Show more of the model by default
                config.DefaultModelExpandDepth(2);

                //Close all of the major nodes
                config.DocExpansion(DocExpansion.None);

                //Show the example by default
                config.DefaultModelRendering(ModelRendering.Example);

                //Turn on Try it by default
                //config.EnableTryItOutByDefault();

                config.ConfigObject.AdditionalItems.Add("syntaxHighlight", false);
            });

#if !DEBUG
            app.UseHttpsRedirection();      
#endif
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.HandlerErrors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
