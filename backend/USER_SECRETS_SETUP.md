# User Secrets Setup Guide

## Overview
This guide explains how to set up User Secrets for local development to securely store Blizzard API credentials.

## Prerequisites
- .NET 10 SDK installed
- Blizzard Battle.net Developer account with 2FA enabled
- Client ID and Client Secret from [Battle.net Developer Portal](https://develop.battle.net/)

## Step 1: Initialize User Secrets (Already Done)
The User Secrets have already been initialized for the WebApi project.

User Secrets ID: `e562748e-d4e8-4b73-bee5-2a246647deb6`

## Step 2: Set Your Blizzard API Credentials

Navigate to the WebApi project directory:
```powershell
cd backend\src\WarcraftArmory.WebApi
```

Set your actual Client ID and Client Secret:
```powershell
dotnet user-secrets set "BlizzardApi:ClientId" "YOUR_ACTUAL_CLIENT_ID"
dotnet user-secrets set "BlizzardApi:ClientSecret" "YOUR_ACTUAL_CLIENT_SECRET"
```

**Important**: Replace `YOUR_ACTUAL_CLIENT_ID` and `YOUR_ACTUAL_CLIENT_SECRET` with your real credentials from the Battle.net Developer Portal.

## Step 3: Verify User Secrets

List all configured secrets:
```powershell
dotnet user-secrets list
```

Expected output:
```
BlizzardApi:ClientId = your_client_id_here
BlizzardApi:ClientSecret = your_client_secret_here
```

## Step 4: Clear User Secrets (If Needed)

To remove all secrets:
```powershell
dotnet user-secrets clear
```

To remove a specific secret:
```powershell
dotnet user-secrets remove "BlizzardApi:ClientId"
```

## How User Secrets Work

### Storage Location
User Secrets are stored outside the project directory in:
```
Windows: %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json
macOS/Linux: ~/.microsoft/usersecrets/<user_secrets_id>/secrets.json
```

### Configuration Priority
.NET configuration system follows this priority (highest to lowest):
1. **Command-line arguments**
2. **Environment variables**
3. **User Secrets** (Development only)
4. **appsettings.{Environment}.json**
5. **appsettings.json**

### Security Benefits
- ✅ Credentials never stored in source control
- ✅ Per-developer configuration
- ✅ Works automatically in Development environment
- ✅ Easy to rotate credentials

## Production Deployment

User Secrets are **only for local development**. For production deployments, use:

### AWS Secrets Manager (Recommended for Production)
```csharp
// In Program.cs (production)
builder.Configuration.AddSecretsManager(configurator: options =>
{
    options.SecretFilter = secret => secret.Name.StartsWith("WarcraftArmory");
});
```

### Environment Variables
```powershell
# PowerShell
$env:BlizzardApi__ClientId = "your_client_id"
$env:BlizzardApi__ClientSecret = "your_client_secret"

# Bash
export BlizzardApi__ClientId="your_client_id"
export BlizzardApi__ClientSecret="your_client_secret"
```

Note: Use double underscores `__` for nested configuration keys in environment variables.

### Docker Compose
```yaml
services:
  backend:
    environment:
      - BlizzardApi__ClientId=${BLIZZARD_CLIENT_ID}
      - BlizzardApi__ClientSecret=${BLIZZARD_CLIENT_SECRET}
```

Then create a `.env` file (not committed to Git):
```env
BLIZZARD_CLIENT_ID=your_client_id
BLIZZARD_CLIENT_SECRET=your_client_secret
```

## Getting Blizzard API Credentials

1. Go to [Battle.net Developer Portal](https://develop.battle.net/)
2. Sign in with your Battle.net account (2FA required)
3. Click **Create Client**
4. Fill in the application details:
   - **Client Name**: Warcraft Armory (or your preferred name)
   - **Redirect URIs**: Not needed for Game Data API
5. Save and copy your **Client ID** and **Client Secret**
6. Accept the API Terms of Use

## Troubleshooting

### Error: "BlizzardApi:ClientId is not configured"
**Solution**: Make sure you've set the User Secrets correctly using the commands above.

### Error: "User secrets are not initialized"
**Solution**: Run `dotnet user-secrets init` in the WebApi project directory.

### Secrets not loading
**Solution**: Ensure you're running in Development environment:
```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
```

### Need to check current secrets
```powershell
cd backend\src\WarcraftArmory.WebApi
dotnet user-secrets list
```

## Additional Resources

- [Blizzard API Documentation](https://develop.battle.net/documentation)
- [Safe Storage of App Secrets in .NET](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Configuration in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
