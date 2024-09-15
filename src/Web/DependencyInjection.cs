using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.Dashboard.Themes;
using Hangfire.MissionControl;
using Hangfire.SqlServer;
using Hangfire.Tags;
using Hangfire.Tags.SqlServer;
using Microsoft.AspNetCore.Mvc;
using Riton.Infrastructure.Data;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddHttpContextAccessor();

        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        services.AddExceptionHandler<CustomExceptionHandler>();

        services.AddRazorPages();

        // Customise default API behaviour
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddEndpointsApiExplorer();

        services.AddOpenApiDocument((configure, sp) =>
        {
            configure.Title = "Riton API";
        });

        return services;
    }

    public static IServiceCollection AddHangfireAndServer(this IServiceCollection services, ConfigurationManager configuration)
    {
        var tagsOptions = new TagsOptions() { TagsListStyle = TagsListStyle.LinkButton };
        var hangfireSqlOptions = new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true,
        };

        services.AddHangfire((provider, config) => config
            .UseSimpleAssemblyNameTypeSerializer()
            .UseConsole()
            .UseRecommendedSerializerSettings()
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"), hangfireSqlOptions)
            .UseTagsWithSql(tagsOptions, hangfireSqlOptions)
            .UseCustomTheme(DashboardThemes.Glass)
            .UseMissionControl(
                new MissionControlOptions
                {
                    RequireConfirmation = true, // disables confirmation popup
                    HideCodeSnippet = false // hides code snippet and queue on missions page
                }, typeof(Riton.Web.Endpoints.Todos).Assembly));

        services.AddHangfireConsoleExtensions();

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = Environment.ProcessorCount * 5;
            options.SchedulePollingInterval = TimeSpan.FromSeconds(10);
        });

        return services;
    }
}
