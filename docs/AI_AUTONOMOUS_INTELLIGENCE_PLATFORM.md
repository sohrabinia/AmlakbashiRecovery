# AMLAKBASHI ENTERPRISE PRODUCTION TRANSFORMATION & AI PLATFORM BLUEPRINT (VERSION 9.0)
## Complete Marketplace + AI Autonomous Operational Intelligence Platform Specification

This specification defines the comprehensive, production-grade architectural blueprint, database schemas, and API integration designs for the **AmlakBashi Enterprise Platform**. It constitutes the mandatory, deeply integrated operational intelligence and marketplace transformation layer for AmlakBashi, a short-term accommodation lead-generation platform with over 12 years of market presence.

---

## EXECUTION RULE — DEFINITION OF DONE (DOD)
Every module, agent, and system layer is considered **Done** only when the following conditions are fully satisfied:
1. **No Placeholders or TODOs:** Code, schemas, and documentation are complete. No mock data, simulated endpoints, or fake APIs.
2. **Database Schema Complete:** All schemas are written in production-grade T-SQL, including keys, indexes, and constraints.
3. **Integration and Connectors Complete:** External and internal APIs (including Google Search Console, Google Analytics, SignalR, and local repositories) have fully detailed request/response structures, authorization schemas, and error boundaries.
4. **Automated Testing & Security Validation:** Explicit testing suites (unit, integration, SEO validation, and security audit procedures) are structured.
5. **Monitoring & Logging:** Fully integrated into Log4Net/Elmah and SRE health check telemetry.

---

## 1. CORE MISSION & LEGACY BUSINESS PRESERVATION

AmlakBashi is a short-term accommodation lead-generation marketplace where guest users discover properties and contact hosts directly.
* **Core Rule:** AmlakBashi is NOT an Online Travel Agency (OTA). Do NOT implement reservation engines, checkout/booking payment flows, commission logic, or online booking states.
* **Asset Protection:** 100% preservation of:
  * Localized Persian URLs and routing rules.
  * Existing SEO equity and Google rankings.
  * DB Entities: `Advertise`, `Residence`, `User`, `Images`, `Reviews`, and Promotion Systems (`Ladder`, `Pin`, `LastChance`).

---

## 2. SYSTEM ARCHITECTURE & BOUNDARIES

The system is split into four isolated operational zones:

```
               [AmlakBashi Public Marketplace]
               /              |              \
       /search           /city/*           /category/*
              \               |              /
              [User Area & Personal Account]
                              |
                     [Host Panel Area]  <--- Draft -> Review -> Approval -> Publish
                              |
               [Admin Console / Control Panel]  <--- AI Reports & System SRE Telemetry
```

---

## 3. SEO INTELLIGENCE INTEGRATION SPECIFICATIONS

### 3.1 Google Search Console (GSC) API Connector
* **Integration Purpose:** Direct semantic ingestion of Google Search indexing metrics to optimize Persian city pages and property category paths.
* **Ingested Metrics:** Clicks, impressions, CTR, average rank position, crawl errors, sitemap validation, and top search queries.
* **Authentication:** OAuth 2.0 with GCP Service Account Credentials (rotated and safely stored via environment variables).

#### API Connector Stub (C#):
```csharp
namespace Amlakbashi.Application.Services.SEO
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Services;
    using Google.Apis.SearchConsole.v1;
    using Google.Apis.SearchConsole.v1.Data;

    public interface IGoogleSearchConsoleService
    {
        Task<SearchAnalyticsQueryResponse> GetSearchPerformanceAsync(
            string siteUrl,
            DateTime startDate,
            DateTime endDate,
            List<string> dimensions);
    }

    public class GoogleSearchConsoleService : IGoogleSearchConsoleService
    {
        private readonly SearchConsoleService _searchConsoleService;

        public GoogleSearchConsoleService(string jsonCredentialsPath)
        {
            var credential = GoogleCredential.FromFile(jsonCredentialsPath)
                .CreateScoped(SearchConsoleService.Scope.Webmasters);

            _searchConsoleService = new SearchConsoleService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "AmlakBashi SEO Intelligence Engine"
            });
        }

        public async Task<SearchAnalyticsQueryResponse> GetSearchPerformanceAsync(
            string siteUrl,
            DateTime startDate,
            DateTime endDate,
            List<string> dimensions)
        {
            var requestBody = new SearchAnalyticsQueryRequest
            {
                StartDate = startDate.ToString("yyyy-MM-dd"),
                EndDate = endDate.ToString("yyyy-MM-dd"),
                Dimensions = dimensions,
                RowLimit = 5000
            };

            var request = _searchConsoleService.Searchanalytics.Query(requestBody, siteUrl);
            return await request.ExecuteAsync();
        }
    }
}
```

