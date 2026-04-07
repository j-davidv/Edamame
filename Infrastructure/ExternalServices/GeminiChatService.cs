using Google.GenAI;
using Google.GenAI.Types;
using Edamam.Domain.Interfaces;

namespace Edamam.Infrastructure.ExternalServices;

/// Implements IGeminiChatService
/// conversational AI assistant

public class GeminiChatService : IGeminiChatService
{
    private readonly Client _geminiClient;
    private const string MODEL_NAME = "gemini-2.5-flash";
    private readonly List<(string role, string text)> _conversationHistory;

    public GeminiChatService(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key cannot be null or empty.", nameof(apiKey));

        _geminiClient = new Client(apiKey: apiKey);
        _conversationHistory = new List<(string, string)>();
    }


    // sends a message to the Gemini chatbot and gets a response
    public async Task<string> ChatAsync(string userMessage)
    {
        return await ChatWithContextAsync(userMessage, null);
    }


    // Sends a message with optional context about nutrition/meals

    public async Task<string> ChatWithContextAsync(string userMessage, string? context = null)
    {
        if (string.IsNullOrWhiteSpace(userMessage))
        {
            throw new ArgumentException("User message cannot be empty.", nameof(userMessage));
        }

        try
        {
            var fullMessage = context != null 
                ? $"{userMessage}\n\nContext:\n{context}" 
                : userMessage;

            System.Diagnostics.Debug.WriteLine($"Gemini Chat Request: {userMessage}");

            // Add user message to history
            _conversationHistory.Add(("user", fullMessage));

            // Build prompt with system instruction and history
            var systemInstruction = @"You are a helpful nutrition and meal planning assistant. You provide accurate, evidence-based advice about nutrition, dietary planning, and healthy eating habits. 
You are friendly, supportive, and encouraging. Keep responses concise but informative.";

            var promptBuilder = new System.Text.StringBuilder();
            promptBuilder.AppendLine(systemInstruction);
            promptBuilder.AppendLine();

            // Add conversation history
            foreach (var (role, text) in _conversationHistory)
            {
                if (role == "user")
                {
                    promptBuilder.AppendLine($"User: {text}");
                }
                else if (role == "assistant")
                {
                    promptBuilder.AppendLine($"Assistant: {text}");
                }
            }

            var response = await _geminiClient.Models.GenerateContentAsync(
                model: MODEL_NAME,
                contents: promptBuilder.ToString()
            );

            if (response?.Candidates == null || response.Candidates.Count == 0)
            {
                throw new InvalidOperationException("Gemini API returned no candidates in response.");
            }

            var candidate = response.Candidates[0];
            if (candidate?.Content?.Parts == null || candidate.Content.Parts.Count == 0)
            {
                throw new InvalidOperationException("Gemini API returned no content parts in response.");
            }

            var responseText = candidate.Content.Parts[0].Text;
            if (string.IsNullOrWhiteSpace(responseText))
            {
                throw new InvalidOperationException("Gemini API returned empty response text.");
            }

            System.Diagnostics.Debug.WriteLine($"Gemini Chat Response: {responseText}");

            // Add assistant response to history
            _conversationHistory.Add(("assistant", responseText));

            return responseText;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error calling Gemini Chat API: {ex.Message}", ex);
        }
    }

    /// clears the conversation history

    public async Task ClearHistoryAsync()
    {
        _conversationHistory.Clear();
        await Task.CompletedTask;
    }
}
