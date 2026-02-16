using Brupper.Jobs.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Brupper.Jobs.FileTransfer;

public static class Module
{
    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddBackgroundJobAbstractions();

        services.AddDbContextFactory<BackgroundTasksDbContext>(o =>
        {
            o.UseSqlite(Module.DefaultConnectionString, sqliteOptionsAction => sqliteOptionsAction.MaxBatchSize(250));
            o.EnableSensitiveDataLogging(true);
        });

        services.AddScoped<IFileUploader, FileUploader>();
        services.AddScoped<IUploader, Uploader>();

        return services;
    }

    public const string DefaultFileNameString = "upload.db";
    public const string DefaultConnectionString = "Data Source=" + DefaultFileNameString;

    public static DbContextOptionsBuilder<BackgroundTasksDbContext> CreateDefaultOptionsFromFileName(string filePath)
         => CreateDefaultOptions($"Data Source={filePath}");

    public static DbContextOptionsBuilder<BackgroundTasksDbContext> CreateDefaultOptions(string connectionString = DefaultConnectionString)
        => new DbContextOptionsBuilder<BackgroundTasksDbContext>()
#if DEBUG
            .EnableSensitiveDataLogging(true)
#endif
            .UseSqlite(connectionString, sqliteOptionsAction => sqliteOptionsAction.MaxBatchSize(250))
            ;
}