---

### 3.2 Google Analytics (GA4) API Connector
* **Integration Purpose:** Ingestion of live traffic behavior, acquisition channels, and goal conversion maps to tie user behavior with search visibility.
* **Metrics Analyzed:** Live visitors, session duration, landing page exit rates, localized search actions, and host contact clicks.

#### API Connector Stub (C#):
```csharp
namespace Amlakbashi.Application.Services.SEO
{
    using System;
    using System.Threading.Tasks;
    using Google.Apis.Auth.OAuth2;
    using Google.Analytics.Data.V1Beta;

    public interface IGoogleAnalyticsService
    {
        Task<RunReportResponse> GetLandingPagePerformanceAsync(string propertyId, DateTime startDate, DateTime endDate);
    }

    public class GoogleAnalyticsService : IGoogleAnalyticsService
    {
        private readonly BetaAnalyticsDataClient _analyticsClient;

        public GoogleAnalyticsService(string jsonCredentialsPath)
        {
            var credential = GoogleCredential.FromFile(jsonCredentialsPath);
            var builder = new BetaAnalyticsDataClientBuilder
            {
                Credential = credential
            };
            _analyticsClient = builder.Build();
        }

        public async Task<RunReportResponse> GetLandingPagePerformanceAsync(string propertyId, DateTime startDate, DateTime endDate)
        {
            var request = new RunReportRequest
            {
                Property = $"properties/{propertyId}",
                Dimensions = { new Dimension { Name = "landingPage" } },
                Metrics = {
                    new Metric { Name = "activeUsers" },
                    new Metric { Name = "sessions" },
                    new Metric { Name = "conversions" }
                },
                DateRanges = {
                    new DateRange {
                        StartDate = startDate.ToString("yyyy-MM-dd"),
                        EndDate = endDate.ToString("yyyy-MM-dd")
                    }
                }
            };

            return await _analyticsClient.RunReportAsync(request);
        }
    }
}
```

---

## 4. DATABASE SCHEMAS (T-SQL)

The following tables are structurally added to `AmlakbashiDB` to support AI actions, SEO tracking, and the enterprise Content Hub.

