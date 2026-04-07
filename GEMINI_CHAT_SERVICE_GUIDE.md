# Gemini Chat Service - Quick Reference

## Overview
The new `IGeminiChatService` enables conversational AI interactions with Gemini 2.5 Flash. Perfect for nutrition-related questions, meal planning advice, and general assistance.

## Interface Definition

```csharp
public interface IGeminiChatService
{
    Task<string> ChatAsync(string userMessage);
    Task<string> ChatWithContextAsync(string userMessage, string? context = null);
    Task ClearHistoryAsync();
}
```

## Basic Usage

### Simple Chat
```csharp
public class NutritionAdvisor
{
    private readonly IGeminiChatService _chatService;
    
    public NutritionAdvisor(IGeminiChatService chatService)
    {
        _chatService = chatService;
    }
    
    public async Task<string> AskQuestion(string question)
    {
        // Simple chat without context
        return await _chatService.ChatAsync(question);
    }
}

// Usage
var response = await advisor.AskQuestion("What are good protein sources for vegans?");
// Response: "Great question! Here are excellent plant-based protein sources..."
```

### Chat with Nutritional Context
```csharp
public async Task<string> GetAdviceForMeal(Meal meal)
{
    var context = $@"
        Meal: {meal.Name}
        Calories: {meal.Nutritionals?.Calories}
        Protein: {meal.Nutritionals?.Protein}g
        Carbs: {meal.Nutritionals?.Carbohydrates}g
        Fat: {meal.Nutritionals?.Fat}g";
    
    var question = "Is this meal balanced for a workout recovery diet?";
    
    return await _chatService.ChatWithContextAsync(question, context);
}

// Usage with DI
var response = await GetAdviceForMeal(myMeal);
// Response: "Based on the nutritional data provided, this meal is excellent for post-workout recovery because..."
```

### Conversation History
The chat service maintains conversation history automatically, enabling multi-turn conversations:

```csharp
// First message
var response1 = await _chatService.ChatAsync("What's the recommended daily protein intake?");
// Response: "The recommended daily protein intake is..."

// Follow-up (maintains context)
var response2 = await _chatService.ChatAsync("How can I meet that goal on a vegetarian diet?");
// Response: "Since you're vegetarian, you can meet your protein goals by..."

// Clear history when needed
await _chatService.ClearHistoryAsync();
```

## Dependency Injection Setup

The service is already registered in `ServiceCollectionExtensions.cs`:

```csharp
services.AddScoped<IGeminiChatService>(provider =>
    new GeminiChatService());
```

### Use in a Controller
```csharp
[ApiController]
[Route("api/[controller]")]
public class NutritionChatController : ControllerBase
{
    private readonly IGeminiChatService _chatService;
    
    public NutritionChatController(IGeminiChatService chatService)
    {
        _chatService = chatService;
    }
    
    [HttpPost("ask")]
    public async Task<IActionResult> AskQuestion([FromBody] string question)
    {
        try
        {
            var response = await _chatService.ChatAsync(question);
            return Ok(new { response });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpPost("ask-with-context")]
    public async Task<IActionResult> AskWithContext(
        [FromBody] ChatRequestWithContext request)
    {
        try
        {
            var response = await _chatService.ChatWithContextAsync(
                request.Question, 
                request.Context);
            return Ok(new { response });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpPost("clear-history")]
    public async Task<IActionResult> ClearHistory()
    {
        await _chatService.ClearHistoryAsync();
        return Ok();
    }
}

public class ChatRequestWithContext
{
    public string Question { get; set; }
    public string Context { get; set; }
}
```

## UI Integration Example (WinForms)

