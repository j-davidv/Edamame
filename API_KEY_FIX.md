# Gemini API 404 Error - Fixed!

## Problem Found & Fixed

The application had a **hardcoded incorrect API key** in `ServiceCollectionExtensions.cs` instead of reading from your `GEMINI_API_KEY` environment variable.

### What Was Wrong
```csharp
// ❌ WRONG - Hardcoded fake key
geminiApiKey = Environment.GetEnvironmentVariable("AIzaSyB4IRVjO-YEdYwQk_w9N3GxphVNQiN941o");
```

### What's Fixed
```csharp
// ✅ CORRECT - Reads from GEMINI_API_KEY environment variable
geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
```

## How to Fix (3 Steps)

### Step 1: Get Your Real API Key
1. Visit https://ai.google.dev
2. Click **"Get API Key"** (blue button)
3. Create or select a Google Cloud project
4. Copy your key (should start with `AIza...`)

### Step 2: Set Environment Variable

**PowerShell (Current Session)**:
```powershell
$env:GEMINI_API_KEY = "your_api_key_here"
```

**PowerShell (Permanent - All Sessions)**:
```powershell
[System.Environment]::SetEnvironmentVariable('GEMINI_API_KEY', 'your_api_key_here', 'User')
# Restart PowerShell and Visual Studio after
```

**Windows (Permanent - GUI)**:
1. Press `Win + X` → Click "System"
2. Advanced system settings → "Environment Variables"
3. New → Variable name: `GEMINI_API_KEY` → Variable value: your key
4. OK → OK → OK
5. **Close and restart Visual Studio**

### Step 3: Test
1. Rebuild the application (Ctrl+Shift+B)
2. Run the app (F5)
3. Create a test meal:
   - Name: "Grilled Chicken"
   - Type: "Lunch"
   - Click "Create Meal"
4. ✅ Should succeed with nutrition data!

## Important Notes

- ⚠️ After setting environment variable, **restart Visual Studio**
- ⚠️ The variable must be set BEFORE launching the app
- ⚠️ Running `dotnet run` from same terminal is the most reliable
- 🔑 Never share your API key publicly (it's in the screenshot!)

## What Changed

**File**: `Infrastructure/Configuration/ServiceCollectionExtensions.cs`
- Line 24: Fixed environment variable name from hardcoded key to `GEMINI_API_KEY`
- Code now properly reads your actual API key
- App will throw error if key not set (instead of using invalid key)

## Verify the Fix

Check that your environment variable is set:
```powershell
Write-Output $env:GEMINI_API_KEY
# Should output: AIza... (your actual key)
```

## If Still Getting Errors

1. **Check the error message** - should now be more descriptive
2. **Verify API key format**: Should be ~39 characters starting with "AIza"
3. **Check API is enabled**: https://console.cloud.google.com
4. **Test manual API call**:
```powershell
$key = $env:GEMINI_API_KEY
$body = @{
    contents = @(@{ parts = @(@{ text = "Test" }) })
} | ConvertTo-Json

Invoke-WebRequest `
    -Uri "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key=$key" `
    -Method POST `
    -ContentType "application/json" `
    -Body $body
```

## Summary

✅ **Problem**: Hardcoded wrong API key  
✅ **Fix**: Now reads from GEMINI_API_KEY environment variable  
✅ **Action Needed**: Set your real API key in environment  
✅ **Result**: App will now work with real API calls  

Now try creating a meal again - it should work!
