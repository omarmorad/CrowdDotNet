# Crowdfunding API Test Script (PowerShell)
# Usage: .\test-endpoints.ps1

$BaseUrl = "http://localhost:5001"
$ApiUrl = "$BaseUrl/api/v1"

Write-Host "🚀 Testing Crowdfunding API Endpoints" -ForegroundColor Green
Write-Host "Base URL: $BaseUrl" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor White

# Function to test endpoints
function Test-Endpoint {
    param(
        [string]$Method,
        [string]$Url,
        [string]$Body = $null,
        [hashtable]$Headers = @{},
        [string]$Description
    )
    
    Write-Host "`n🧪 Testing: $Description" -ForegroundColor Blue
    Write-Host "Method: $Method" -ForegroundColor Gray
    Write-Host "URL: $Url" -ForegroundColor Gray
    
    try {
        $requestParams = @{
            Uri = $Url
            Method = $Method
            SkipCertificateCheck = $true
            Headers = $Headers
        }
        
        if ($Body) {
            $requestParams.Body = $Body
            $requestParams.ContentType = "application/json"
        }
        
        $response = Invoke-RestMethod @requestParams
        Write-Host "✅ Success" -ForegroundColor Green
        
        if ($response) {
            $jsonResponse = $response | ConvertTo-Json -Depth 10
            Write-Host "Response: $jsonResponse" -ForegroundColor White
        }
        
        return $response
    }
    catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        if ($statusCode -ge 400 -and $statusCode -lt 500) {
            Write-Host "⚠️  Client Error ($statusCode)" -ForegroundColor Yellow
        } else {
            Write-Host "❌ Error ($statusCode)" -ForegroundColor Red
        }
        
        if ($_.Exception.Response) {
            $errorStream = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($errorStream)
            $errorContent = $reader.ReadToEnd()
            Write-Host "Error: $errorContent" -ForegroundColor Red
        } else {
            Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        }
        return $null
    }
}

# 1. Health Check
Test-Endpoint -Method "GET" -Url "$BaseUrl/health" -Description "Health Check"

# 2. Register User
$registerData = @{
    firstName = "Test"
    lastName = "User"
    email = "test$(Get-Random)@example.com"
    password = "Password123"
    confirmPassword = "Password123"
    bio = "Test user for API testing"
} | ConvertTo-Json

Write-Host "`n👤 Registering new user..." -ForegroundColor Yellow
$registerResponse = Test-Endpoint -Method "POST" -Url "$ApiUrl/auth/register" -Body $registerData -Description "User Registration"

$authToken = $null
if ($registerResponse -and $registerResponse.success -and $registerResponse.data.token) {
    $authToken = $registerResponse.data.token
    Write-Host "✅ Registration successful, token obtained" -ForegroundColor Green
} else {
    Write-Host "⚠️  Registration failed, trying login with seeded user" -ForegroundColor Yellow
    
    # Try login with seeded user
    $loginData = @{
        email = "creator@example.com"
        password = "Password123"
    } | ConvertTo-Json
    
    $loginResponse = Test-Endpoint -Method "POST" -Url "$ApiUrl/auth/login" -Body $loginData -Description "User Login"
    
    if ($loginResponse -and $loginResponse.success -and $loginResponse.data.token) {
        $authToken = $loginResponse.data.token
        Write-Host "✅ Login successful with seeded user" -ForegroundColor Green
    } else {
        Write-Host "❌ Could not obtain authentication token" -ForegroundColor Red
    }
}

# 3. Test authenticated endpoints
if ($authToken) {
    $authHeaders = @{
        "Authorization" = "Bearer $authToken"
    }
    
    # Get Campaigns
    Test-Endpoint -Method "GET" -Url "$ApiUrl/campaigns" -Headers $authHeaders -Description "Get All Campaigns"
    
    # Create Campaign
    $campaignData = @{
        title = "Test Campaign via PowerShell"
        description = "This is a test campaign created via PowerShell script to verify the API functionality. It includes all required fields and demonstrates the campaign creation process."
        shortDescription = "Test campaign for API verification"
        goalAmount = 50000
        startDate = "2024-01-01T00:00:00Z"
        endDate = "2024-12-31T23:59:59Z"
        coverImageUrl = "https://example.com/test-campaign.jpg"
        videoUrl = "https://example.com/test-video.mp4"
        category = "Technology"
        rewardTiers = @(
            @{
                title = "Early Bird"
                description = "Get early access to the product"
                minimumAmount = 25
                maxBackers = 100
                estimatedDelivery = "2024-06-01T00:00:00Z"
            }
        )
    } | ConvertTo-Json -Depth 10
    
    Write-Host "`n🏗️  Creating test campaign..." -ForegroundColor Yellow
    $campaignResponse = Test-Endpoint -Method "POST" -Url "$ApiUrl/campaigns" -Body $campaignData -Headers $authHeaders -Description "Create Campaign"
    
    if ($campaignResponse -and $campaignResponse.success -and $campaignResponse.data.id) {
        $campaignId = $campaignResponse.data.id
        Write-Host "✅ Campaign created with ID: $campaignId" -ForegroundColor Green
        
        # Get specific campaign
        Test-Endpoint -Method "GET" -Url "$ApiUrl/campaigns/$campaignId" -Description "Get Campaign by ID"
        
        # Create pledge
        $pledgeData = @{
            amount = 100
            rewardTierId = $null
        } | ConvertTo-Json
        
        Test-Endpoint -Method "POST" -Url "$ApiUrl/campaigns/$campaignId/pledge" -Body $pledgeData -Headers $authHeaders -Description "Create Pledge"
        
        # Get user pledges
        Test-Endpoint -Method "GET" -Url "$ApiUrl/pledges/my-pledges" -Headers $authHeaders -Description "Get My Pledges"
    }
} else {
    Write-Host "❌ Skipping authenticated endpoints - no auth token available" -ForegroundColor Red
}

Write-Host "`n🎉 API Testing Complete!" -ForegroundColor Green
Write-Host "📖 For interactive testing, visit: $BaseUrl" -ForegroundColor Cyan
Write-Host "📋 Import the Postman collection from: ./postman/Crowdfunding-API-Collection.json" -ForegroundColor Cyan