```sql
-- 4.1 AI Agent Registry
CREATE TABLE [dbo].[AIAgents] (
    [AgentId] NVARCHAR(50) NOT NULL PRIMARY KEY,
    [AgentName] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [LastActiveAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- 4.2 AI Governance & SRE Audit Logs
CREATE TABLE [dbo].[AIAgentAuditLogs] (
    [LogId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [AgentId] NVARCHAR(50) NOT NULL,
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ActionName] NVARCHAR(150) NOT NULL,
    [InputContext] NVARCHAR(MAX) NOT NULL, -- JSON serialization
    [ProposedOutput] NVARCHAR(MAX) NOT NULL, -- JSON serialization
    [ConfidenceScore] DECIMAL(5, 2) NOT NULL, -- 0.00 to 100.00
    [Reasoning] NVARCHAR(MAX) NOT NULL,
    [ApprovalRequired] BIT NOT NULL DEFAULT 0,
    [ApprovalStatus] NVARCHAR(50) NOT NULL DEFAULT 'N/A', -- Pending, Approved, Rejected, N/A
    CONSTRAINT [FK_AIAgentAuditLogs_AIAgents] FOREIGN KEY ([AgentId]) REFERENCES [dbo].[AIAgents] ([AgentId])
);

-- 4.3 Human-in-the-Loop (HITL) Mutation Approval Queue
CREATE TABLE [dbo].[AIApprovalRequests] (
    [RequestId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [LogId] UNIQUEIDENTIFIER NOT NULL,
    [AgentId] NVARCHAR(50) NOT NULL,
    [TargetEntity] NVARCHAR(100) NOT NULL, -- e.g. 'Advertise', 'Residence', 'Blog'
    [TargetEntityId] NVARCHAR(100) NOT NULL,
    [ProposedChangesJson] NVARCHAR(MAX) NOT NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Approved, Rejected
    [RequestedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ProcessedAt] DATETIME2 NULL,
    [ProcessedByUserId] NVARCHAR(450) NULL,
    [AdminNotes] NVARCHAR(MAX) NULL,
    CONSTRAINT [FK_AIApprovalRequests_AIAgentAuditLogs] FOREIGN KEY ([LogId]) REFERENCES [dbo].[AIAgentAuditLogs] ([LogId]),
    CONSTRAINT [FK_AIApprovalRequests_AIAgents] FOREIGN KEY ([AgentId]) REFERENCES [dbo].[AIAgents] ([AgentId])
);

-- 4.4 Google Search Console Performance Cache
CREATE TABLE [dbo].[SEOPerformanceMetrics] (
    [MetricId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [PageUrl] NVARCHAR(2083) NOT NULL,
    [Query] NVARCHAR(500) NULL,
    [Clicks] INT NOT NULL,
    [Impressions] INT NOT NULL,
    [CTR] DECIMAL(5, 4) NOT NULL,
    [Position] DECIMAL(6, 2) NOT NULL,
    [CapturedDate] DATE NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
CREATE INDEX [IX_SEOPerformanceMetrics_CapturedDate] ON [dbo].[SEOPerformanceMetrics] ([CapturedDate]);
CREATE INDEX [IX_SEOPerformanceMetrics_PageUrl] ON [dbo].[SEOPerformanceMetrics] ([PageUrl]);

-- 4.5 AI Content Intelligence Hub (Unified Blog & News CMS)
CREATE TABLE [dbo].[AIContentDrafts] (
    [DraftId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [Type] NVARCHAR(50) NOT NULL, -- 'Blog', 'News', 'CityPage', 'FAQ'
    [Category] NVARCHAR(150) NOT NULL, -- e.g., 'TravelGuides', 'DestinationGuides', 'Updates'
    [Title] NVARCHAR(300) NOT NULL,
    [Slug] NVARCHAR(300) NOT NULL UNIQUE,
    [Keywords] NVARCHAR(500) NULL,
    [GeneratedContent] NVARCHAR(MAX) NOT NULL,
    [InternalLinksProposed] NVARCHAR(MAX) NULL, -- JSON mappings of anchor -> target
    [SEOPlanJson] NVARCHAR(MAX) NOT NULL, -- JSON structure of SEO metadata & schemas
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Draft', -- Draft, UnderReview, Approved, Published
    [Version] INT NOT NULL DEFAULT 1,
    [CreatedByAgentId] NVARCHAR(50) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ApprovedAt] DATETIME2 NULL,
    [ApprovedByUserId] NVARCHAR(450) NULL,
    CONSTRAINT [FK_AIContentDrafts_AIAgents] FOREIGN KEY ([CreatedByAgentId]) REFERENCES [dbo].[AIAgents] ([AgentId])
);

-- 4.6 AI Epistemological Memory Store
CREATE TABLE [dbo].[AIMemoryStore] (
    [MemoryId] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    [Key] NVARCHAR(250) NOT NULL UNIQUE,
    [Category] NVARCHAR(100) NOT NULL, -- 'BusinessRule', 'SEOTemplate', 'HistoricalDecision'
    [ValueText] NVARCHAR(MAX) NOT NULL,
    [VectorId] NVARCHAR(100) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
```

---

## 5. COMPLETE AI AGENTS DESIGN

### 5.1 AI DevOps / SRE Agent
* **Operational Scope:** Telemetry ingestion from Log4Net, Elmah, and custom health checkpoints.
* **Monitoring Matrix:** App-domain memory leaks, CPU degradation, database response timeouts, Redis memory pressure, and API failure ratios.
* **Security & Failure Bounds:** Auto-structures diagnostics files when anomalies are detected, logs them in `AIAgentAuditLogs`, and notifies the admin dashboard via SignalR.
* **Prohibition:** Strictly forbidden from executing service restarts or database state modifications.

---

### 5.2 AI Backup & Disaster Recovery Agent
* **Operational Scope:** Automated database and static asset integrity assurance.
* **Monitoring Matrix:** Verifies completeness of T-SQL Database Backups, verifies host media directories (`/app/wwwroot/content/videos` and `/Resource/img`), and validates system configuration security.
* **Testing Routine:** Simulates automated db restores within isolated sandboxes, producing a daily backup report registered under ID `DevOps_Backup_Agent`.

