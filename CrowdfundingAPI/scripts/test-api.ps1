# Test script for Crowdfunding API

Write-Host "🧪 Testing Crowdfunding API" -ForegroundColor Green

$baseUrl = "http://localhost:5001"
$apiUrl = "$baseUrl/api/v1"

# Test if API is running
try {
    Write-Host "📡 Testing API health..." -ForegroundColor Yellow
    $healthResponse = Invoke-RestMethod -Uri "$baseUrl/health" -Method Get -SkipCertificateCheck
    Write-Host "✅ API Health Check: OK" -ForegroundColor Green
} catch {
    Write-Host "❌ API Health Check failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "💡 Make sure the API is running with 'dotnet run'" -ForegroundColor Yellow
    exit 1
}

# Test Swagger endpoint
try {
    Write-Host "📖 Testing Swagger documentation..." -ForegroundColor Yellow
    $swaggerResponse = Invoke-WebRequest -Uri "$baseUrl/swagger/v1/swagger.json" -Method Get -SkipCertificateCheck
    if ($swaggerResponse.StatusCode -eq 200) {
        Write-Host "✅ Swagger Documentation: Available" -ForegroundColor Green
    }
} catch {
    Write-Host "⚠️  Swagger endpoint test failed: $($_.Exception.Message)" -ForegroundColor Yellow
}

# Test user registration
try {
    Write-Host "👤 Testing user registration..." -ForegroundColor Yellow
    
    $registerData = @{
        firstName = "Test"
        lastName = "User"
        email = "test$(Get-Random)@example.com"
        password = "TestPassword123"
        confirmPassword = "TestPassword123"
        bio = "Test user for API testing"
    } | ConvertTo-Json

    $headers = @{
        "Content-Type" = "application/json"
    }

    $registerResponse = Invoke-RestMethod -Uri "$apiUrl/auth/register" -Method Post -Body $registerData -Headers $headers -SkipCertificateCheck
    
    if ($registerResponse.success) {
        Write-Host "✅ User Registration: Success" -ForegroundColor Green
        Write-Host "🔑 JWT Token received: $($registerResponse.data.token.Substring(0, 20))..." -ForegroundColor Cyan
        
        # Test getting campaigns with the token
        $authHeaders = @{
            "Authorization" = "Bearer $($registerResponse.data.token)"
            "Content-Type" = "application/json"
        }
        
        Write-Host "📋 Testing campaigns endpoint..." -ForegroundColor Yellow
        $campaignsResponse = Invoke-RestMethod -Uri "$apiUrl/campaigns" -Method Get -Headers $authHeaders -SkipCertificateCheck
        
        if ($campaignsResponse.success) {
            Write-Host "✅ Campaigns Endpoint: Success" -ForegroundColor Green
            Write-Host "📊 Total campaigns: $($campaignsResponse.data.totalCount)" -ForegroundColor Cyan
        }
    }
} catch {
    Write-Host "❌ API Test failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $errorResponse = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($errorResponse)
        $errorContent = $reader.ReadToEnd()
        Write-Host "Error details: $errorContent" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "🎉 API Testing Complete!" -ForegroundColor Green
Write-Host "🌐 Swagger UI: $baseUrl" -ForegroundColor Cyan
Write-Host "🔗 API Base URL: $apiUrl" -ForegroundColor Cyan