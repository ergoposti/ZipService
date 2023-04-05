using ZipService.BLL.Services;
using ZipService.DAL;
using ZipService.Domain;
using ZipService.Mappers;
using ZipService.Shared.Providers;
using ZipService.Validation;

namespace ZipService
{
    public static class AppConfiguration
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddScoped<IZipFileContentProvider, ZipFileContentProvider>();
            services.AddScoped<IGameDirectoryStructureValidator, GameDirectoryStructureValidator>();
            services.AddScoped<IZipFileListDtoMapper, ZipFileListDtoMapper>();
            services.AddSingleton<IBlobService, LocalBlobService>();
            services.AddScoped<IUnitOfWork<FileEntity>, UnitOfWork<FileEntity>>();

            // Add CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5173")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .SetIsOriginAllowedToAllowWildcardSubdomains()
                               .AllowCredentials();
                    });
            });
        }

        public static void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapControllers();

            // Use the CORS middleware with the policy defined above
            app.UseCors("AllowLocalhost");
        }
    }
}
