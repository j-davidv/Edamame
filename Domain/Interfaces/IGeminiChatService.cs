namespace TEST.Domain.Interfaces;

/// <summary>
/// Abstraction for Gemini chat service.
/// Defines contract for conversational AI interactions.
/// </summary>
public interface IGeminiChatService
{
    /// <summary>
    /// Sends a message to the Gemini chatbot and gets a response.
    /// </summary>
    /// <param name="userMessage">The user's message or question.</param>
    /// <returns>The AI's response as a string.</returns>
    Task<string> ChatAsync(string userMessage);

    /// <summary>
    /// Sends a message with context about nutrition/meals.
    /// </summary>
    /// <param name="userMessage">The user's message or question.</param>
    /// <param name="context">Optional context about the meal/nutrition data.</param>
    /// <returns>The AI's response as a string.</returns>
    Task<string> ChatWithContextAsync(string userMessage, string? context = null);

    /// <summary>
    /// Clears the conversation history.
    /// </summary>
    Task ClearHistoryAsync();
}
