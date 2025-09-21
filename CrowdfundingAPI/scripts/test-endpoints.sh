#!/bin/bash

# Crowdfunding API Test Script
# Usage: ./test-endpoints.sh

BASE_URL="http://localhost:5001"
API_URL="$BASE_URL/api/v1"

echo "🚀 Testing Crowdfunding API Endpoints"
echo "Base URL: $BASE_URL"
echo "=================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to make HTTP requests and show results
test_endpoint() {
    local method=$1
    local url=$2
    local data=$3
    local headers=$4
    local description=$5
    
    echo -e "\n${BLUE}Testing: $description${NC}"
    echo "Method: $method"
    echo "URL: $url"
    
    if [ -n "$data" ]; then
        if [ -n "$headers" ]; then
            response=$(curl -k -s -w "\n%{http_code}" -X $method "$url" -H "Content-Type: application/json" -H "$headers" -d "$data")
        else
            response=$(curl -k -s -w "\n%{http_code}" -X $method "$url" -H "Content-Type: application/json" -d "$data")
        fi
    else
        if [ -n "$headers" ]; then
            response=$(curl -k -s -w "\n%{http_code}" -X $method "$url" -H "$headers")
        else
            response=$(curl -k -s -w "\n%{http_code}" -X $method "$url")
        fi
    fi
    
    # Extract HTTP status code (last line)
    http_code=$(echo "$response" | tail -n1)
    # Extract response body (all but last line)
    response_body=$(echo "$response" | sed '$d')
    
    if [[ $http_code -ge 200 && $http_code -lt 300 ]]; then
        echo -e "${GREEN}✅ Success ($http_code)${NC}"
    elif [[ $http_code -ge 400 && $http_code -lt 500 ]]; then
        echo -e "${YELLOW}⚠️  Client Error ($http_code)${NC}"
    else
        echo -e "${RED}❌ Error ($http_code)${NC}"
    fi
    
    echo "Response: $response_body" | jq '.' 2>/dev/null || echo "Response: $response_body"
}

# 1. Health Check
test_endpoint "GET" "$BASE_URL/health" "" "" "Health Check"

# 2. Register User
REGISTER_DATA='{
  "firstName": "Test",
  "lastName": "User",
  "email": "test'$(date +%s)'@example.com",
  "password": "Password123",
  "confirmPassword": "Password123",
  "bio": "Test user for API testing"
}'

echo -e "\n${YELLOW}Registering new user...${NC}"
register_response=$(curl -k -s -X POST "$API_URL/auth/register" -H "Content-Type: application/json" -d "$REGISTER_DATA")
echo "Register Response: $register_response" | jq '.' 2>/dev/null || echo "Register Response: $register_response"

# Extract token from registration response
AUTH_TOKEN=$(echo "$register_response" | jq -r '.data.token // empty' 2>/dev/null)

if [ -n "$AUTH_TOKEN" ] && [ "$AUTH_TOKEN" != "null" ]; then
    echo -e "${GREEN}✅ Registration successful, token obtained${NC}"
    AUTH_HEADER="Authorization: Bearer $AUTH_TOKEN"
else
    echo -e "${YELLOW}⚠️  Registration failed or no token received, trying login with existing user${NC}"
    
    # Try login with seeded user
    LOGIN_DATA='{
      "email": "creator@example.com",
      "password": "Password123"
    }'
    
    login_response=$(curl -k -s -X POST "$API_URL/auth/login" -H "Content-Type: application/json" -d "$LOGIN_DATA")
    AUTH_TOKEN=$(echo "$login_response" | jq -r '.data.token // empty' 2>/dev/null)
    
    if [ -n "$AUTH_TOKEN" ] && [ "$AUTH_TOKEN" != "null" ]; then
        echo -e "${GREEN}✅ Login successful with seeded user${NC}"
        AUTH_HEADER="Authorization: Bearer $AUTH_TOKEN"
    else
        echo -e "${RED}❌ Could not obtain authentication token${NC}"
        AUTH_HEADER=""
    fi
fi

# 3. Test authenticated endpoints
if [ -n "$AUTH_HEADER" ]; then
    # Get Campaigns
    test_endpoint "GET" "$API_URL/campaigns" "" "$AUTH_HEADER" "Get All Campaigns"
    
    # Create Campaign
    CAMPAIGN_DATA='{
      "title": "Test Campaign via Script",
      "description": "This is a test campaign created via the test script to verify the API functionality. It includes all required fields and demonstrates the campaign creation process.",
      "shortDescription": "Test campaign for API verification",
      "goalAmount": 50000,
      "startDate": "2024-01-01T00:00:00Z",
      "endDate": "2024-12-31T23:59:59Z",
      "coverImageUrl": "https://example.com/test-campaign.jpg",
      "videoUrl": "https://example.com/test-video.mp4",
      "category": "Technology",
      "rewardTiers": [
        {
          "title": "Early Bird",
          "description": "Get early access to the product",
          "minimumAmount": 25,
          "maxBackers": 100,
          "estimatedDelivery": "2024-06-01T00:00:00Z"
        }
      ]
    }'
    
    echo -e "\n${YELLOW}Creating test campaign...${NC}"
    campaign_response=$(curl -k -s -X POST "$API_URL/campaigns" -H "Content-Type: application/json" -H "$AUTH_HEADER" -d "$CAMPAIGN_DATA")
    echo "Campaign Response: $campaign_response" | jq '.' 2>/dev/null || echo "Campaign Response: $campaign_response"
    
    # Extract campaign ID
    CAMPAIGN_ID=$(echo "$campaign_response" | jq -r '.data.id // empty' 2>/dev/null)
    
    if [ -n "$CAMPAIGN_ID" ] && [ "$CAMPAIGN_ID" != "null" ]; then
        echo -e "${GREEN}✅ Campaign created with ID: $CAMPAIGN_ID${NC}"
        
        # Get specific campaign
        test_endpoint "GET" "$API_URL/campaigns/$CAMPAIGN_ID" "" "" "Get Campaign by ID"
        
        # Create pledge
        PLEDGE_DATA='{
          "amount": 100,
          "rewardTierId": null
        }'
        
        test_endpoint "POST" "$API_URL/campaigns/$CAMPAIGN_ID/pledge" "$PLEDGE_DATA" "$AUTH_HEADER" "Create Pledge"
        
        # Get user pledges
        test_endpoint "GET" "$API_URL/pledges/my-pledges" "" "$AUTH_HEADER" "Get My Pledges"
    fi
    
else
    echo -e "${RED}❌ Skipping authenticated endpoints - no auth token available${NC}"
fi

echo -e "\n${GREEN}🎉 API Testing Complete!${NC}"
echo -e "${BLUE}📖 For interactive testing, visit: $BASE_URL${NC}"