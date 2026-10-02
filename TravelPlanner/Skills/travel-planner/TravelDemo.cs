using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace TravelPlanner
{
    /// <summary>
    /// Travel Planner Agent using Semantic Kernel.
    ///
    /// Features:
    /// - Function Calling
    /// - Travel Planning Tools
    /// - SKILL.md Loading
    /// - Chat History
    /// - Ollama Integration
    /// </summary>
    public static class TravelDemo
    {
        // Single HttpClient for the app lifetime — avoids socket leak from
        // creating a new one on every call. Timeout is read from appsettings.json.
        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = AppConfig.Timeout
        };

        public static async Task RunAsync()
        {
            Console.WriteLine();
            Console.WriteLine("==========================================");
            Console.WriteLine("      AI Travel Planner Agent");
            Console.WriteLine("==========================================");
            Console.WriteLine("Model : " + AppConfig.ChatModel);
            Console.WriteLine();

            Kernel kernel = CreateKernel();

            AddPlugins(kernel);

            IChatCompletionService chatService =
                kernel.GetRequiredService<IChatCompletionService>();

            ChatHistory history = new ChatHistory();

            history.AddSystemMessage(GetSystemPrompt());

            OpenAIPromptExecutionSettings settings =
                new OpenAIPromptExecutionSettings();

            settings.FunctionChoiceBehavior =
                FunctionChoiceBehavior.Auto();

            await ChatLoopAsync(
                kernel,
                chatService,
                history,
                settings);
        }

        /// <summary>
        /// Creates Semantic Kernel and connects it to Ollama.
        /// Uses a shared HttpClient with configured timeout.
        /// </summary>
        private static Kernel CreateKernel()
        {
            var builder = Kernel.CreateBuilder();

            builder.AddOpenAIChatCompletion(
                modelId: ModelFactory.ChatModelName,
                endpoint: ModelFactory.OllamaEndpoint,
                apiKey: "ollama",
                httpClient: _httpClient);

            return builder.Build();
        }

        /// <summary>
        /// Registers all plugins.
        /// </summary>
        private static void AddPlugins(
            Kernel kernel)
        {
            kernel.Plugins.AddFromType<TravelTools>(
                "TravelTools");

            kernel.Plugins.AddFromType<TravelSkills>(
                "TravelSkills");

            Console.WriteLine("Loaded Plugins:");
            Console.WriteLine(" - TravelTools");
            Console.WriteLine(" - TravelSkills");
            Console.WriteLine();
        }

        /// <summary>
        /// System instructions given to the model.
        /// </summary>
        private static string GetSystemPrompt()
        {
            return
"""
You are TravelPlanner, an AI travel planning assistant.

Before creating any travel itinerary,
load the travel-planner skill.

Use available tools whenever appropriate.

The available travel skill is:

- travel-planner

If a user asks to:
- plan a trip
- create an itinerary
- estimate travel expenses
- suggest attractions
- suggest packing items
- recommend travel dates

then load the travel-planner skill first.

Always prefer tools over guessing.

Create structured and professional responses.
""";
        }

        /// <summary>
        /// Keeps only the system message plus the last 6 messages (3 turns).
        /// Prevents context overflow after many conversation turns.
        /// </summary>
        private static void TrimHistory(ChatHistory history)
        {
            const int MaxMessages = 6;

            // Count non-system messages
            int nonSystemCount = history.Count(m => m.Role != AuthorRole.System);

            if (nonSystemCount <= MaxMessages)
                return;

            // Rebuild: keep system message first, then only the recent messages
            var systemMessage = history.First(m => m.Role == AuthorRole.System);
            var recentMessages = history
                .Where(m => m.Role != AuthorRole.System)
                .TakeLast(MaxMessages)
                .ToList();

            history.Clear();
            history.Add(systemMessage);
            foreach (var msg in recentMessages)
                history.Add(msg);
        }

        /// <summary>
        /// Main conversation loop.
        /// Press Ctrl+C to cancel an in-progress generation cleanly.
        /// </summary>
        private static async Task ChatLoopAsync(
            Kernel kernel,
            IChatCompletionService chatService,
            ChatHistory history,
            OpenAIPromptExecutionSettings settings)
        {
            Console.WriteLine("Examples:");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Plan a Goa trip from 2026-12-20 to 2026-12-25");
            Console.WriteLine("Plan a Jaipur weekend trip");
            Console.WriteLine("Best time to visit Kerala");
            Console.WriteLine("--------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Type 'exit' to quit. Press Ctrl+C to cancel a response.");
            Console.WriteLine();

            // Allow Ctrl+C to cancel the current model call without killing the app
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // Don't kill the process, just cancel the request
                cts.Cancel();
                Console.WriteLine("\n[Cancelled]");
            };

            while (true)
            {
                Console.Write("You : ");

                string input =
                    Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                if (input.Trim().Equals(
                    "exit",
                    StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                history.AddUserMessage(input);

                // Trim history before each call to avoid context overflow
                TrimHistory(history);

                try
                {
                    ChatMessageContent response =
                        await chatService.GetChatMessageContentAsync(
                            history,
                            settings,
                            kernel,
                            cts.Token);

                    string content = response.Content ?? "(No response returned)";

                    history.AddAssistantMessage(content);

                    Console.WriteLine();
                    Console.WriteLine("Travel Planner:");
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine(content);
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine();
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Request was cancelled.");
                    Console.WriteLine();
                }
            }
        }
    }
}