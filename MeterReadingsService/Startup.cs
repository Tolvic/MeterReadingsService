using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO.Abstractions;
using MeterReadingsService.Builders;
using MeterReadingsService.Data;
using MeterReadingsService.Repositories;
using MeterReadingsService.Services;
using MeterReadingsService.Validator;
using Microsoft.EntityFrameworkCore;

namespace MeterReadingsService
{
    [ExcludeFromCodeCoverage]
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

            services.AddControllers();
            services.AddDbContext <MeterReadingServiceContext> (options =>
                options.UseSqlServer(Configuration.GetConnectionString("MeterReadingServiceContext")));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MeterReadingsService", Version = "v1" });
            });
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IFileSystem, FileSystem>();
            services.AddTransient<IGuidGenerator, GuidGenerator>();
            services.AddTransient<ICsvReaderBuilder, CsvReaderBuilder>();
            services.AddScoped<ICsvParser, CsvParser>();
            services.AddTransient<IMeterReadingsBuilder, MeterReadingsBuilder>();
            services.AddTransient<IMeterReadingUploadsValidator, MeterReadingUploadsValidator>();
            services.AddTransient<IUploadResultBuilder, UploadResultBuilder>();
            services.AddTransient<IUploadResultBuilder, UploadResultBuilder>();
            services.AddTransient<IMeterReadingsRepository, MeterReadingsRepository>();
            services.AddTransient<IAccountsRepository, AccountsRepository>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MeterReadingsService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
