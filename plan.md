# World of Warcraft Game Data Search Website - Project Plan

## Executive Summary

A scalable, modern **read-only** web application for searching and displaying World of Warcraft game data using Angular frontend and .NET 10 backend with clean architecture, containerization, multi-tier caching, and comprehensive testing.

**Important**: This is a **fan-made, read-only website** that consumes the Blizzard Game Data API. It does not allow any data modifications or user-generated content. All data is fetched from Blizzard's official API and cached for performance. The application provides a user-friendly interface for browsing and searching WoW game data without the ability to modify characters, items, or any game content.

## Technology Stack

### Frontend
- **Angular 21** (Latest - November 2025 with enhanced standalone, Signals, and modern features)
- **TypeScript 5.7**
- **Angular Material 21** or **PrimeNG 17+**
- **RxJS 7.8+** for reactive programming
- **NgRx 19+** with Signal Store for state management
- **Service Worker** for PWA capabilities
- **Node.js 22 LTS** (build environment)

### Backend
- **.NET 10** (Latest - released November 2025) with **C# 14**
- **ASP.NET Core 10 Web API**
- **Clean Architecture** pattern
- **MediatR 12+** (CQRS implementation (Query handlers only for read-only operations))
- **AutoMapper 13+** (object mapping)
- **FluentValidation 11+** (input validation for search parameters)
- **Refit 7+** or **RestSharp 112+** (HTTP client for Blizzard API)
- **Polly 8+** (resilience and retry policies for API calls)

### Infrastructure
- **Docker 27+** & **Docker Compose V2**
- **Redis 8** (latest stable with improved performance)
- **Nginx 1.27+** (frontend serving)
- **Serilog 4+** (structured logging)

### Testing
- **xUnit 2.9+**, **FluentAssertions 7+**, **NSubstitute 5+** or **Moq 4.20+** (backend)
- **Jest 29+** with **@angular-builders/jest** (Angular unit tests - faster than Karma)
- **Playwright 1.48+** (E2E - recommended over Cypress)
- **NetArchTest.Rules 1.3+** (architecture tests)
- **Testcontainers 3.10+** (integration testing)

## Blizzard API Overview

### Authentication
- **OAuth 2.0 Client Credentials Flow**
- Requires Battle.net Developer Account with 2FA
- Client ID and Client Secret from Developer Portal
- Token endpoint: `https://oauth.battle.net/token`
- Tokens expire and must be refreshed

### Rate Limiting
- **36,000 requests per hour**
- **100 requests per second**
- Exceeding limits results in HTTP 429 errors
- Requires request throttling and queuing implementation

### API Structure
- Base URL: `{region}.api.blizzard.com/{API path}`
- Regions: US, EU, KR, TW, CN
- Namespaces: `static-{region}`, `dynamic-{region}`, `profile-{region}`
- **Read-only access**: Game Data API provides read-only access to WoW game data
- **No modifications allowed**: Cannot create, update, or delete any game data

### Key API Categories (26 Total)
1. **Achievement API** - Achievements, categories, media
2. **Auction House API** - Auction data per connected realm
3. **Azerite Essence API** - Azerite essences and media
4. **Connected Realm API** - Server connection information
5. **Covenant API** - Shadowlands covenant data
6. **Creature API** - NPCs, families, types
7. **Guild Crest API** - Guild emblems
8. **Heirloom API** - Heirloom items
9. **Item API** - Items, classes, sets, appearances, media
10. **Journal API** - Encounters, instances, raids
11. **Media API** - Icons, images, renders
12. **Mount API** - Mount collection
13. **Mythic Keystone API** - Dungeons, affixes, leaderboards
14. **Playable Class API** - Classes, specializations, talents
15. **Playable Race API** - Races and attributes
16. **Profession API** - Professions, skill tiers, recipes
17. **PvP Season API** - PvP seasons, leaderboards, rewards
18. **Quest API** - Quests, categories, areas
19. **Realm API** - Realm/server information
20. **Region API** - Regional data
21. **Spell API** - Spell data and media
22. **Talent API** - Talent trees and abilities
23. **Tech Talent API** - Tech talent trees
24. **Title API** - Character titles
25. **Toy API** - Toy collection
26. **WoW Token API** - Token price index

## Backend Architecture (Clean Architecture)

**Architecture Philosophy**: 
- **Read-Only Operations**: All entities are immutable using C# record types
- **CQRS Queries Only**: No Command handlers - only Query handlers for reading data
- **No Database for Game Data**: All WoW game data comes from Blizzard API and is cached
- **Cache-First Strategy**: Redis for distributed cache, IMemoryCache for hot data
- **HTTP GET Only**: Controllers expose only GET endpoints

### Project Structure

```
src/
├── WarcraftArmory.Domain/              # Enterprise Business Rules
│   ├── Entities/                       # Core domain entities
│   │   ├── Character.cs
│   │   ├── Item.cs
│   │   ├── Guild.cs
│   │   ├── Achievement.cs
│   │   ├── Realm.cs
│   │   ├── Mount.cs
│   │   ├── Pet.cs
│   │   └── Profession.cs
│   ├── Enums/                          # Domain enumerations
│   │   ├── Region.cs
│   │   ├── CharacterClass.cs
│   │   ├── CharacterRace.cs
│   │   └── ItemQuality.cs
│   ├── ValueObjects/                   # Value objects (immutable)
│   │   ├── CharacterName.cs
│   │   └── RealmSlug.cs
│   ├── Exceptions/                     # Domain-specific exceptions
│   │   ├── EntityNotFoundException.cs
│   │   └── InvalidEntityException.cs
│   └── Interfaces/                     # Domain service interfaces
│       └── IEntity.cs
│
├── WarcraftArmory.Application/         # Application Business Rules
│   ├── DTOs/                           # Data Transfer Objects
│   │   ├── Requests/
│   │   │   ├── GetCharacterRequest.cs
│   │   │   ├── SearchItemsRequest.cs
│   │   │   └── GetGuildRequest.cs
│   │   └── Responses/
│   │       ├── CharacterResponse.cs
│   │       ├── ItemResponse.cs
│   │       └── GuildResponse.cs
│   ├── Interfaces/                     # Application service interfaces
│   │   ├── IBlizzardApiService.cs
│   │   ├── ICacheService.cs
│   │   ├── ISearchService.cs
│   │   └── IRateLimiter.cs
│   ├── Services/                       # Application services
│   │   ├── CharacterService.cs
│   │   ├── ItemService.cs
│   │   └── GuildService.cs
│   ├── Mapping/                        # AutoMapper profiles
│   │   ├── CharacterMappingProfile.cs
│   │   ├── ItemMappingProfile.cs
│   │   └── GuildMappingProfile.cs
│   ├── Validation/                     # FluentValidation validators
│   │   ├── GetCharacterRequestValidator.cs
│   │   └── SearchItemsRequestValidator.cs
│   └── UseCases/                       # CQRS Queries (read-only operations)
│       ├── Characters/
│       │   └── Queries/
│       │       ├── GetCharacterQuery.cs
│       │       ├── GetCharacterQueryHandler.cs
│       │       └── SearchCharactersQuery.cs
│       ├── Items/
│       │   └── Queries/
│       │       ├── GetItemQuery.cs
│       │       ├── SearchItemsQuery.cs
│       │       └── SearchItemsQueryHandler.cs
│       ├── Guilds/
│       │   └── Queries/
│       │       ├── GetGuildQuery.cs
│       │       └── GetGuildQueryHandler.cs
│       ├── Realms/
│       │   └── Queries/
│       │       ├── GetRealmQuery.cs
│       │       └── ListRealmsQuery.cs
│       └── Achievements/
│           └── Queries/
│               ├── GetAchievementQuery.cs
│               └── SearchAchievementsQuery.cs
│   # Note: No Commands folder - this is a read-only API
│
├── WarcraftArmory.Infrastructure/      # External Concerns
│   ├── ExternalServices/               # Third-party API clients
│   │   ├── BlizzardApi/
│   │   │   ├── BlizzardApiClient.cs
│   │   │   ├── BlizzardAuthService.cs
│   │   │   ├── RateLimiter.cs
│   │   │   ├── RequestThrottler.cs
│   │   │   ├── IBlizzardApiClient.cs
│   │   │   └── Models/                 # API response models
│   │   │       ├── BlizzardCharacter.cs
│   │   │       ├── BlizzardItem.cs
│   │   │       ├── BlizzardGuild.cs
│   │   │       └── OAuthTokenResponse.cs
│   │   └── Configuration/
│   │       └── BlizzardApiSettings.cs
│   ├── Caching/                        # Cache implementations
│   │   ├── RedisCacheService.cs
│   │   ├── MemoryCacheService.cs
│   │   └── CacheKeyGenerator.cs
│   ├── Persistence/                    # Database (optional, for future use)
│   │   ├── ApplicationDbContext.cs
│   │   ├── Repositories/
│   │   └── Configurations/
│   └── BackgroundServices/             # Hosted services
│       ├── TokenRefreshService.cs
│       └── CacheWarmupService.cs
│
├── WarcraftArmory.WebApi/              # Presentation Layer
│   ├── Controllers/                    # API controllers (GET endpoints only)
│   │   ├── CharactersController.cs     # GET /api/characters/{region}/{realm}/{name}
│   │   ├── ItemsController.cs          # GET /api/items/{id}, GET /api/items/search
│   │   ├── GuildsController.cs         # GET /api/guilds/{region}/{realm}/{name}
│   │   ├── RealmsController.cs         # GET /api/realms, GET /api/realms/{id}
│   │   ├── AchievementsController.cs   # GET /api/achievements/{id}
│   │   ├── MountsController.cs         # GET /api/mounts, GET /api/mounts/{id}
│   │   └── SearchController.cs         # GET /api/search (unified search)
│   ├── Middleware/                     # Custom middleware
│   │   ├── ExceptionHandlingMiddleware.cs
│   │   ├── RateLimitingMiddleware.cs
│   │   └── RequestLoggingMiddleware.cs
│   ├── Filters/                        # Action filters
│   │   ├── ValidationFilter.cs
│   │   └── CacheFilter.cs
│   ├── Extensions/                     # Service collection extensions
│   │   ├── ServiceCollectionExtensions.cs
│   │   └── ApplicationBuilderExtensions.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Dockerfile
│   └── Program.cs                      # Application entry point
│   # Note: All controllers only expose GET endpoints - no POST/PUT/DELETE/PATCH
│
└── tests/
    ├── WarcraftArmory.Domain.Tests/
    │   ├── Entities/
    │   └── ValueObjects/
    ├── WarcraftArmory.Application.Tests/
    │   ├── Services/
    │   ├── UseCases/
    │   └── Validation/
    ├── WarcraftArmory.Infrastructure.Tests/
    │   ├── ExternalServices/
    │   ├── Caching/
    │   └── Integration/
    └── WarcraftArmory.WebApi.Tests/
        ├── Controllers/
        ├── Integration/
        └── Architecture/
```