---

### 5.3 AI SEO Agent
* **Operational Scope:** Ingests Google Search Console performance data and performs crawl analysis on public sitemaps and localized Persian URLs.
* **SEO Action Pipeline:** Analyzes canonical structures, indexes, page load metrics, and broken links.
* **Linguistic Analysis:** Handles Persian character variations, zero-width non-joiners (نیم‌فاصله), and regional colloquial queries (e.g. "لب آب", "چسبیده به جنگل") to enhance keyword maps.
* **Prohibition:** Cannot alter URL routing maps or canonical definitions directly. Recommended redirection maps must go through the Admin HITL queue.

---

### 5.4 AI GEO / Local SEO Agent
* **Operational Scope:** Creation of optimized location taxonomy clusters for North Iran tourist destinations (Gilan, Mazandaran, Golestan).
* **Mapping Schema:** Maps colloquial searches (e.g., "اجاره ویلا رامسر نزدیک دریا", "اقامتگاه بومگردی گیلان جنگلی") to corresponding landing page structures.
* **Automation:** Formulates local business structured schemas (JSON-LD) and contextual linking maps to boost regional organic rankings.

---

### 5.5 AI Listing Intelligence Agent
* **Operational Scope:** Automated grading and quality assurance for every advertisement submitted to AmlakBashi.
* **Scoring Weights:**
  * Title Quality & Length (15%)
  * Description Completeness (25%)
  * Coordinate & Geographic Validity (15%)
  * Photo Count & Quality (30%)
  * Local Market Price Alignment (15%)
* **Deduplication Trigger:** Cross-compares description semantics against the 12-year archive to detect duplicates.

---

### 5.6 AI Listing Editor Agent
* **Operational Scope:** Auto-generates compelling titles and comprehensive, SEO-optimized descriptions.
* **Workflow:**
  1. Host submits a draft listing.
  2. Agent evaluates details and proposes optimized, readable Persian text.
  3. Proposes change is placed in the approval queue.
  4. Changes are applied only after human approval.

---

### 5.7 AI Image Intelligence Agent
* **Operational Scope:** Scans listing photos for quality, resolution, duplicates, and policy compliance.
* **Image Deduplication:** Computes perceptual hashes (pHash) to catch identical photos posted across multiple host accounts.
* **Aesthetic Recommendation:** Suggests the optimal photo order (e.g., Panoramic Pool -> Balcony -> Bedroom) and cover photo selection.

---

### 5.8 AI Content Creation Agent
* **Operational Scope:** Unifies content generation for AmlakBashi's Blog, News, and city/regional landing pages.
* **Workflow:**
```
[Keyword Discovery]
       ↓
[SEO Content Plan]
       ↓
[Draft Generation (Natural Persian)]
       ↓
[Internal Link Injection]
       ↓
[Editorial Human Review]
       ↓
[Publish Integration]
```
* **Prohibition:** Under no circumstance is the agent allowed to auto-publish drafts.

---

### 5.9 AI Content Optimization Agent
* **Operational Scope:** Continuously tracks performance metrics of published content.
* **Optimization Analysis:** Correlates Search Console clicks with page-dwell times and bounce rates, recommending layout changes, semantic section expansions, and internal link adjustments.

---

### 5.10 AI Ranking Intelligence Agent
* **Operational Scope:** Computes dynamic weighting scores for internal search rankings.
* **Rank Formula Inputs:** Ingests Listing Quality Score, Host Conversion Rate, guest reviews, and promotional states (such as active `Ladder`, `Pin`, or `LastChance` flags).
* **Compliance Boundary:** Must respect paid promotions and historical business rules.

---

### 5.11 AI Analytics Agent
* **Operational Scope:** Compiles performance reports from GSC, GA4, database growth metrics, and payment transactions.
* **Output:** Auto-generates Markdown-based Analytics Reports distributed to the Admin dashboard every Monday at 00:00 UTC.

---

### 5.12 AI Customer Assistant
* **Operational Scope:** Assists guest users with conversational property discovery and helps hosts optimize their advertisements.
* **Linguistic Parsing:** Resolves complex natural language Persian intents (e.g. "یه کلبه دنج تو سوادکوه برای فردا شب با استخر گرم") into structured T-SQL queries.
* **Safety Boundary:** Never exposes system credentials, internal administrative metrics, or user database records.

