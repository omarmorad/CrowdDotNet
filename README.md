# Crowdfunding API

A comprehensive, production-ready .NET 8 Web API for a crowdfunding platform built with Clean Architecture principles.

## 🚀 Features

### Core Functionality

- **User Management**: Registration, authentication, and role-based authorization
- **Campaign Management**: Create, update, and manage crowdfunding campaigns
- **Pledge System**: Support campaigns with flexible reward tiers
- **Payment Processing**: Simulated payment processing with success/failure scenarios
- **Admin Panel**: Administrative controls for platform management

### Technical Features

- **Clean Architecture**: Separation of concerns with Domain, Application, Infrastructure, and API layers
- **CQRS Pattern**: Command Query Responsibility Segregation using MediatR
- **JWT Authentication**: Secure token-based authentication with refresh tokens
- **Entity Framework Core**: Code-first approach with SQL Server
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Comprehensive input validation
- **Serilog**: Structured logging to console and files
- **Swagger/OpenAPI**: Interactive API documentation
- **Docker Support**: Containerized deployment
- **Unit & Integration Tests**: Comprehensive test coverage

## 🏗️ Architecture

```
CrowdfundingAPI/
├── src/
│   ├── CrowdfundingAPI.API/           # Controllers, Middleware, Program.cs
│   ├── CrowdfundingAPI.Application/   # Services, DTOs, Commands/Queries, Handlers
│   ├── CrowdfundingAPI.Domain/        # Entities, Interfaces, Enums
│   └── CrowdfundingAPI.Infrastructure/ # DbContext, Repositories, External Services
├── tests/
│   ├── CrowdfundingAPI.UnitTests/
│   └── CrowdfundingAPI.IntegrationTests/
├── docker-compose.yml
├── Dockerfile
└── README.md
```

## 🛠️ Technology Stack

- **.NET 8** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Primary database
- **JWT Bearer Authentication** - Secure authentication
- **AutoMapper** - Object mapping
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **Serilog** - Structured logging
- **xUnit** - Unit testing framework
- **Moq** - Mocking framework
- **Docker** - Containerization
- **Swagger/OpenAPI** - API documentation

## 🚦 Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (or Docker for containerized setup)
- Docker Desktop (optional, for containerized deployment)

### Local Development Setup

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd CrowdfundingAPI
   ```

2. **Update connection string**
   Edit `src/CrowdfundingAPI.API/appsettings.json`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=CrowdfundingDB;Trusted_Connection=true;TrustServerCertificate=true;"
     }
   }
   ```

3. **Restore packages and build**

   ```bash
   dotnet restore
   dotnet build
   ```

4. **Run database migrations**

   ```bash
   cd src/CrowdfundingAPI.API
   dotnet ef database update
   ```

5. **Run the application**

   ```bash
   dotnet run
   ```

6. **Access Swagger UI**
   Navigate to `https://localhost:7001` or `http://localhost:5001`

### Docker Deployment

1. **Using Docker Compose (Recommended)**

   ```bash
   docker-compose up -d
   ```

   This will start both the API and SQL Server containers.

2. **Access the API**
   - API: `http://localhost:8080`
   - Swagger UI: `http://localhost:8080`

## 📚 API Endpoints

### Authentication

- `POST /api/v1/auth/register` - Register new user
- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/refresh` - Refresh JWT token
- `POST /api/v1/auth/forgot-password` - Request password reset
- `POST /api/v1/auth/reset-password` - Reset password
- `POST /api/v1/auth/verify-email` - Verify email address

### Campaigns

- `GET /api/v1/campaigns` - Get campaigns (with filtering and pagination)
- `POST /api/v1/campaigns` - Create new campaign
- `GET /api/v1/campaigns/{id}` - Get campaign by ID
- `PUT /api/v1/campaigns/{id}` - Update campaign
- `DELETE /api/v1/campaigns/{id}` - Cancel campaign
- `POST /api/v1/campaigns/{id}/pledge` - Create pledge
- `GET /api/v1/campaigns/{id}/pledges` - Get campaign pledges
- `POST /api/v1/campaigns/{id}/updates` - Post campaign update
- `GET /api/v1/campaigns/{id}/analytics` - Get campaign analytics

### Pledges

- `GET /api/v1/pledges/my-pledges` - Get user's pledges
- `GET /api/v1/pledges/{id}` - Get pledge by ID
- `PUT /api/v1/pledges/{id}` - Update pledge
- `DELETE /api/v1/pledges/{id}` - Cancel pledge

### Admin

- `GET /api/v1/admin/campaigns/pending` - Get pending campaigns
- `PUT /api/v1/admin/campaigns/{id}/approve` - Approve campaign
- `PUT /api/v1/admin/campaigns/{id}/reject` - Reject campaign
- `GET /api/v1/admin/users` - Get all users
- `PUT /api/v1/admin/users/{id}/ban` - Ban user
- `GET /api/v1/admin/analytics/overview` - Platform analytics

## 🧪 Testing

### Run Unit Tests

```bash
dotnet test tests/CrowdfundingAPI.UnitTests/
```

### Run Integration Tests

```bash
dotnet test tests/CrowdfundingAPI.IntegrationTests/
```

### Run All Tests with Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 🔧 Configuration

### JWT Settings

Configure JWT authentication in `appsettings.json`:

```json
{
  "JwtSettings": {
    "Secret": "YourSecretKeyHere",
    "Issuer": "CrowdfundingAPI",
    "Audience": "CrowdfundingAPI",
    "ExpiryInMinutes": 60
  }
}
```

### Logging Configuration

Serilog is configured to log to both console and files:

- Console output for development
- Rolling file logs in `/logs` directory
- Structured JSON logging for production

### Database Seeding

The application includes seed data:

- Admin user: `admin@crowdfunding.com`
- Campaign owner: `creator@example.com`
- Regular user: `backer@example.com`
- Sample campaign with reward tiers

## 🔒 Security Features

- **JWT Authentication** with refresh tokens
- **Role-based Authorization** (User, CampaignOwner, Admin)
- **Input Validation** using FluentValidation
- **SQL Injection Prevention** through Entity Framework
- **CORS Configuration** for frontend integration
- **HTTPS Enforcement** in production
- **Password Hashing** using ASP.NET Identity

## 📊 Business Logic

### Campaign Funding Logic

- Auto-calculate funding percentage
- Mark campaigns as "Funded" when goal is reached
- Auto-close campaigns past deadline
- Handle over-funding scenarios

### Pledge Processing

- Validate pledge amounts against reward tiers
- Check campaign is still active before accepting pledges
- Simulate payment processing with random success/failure
- Update campaign current amount on successful pledges

### Search & Filtering

- Search by title, description, creator name
- Filter by category, status, funding percentage
- Sort by creation date, deadline, funding amount
- Pagination with skip/take parameters

## 🚀 Deployment

### Production Considerations

1. **Environment Variables**: Use environment-specific configurations
2. **Database**: Use managed SQL Server instance
3. **Logging**: Configure centralized logging (e.g., Application Insights)
4. **Monitoring**: Implement health checks and monitoring
5. **Security**: Use HTTPS, secure JWT secrets, implement rate limiting
6. **Scaling**: Consider load balancing and database optimization

### Health Checks

The API includes health check endpoints:

- `/health` - Basic health check
- Database connectivity check included

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

For support and questions:

- Create an issue in the repository
- Contact: support@crowdfunding.com

---

**Built with ❤️ using .NET 8 and Clean Architecture principles**