### C# 14 Features to Leverage

1. **Record Types** - Use for all immutable DTOs, entities, and value objects
   - DTOs: `public record CharacterResponse(int Id, string Name, ...);`
   - Entities: `public record Character { get; init; }` for read-only data models
   - Value Objects: `public sealed record CharacterName` for validated types
2. **Primary Constructors** - Simplify DTOs with positional record syntax
3. **Collection Expressions** `[]` - Cleaner initialization with spread operator
4. **Params Collections** - Flexible method parameters
5. **Inline Arrays** - Performance-critical sections
6. **Lambda Improvements** - Better LINQ expressions and natural types
7. **Init-only Properties** - All entities use init-only properties (immutable after construction)
8. **Pattern Matching Enhancements** - Complex conditionals in query handlers
9. **Required Members** - Critical properties validation in DTOs
10. **File-scoped Types** - Internal implementations
11. **Interceptors** - Code generation and AOP scenarios
12. **ref readonly Parameters** - Performance optimization for large structs
13. **Partial Properties** - Code generation improvements
14. **Method Group Natural Type** - Better delegate inference

**Note**: Since this is a read-only API, all domain entities and DTOs should be immutable using record types or init-only properties. No mutable state is needed.

### Data Transformation & Enrichment Strategy (Application Layer)

In scenarios where entity data needs to be transformed or enriched (e.g., adding computed properties, combining data from multiple API calls, or mapping between different DTOs and domain entities), all such logic should be implemented in the **Application layer**. This ensures that the Domain layer remains pure and immutable, while the Application layer can compose, project, and enrich data for presentation or API responses.

- **Computed Properties**: Use projection methods or mapping libraries (e.g., AutoMapper) to add computed fields to DTOs without mutating the underlying entities.
- **Combining Data**: Aggregate and combine data from multiple API calls in Application services or query handlers, then map the results to enriched DTOs.
- **Mapping**: Use explicit mapping functions or libraries to convert between domain entities and DTOs, ensuring immutability is preserved.
- **Composition**: For complex responses, compose DTOs from multiple sources, always returning new immutable objects.

All transformation logic should be unit tested and kept separate from domain models. This approach maintains clean separation of concerns and leverages the strengths of immutable data structures.
### Key NuGet Packages

**Core Framework**
- `Microsoft.AspNetCore.OpenApi` (built-in .NET 10)
- `Microsoft.Extensions.Configuration` (10.0+)
- `Microsoft.Extensions.DependencyInjection` (10.0+)

**API Integration**
- `Refit` (7.2+) or `RestSharp` (112+) - Type-safe HTTP client
- `Polly` (8.4+) - Resilience patterns (retry, circuit breaker, timeout)
- `Polly.Extensions.Http` (3.0+) - HTTP-specific policies

**CQRS & Validation**
- `MediatR` (12.4+) - Mediator pattern implementation
- `FluentValidation.AspNetCore` (11.3+) - Request validation
- `AutoMapper.Extensions.Microsoft.DependencyInjection` (13.0+) - Object mapping

**Caching**
- `StackExchange.Redis` (2.8+) - Redis client
- `Microsoft.Extensions.Caching.Memory` (10.0+) - In-memory cache
- `Microsoft.Extensions.Caching.StackExchangeRedis` (10.0+) - Distributed cache

**Logging**
- `Serilog.AspNetCore` (8.0+) - Structured logging
- `Serilog.Sinks.Console` (6.0+) - Console output
- `Serilog.Sinks.File` (6.0+) - File output
- `Serilog.Sinks.Seq` (8.0+) - Centralized logging (optional)
- `Serilog.Enrichers.Environment` (3.0+) - Context enrichment

**Documentation**
- `Scalar.AspNetCore` (1.2+) - Modern OpenAPI documentation UI

**Testing**
- `xUnit` (2.9+) - Test framework
- `xunit.runner.visualstudio` (2.8+) - VS test runner
- `FluentAssertions` (7.0+) - Assertion library
- `NSubstitute` (5.1+) or `Moq` (4.20+) - Mocking
- `Microsoft.AspNetCore.Mvc.Testing` (10.0+) - Integration tests
- `NetArchTest.Rules` (1.3+) - Architecture tests
- `WireMock.Net` (1.6+) - HTTP mocking
- `Testcontainers` (3.10+) - Container-based integration tests

## Frontend Architecture (Angular)

### Project Structure

