using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace TravelPlanner
{
    /// <summary>
    /// Reads configuration values from appsettings.json.
    /// This class is used by the Travel Planner agent.
    /// </summary>
    public static class AppConfig
    {
        // Lazy<T> is thread-safe by default — built only once, on first access.
        private static readonly Lazy<IConfiguration> _lazyConfig = new(() =>
            new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, false)
                .Build());

        /// <summary>
        /// Loads appsettings.json only once and reuses it.
        /// </summary>
        public static IConfiguration Settings => _lazyConfig.Value;

        /// <summary>
        /// Ollama endpoint.
        /// Example:
        /// http://localhost:11434
        /// </summary>
        public static string OllamaEndpoint
        {
            get { return Settings["Ollama:Endpoint"]!; }
        }

        /// <summary>
        /// Ollama chat model.
        /// Example:
        /// llama3.1
        /// </summary>
        public static string ChatModel
        {
            get { return Settings["Ollama:ChatModel"]!; }
        }

        /// <summary>
        /// HTTP client timeout for Ollama requests.
        /// Configured via Ollama:TimeoutSeconds in appsettings.json.
        /// Defaults to 300 seconds (5 minutes) if not configured.
        /// </summary>
        public static TimeSpan Timeout
        {
            get
            {
                if (int.TryParse(Settings["Ollama:TimeoutSeconds"], out int seconds))
                {
                    return TimeSpan.FromSeconds(seconds);
                }

                return TimeSpan.FromMinutes(5);
            }
        }

        /// <summary>
        /// Travel Agent ID.
        /// </summary>
        public static string AgentId
        {
            get { return Settings["TravelAgent:AgentId"]!; }
        }

        /// <summary>
        /// Travel Agent Name.
        /// </summary>
        public static string AgentName
        {
            get { return Settings["TravelAgent:AgentName"]!; }
        }

        /// <summary>
        /// Travel Agent Description.
        /// </summary>
        public static string AgentDescription
        {
            get { return Settings["TravelAgent:AgentDescription"]!; }
        }

        /// <summary>
        /// Skills folder location.
        /// Example:
        /// Skills/travel-planner/SKILL.md
        /// </summary>
        public static string SkillsFolder
        {
            get
            {
                return Path.Combine(
                    AppContext.BaseDirectory,
                    "Skills");
            }
        }
    }
}