using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Edamam.Application.Services;
using Edamam.Domain.Entities;
using Edamam.Domain.Interfaces;
using Edamam.Infrastructure.ExternalServices;
using Edamam.Infrastructure.Persistence;

namespace Edamam.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{
    // registers all application services with DI container
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

        // persistence
        var connectionFactory = new LiteDbConnectionFactory(databasePath);
        var database = connectionFactory.GetDatabase();
        services.AddSingleton(database);
        services.AddSingleton(connectionFactory);

        // infra repos
        services.AddScoped<IRepository<Meal>>(provider =>
            new LiteDbRepository<Meal>(provider.GetRequiredService<ILiteDatabase>()));

        services.AddScoped<IRepository<Recipe>>(provider =>
            new LiteDbRepository<Recipe>(provider.GetRequiredService<ILiteDatabase>()));

        // external svc
        services.AddScoped<INutritionAnalysisService>(provider =>
            new GeminiNutritionAnalysisService(geminiApiKey));

        services.AddScoped<IGeminiChatService>(provider =>
            new GeminiChatService(geminiApiKey));

        // aggregator
        services.AddScoped<IDailyMealAggregator>(provider =>
            new DailyMealAggregator(provider.GetRequiredService<IRepository<Meal>>()));

        services.AddScoped<MealService>();

        return services;
    }
}
