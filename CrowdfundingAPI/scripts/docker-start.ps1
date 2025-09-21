# Docker startup script for Crowdfunding API

Write-Host "🐳 Starting Crowdfunding API with Docker" -ForegroundColor Green

# Check if Docker is running
try {
    docker version | Out-Null
    Write-Host "✅ Docker is running" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker is not running. Please start Docker Desktop first." -ForegroundColor Red
    exit 1
}

# Navigate to project root
Set-Location ".."

# Stop any existing containers
Write-Host "🛑 Stopping existing containers..." -ForegroundColor Yellow
docker-compose down

# Build and start containers
Write-Host "🔨 Building and starting containers..." -ForegroundColor Yellow
docker-compose up --build -d

# Wait for services to be ready
Write-Host "⏳ Waiting for services to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Check container status
Write-Host "📊 Container Status:" -ForegroundColor Cyan
docker-compose ps

Write-Host ""
Write-Host "🎉 Crowdfunding API is now running!" -ForegroundColor Green
Write-Host "📖 Swagger UI: http://localhost:8080" -ForegroundColor Cyan
Write-Host "🔗 API Base URL: http://localhost:8080/api/v1" -ForegroundColor Cyan
Write-Host "🗄️  SQL Server: localhost:1433" -ForegroundColor Cyan
Write-Host ""
Write-Host "To stop the containers, run: docker-compose down" -ForegroundColor Gray