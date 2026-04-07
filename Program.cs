global using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Edamam.Infrastructure.Configuration;

namespace Edamam
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Setup Dependency Injection container
            var services = new ServiceCollection();

            // Load configuration from appsettings.json
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            // Register Google Gemini API credentials (from environment variables or appsettings.json)
            string? geminiApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY") 
                ?? config["GeminiApiKey"];
            services.AddMealTrackerServices(geminiApiKey);

            var serviceProvider = services.BuildServiceProvider();

            // Create and run the main form with DI container
            var form = new Form1(serviceProvider);
            RunWindowsFormsApp(form);
        }

        private static void RunWindowsFormsApp(Form mainForm)
        {
            global::System.Windows.Forms.Application.Run(mainForm);
        }
    }
}