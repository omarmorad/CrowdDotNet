# Crowdfunding API - Testing Collection

This directory contains comprehensive testing resources for the Crowdfunding API.

## 📁 Files Included

- **`Crowdfunding-API-Collection.json`** - Complete Postman collection with all endpoints
- **`Crowdfunding-API-Environment.json`** - Postman environment variables

## 🚀 Quick Start with Postman

### 1. Import Collection

1. Open Postman
2. Click **Import** button
3. Select `Crowdfunding-API-Collection.json`
4. Click **Import**

### 2. Import Environment

1. Click the **Environment** tab (gear icon)
2. Click **Import**
3. Select `Crowdfunding-API-Environment.json`
4. Click **Import**
5. Select the **Crowdfunding API Environment** from the dropdown

### 3. Update Base URL (if needed)

- In the environment, update `baseUrl` to match your API:
  - `https://localhost:7001` (default HTTPS)
  - `http://localhost:5001` (HTTP)

## 🧪 Testing Workflow

### Step 1: Authentication

1. **Register User** - Creates a new user and automatically saves the auth token
2. **Login User** - Alternative login with existing credentials

### Step 2: Campaign Management

1. **Get All Campaigns** - View existing campaigns
2. **Create Campaign** - Create a new campaign (saves campaign ID)
3. **Get Campaign by ID** - View specific campaign details
4. **Update Campaign** - Modify campaign information
5. **Create Pledge** - Make a pledge to the campaign

### Step 3: Pledge Management

1. **Get My Pledges** - View user's pledges
2. **Update Pledge** - Modify pledge amount
3. **Cancel Pledge** - Remove a pledge

### Step 4: Admin Functions (requires Admin role)

1. **Get Platform Analytics** - View platform statistics
2. **Approve/Reject Campaigns** - Moderate campaigns
3. **Manage Users** - View and ban users

## 🔑 Authentication Notes

- The collection automatically saves JWT tokens from login/register responses
- Tokens are stored in environment variables and used in subsequent requests
- For admin endpoints, you'll need to login with an admin account first

## 📊 Available Test Data

The API includes seeded data:

- **Admin User**: `admin@crowdfunding.com`
- **Campaign Owner**: `creator@example.com`
- **Regular User**: `backer@example.com`
- **Sample Campaign**: "Revolutionary Smart Watch"

## 🛠️ Environment Variables

The collection uses these environment variables:

- `baseUrl` - API base URL
- `authToken` - JWT authentication token
- `refreshToken` - JWT refresh token
- `adminToken` - Admin JWT token
- `userId` - Current user ID
- `campaignId` - Created campaign ID
- `pledgeId` - Created pledge ID

## 🔧 Alternative Testing Methods

### PowerShell Script

```powershell
cd CrowdfundingAPI
.\scripts\test-endpoints.ps1
```

### Bash Script (Linux/Mac)

```bash
cd CrowdfundingAPI
chmod +x scripts/test-endpoints.sh
./scripts/test-endpoints.sh
```

### cURL Examples

#### Register User

```bash
curl -k -X POST https://localhost:7001/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Test",
    "lastName": "User",
    "email": "test@example.com",
    "password": "Password123",
    "confirmPassword": "Password123",
    "bio": "Test user"
  }'
```

#### Get Campaigns

```bash
curl -k -X GET https://localhost:7001/api/v1/campaigns \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## 🎯 Testing Checklist

- [ ] Health check endpoint works
- [ ] User registration successful
- [ ] User login successful
- [ ] JWT token authentication works
- [ ] Campaign creation works
- [ ] Campaign retrieval works
- [ ] Pledge creation works
- [ ] Search and filtering works
- [ ] Pagination works
- [ ] Input validation works
- [ ] Error handling works
- [ ] Admin functions work (with admin token)

## 🐛 Troubleshooting

### Common Issues

1. **Connection Refused**

   - Ensure API is running (`dotnet run`)
   - Check the correct port in `baseUrl`

2. **SSL Certificate Errors**

   - Use `-k` flag with cURL
   - Enable "SSL certificate verification" off in Postman

3. **Authentication Errors**

   - Ensure token is saved in environment
   - Check token hasn't expired
   - Verify user has correct role for endpoint

4. **404 Errors**
   - Verify API is running
   - Check endpoint URLs match the API routes
   - Ensure base URL is correct

## 📈 Expected Response Formats

### Success Response

```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... },
  "errors": []
}
```

### Error Response

```json
{
  "success": false,
  "message": "Error message",
  "data": null,
  "errors": ["Detailed error 1", "Detailed error 2"]
}
```

Happy Testing! 🚀
