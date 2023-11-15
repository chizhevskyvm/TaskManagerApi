using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Services.Services;
using TaskManager.Services.Services.Impl;
using TaskManagerApi.Model;

namespace TaskManager.Services.Extansions;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtansions
{
    /// <summary>
    /// Inject main context into container
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString"></param>
    public static void ConfigureDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<DbContext, TaskManagerDbContext>(option =>
        {
            option.UseSqlite(connectionString, x =>
            {
                x.MigrationsAssembly(typeof(TaskManagerDbContext).Assembly.GetName().Name);
            });
        });
    }
    
    /// <summary>
    /// Injects mapper into container
    /// </summary>
    public static void AddAutoMapper(this IServiceCollection services) =>
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    
    /// <summary>
    /// Injects query handlers into container
    /// </summary>
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITaskMaganerService, TaskMaganerService>();
    }
}