```csharp
public partial class NutritionChatForm : Form
{
    private readonly IGeminiChatService _chatService;
    private TextBox inputBox;
    private TextBox outputBox;
    
    public NutritionChatForm(IGeminiChatService chatService)
    {
        _chatService = chatService;
        InitializeComponent();
    }
    
    private async void SendButton_Click(object sender, EventArgs e)
    {
        try
        {
            var userMessage = inputBox.Text;
            outputBox.AppendText($"You: {userMessage}\n");
            
            var response = await _chatService.ChatAsync(userMessage);
            outputBox.AppendText($"Assistant: {response}\n\n");
            
            inputBox.Clear();
            inputBox.Focus();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Chat Error");
        }
    }
    
    private async void ClearButton_Click(object sender, EventArgs e)
    {
        await _chatService.ClearHistoryAsync();
        outputBox.Clear();
        inputBox.Clear();
    }
}
```

## Advanced Example: Meal Planning Assistant

```csharp
public class MealPlanningAssistant
{
    private readonly IGeminiChatService _chatService;
    private readonly INutritionAnalysisService _nutritionService;
    
    public MealPlanningAssistant(
        IGeminiChatService chatService,
        INutritionAnalysisService nutritionService)
    {
        _chatService = chatService;
        _nutritionService = nutritionService;
    }
    
    public async Task<string> PlanMealForGoals(
        string dietaryGoal,
        int targetCalories,
        Dictionary<string, int> macros) // protein, carbs, fat targets
    {
        var context = $@"
            Dietary Goal: {dietaryGoal}
            Target Calories: {targetCalories}
            Target Protein: {macros["protein"]}g
            Target Carbs: {macros["carbs"]}g
            Target Fat: {macros["fat"]}g";
        
        var question = "Create a meal plan for today that fits these nutritional targets. Include specific ingredients and portions.";
        
        return await _chatService.ChatWithContextAsync(question, context);
    }
    
    public async Task<string> GetRecipeModification(
        Recipe originalRecipe,
        string desiredModification)
    {
        var nutritionals = await _nutritionService.AnalyzeRecipeAsync(originalRecipe);
        
        var context = $@"
            Original Recipe: {originalRecipe.Name}
            Current Calories: {nutritionals.Calories}
            Current Protein: {nutritionals.Protein}g
            Current Fat: {nutritionals.Fat}g
            Current Carbs: {nutritionals.Carbohydrates}g";
        
        var question = desiredModification; // e.g., "Make this lower in fat while maintaining protein"
        
        return await _chatService.ChatWithContextAsync(question, context);
    }
}
```

## Configuration

### Required Environment Variable
```
GEMINI_API_KEY=your-gemini-api-key
```

Set this before running the application:
```powershell
$env:GEMINI_API_KEY = "AIza..."
```

## Features

### ✅ Conversation History
- Automatically maintains conversation context
- Multi-turn conversations supported
- Manual history clear available

### ✅ System Instructions
- Nutrition-focused responses
- Evidence-based advice
- Friendly and supportive tone
- Concise but informative

### ✅ Context Support
- Add nutritional data for better advice
- Include meal information for analysis
- Support dietary goal context

### ✅ Error Handling
- Validates non-empty messages
- Throws informative exceptions
- Debug logging of requests/responses

## Common Use Cases

1. **Nutrition Questions**
   ```csharp
   "What are the health benefits of omega-3 fatty acids?"
   ```

2. **Meal Planning**
   ```csharp
   "Plan a high-protein, low-carb dinner for 2 people"
   ```

3. **Diet Compatibility**
   ```csharp
   "Can I eat this meal on a keto diet?" (with meal context)
   ```

4. **Cooking Advice**
   ```csharp
   "How do I reduce calories in this recipe?" (with recipe context)
   ```

5. **Dietary Goals**
   ```csharp
   "I want to gain muscle mass. What should I eat?"
   ```

## Notes

- The service uses `gemini-2.5-flash` model
- Responses are streamed and provided as complete text
- API key is automatically retrieved from environment
- Service is registered as scoped in DI container
- Thread-safe for concurrent use

## Troubleshooting

**Issue:** "GEMINI_API_KEY not found"
- Set the environment variable before running the app
- Restart the application after setting the variable

**Issue:** Empty or null responses
- Ensure the API key is valid
- Check internet connection
- Verify Gemini API is accessible

**Issue:** Inconsistent response quality
- Provide more specific context
- Ask clearer questions
- Use ChatWithContextAsync for better results
