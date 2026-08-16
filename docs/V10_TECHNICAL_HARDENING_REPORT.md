# AmlakBashi V10 Technical Hardening Report

## Executive Summary
This report analyzes database access patterns, Redis caching configurations, background task processing via Hangfire, and system hardening strategies for AmlakBashi V10.

## Key Hardening Areas

### 1. Database Query Optimization (EF Core)
- **Lazy Loading Strategy**: `AmlakbashiDB` uses lazy loading proxies configured with `DetachedLazyLoadingWarning` suppression.
- **Query Hardening**: Explicit `.Include(...)` navigation loading is utilized in critical listing pipelines (`AdvertiseAppService`, `UserAppService`) to mitigate N+1 query overhead.
- **Lead Events Indexing**: Added `DeduplicationKey` lookup checks to prevent race conditions during high-volume contact reveal requests.

### 2. Redis & In-Memory Caching
- **Distributed Caching**: Configured with `AddStackExchangeRedisCache` and `IConnectionMultiplexer` in `Startup.cs`.
- **Cache Eviction**: Mediator commands (`RemoveAdvertiseCacheCommand`, `RemoveCategoryItemCacheCommand`) clear stale Redis keys upon advertisement updates.

### 3. Background Job Automation (Hangfire)
- **Storage**: Dedicated `JobDb` SQL Server instance.
- **Concurrency & Reliability**: Configured sliding invisibility timeout (5 min) and isolated worker queues.

### 4. Security Boundaries
- **AntiXSS & CORS**: Configured domain whitelist for CORS and `AntiXssMiddleware` in non-debug runtimes.
- **Session & Identity**: Persistent cookie expiration (60 days) and JWT Bearer authorization for API controllers.
