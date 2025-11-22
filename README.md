# World of Warcraft Armory

A modern, scalable web application for searching and displaying World of Warcraft game data.

## Technology Stack

### Backend
- .NET 10 with C# 14
- ASP.NET Core Web API
- Clean Architecture
- Redis 8 for caching
- MediatR for CQRS
- Refit for Blizzard API integration

### Frontend
- Angular 21
- TypeScript 5.7
- Angular Material
- Signals for state management
- Jest for testing

### Infrastructure
- Docker & Docker Compose
- Redis 8
- Nginx

## Prerequisites

- .NET 10 SDK
- Node.js 22 LTS
- Docker Desktop
- Battle.net Developer Account with API credentials

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd warcraft-armory
```

### 2. Configure Blizzard API Credentials

#### For Local Development (User Secrets)

```bash
cd backend/src/WarcraftArmory.WebApi
dotnet user-secrets init
dotnet user-secrets set "BlizzardApi:ClientId" "your-client-id"
dotnet user-secrets set "BlizzardApi:ClientSecret" "your-client-secret"
```

#### For Docker (Environment Variables)

Create a `.env` file in the root directory:

```env
BLIZZARD_CLIENT_ID=your_client_id_here
BLIZZARD_CLIENT_SECRET=your_client_secret_here
REDIS_PASSWORD=your_redis_password
```

### 3. Run with Docker Compose

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

### 4. Run Locally (Development)

#### Backend

```bash
cd backend/src/WarcraftArmory.WebApi
dotnet run
```

API will be available at: `https://localhost:5001`
Swagger documentation: `https://localhost:5001/swagger`

#### Frontend

```bash
cd frontend
npm install
npm start
```

Application will be available at: `http://localhost:4200`

## Project Structure

```
warcraft-armory/
├── backend/
│   ├── src/
│   │   ├── WarcraftArmory.Domain/
│   │   ├── WarcraftArmory.Application/
│   │   ├── WarcraftArmory.Infrastructure/
│   │   └── WarcraftArmory.WebApi/
│   └── tests/
│       ├── WarcraftArmory.Domain.Tests/
│       ├── WarcraftArmory.Application.Tests/
│       ├── WarcraftArmory.Infrastructure.Tests/
│       └── WarcraftArmory.WebApi.Tests/
├── frontend/
│   └── src/
│       └── app/
│           ├── core/
│           ├── shared/
│           ├── features/
│           └── layout/
├── docker-compose.yml
└── plan.md
```

## Testing

### Backend Tests

```bash
cd backend
dotnet test
```

### Frontend Tests

```bash
cd frontend
npm test
```

### E2E Tests

```bash
cd frontend
npm run e2e
```

## Development Workflow

1. Create a feature branch from `develop`
2. Implement changes following TDD approach
3. Write tests for new functionality
4. Run tests locally
5. Create pull request to `develop`
6. CI/CD pipeline runs automated tests
7. After approval, merge to `develop`
8. Release branch merges to `main` for production

## API Documentation

Swagger UI is available at `/swagger` when running the backend API.

Key endpoints:
- `GET /api/characters/{region}/{realm}/{name}` - Get character details
- `GET /api/items/{region}/{id}` - Get item details
- `GET /api/guilds/{region}/{realm}/{name}` - Get guild details
- `GET /api/realms/{region}` - List all realms

## Architecture

This project follows Clean Architecture principles:

- **Domain Layer**: Core business entities and rules
- **Application Layer**: Use cases, DTOs, business logic
- **Infrastructure Layer**: External services (Blizzard API, Redis, etc.)
- **Presentation Layer**: API controllers and middleware

## Caching Strategy

Three-tier caching:
1. **Client-side**: Service Worker and local storage
2. **Server-side**: IMemoryCache for hot data
3. **Distributed**: Redis for shared cache across instances

## Contributing

1. Follow the existing code structure and patterns
2. Write tests for all new features
3. Follow C# and TypeScript coding conventions
4. Update documentation as needed

## License

[Your License Here]

## Support

For issues and questions, please create an issue in the repository.