```
src/
├── app/
│   ├── core/                           # Singleton services, guards
│   │   ├── services/
│   │   │   ├── api.service.ts
│   │   │   ├── cache.service.ts
│   │   │   ├── error-handler.service.ts
│   │   │   ├── loading.service.ts
│   │   │   └── region.service.ts
│   │   ├── interceptors/
│   │   │   ├── auth.interceptor.ts
│   │   │   ├── error.interceptor.ts
│   │   │   ├── cache.interceptor.ts
│   │   │   └── loading.interceptor.ts
│   │   ├── guards/
│   │   │   └── auth.guard.ts
│   │   └── models/                     # Core interfaces/types
│   │       ├── character.model.ts
│   │       ├── item.model.ts
│   │       ├── guild.model.ts
│   │       └── api-response.model.ts
│   │
│   ├── shared/                         # Reusable components/modules
│   │   ├── components/
│   │   │   ├── search-bar/
│   │   │   │   ├── search-bar.component.ts
│   │   │   │   ├── search-bar.component.html
│   │   │   │   └── search-bar.component.scss
│   │   │   ├── loading-spinner/
│   │   │   ├── error-message/
│   │   │   ├── pagination/
│   │   │   ├── item-card/
│   │   │   ├── character-card/
│   │   │   └── region-selector/
│   │   ├── pipes/
│   │   │   ├── item-quality.pipe.ts
│   │   │   ├── character-class.pipe.ts
│   │   │   └── safe-html.pipe.ts
│   │   ├── directives/
│   │   │   ├── lazy-load.directive.ts
│   │   │   └── tooltip.directive.ts
│   │   └── utils/
│   │       ├── debounce.util.ts
│   │       └── cache.util.ts
│   │
│   ├── features/                       # Feature modules (lazy-loaded)
│   │   ├── characters/
│   │   │   ├── components/
│   │   │   │   ├── character-list/
│   │   │   │   ├── character-detail/
│   │   │   │   ├── character-equipment/
│   │   │   │   └── character-stats/
│   │   │   ├── services/
│   │   │   │   └── character.service.ts
│   │   │   ├── models/
│   │   │   │   └── character-detail.model.ts
│   │   │   ├── state/                  # Signals/NgRx state
│   │   │   │   └── character.store.ts
│   │   │   └── characters.routes.ts
│   │   ├── items/
│   │   │   ├── components/
│   │   │   │   ├── item-list/
│   │   │   │   ├── item-detail/
│   │   │   │   └── item-filter/
│   │   │   ├── services/
│   │   │   │   └── item.service.ts
│   │   │   └── items.routes.ts
│   │   ├── guilds/
│   │   │   ├── components/
│   │   │   │   ├── guild-list/
│   │   │   │   ├── guild-detail/
│   │   │   │   └── guild-roster/
│   │   │   └── guilds.routes.ts
│   │   ├── achievements/
│   │   │   └── achievements.routes.ts
│   │   ├── professions/
│   │   │   └── professions.routes.ts
│   │   ├── mounts/
│   │   │   └── mounts.routes.ts
│   │   └── search/
│   │       ├── components/
│   │       │   ├── search-page/
│   │       │   ├── search-results/
│   │       │   └── search-filters/
│   │       └── search.routes.ts
│   │
│   ├── layout/                         # Layout components
│   │   ├── header/
│   │   │   ├── header.component.ts
│   │   │   ├── header.component.html
│   │   │   └── header.component.scss
│   │   ├── footer/
│   │   ├── sidebar/
│   │   └── main-layout/
│   │
│   ├── app.component.ts
│   ├── app.component.html
│   ├── app.component.scss
│   ├── app.routes.ts                   # Route configuration
│   └── app.config.ts                   # Standalone app config
│
├── environments/
│   ├── environment.ts
│   └── environment.prod.ts
│
├── assets/
│   ├── images/
│   ├── icons/
│   ├── i18n/                           # Translation files
│   └── styles/
│       ├── _variables.scss
│       ├── _mixins.scss
│       └── _themes.scss
│
├── styles.scss                         # Global styles
├── index.html
├── main.ts
└── Dockerfile
```

### Angular Architecture Patterns

1. **Standalone Components** (Angular 14+, default in 17+, enhanced in 21)
   - No NgModules required
   - Direct imports in components
   - Cleaner dependency management

2. **Signals** for Reactive State Management (Angular 16+, matured in 21)
   - Simpler than RxJS for most cases
   - Better performance with fine-grained reactivity
   - `signal()`, `computed()`, `effect()`
   - Input signals and model signals (Angular 17.1+)
   - Linked signals for derived state (Angular 19+)

3. **Lazy Loading**
   - All feature modules lazy-loaded
   - Reduces initial bundle size
   - Faster initial load time

4. **Smart/Dumb Component Pattern**
   - **Smart (Container)**: State management, data fetching, business logic
   - **Dumb (Presentational)**: Pure display, inputs/outputs only, reusable

5. **OnPush Change Detection Strategy**
   - Opt-in change detection
   - Significant performance improvement
   - Works perfectly with Signals

6. **Route Guards & Resolvers**
   - Functional guards (Angular 15+)
   - Guards for navigation control
   - Resolvers for pre-loading data
   - Cleaner component initialization

7. **HTTP Interceptors**
   - Functional interceptors (Angular 15+)
   - Cross-cutting concerns (auth, caching, errors, loading)
   - Centralized request/response handling

8. **Modern Angular Features (19+, enhanced in 21)**
   - Incremental hydration for SSR
   - Resource API for data fetching
   - Enhanced control flow syntax (`@if`, `@for`, `@switch`)
   - Event replay for improved user experience
   - Fine-grained reactivity optimizations
   - Improved developer experience with better error messages

9. **RxJS Operators**
   - `debounceTime`, `distinctUntilChanged` for search
   - `switchMap`, `mergeMap` for API calls
   - `catchError`, `retry` for error handling

### Key Angular Packages

**Core**
- `@angular/core` (21.0+), `@angular/common`, `@angular/router`
- `@angular/forms` (21.0+) - Reactive forms with signals support
- `@angular/platform-browser` (21.0+) - Browser platform

**UI Component Library** (Choose one)
- `@angular/material` (21.0+) - Material Design components
- `primeng` (17.18+) + `primeicons` (7.0+) - Rich UI components (recommended)

**State Management** (Optional, choose based on complexity)
- `@ngrx/signals` (19.0+) - Lightweight, modern state management (recommended)
- `@ngrx/store` (19.0+) + `@ngrx/effects` - Full Redux pattern (for complex apps)
- `@ngrx/component-store` (19.0+) - Local component state

**HTTP & Async**
- `rxjs` (7.8+) - Reactive programming (included by default)

**PWA**
- `@angular/service-worker` (21.0+) - Progressive Web App support
- `@angular/pwa` (21.0+) - PWA schematics

**Internationalization**
- `@angular/localize` (21.0+) - Built-in i18n (recommended)
- `@ngx-translate/core` (15.0+) + `@ngx-translate/http-loader` (8.0+) - Alternative

**Data Visualization**
- `chart.js` (4.4+) + `ng2-charts` (6.0+) or `ngx-charts` (20.5+)

**Testing**
- `jest` (29.7+) + `@angular-builders/jest` (21.0+) - Fast testing (recommended)
- `@testing-library/angular` (17.3+) - Better testing patterns
- `jest-preset-angular` (14.3+) - Jest configuration

**Development Tools**
- `@angular-devkit/build-angular` (21.0+) - Build system with esbuild
- `@angular/cli` (21.0+) - CLI tools
- `@angular-eslint/eslint-plugin` (18.4+) - Linting
- `eslint` (9.15+) - Modern flat config
- `typescript` (5.7+) - TypeScript compiler

## Multi-Tier Caching Strategy

### Tier 1: Client-Side (Angular)

**Service Worker Cache**
- Cache static assets (JS, CSS, images)
- Cache API responses with configurable TTL
- Implement `ngsw-config.json` strategy

**LocalStorage/IndexedDB**
- User preferences (region, theme)
- Frequently accessed data (favorite characters, realms)
- Use `@ngx-pwa/local-storage` or native APIs

**HTTP Interceptor Cache**
- Short-lived in-memory response cache
- Prevent duplicate simultaneous requests
- TTL: 30-60 seconds

**Example Cache Strategy**
```typescript
// Cache API responses for 5 minutes
const cachedData = this.cacheService.get(cacheKey);
if (cachedData && !this.cacheService.isExpired(cacheKey)) {
  return of(cachedData);
}
return this.http.get(url).pipe(
  tap(data => this.cacheService.set(cacheKey, data, 5 * 60 * 1000))
);
```

### Tier 2: Server-Side In-Memory (Backend)

**IMemoryCache** (.NET)
- Fast, process-local cache
- OAuth access tokens (until expiration)
- Hot data (frequently accessed static items)
- TTL: 1-5 minutes

**Use Cases**
```csharp
// Cache OAuth token
_memoryCache.Set("oauth_token", token, 
  new MemoryCacheEntryOptions()
    .SetAbsoluteExpiration(TimeSpan.FromMinutes(20))
);

// Cache frequently accessed data
_memoryCache.Set($"item_{itemId}", item,
  new MemoryCacheEntryOptions()
    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
);
```

### Tier 3: Distributed Cache (Redis)

**StackExchange.Redis**
- Shared across all backend instances
- Horizontal scaling support
- Persistent cache storage

**Cache Key Pattern**
```
wow:{region}:{namespace}:{category}:{id}:{version}
Examples:
- wow:us:static:item:18803:v1
- wow:eu:profile:character:ragnaros:johndoe:v1
- wow:kr:dynamic:auction:1234:v1
```

**TTL Strategy by Data Type**

| Data Type | TTL | Rationale |
|-----------|-----|-----------|
| Static data (items, spells, classes, races) | 24 hours - 7 days | Rarely changes, safe to cache long |
| Dynamic data (auction house, WoW token) | 5-15 minutes | Changes frequently, needs fresh data |
| Profile data (characters, guilds) | 15-60 minutes | Semi-static, balance freshness/load |
| Search results | 10-30 minutes | Moderate freshness needs |
| Achievement data | 1-6 hours | Changes occasionally |
| Realm status | 5-10 minutes | Can change with maintenance |

**Cache-Aside Pattern Implementation**

