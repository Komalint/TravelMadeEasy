using System;
using TravelPlanner;

namespace TravelPlanner
{
    /// <summary>
    /// Holds the Ollama connection properties used by TravelDemo.
    ///
    /// Ollama exposes an OpenAI-compatible endpoint:
    /// http://localhost:11434/v1
    ///
    /// This allows Semantic Kernel to communicate with local models
    /// such as:
    /// - llama3.1
    /// - qwen3
    /// - mistral
    /// - deepseek
    /// </summary>
    public static class ModelFactory
    {
        /// <summary>
        /// Model name configured in appsettings.json.
        /// Example:
        /// llama3.1
        /// </summary>
        public static string ChatModelName
        {
            get { return AppConfig.ChatModel; }
        }

        /// <summary>
        /// Ollama OpenAI-compatible endpoint.
        /// Example:
        /// http://localhost:11434/v1
        /// </summary>
        public static Uri OllamaEndpoint
        {
            get
            {
                return new Uri(
                    AppConfig.OllamaEndpoint.TrimEnd('/') + "/v1");
            }
        }
    }
}