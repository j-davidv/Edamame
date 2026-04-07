# Gemini API Troubleshooting Guide

## Common Issues & Solutions

### Issue 1: 404 Not Found Error

**Causes**:
- ❌ API key is invalid or expired
- ❌ API key was not set as environment variable
- ❌ API key has spaces or typo
- ❌ API endpoint URL is incorrect (unlikely)

**Solutions**:

**Step 1: Verify API Key**
```powershell
# In PowerShell, check the environment variable
$env:GEMINI_API_KEY

# Output should show: AIza... (40-50 characters)
# If empty: API key not set or terminal needs restart
```

**Step 2: Get a New API Key**
1. Go to https://ai.google.dev
2. Click "Get API Key" (blue button)
3. Create or select a Google Cloud project
4. Copy the key (should start with "AIza")
5. Don't share this key publicly

**Step 3: Set Environment Variable (Windows)**

**Option A - PowerShell (Current Session Only)**:
```powershell
$env:GEMINI_API_KEY = "your_api_key_here"
# Then restart the app
```

**Option B - PowerShell (Permanent - All Sessions)**:
```powershell
[System.Environment]::SetEnvironmentVariable('GEMINI_API_KEY', 'your_api_key_here', 'User')
# Close and restart PowerShell/Visual Studio
```

**Option C - Command Prompt (Current Session Only)**:
```cmd
set GEMINI_API_KEY=your_api_key_here
# Then restart the app
```

**Option D - Windows Environment Variables GUI (Permanent)**:
1. Press `Win + X` → Click "System"
2. Click "Advanced system settings"
3. Click "Environment Variables" button
4. Click "New..." under "User variables"
5. Variable name: `GEMINI_API_KEY`
6. Variable value: `your_api_key_here` (paste your actual key)
7. Click OK three times
8. **Close and restart Visual Studio**

**Step 4: Verify the App Can Access the Key**
- Run the app from the same terminal where you set the variable
- Don't run from Visual Studio's debugger (it might not see the variable)
- Try: `dotnet run` from command line instead

### Issue 2: API Rate Limit Exceeded

**Sign**: 429 Too Many Requests error

**Solution**:
- Free tier: 60 requests per minute, 1500 per day
- Wait a few minutes before creating more meals
- Check if the app is making multiple requests per meal

### Issue 3: Network Connectivity Issues

**Sign**: Timeout error or connection refused

**Solutions**:
1. Check internet connection: `ping google.com`
2. Check firewall isn't blocking HTTPS
3. Verify you can reach: https://generativelanguage.googleapis.com
4. Try creating meal again

### Issue 4: API Key Rejected

**Sign**: 401 Unauthorized or 403 Forbidden

**Solutions**:
1. API key might have special characters or spaces - check the copied key
2. API key might be revoked - generate a new one
3. Project might have API disabled - enable "Generative Language API" in Google Cloud Console

## Step-by-Step Fix

1. **Get New Key**:
   - Visit https://ai.google.dev
   - Click "Get API Key"
   - Copy key (Ctrl+C)

2. **Set in PowerShell**:
   ```powershell
   $env:GEMINI_API_KEY = "paste_your_key_here"
   ```

3. **Run from Terminal**:
   ```powershell
   cd C:\path\to\your\project
   dotnet run
   ```

4. **Create a Meal**:
   - Enter: "Grilled Chicken"
   - Type: "Lunch"
   - Click "Create Meal"

5. **Check Result**:
   - If success: Meal appears in grid with nutrition data ✅
   - If error: Note the exact error message and check below

## Error Message Reference

| Error | Cause | Fix |
|-------|-------|-----|
| 404 Not Found | Invalid key or endpoint wrong | Verify API key at ai.google.dev |
| 401 Unauthorized | API key invalid/expired | Get new key from ai.google.dev |
| 429 Too Many Requests | Rate limit exceeded | Wait a few minutes |
| Connection timeout | Network issue | Check internet, try again |
| JSON deserialization failed | API response format wrong | Check API key is valid |
| No text content in response | API returned empty response | Verify network and API key |

## Advanced Debugging

### Check API Key Format
Your API key should:
- ✅ Start with "AIza"
- ✅ Be 39 characters long
- ✅ Contain only alphanumeric characters and hyphens
- ✅ Not have spaces at start/end

### Verify Endpoint URL
Current: `https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent`

This is correct for Gemini 1.5 Flash model (free tier).

### Check if API is Enabled
1. Go to https://console.cloud.google.com
2. Select your project
3. Search for "Generative Language API"
4. Click "Enable"

### Manual API Test (Optional)
Using PowerShell:
```powershell
$key = $env:GEMINI_API_KEY
$body = @{
    contents = @(
        @{
            parts = @(
                @{ text = "Test: What is 2+2?" }
            )
        }
    )
} | ConvertTo-Json

Invoke-WebRequest -Uri "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key=$key" `
    -Method POST `
    -ContentType "application/json" `
    -Body $body
```

If successful, you'll get a JSON response with results.

## Fresh Start Checklist

- [ ] Got new API key from ai.google.dev
- [ ] Set GEMINI_API_KEY environment variable
- [ ] Closed and restarted Visual Studio or terminal
- [ ] Ran app from same terminal where variable was set
- [ ] Tried creating meal again
- [ ] Checked exact error message (if still failing)

## Need More Help?

1. **Check the exact error message** in the UI (should be more detailed now)
2. **Try the manual API test** above
3. **Verify environment variable** with: `$env:GEMINI_API_KEY`
4. **Check firewall/antivirus** isn't blocking the API
5. **Try from different network** to rule out ISP blocking

## Alternative: Test Without API Key

If you want to test the UI without using the real API:
1. Create a mock implementation of `INutritionAnalysisService`
2. Return hardcoded nutrition values
3. Replace in DI container

Let me know the exact error message and I can provide more specific help!
