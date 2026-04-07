using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Edamam.Application.Services;
using Edamam.Domain.Entities;
using Edamam.Domain.Interfaces;
using Edamam.Infrastructure.ExternalServices;
using Edamam.Infrastructure.Persistence;

namespace Edamam.Infrastructure.Configuration;

/// <summary>
/// Dependency Injection configuration following SOLID principles.
/// Centralizes service registration and dependency setup.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services with DI container.
    /// </summary>
    public static IServiceCollection AddMealTrackerServices(
        this IServiceCollection services,
        string? geminiApiKey = null,
        string? databasePath = null)
    {
        if (string.IsNullOrWhiteSpace(geminiApiKey))
        {
            geminiApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
        }

        if (string.IsNullOrWhiteSpace(geminiApiKey))
        {
            throw new InvalidOperationException(
                "Gemini API key is required. Set GOOGLE_API_KEY environment variable.");
        }

        // Infrastructure: Persistence
        var connectionFactory = new LiteDbConnectionFactory(databasePath);
        var database = connectionFactory.GetDatabase();
        services.AddSingleton(database);
        services.AddSingleton(connectionFactory);

        // Infrastructure: Repositories (Dependency Inversion)
        services.AddScoped<IRepository<Meal>>(provider =>
            new LiteDbRepository<Meal>(provider.GetRequiredService<ILiteDatabase>()));

        services.AddScoped<IRepository<Recipe>>(provider =>
            new LiteDbRepository<Recipe>(provider.GetRequiredService<ILiteDatabase>()));

        // Infrastructure: External Services (Dependency Inversion)
        services.AddScoped<INutritionAnalysisService>(provider =>
            new GeminiNutritionAnalysisService(geminiApiKey));

        services.AddScoped<IGeminiChatService>(provider =>
            new GeminiChatService(geminiApiKey));

        // Application: Business Logic Services
        services.AddScoped<IDailyMealAggregator>(provider =>
            new DailyMealAggregator(provider.GetRequiredService<IRepository<Meal>>()));

        services.AddScoped<MealService>();

        return services;
    }
}
