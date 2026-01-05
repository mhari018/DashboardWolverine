using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using DashboardWolverine.Repositories;

namespace DashboardWolverine;

/// <summary>
/// Extension methods untuk setup Monitoring Dashboard
/// </summary>
public static class MonitoringDashboardExtensions
{
    /// <summary>
    /// Menambahkan Monitoring Dashboard middleware ke aplikasi ASP.NET Core.
    /// Dashboard akan accessible di route yang ditentukan (default: /monitoring)
    /// </summary>
    public static IApplicationBuilder UseMonitoringDashboard(
        this IApplicationBuilder app,
        Action<MonitoringDashboardOptions>? configure = null)
    {
        var options = new MonitoringDashboardOptions();
        configure?.Invoke(options);
        options.Validate();

        // Register WolverineRepository if connection string is provided
        if (!string.IsNullOrWhiteSpace(options.WolverineConnectionString))
        {
            var serviceProvider = app.ApplicationServices;
            
            // Check if WolverineRepository is already registered
            var existingRepository = serviceProvider.GetService<WolverineRepository>();
            if (existingRepository == null)
            {
                // Register as singleton in the service collection
                var services = serviceProvider.GetService<IServiceCollection>();
                if (services != null)
                {
                    services.AddSingleton(new WolverineRepository(options.WolverineConnectionString, options.Schema));
                }
            }
        }

        app.UseMiddleware<MonitoringDashboardMiddleware>(options);

        return app;
    }

    /// <summary>
    /// Menambahkan Monitoring Dashboard services ke dependency injection container.
    /// Optional - gunakan ini jika ingin inject MonitoringDashboardOptions ke service lain.
    /// </summary>
    public static IServiceCollection AddMonitoringDashboard(
        this IServiceCollection services,
        Action<MonitoringDashboardOptions>? configure = null)
    {
        var options = new MonitoringDashboardOptions();
        configure?.Invoke(options);
        options.Validate();

        services.AddSingleton(options);

        // Register WolverineRepository if connection string is provided
        if (!string.IsNullOrWhiteSpace(options.WolverineConnectionString))
        {
            services.AddSingleton(new WolverineRepository(options.WolverineConnectionString, options.Schema));
        }

        return services;
    }
}