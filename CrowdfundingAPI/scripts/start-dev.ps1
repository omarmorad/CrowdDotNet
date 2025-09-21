# Development startup script for Crowdfunding API

Write-Host "🚀 Starting Crowdfunding API Development Environment" -ForegroundColor Green

# Check if .NET 8 is installed
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET Version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ .NET 8 SDK is not installed. Please install it first." -ForegroundColor Red
    exit 1
}

# Navigate to API project directory
Set-Location "src/CrowdfundingAPI.API"

# Restore packages
Write-Host "📦 Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore

# Build the project
Write-Host "🔨 Building the project..." -ForegroundColor Yellow
dotnet build

# Check if build was successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build successful!" -ForegroundColor Green
    
    # Start the application
    Write-Host "🌐 Starting the API server..." -ForegroundColor Yellow
    Write-Host "📖 Swagger UI will be available at: https://localhost:7001" -ForegroundColor Cyan
    Write-Host "🔗 API Base URL: https://localhost:7001/api/v1" -ForegroundColor Cyan
    Write-Host "" 
    Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Gray
    
    dotnet run
} else {
    Write-Host "❌ Build failed. Please check the errors above." -ForegroundColor Red
    exit 1
}