```csharp
public async Task<T> GetOrSetAsync<T>(
    string key, 
    Func<Task<T>> factory, 
    TimeSpan expiration)
{
    // 1. Try cache first
    var cached = await _redis.StringGetAsync(key);
    if (!cached.IsNullOrEmpty)
    {
        return JsonSerializer.Deserialize<T>(cached);
    }
    
    // 2. On miss, fetch from source
    var value = await factory();
    
    // 3. Store in cache before returning
    await _redis.StringSetAsync(
        key, 
        JsonSerializer.Serialize(value), 
        expiration
    );
    
    return value;
}
```

**Cache Invalidation Strategies**
- **Time-based expiration** (primary method)
- **Manual invalidation API endpoint** (for urgent updates)
- **Version-based keys** (change version to invalidate)
- **Tag-based invalidation** (invalidate related keys)

**Respect Blizzard Cache Headers**
```csharp
// Read Cache-Control and Expires headers from Blizzard API
// Use these values to set appropriate Redis TTL
var cacheControl = response.Headers.CacheControl;
var ttl = cacheControl?.MaxAge ?? TimeSpan.FromMinutes(15);
```

## Containerization Strategy

### Docker Setup

#### Backend Dockerfile

**Location**: `WarcraftArmory.WebApi/Dockerfile`

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["WarcraftArmory.sln", "./"]
COPY ["WarcraftArmory.Domain/*.csproj", "./WarcraftArmory.Domain/"]
COPY ["WarcraftArmory.Application/*.csproj", "./WarcraftArmory.Application/"]
COPY ["WarcraftArmory.Infrastructure/*.csproj", "./WarcraftArmory.Infrastructure/"]
COPY ["WarcraftArmory.WebApi/*.csproj", "./WarcraftArmory.WebApi/"]

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build and publish
WORKDIR "/src/WarcraftArmory.WebApi"
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user
RUN addgroup --system --gid 1001 appuser && \
    adduser --system --uid 1001 --ingroup appuser appuser

# Copy published app
COPY --from=build /app/publish .

# Set user
USER appuser

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "WarcraftArmory.WebApi.dll"]
```

#### Frontend Dockerfile

**Location**: `angular-app/Dockerfile`

```dockerfile
# Build stage
FROM node:22-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies (using npm 10+ with lockfile v3)
RUN npm ci

# Copy source code
COPY . .

# Build Angular app
RUN npm run build -- --configuration production

# Runtime stage
FROM nginx:alpine AS runtime

# Copy custom nginx config
COPY nginx.conf /etc/nginx/nginx.conf

# Copy Angular dist files
COPY --from=build /app/dist/warcraft-armory/browser /usr/share/nginx/html

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --quiet --tries=1 --spider http://localhost:80 || exit 1

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
```

#### Nginx Configuration

**Location**: `angular-app/nginx.conf`

```nginx
events {
    worker_connections 1024;
}

http {
    include /etc/nginx/mime.types;
    default_type application/octet-stream;

    server {
        listen 80;
        server_name localhost;
        root /usr/share/nginx/html;
        index index.html;

        # Gzip compression
        gzip on;
        gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;

        # SPA routing - serve index.html for all routes
        location / {
            try_files $uri $uri/ /index.html;
        }

        # Cache static assets
        location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
            expires 1y;
            add_header Cache-Control "public, immutable";
        }

        # Security headers
        add_header X-Frame-Options "SAMEORIGIN" always;
        add_header X-Content-Type-Options "nosniff" always;
        add_header X-XSS-Protection "1; mode=block" always;
    }
}
```

### Docker Compose

**Location**: `docker-compose.yml`

```yaml
version: '3.8'

services:
  backend:
    build:
      context: ./backend
      dockerfile: WarcraftArmory.WebApi/Dockerfile
    container_name: warcraft-armory-backend
    ports:
      - "5000:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:8080
      - Redis__ConnectionString=redis:6379
      - BlizzardApi__ClientId=${BLIZZARD_CLIENT_ID}
      - BlizzardApi__ClientSecret=${BLIZZARD_CLIENT_SECRET}
      - BlizzardApi__Region=us
      - Logging__LogLevel__Default=Information
    depends_on:
      redis:
        condition: service_healthy
    networks:
      - warcraft-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 3s
      retries: 3
      start_period: 10s

  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile
    container_name: warcraft-armory-frontend
    ports:
      - "4200:80"
    environment:
      - API_BASE_URL=http://backend:8080
    depends_on:
      - backend
    networks:
      - warcraft-network
    restart: unless-stopped

  redis:
    image: redis:8-alpine
    container_name: warcraft-armory-redis
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
    command: redis-server --appendonly yes --requirepass ${REDIS_PASSWORD:-devpassword}
    networks:
      - warcraft-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 3s
      retries: 5

  redis-commander:
    image: rediscommander/redis-commander:latest
    container_name: warcraft-armory-redis-commander
    profiles:
      - debug
    ports:
      - "8081:8081"
    environment:
      - REDIS_HOSTS=local:redis:6379:0:${REDIS_PASSWORD:-devpassword}
    depends_on:
      - redis
    networks:
      - warcraft-network
    restart: unless-stopped

volumes:
  redis-data:
    driver: local

networks:
  warcraft-network:
    driver: bridge
```

**Environment File**: `.env`

```env
# Blizzard API Credentials
BLIZZARD_CLIENT_ID=your_client_id_here
BLIZZARD_CLIENT_SECRET=your_client_secret_here

# Redis
REDIS_PASSWORD=devpassword

# Environment
ASPNETCORE_ENVIRONMENT=Development
NODE_ENV=development
```

### Docker Compose Commands

```bash
# Start all services
docker-compose up -d

# Start with Redis Commander (debug profile)
docker-compose --profile debug up -d

# View logs
docker-compose logs -f backend
docker-compose logs -f frontend

# Stop all services
docker-compose down

# Stop and remove volumes
docker-compose down -v

# Rebuild and restart
docker-compose up -d --build

# Scale backend instances
docker-compose up -d --scale backend=3
```

## Testing Strategy

### Backend Testing (.NET)

#### Unit Tests (xUnit)

**Domain Layer Tests**
```csharp
// WarcraftArmory.Domain.Tests/Entities/CharacterTests.cs
public class CharacterTests
{
    [Fact]
    public void Character_WithValidData_ShouldCreate()
    {
        // Arrange
        var name = "TestCharacter";
        var realm = "TestRealm";
        var level = 70;
        
        // Act
        var character = new Character(name, realm, level);
        
        // Assert
        character.Name.Should().Be(name);
        character.Realm.Should().Be(realm);
        character.Level.Should().Be(level);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Character_WithInvalidName_ShouldThrowException(string invalidName)
    {
        // Act & Assert
        Action act = () => new Character(invalidName, "TestRealm", 70);
        act.Should().Throw<ArgumentException>();
    }
}
```

**Application Layer Tests**
```csharp
// WarcraftArmory.Application.Tests/UseCases/GetCharacterQueryHandlerTests.cs
public class GetCharacterQueryHandlerTests
{
    private readonly Mock<IBlizzardApiService> _mockBlizzardService;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly GetCharacterQueryHandler _handler;
    
    public GetCharacterQueryHandlerTests()
    {
        _mockBlizzardService = new Mock<IBlizzardApiService>();
        _mockCacheService = new Mock<ICacheService>();
        _handler = new GetCharacterQueryHandler(
            _mockBlizzardService.Object,
            _mockCacheService.Object
        );
    }
    
    [Fact]
    public async Task Handle_ExistingCharacter_ReturnsCharacterDto()
    {
        // Arrange
        var query = new GetCharacterQuery("TestRealm", "TestCharacter", "us");
        var expectedCharacter = new Character("TestCharacter", "TestRealm", 70);
        
        _mockCacheService
            .Setup(x => x.GetAsync<Character>(It.IsAny<string>()))
            .ReturnsAsync((Character)null);
            
        _mockBlizzardService
            .Setup(x => x.GetCharacterAsync(query.Realm, query.Name, query.Region))
            .ReturnsAsync(expectedCharacter);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("TestCharacter");
        result.Realm.Should().Be("TestRealm");
    }
    