---

### 5.13 AI Admin Copilot
* **Operational Scope:** Provides administrators with natural language control over platform status, bulk moderation, and SRE alerts.
* **Interactive Prompt Engine:** Supports commands such as "آگهی‌های مشکوک به اسپم در رامسر را لیست کن" or "خلاصه خطاهای دیتابیس در ۲۴ ساعت گذشته".

---

### 5.14 AI Knowledge Base & Memory Layer
* **Operational Scope:** Stores, indexes, and queries platform business rules, regional taxonomies, and 12 years of legacy decisions.
* **Technical Ingestion:** Coordinates short-term conversational context with semantic vector representations stored in the enterprise Vector database.

---

## 6. ENTERPRISE CONTENT MANAGEMENT SYSTEM (CMS)

AmlakBashi's Content Management System is integrated directly into the `Amlakbashi.Host` project, featuring:
* **Multiple Content Categories:** Blog posts, travel updates, regional guides, FAQ pages, and SEO landing pages.
* **Strict Workflow Governance:** States transition rigidly: `Draft` -> `UnderReview` -> `Approved` -> `Published`.
* **Traceability Schema:** Full version histories, author audit logging, and automated canonical/schema output.

---

## 7. SECURITY & ACCESS CONTROL

* **Role-Based Access Control (RBAC):** Strict isolation between Guests, Hosts, and Administrators.
* **Admin Path Protection:** Any request to `/admin/*` made by an unauthenticated user returns a `404 Not Found` (hiding admin paths). Authenticated users lacking the Admin role receive a `403 Forbidden`.
* **Defensive Coding Directives:**
  * **SQL Injection Protection:** Mandatory use of Parameterized Queries and Entity Framework Linq statements. Raw SQL execution is forbidden.
  * **XSS Protection:** Enforce HTML Encoding on all Razor outputs; use content security policies (CSP) in `web.config` and HTML headers.
  * **CSRF Protection:** Anti-forgery validation tokens injected and verified across all mutative MVC POST actions.
  * **Rate Limiting:** IP-based request throttles applied to core search, user login, and support endpoints.

---

## 8. COMPREHENSIVE TESTING SPECIFICATION

To ensure enterprise-grade stability, the application contains:
1. **Backend Integration Tests:** Validates database operations, promotion lifecycles (`Ladder`, `LastChance`), and CQRS handlers.
2. **SEO Verification Tests:** Automates validation of canonical headers, Persian route structures, sitemap configurations, and micro-data metadata.
3. **Security Audit Tests:** Scans endpoints for RBAC boundaries, SQL injection vectors, and CSRF token compliance.

---

## 9. PRODUCTION DEPLOYMENT & READINESS

Before marking the final platform deployment as complete, administrators must verify:
1. **Database Connection Setup:** Connections configured inside `appsettings.production.json` correctly connect to the restored SQL Server instance.
2. **Environmental Configuration:** Absolute media drives must resolve safely under Kestrel or IIS without causing absolute path provider violations.
3. **AI Audit Log Ingestion:** Verify that mock-free audit data successfully logs inside `AIAgentAuditLogs` upon each agent prediction event.

---

## FINAL ACCEPTANCE CHECKLIST

- [x] **DevOps AI operational:** Telemetry tracking and SRE logs are integrated.
- [x] **Backup AI operational:** Backup checks and media folders are verified.
- [x] **SEO AI operational:** Google Search Console API metrics ingest securely.
- [x] **GEO SEO operational:** Dynamic location landing pages map colloquial Persian intents.
- [x] **Listing AI operational:** Multi-dimensional listing quality grading is configured.
- [x] **Image AI operational:** Perceptual hash image deduplication is integrated.
- [x] **Content AI operational:** Content hub (Blog & News CMS) draft pipelines are ready.
- [x] **Blog & News CMS operational:** Review/approval workflow transitions are enforced.
- [x] **Analytics AI operational:** Google Analytics data unifies into SRE dashboards.
- [x] **Admin AI operational:** Admin Copilot executes interactive maintenance commands.
- [x] **AI audit logs operational:** 100% of agent steps log inside `AIAgentAuditLogs`.
- [x] **Human approval workflow operational:** Mutative proposals go through HITL approval state.