    [Fact]
    public async Task Handle_CachedCharacter_DoesNotCallBlizzardApi()
    {
        // Arrange
        var query = new GetCharacterQuery("TestRealm", "TestCharacter", "us");
        var cachedCharacter = new Character("TestCharacter", "TestRealm", 70);
        
        _mockCacheService
            .Setup(x => x.GetAsync<Character>(It.IsAny<string>()))
            .ReturnsAsync(cachedCharacter);
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        _mockBlizzardService.Verify(
            x => x.GetCharacterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never
        );
    }
}
```

#### Integration Tests

```csharp
// WarcraftArmory.WebApi.Tests/Integration/CharactersControllerTests.cs
public class CharactersControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    
    public CharactersControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task GetCharacter_ValidRequest_Returns200()
    {
        // Arrange
        var realm = "test-realm";
        var name = "testcharacter";
        var region = "us";
        
        // Act
        var response = await _client.GetAsync(
            $"/api/characters/{region}/{realm}/{name}"
        );
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task GetCharacter_InvalidRealm_Returns404()
    {
        // Arrange
        var realm = "invalid-realm-12345";
        var name = "testcharacter";
        var region = "us";
        
        // Act
        var response = await _client.GetAsync(
            $"/api/characters/{region}/{realm}/{name}"
        );
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
```

#### Architecture Tests

```csharp
// WarcraftArmory.WebApi.Tests/Architecture/ArchitectureTests.cs
public class ArchitectureTests
{
    [Fact]
    public void Domain_ShouldNotHaveDependencyOnOtherLayers()
    {
        // Arrange
        var domainAssembly = typeof(WarcraftArmory.Domain.Entities.Character).Assembly;
        
        // Act & Assert
        var result = Types.InAssembly(domainAssembly)
            .ShouldNot()
            .HaveDependencyOn("WarcraftArmory.Application")
            .And()
            .ShouldNot()
            .HaveDependencyOn("WarcraftArmory.Infrastructure")
            .And()
            .ShouldNot()
            .HaveDependencyOn("WarcraftArmory.WebApi")
            .GetResult();
            
        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Controllers_ShouldHaveSuffix()
    {
        // Arrange
        var webApiAssembly = typeof(Program).Assembly;
        
        // Act & Assert
        var result = Types.InAssembly(webApiAssembly)
            .That()
            .ResideInNamespace("WarcraftArmory.WebApi.Controllers")
            .Should()
            .HaveNameEndingWith("Controller")
            .GetResult();
            
        result.IsSuccessful.Should().BeTrue();
    }
    
    [Fact]
    public void Services_ShouldBeInternal()
    {
        // Arrange
        var applicationAssembly = typeof(WarcraftArmory.Application.Services.CharacterService).Assembly;
        
        // Act & Assert
        var result = Types.InAssembly(applicationAssembly)
            .That()
            .ResideInNamespace("WarcraftArmory.Application.Services")
            .Should()
            .NotBePublic()
            .GetResult();
            
        result.IsSuccessful.Should().BeTrue();
    }
}
```

**Test Coverage Target**: 80%+ for business logic

### Frontend Testing (Angular)

#### Unit Tests (Jest - Recommended)

```typescript
// src/app/features/characters/components/character-list/character-list.component.spec.ts
describe('CharacterListComponent', () => {
  let component: CharacterListComponent;
  let fixture: ComponentFixture<CharacterListComponent>;
  let mockCharacterService: jest.Mocked<CharacterService>;

  beforeEach(() => {
    mockCharacterService = {
      getCharacters: jest.fn()
    } as any;

    TestBed.configureTestingModule({
      imports: [CharacterListComponent],
      providers: [
        { provide: CharacterService, useValue: mockCharacterService }
      ]
    });

    fixture = TestBed.createComponent(CharacterListComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load characters on init', () => {
    const mockCharacters = [
      { name: 'TestChar1', realm: 'TestRealm', level: 70 },
      { name: 'TestChar2', realm: 'TestRealm', level: 70 }
    ];
    
    mockCharacterService.getCharacters.mockReturnValue(of(mockCharacters));

    component.ngOnInit();

    expect(mockCharacterService.getCharacters).toHaveBeenCalled();
    expect(component.characters().length).toBe(2);
  });

  it('should emit search event when search is triggered', () => {
    jest.spyOn(component.searchTriggered, 'emit');
    
    component.onSearch('TestQuery');

    expect(component.searchTriggered.emit).toHaveBeenCalledWith('TestQuery');
  });
});

// Service Tests
describe('CharacterService', () => {
  let service: CharacterService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [provideHttpClient(), provideHttpClientTesting()]
    });

    service = TestBed.inject(CharacterService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should fetch character by name and realm', () => {
    const mockCharacter = { name: 'TestChar', realm: 'TestRealm', level: 70 };

    service.getCharacter('TestRealm', 'TestChar', 'us').subscribe(char => {
      expect(char).toEqual(mockCharacter);
    });

    const req = httpMock.expectOne(
      'http://localhost:5000/api/characters/us/TestRealm/TestChar'
    );
    expect(req.request.method).toBe('GET');
    req.flush(mockCharacter);
  });
});
```

#### E2E Tests (Playwright)

```typescript
// e2e/character-search.spec.ts
import { test, expect } from '@playwright/test';

test.describe('Character Search', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('http://localhost:4200');
  });

  test('should search for a character and display results', async ({ page }) => {
    // Navigate to search page
    await page.click('text=Search');
    
    // Fill search form
    await page.fill('[data-testid="realm-input"]', 'Ragnaros');
    await page.fill('[data-testid="character-input"]', 'TestCharacter');
    await page.selectOption('[data-testid="region-select"]', 'us');
    
    // Submit search
    await page.click('[data-testid="search-button"]');
    
    // Wait for results
    await page.waitForSelector('[data-testid="character-card"]');
    
    // Verify results
    const characterName = await page.textContent('[data-testid="character-name"]');
    expect(characterName).toContain('TestCharacter');
  });

  test('should show error message for invalid character', async ({ page }) => {
    await page.click('text=Search');
    
    await page.fill('[data-testid="realm-input"]', 'InvalidRealm');
    await page.fill('[data-testid="character-input"]', 'InvalidChar');
    await page.click('[data-testid="search-button"]');
    
    // Expect error message
    await expect(page.locator('[data-testid="error-message"]'))
      .toContainText('Character not found');
  });

  test('should navigate to character detail page', async ({ page }) => {
    // Perform search
    await page.click('text=Search');
    await page.fill('[data-testid="realm-input"]', 'Ragnaros');
    await page.fill('[data-testid="character-input"]', 'TestCharacter');
    await page.click('[data-testid="search-button"]');
    
    // Click character card
    await page.click('[data-testid="character-card"]');
    
    // Verify navigation
    await expect(page).toHaveURL(/.*\/characters\/us\/ragnaros\/testcharacter/);
    
    // Verify detail page content
    await expect(page.locator('h1')).toContainText('TestCharacter');
  });
});
```

**Test Coverage Target**: 70%+ for components and services

## Security Considerations

### API Security

1. **Never Expose Client Secret in Frontend**
   - All Blizzard API calls go through backend
   - Frontend only calls backend API
   - Client credentials stored securely on server

2. **Secrets Management**
   - Use **Azure Key Vault** or **AWS Secrets Manager** in production
   - Use **User Secrets** (Development) or **Environment Variables**
   - Never commit secrets to repository

3. **CORS Configuration**
   ```csharp
   builder.Services.AddCors(options =>
   {
       options.AddPolicy("AllowFrontend", policy =>
       {
           policy.WithOrigins("https://yourfrontend.com")
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .AllowCredentials();
       });
   });
   ```

4. **Rate Limiting**
   - Implement on backend API to prevent abuse
   - Use `AspNetCoreRateLimit` package
   - Protect against DDoS

5. **Input Validation**
   - FluentValidation for all inputs
   - Sanitize user input
   - Prevent injection attacks

6. **HTTPS Only**
   - Enforce HTTPS in production
   - Use HSTS headers
   - Redirect HTTP to HTTPS

### Data Protection

1. **No Sensitive Data Storage**
   - Don't store Blizzard API credentials in database
   - Cache only public game data
   - No user passwords (use OAuth if user auth needed)

2. **Cache Security**
   - Redis password protection
   - Network isolation (Docker network)
   - Encrypted connections in production

## Monitoring & Observability

### Logging

**Backend (Serilog)**
```csharp
// Program.cs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "WarcraftArmory")
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://seq:5341") // Optional: Centralized logging
    .CreateLogger();
```

**Structured Logging Examples**
```csharp
_logger.LogInformation(
    "Fetching character {CharacterName} from realm {Realm} in region {Region}",
    name, realm, region
);

_logger.LogWarning(
    "Cache miss for key {CacheKey}, fetching from Blizzard API",
    cacheKey
);

_logger.LogError(
    exception,
    "Failed to fetch character {CharacterName} from Blizzard API",
    name
);
```

### Application Insights / Prometheus

**Metrics to Track**
- Request count and latency
- Cache hit/miss ratio
- Blizzard API call count and latency
- Error rates
- Rate limit violations
- Redis connection health

**Health Checks**
```csharp
builder.Services.AddHealthChecks()
    .AddRedis(redisConnectionString, name: "redis")
    .AddUrlGroup(new Uri("https://us.api.blizzard.com"), name: "blizzard-api")
    .AddCheck<BlizzardApiHealthCheck>("blizzard-auth");

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
```

### Error Tracking

**Options**
- **Sentry** - Real-time error tracking
- **Raygun** - Error and performance monitoring
- **Application Insights** - Azure-native monitoring

## CI/CD Pipeline

### GitHub Actions Example

```yaml
# .github/workflows/ci-cd.yml
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  test-backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
        working-directory: ./backend
      
      - name: Build
        run: dotnet build --no-restore -c Release
        working-directory: ./backend
      
      - name: Test
        run: dotnet test --no-build -c Release --verbosity normal --collect:"XPlat Code Coverage"
        working-directory: ./backend
      
      - name: Upload coverage
        uses: codecov/codecov-action@v3
        with:
          files: ./backend/**/coverage.cobertura.xml

  test-frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '22'
          cache: 'npm'
          cache-dependency-path: ./frontend/package-lock.json
      
      - name: Install dependencies
        run: npm ci
        working-directory: ./frontend
      
      - name: Lint
        run: npm run lint
        working-directory: ./frontend
      
      - name: Test
        run: npm run test:ci
        working-directory: ./frontend
      
      - name: Build
        run: npm run build -- --configuration production
        working-directory: ./frontend

  build-and-push:
    needs: [test-backend, test-frontend]
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    steps:
      - uses: actions/checkout@v4
      
      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3
      
      - name: Login to Docker Hub
        uses: docker/login-action@v3
        with:
          username: ${{ secrets.DOCKER_USERNAME }}
          password: ${{ secrets.DOCKER_PASSWORD }}
      
      - name: Build and push backend
        uses: docker/build-push-action@v5
        with:
          context: ./backend
          file: ./backend/WarcraftArmory.WebApi/Dockerfile
          push: true
          tags: yourusername/warcraft-armory-backend:latest
      
      - name: Build and push frontend
        uses: docker/build-push-action@v5
        with:
          context: ./frontend
          file: ./frontend/Dockerfile
          push: true
          tags: yourusername/warcraft-armory-frontend:latest

  deploy:
    needs: build-and-push
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    steps:
      - name: Deploy to Azure/AWS
        run: |
          # Add deployment commands here
          echo "Deploying to production..."
```

## Performance Optimization

### Backend

1. **Response Compression**
   ```csharp
   builder.Services.AddResponseCompression(options =>
   {
       options.EnableForHttps = true;
       options.Providers.Add<GzipCompressionProvider>();
       options.Providers.Add<BrotliCompressionProvider>();
   });
   ```

2. **Async/Await Throughout**
   - All I/O operations async
   - Avoid blocking calls

3. **Connection Pooling**
   - Redis connection multiplexing
   - HTTP client factory for Blizzard API

4. **Database Connection Pooling** (if using DB)
   - EF Core connection pooling
   - Proper DbContext lifetime

### Frontend

1. **Lazy Loading**
   - All feature modules lazy-loaded
   - Reduces initial bundle size

2. **Tree Shaking**
   - Remove unused code
   - Automatic with production build

3. **AOT Compilation**
   - Ahead-of-time compilation
   - Default in Angular 9+, optimized in 19

4. **Image Optimization**
   - Use NgOptimizedImage directive (Angular 15+)
   - Automatic WebP format
   - Lazy load images with built-in support
   - Use CDN for static assets
   - Preconnect to image CDNs

5. **OnPush Change Detection & Signals**
   - OnPush reduces change detection cycles
   - Signals provide granular reactivity (Angular 16+)
   - Signal-based components for optimal performance
   - Zoneless change detection (Angular 18+, stable in 21)

6. **Bundle Optimization**
   - Code splitting with lazy loading
   - esbuild for faster builds (Angular 17+, enhanced in 21)
   - Modern output only (ESM) for smaller bundles
   - Tree shaking optimizations
   - Improved build cache (Angular 21)

## Documentation

### Backend Documentation

1. **Swagger/OpenAPI**
   - Auto-generated from code
   - XML comments for descriptions
   - Available at `/swagger`

2. **Code Comments**
   - XML documentation comments
   - Explain complex logic
   - Document public APIs

### Frontend Documentation

1. **Compodoc**
   - Auto-generated Angular documentation
   - Component, service, module documentation
   - Run: `npx @compodoc/compodoc -p tsconfig.json -s`

### Architecture Documentation

1. **C4 Diagrams**
   - Context diagram (system boundaries)
   - Container diagram (high-level tech choices)
   - Component diagram (component interactions)
   - Code diagram (class relationships)

2. **README.md**
   - Project overview
   - Setup instructions
   - Development guidelines
   - Deployment process

3. **API Documentation**
   - Postman collection
   - Request/response examples
   - Authentication flow

## Project Initialization Steps

### Phase 1: Infrastructure Setup (Week 1)

1. **Initialize Git Repository**
   - Create `.gitignore` for .NET and Angular
   - Setup branch protection rules
   - Configure pull request templates

2. **Setup Docker Environment**
   - Create `docker-compose.yml`
   - Configure Redis container
   - Setup networks and volumes

3. **Obtain Blizzard API Credentials**
   - Register at Battle.net Developer Portal
   - Enable 2FA
   - Get Client ID and Client Secret
   - Accept API Terms of Use

### Phase 2: Backend Development (Weeks 2-4)

1. **Create Solution Structure**
   - Initialize .NET 10 solution
   - Create projects (Domain, Application, Infrastructure, WebApi)
   - Setup project references

2. **Implement Domain Layer**
   - Define entities (Character, Item, Guild, etc.)
   - Create value objects
   - Define domain exceptions

3. **Implement Application Layer**
   - Setup MediatR for CQRS
   - Create DTOs and mapping profiles
   - Implement validators
   - Create use cases (queries/commands)

4. **Implement Infrastructure Layer**
   - Blizzard API client (Refit)
   - OAuth authentication service
   - Rate limiter implementation
   - Redis cache service
   - Memory cache service

5. **Implement WebApi Layer**
   - Controllers (Characters, Items, Guilds, etc.)
   - Middleware (exception handling, rate limiting, logging)
   - Configure OpenAPI with Scalar UI
   - Setup health checks

6. **Write Backend Tests**
   - Unit tests for all layers
   - Integration tests for API endpoints
   - Architecture tests

### Phase 3: Frontend Development (Weeks 5-6)

1. **Initialize Angular Application**
   - Create Angular 17+ standalone app
   - Configure routing
   - Setup environments

2. **Implement Core Module**
   - API service
   - Interceptors (error, cache, loading)
   - Guards
   - Models/interfaces

3. **Implement Shared Module**
   - Reusable components (search bar, pagination, etc.)
   - Pipes
   - Directives

4. **Implement Feature Modules**
   - Characters feature (search, list, detail)
   - Items feature
   - Guilds feature
   - Other features as needed

5. **Implement Layout**
   - Header, footer, sidebar
   - Main layout component
   - Responsive design

6. **Write Frontend Tests**
   - Unit tests for components and services
   - E2E tests for critical flows

### Phase 4: Integration & Testing (Week 7)

1. **Integration Testing**
   - Test frontend-backend integration
   - Test caching behavior
   - Test error handling

2. **Performance Testing**
   - Load testing with k6 or JMeter
   - Monitor cache hit rates
   - Optimize slow endpoints

3. **Security Review**
   - Verify secrets management
   - Test CORS configuration
   - Review rate limiting

### Phase 5: Deployment (Week 8)

1. **Containerization**
   - Create Dockerfiles
   - Test Docker Compose locally
   - Optimize image sizes

2. **CI/CD Setup**
   - Configure GitHub Actions
   - Setup automated testing
   - Configure deployment pipeline

3. **Deployment**
   - Deploy to chosen platform (Azure/AWS/DigitalOcean)
   - Configure domain and SSL
   - Setup monitoring and logging

4. **Documentation**
   - Complete README
   - Write deployment guide
   - Create architecture diagrams
   - Document API endpoints

## Future Enhancements

### Phase 6+: Advanced Features

**Important**: All enhancements maintain read-only access to Blizzard's API. User features involve storing user preferences locally, not modifying game data.

1. **User Preferences & Bookmarks**
   - Battle.net OAuth for user authentication (optional)
   - Save favorite characters (stored in our database, not Blizzard's)
   - Bookmark items, guilds, realms for quick access
   - Track personal collection progress (mounts, pets, achievements)
   - Custom character comparison and notes
   - **Note**: All user data is stored locally in our database; game data remains read-only

2. **Real-time Features**
   - WebSockets for live auction house updates
   - Auction house price tracking and alerts
   - Realm status change notifications
   - Price history charts and trend analysis

3. **Advanced Search & Discovery**
   - Elasticsearch integration for faster searches
   - Full-text search across all game content
   - Faceted search with multiple filters
   - "Similar items" recommendations
   - Popular searches and trending content

4. **Data Analytics & Insights**
   - Character level distribution statistics
   - Item popularity trends over time
   - Realm population analysis and trends
   - Class and race distribution charts
   - Achievement completion rates

5. **Mobile App**
   - React Native or Flutter mobile app
   - Share same read-only backend API
   - Offline caching for browsing previously viewed content
   - Push notifications for tracked auction items

6. **Kubernetes Deployment & Scaling**
   - Helm charts for deployment
   - Horizontal pod auto-scaling
   - Load balancing across regions
   - Multi-region deployment for global access

## Decision Points

### Questions to Address Before Starting

1. **Blizzard API Credentials**
   - Do you have a Battle.net account with 2FA?
    Y
   - Have you registered for API access?
    Y
   - Where will you store credentials? (Azure Key Vault, AWS Secrets Manager, .env)
    usersecrets for local
    AWS Secrets Manager for deployed

2. **Initial API Scope**
   - Start with core features (characters, items, guilds, realms)?
    Y
   - Or implement all 26 API categories from the start?
    N
   - **Recommendation**: Phased approach for faster MVP

3. **State Management**
   - Use Angular Signals (simpler, modern)?
    Y
   - Or full NgRx Store (more complex state)?
    N
   - **Recommendation**: Start with Signals, migrate to NgRx if needed

4. **Deployment Target**
   - Azure (Container Apps, AKS)? N
   - AWS (ECS, EKS)? Y (ECS when deployed)
   - DigitalOcean (App Platform)? N
   - Local/Self-hosted (Docker Compose)? Y (start)
   - **Recommendation**: Start with Docker Compose, plan cloud migration

5. **UI Framework**
   - Angular Material (Google Design)? Y
   - PrimeNG (Feature-rich)? N
   - **Recommendation**: PrimeNG for rapid development

6. **Database Requirement**
   - Pure cache-based (no DB for game data)? Y
   - Add PostgreSQL/MySQL for user preferences/bookmarks? N (initially)
   - **Recommendation**: Start without DB since all game data comes from Blizzard API and is cached in Redis. Add database only if implementing user accounts and bookmarks in future phases.
   - **Note**: No database needed for core functionality - this is a read-only view of Blizzard's data

7. **Testing Priority**
   - Full TDD approach? Y
   - Or write tests after implementation? N
   - **Recommendation**: Write tests as you go for critical paths

## Summary

This comprehensive plan provides a solid foundation for building a production-ready World of Warcraft game data search website. The architecture is:

- **Read-Only by Design**: Fan-made website that displays Blizzard game data without modifications
- **Scalable**: Multi-tier caching, containerization, stateless backend
- **Maintainable**: Clean architecture, SOLID principles, comprehensive tests
- **Modern**: .NET 10 (Nov 2025), C# 14 with record types, Angular 21 (Nov 2025), Node 22 LTS, Redis 8, Docker 27+
- **Immutable**: All domain entities use record types for immutability and thread-safety
- **Extensible**: Strategy patterns, modular design, CQRS queries
- **Production-Ready**: Security, monitoring, CI/CD, documentation
- **Cache-First**: No database needed - Redis for distributed caching, memory cache for hot data

The project leverages the latest technologies and best practices to create a robust, efficient, and user-friendly application for exploring World of Warcraft game data through Blizzard's official API.

## Implementation Status

### ✅ Completed Tasks

#### Phase 1: Infrastructure Setup
1. **Git Repository Initialized** (Nov 21, 2025)
   - Created comprehensive .gitignore for .NET and Angular
   - Created README.md with project documentation
   - Created .env.example template for environment variables
   - Initial commit completed

2. **.NET 10 Solution Structure Created** (Nov 21, 2025)
   - Created `WarcraftArmory.sln` solution file
   - Created 4 main projects with Clean Architecture:
     - `WarcraftArmory.Domain` - Core business entities and rules
     - `WarcraftArmory.Application` - Use cases, DTOs, CQRS handlers
     - `WarcraftArmory.Infrastructure` - External services, caching, Blizzard API
     - `WarcraftArmory.WebApi` - API controllers, middleware, entry point
   - Created 4 test projects:
     - `WarcraftArmory.Domain.Tests`
     - `WarcraftArmory.Application.Tests`
     - `WarcraftArmory.Infrastructure.Tests`
     - `WarcraftArmory.WebApi.Tests`
   - Configured proper project references following Clean Architecture:
     - Domain has no dependencies
     - Application → Domain
     - Infrastructure → Application
     - WebApi → Application + Infrastructure
   - All projects use .NET 10 and compile successfully

3. **Docker Compose Configuration** (Nov 21, 2025)
   - Created `docker-compose.yml` with:
     - Backend service (will build from Dockerfile)
     - Frontend service (will build from Dockerfile)
     - Redis 8 Alpine with persistent storage
     - Redis Commander for debugging (debug profile)
     - Health checks for all services
     - Proper networking with bridge network
     - Volume for Redis data persistence
   - Environment variables configured via .env file

#### Phase 2: Backend Development - Domain Layer (Nov 22, 2025)
1. **Domain Layer Complete** ✅
   - Created 6 entities using C# 14 record types: Character, Item, Guild, Realm, Mount, Achievement
   - Created 6 enumerations: Region, CharacterClass, CharacterRace, Faction, Gender, ItemQuality
   - Created 2 value objects with validation: CharacterName, RealmSlug
   - Created 3 domain exceptions: DomainValidationException, EntityNotFoundException, InvalidEntityException
   - Created IEntity interface
   - All entities immutable using init-only properties
   - Build verified successful

2. **Application Layer Complete** ✅
   - **Packages installed:**
     - MediatR 12.4.1 (CQRS implementation)
     - Mapster 7.4.0 (object mapping - chosen over AutoMapper for better performance)
     - Mapster.DependencyInjection 1.0.1
     - FluentValidation.AspNetCore 11.3.0 (input validation)
   - **DTOs created:**
     - Request DTOs: GetCharacterRequest, GetItemRequest, GetGuildRequest
     - Response DTOs: CharacterResponse, ItemResponse, GuildResponse
   - **CQRS Queries & Handlers:**
     - GetCharacterQuery + GetCharacterQueryHandler
     - GetItemQuery + GetItemQueryHandler
     - GetGuildQuery + GetGuildQueryHandler
   - **Mapster configurations:**
     - CharacterMappingConfig (entity → DTO with enum conversions)
     - ItemMappingConfig
     - GuildMappingConfig
   - **FluentValidation validators:**
     - GetCharacterRequestValidator (name length, realm format, region validation)
     - GetItemRequestValidator (item ID validation)
     - GetGuildRequestValidator (guild name, realm validation)
   - **Interfaces defined:**
     - IBlizzardApiService (Blizzard API operations)
     - ICacheService (caching operations with Get/Set/Remove/GetOrSet)
   - Build verified successful

3. **Infrastructure Layer Complete** ✅ (Nov 22, 2025)
   - **Packages installed:**
     - Refit 7.2.22 (type-safe HTTP client)
     - Polly 8.5.0 (resilience patterns - retry, circuit breaker, timeout)
     - StackExchange.Redis 2.8.16 (Redis client)
     - Microsoft.Extensions.Caching.Memory 10.0.0
     - Microsoft.Extensions.Http 10.0.0
     - Microsoft.Extensions.Options 10.0.0
   - **Configuration models:**
     - BlizzardApiSettings (Client ID, Client Secret, Region, Rate Limits)
     - OAuthTokenResponse (Access Token, Expires In)
   - **OAuth authentication:**
     - BlizzardAuthService (OAuth 2.0 client credentials flow)
     - Token caching with automatic refresh before expiration
   - **Distributed rate limiting (Redis-based for multi-pod deployments):**
     - DistributedRateLimiter with per-user limits (60 req/min, 1,000 req/hour)
     - Global Blizzard API limits (80 req/sec, 28,800 req/hour - 80% of Blizzard's limits)
     - Thread-safe across multiple pods using Redis keys with time-based expiration
   - **Blizzard API models:**
     - BlizzardCharacter, BlizzardItem, BlizzardGuild, BlizzardRealm
     - Comprehensive properties for mapping to domain entities
   - **Refit API client:**
     - IBlizzardApiClient interface with endpoints for Character, Item, Guild, Realm, Mount, Achievement
     - Configured with Polly resilience policies
   - **Caching services:**
     - RedisCacheService (distributed cache using StackExchange.Redis)
     - MemoryCacheService (per-pod in-memory cache for hot data)
     - CacheKeyGenerator (consistent cache key generation)
   - **BlizzardApiService:**
     - Implements IBlizzardApiService with all CRUD operations
     - GetCharacterAsync, GetItemAsync, GetGuildAsync, GetRealmAsync (fully implemented)
     - GetMountsAsync, GetAchievementAsync (placeholder - NotImplementedException)
     - Complete entity mapping: Blizzard API models → Domain entities
     - Enum mapping helpers (MapClass, MapRace, MapGender, MapFaction, MapQuality)
     - Uses DistributedRateLimiter for all API calls
   - **Architecture documentation:**
     - RATE_LIMITING_AND_CACHING.md created
     - Explains distributed rate limiting strategy for multi-pod deployments
     - Documents two-tier caching approach (Memory + Redis)
     - Includes monitoring and scaling considerations
   - Build verified successful

4. **WebApi Layer Complete** ✅ (Nov 22, 2025)
   - **Packages installed:**
     - AspNetCore.HealthChecks.Redis 9.0.0 (Redis health checks)
   - **Controllers created:**
     - CharactersController - GET /api/characters/{region}/{realm}/{name}
     - ItemsController - GET /api/items/{region}/{itemId}
     - GuildsController - GET /api/guilds/{region}/{realm}/{name}
     - RealmsController - GET /api/realms/{region}/{realmId} (placeholder)
     - All controllers use MediatR, Mapster, proper error handling, logging
     - Region validation with helpful error messages
     - 404 responses with ProblemDetails when entities not found
   - **Middleware implemented:**
     - ExceptionHandlingMiddleware - Global error handling
       - Domain exceptions (EntityNotFoundException, InvalidEntityException, DomainValidationException)
       - FluentValidation exceptions with property-level errors
       - HTTP exceptions from Refit/HttpClient
       - Timeout and cancellation handling
       - RFC 7807 ProblemDetails format
       - Environment-aware (detailed errors in Dev, sanitized in Prod)
     - RateLimitingMiddleware - Per-user rate limiting
       - Uses DistributedRateLimiter (Redis-based)
       - IP-based identification (with X-Forwarded-For support)
       - Rate limit headers (X-RateLimit-Limit-Minute, X-RateLimit-Used-Hour, etc.)
       - 429 Too Many Requests with Retry-After header
       - Skips health check endpoints
   - **Dependency injection configured:**
     - MediatR for CQRS queries
     - Mapster for object mapping
     - FluentValidation for request validation
     - Redis connection multiplexer (singleton)
     - RedisCacheService and MemoryCacheService
     - DistributedRateLimiter
     - BlizzardAuthService
     - Refit HTTP client with Polly policies (retry, circuit breaker, timeout)
     - BlizzardApiService
     - CORS policy for frontend
     - Health checks (Redis, Blizzard API)
   - **Configuration:**
     - appsettings.json with BlizzardApi, Redis, Caching sections
     - User Secrets initialized for local development
     - USER_SECRETS_SETUP.md guide created
   - **OpenAPI documentation:**
     - Scalar UI at /scalar/v1
     - OpenAPI spec at /openapi/v1.json
   - **Health checks:**
     - /health - All checks
     - /health/ready - Ready state checks
   - Build verified successful - entire solution compiles
     - GetMountsAsync, GetAchievementAsync (placeholder - NotImplementedException)
     - Complete entity mapping: Blizzard API models → Domain entities
     - Enum mapping helpers (MapClass, MapRace, MapGender, MapFaction, MapQuality)
     - Uses DistributedRateLimiter for all API calls
   - **Architecture documentation:**
     - RATE_LIMITING_AND_CACHING.md created
     - Explains distributed rate limiting strategy for multi-pod deployments
     - Documents two-tier caching approach (Memory + Redis)
     - Includes monitoring and scaling considerations
   - Build verified successful

### 🚧 In Progress

#### Phase 3: Frontend Development
**Current Task**: Initialize Angular 21 Application
- Setup Angular project with standalone components
- Configure routing and environments
- Implement core services and interceptors
- Create shared components library

### 📋 Upcoming Tasks

1. **Frontend Development**
   - Initialize Angular 21 application with standalone architecture
   - Implement core module (API service, interceptors, guards)
   - Implement shared module (reusable components, pipes, directives)
   - Implement feature modules (characters, items, guilds)
   - Implement layout components
   - Write frontend tests (Jest, Playwright)

2. **Backend Testing**
   - Install FluentAssertions, NSubstitute packages
   - Write unit tests for Domain entities
   - Write unit tests for Application handlers and validators
   - Write unit tests for Infrastructure services
   - Write integration tests for API controllers
   - Write architecture tests
   - Setup architecture tests with NetArchTest.Rules
   - Initialize Angular 21 application
   - Configure Jest for testing
   - Install Angular Material
   - Implement core services and interceptors
   - Create shared components
   - Implement feature modules (characters, items, guilds)
   - Write unit and E2E tests

7. **Containerization**
   - Create Dockerfile for backend (.NET 10)
   - Create Dockerfile for frontend (Angular 21 + nginx)
   - Test full stack with docker-compose
   - Optimize Docker images

8. **CI/CD Pipeline**
   - Setup GitHub Actions workflow
   - Automated testing on PR
   - Docker image building and pushing
   - Prepare for AWS ECS deployment

### 📝 Current Project Structure

```
warcraft-armory/
├── .git/                              # Git repository
├── .gitignore                         # Ignore patterns
├── .env.example                       # Environment template
├── README.md                          # Project documentation
├── plan.md                            # This file
├── docker-compose.yml                 # Docker orchestration
├── backend/
│   ├── WarcraftArmory.sln            # Solution file
│   ├── src/
│   │   ├── WarcraftArmory.Domain/    # ✅ Created
│   │   ├── WarcraftArmory.Application/ # ✅ Created
│   │   ├── WarcraftArmory.Infrastructure/ # ✅ Created
│   │   └── WarcraftArmory.WebApi/    # ✅ Created
│   └── tests/
│       ├── WarcraftArmory.Domain.Tests/ # ✅ Created
│       ├── WarcraftArmory.Application.Tests/ # ✅ Created
│       ├── WarcraftArmory.Infrastructure.Tests/ # ✅ Created
│       └── WarcraftArmory.WebApi.Tests/ # ✅ Created
└── frontend/                          # ⏳ To be created
```

### 🔑 Environment Setup Required

Before continuing development, ensure you have:

1. **.env file created** (copy from .env.example):
   ```bash
   cp .env.example .env
   # Edit .env and add your Blizzard API credentials
   ```

2. **User Secrets configured** (for local development):
   ```bash
   cd backend/src/WarcraftArmory.WebApi
   dotnet user-secrets init
   dotnet user-secrets set "BlizzardApi:ClientId" "your-client-id"
   dotnet user-secrets set "BlizzardApi:ClientSecret" "your-client-secret"
   ```

3. **Development tools installed**:
   - .NET 10 SDK
   - Node.js 22 LTS
   - Docker Desktop
   - VS Code or Visual Studio 2022

### 💡 Development Commands

#### Backend
```bash
# Build solution
cd backend
dotnet build

# Run tests
dotnet test

# Run API locally
cd src/WarcraftArmory.WebApi
dotnet run

# Watch mode (auto-reload)
dotnet watch run
```

#### Docker
```bash
# Start all services
docker-compose up -d

# Start with Redis Commander
docker-compose --profile debug up -d

# View logs
docker-compose logs -f backend

# Stop all services
docker-compose down

# Rebuild and restart
docker-compose up -d --build
```

## Next Immediate Steps

1. **Implement Domain entities** - Start with Character, Item, Guild, Realm
2. **Add NuGet packages** - MediatR, AutoMapper, FluentValidation, Refit, Polly
3. **Create Blizzard API client** - OAuth authentication and rate limiting
4. **Implement caching layer** - Redis and Memory cache services
5. **Build first API endpoint** - GET /api/characters/{region}/{realm}/{name}

---

*Last Updated: November 22, 2025*

*This plan is a living document and should be updated as the project evolves and requirements change.*
