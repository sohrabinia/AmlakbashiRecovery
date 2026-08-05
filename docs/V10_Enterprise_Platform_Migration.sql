-- =================================================================================
-- AMLAKBASHI V10.0 - MASTER ENTERPRISE PLATFORM T-SQL MIGRATION SCRIPT
-- Enforces: 18 AI Agents, CMS Content Hub, GSC/GA4 Caches, and Stable Session Persistence
-- Target Database: amlakbas_db (Microsoft SQL Server 2016 / 2019 / 2022)
-- Execution: Execute against the restored database using SSMS or sqlcmd
-- =================================================================================

USE [amlakbas_db];
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- 1. Create AIAgents Directory Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIAgents]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AIAgents] (
        [AgentId] NVARCHAR(50) NOT NULL,
        [AgentName] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_AIAgents_IsActive] DEFAULT (1),
        [LastActiveAt] DATETIME2 NOT NULL CONSTRAINT [DF_AIAgents_LastActiveAt] DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_AIAgents] PRIMARY KEY CLUSTERED ([AgentId] ASC)
    );
END;
GO

-- 2. Create AIAgentAuditLogs Governance & SRE Logging Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIAgentAuditLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AIAgentAuditLogs] (
        [LogId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_AIAgentAuditLogs_LogId] DEFAULT (NEWID()),
        [AgentId] NVARCHAR(50) NOT NULL,
        [Timestamp] DATETIME2 NOT NULL CONSTRAINT [DF_AIAgentAuditLogs_Timestamp] DEFAULT (GETUTCDATE()),
        [ActionName] NVARCHAR(150) NOT NULL,
        [InputContext] NVARCHAR(MAX) NOT NULL,
        [ProposedOutput] NVARCHAR(MAX) NOT NULL,
        [ConfidenceScore] DECIMAL(5, 2) NOT NULL,
        [Reasoning] NVARCHAR(MAX) NOT NULL,
        [ApprovalRequired] BIT NOT NULL CONSTRAINT [DF_AIAgentAuditLogs_ApprovalRequired] DEFAULT (0),
        [ApprovalStatus] NVARCHAR(50) NOT NULL CONSTRAINT [DF_AIAgentAuditLogs_ApprovalStatus] DEFAULT ('N/A'),
        CONSTRAINT [PK_AIAgentAuditLogs] PRIMARY KEY CLUSTERED ([LogId] ASC),
        CONSTRAINT [FK_AIAgentAuditLogs_AIAgents] FOREIGN KEY ([AgentId]) REFERENCES [dbo].[AIAgents] ([AgentId])
    );
    CREATE NONCLUSTERED INDEX [IX_AIAgentAuditLogs_AgentId] ON [dbo].[AIAgentAuditLogs] ([AgentId] ASC);
    CREATE NONCLUSTERED INDEX [IX_AIAgentAuditLogs_Timestamp] ON [dbo].[AIAgentAuditLogs] ([Timestamp] DESC);
END;
GO

-- 3. Create AIApprovalRequests Human-in-the-Loop (HITL) Operations Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIApprovalRequests]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AIApprovalRequests] (
        [RequestId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_AIApprovalRequests_RequestId] DEFAULT (NEWID()),
        [LogId] UNIQUEIDENTIFIER NOT NULL,
        [AgentId] NVARCHAR(50) NOT NULL,
        [TargetEntity] NVARCHAR(100) NOT NULL,
        [TargetEntityId] NVARCHAR(100) NOT NULL,
        [ProposedChangesJson] NVARCHAR(MAX) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL CONSTRAINT [DF_AIApprovalRequests_Status] DEFAULT ('Pending'),
        [RequestedAt] DATETIME2 NOT NULL CONSTRAINT [DF_AIApprovalRequests_RequestedAt] DEFAULT (GETUTCDATE()),
        [ProcessedAt] DATETIME2 NULL,
        [ProcessedByUserId] NVARCHAR(450) NULL,
        [AdminNotes] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_AIApprovalRequests] PRIMARY KEY CLUSTERED ([RequestId] ASC),
        CONSTRAINT [FK_AIApprovalRequests_AIAgentAuditLogs] FOREIGN KEY ([LogId]) REFERENCES [dbo].[AIAgentAuditLogs] ([LogId]),
        CONSTRAINT [FK_AIApprovalRequests_AIAgents] FOREIGN KEY ([AgentId]) REFERENCES [dbo].[AIAgents] ([AgentId])
    );
    CREATE NONCLUSTERED INDEX [IX_AIApprovalRequests_Status] ON [dbo].[AIApprovalRequests] ([Status] ASC);
    CREATE NONCLUSTERED INDEX [IX_AIApprovalRequests_RequestedAt] ON [dbo].[AIApprovalRequests] ([RequestedAt] DESC);
END;
GO

-- 4. Create SEOPerformanceMetrics Search Engine Cache Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SEOPerformanceMetrics]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[SEOPerformanceMetrics] (
        [MetricId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_SEOPerformanceMetrics_MetricId] DEFAULT (NEWID()),
        [PageUrl] NVARCHAR(2083) NOT NULL,
        [Query] NVARCHAR(500) NULL,
        [Clicks] INT NOT NULL,
        [Impressions] INT NOT NULL,
        [CTR] DECIMAL(5, 4) NOT NULL,
        [Position] DECIMAL(6, 2) NOT NULL,
        [CapturedDate] DATE NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_SEOPerformanceMetrics_CreatedAt] DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_SEOPerformanceMetrics] PRIMARY KEY CLUSTERED ([MetricId] ASC)
    );
    CREATE NONCLUSTERED INDEX [IX_SEOPerformanceMetrics_CapturedDate] ON [dbo].[SEOPerformanceMetrics] ([CapturedDate] DESC);
    CREATE NONCLUSTERED INDEX [IX_SEOPerformanceMetrics_PageUrl] ON [dbo].[SEOPerformanceMetrics] ([PageUrl] ASC);
END;
GO

-- 5. Create AIContentDrafts Unified Blog & News CMS Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIContentDrafts]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AIContentDrafts] (
        [DraftId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_AIContentDrafts_DraftId] DEFAULT (NEWID()),
        [Type] NVARCHAR(50) NOT NULL,
        [Category] NVARCHAR(150) NOT NULL,
        [Title] NVARCHAR(300) NOT NULL,
        [Slug] NVARCHAR(300) NOT NULL,
        [Keywords] NVARCHAR(500) NULL,
        [GeneratedContent] NVARCHAR(MAX) NOT NULL,
        [InternalLinksProposed] NVARCHAR(MAX) NULL,
        [SEOPlanJson] NVARCHAR(MAX) NOT NULL,
        [Status] NVARCHAR(50) NOT NULL CONSTRAINT [DF_AIContentDrafts_Status] DEFAULT ('Draft'),
        [Version] INT NOT NULL CONSTRAINT [DF_AIContentDrafts_Version] DEFAULT (1),
        [CreatedByAgentId] NVARCHAR(50) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_AIContentDrafts_CreatedAt] DEFAULT (GETUTCDATE()),
        [ApprovedAt] DATETIME2 NULL,
        [ApprovedByUserId] NVARCHAR(450) NULL,
        CONSTRAINT [PK_AIContentDrafts] PRIMARY KEY CLUSTERED ([DraftId] ASC),
        CONSTRAINT [UQ_AIContentDrafts_Slug] UNIQUE NONCLUSTERED ([Slug] ASC),
        CONSTRAINT [FK_AIContentDrafts_AIAgents] FOREIGN KEY ([CreatedByAgentId]) REFERENCES [dbo].[AIAgents] ([AgentId])
    );
    CREATE NONCLUSTERED INDEX [IX_AIContentDrafts_Status] ON [dbo].[AIContentDrafts] ([Status] ASC);
END;
GO

-- 6. Create AIMemoryStore Epistemological Context Table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AIMemoryStore]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AIMemoryStore] (
        [MemoryId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_AIMemoryStore_MemoryId] DEFAULT (NEWID()),
        [Key] NVARCHAR(250) NOT NULL,
        [Category] NVARCHAR(100) NOT NULL,
        [ValueText] NVARCHAR(MAX) NOT NULL,
        [VectorId] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_AIMemoryStore_CreatedAt] DEFAULT (GETUTCDATE()),
        [UpdatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_AIMemoryStore_UpdatedAt] DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_AIMemoryStore] PRIMARY KEY CLUSTERED ([MemoryId] ASC),
        CONSTRAINT [UQ_AIMemoryStore_Key] UNIQUE NONCLUSTERED ([Key] ASC)
    );
END;
GO

-- =================================================================================
-- SESSION PERSISTENCE & SYSTEM SECURITY STABILITY SCHEMAS
-- =================================================================================

-- 7. Create DataProtectionKeys Database Persistence Table
-- This table persists standard ASP.NET Core session/cookie decryption keys.
-- It prevents session invalidation when the application restarts or IIS AppPool recycles.
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataProtectionKeys]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DataProtectionKeys] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [FriendlyName] NVARCHAR(MAX) NULL,
        [Xml] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_DataProtectionKeys] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END;
GO

-- 8. Create UserRefreshTokens Database Table
-- Supports secure Refresh Token Rotation (RTR). JWT Access Tokens expire in 15 mins,
-- and the client dynamically exchanges this rotating token to maintain the session seamlessly.
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserRefreshTokens]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserRefreshTokens] (
        [TokenId] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_UserRefreshTokens_TokenId] DEFAULT (NEWID()),
        [UserId] NVARCHAR(450) NOT NULL, -- Links to AspNetUsers table
        [Token] NVARCHAR(500) NOT NULL,
        [ExpiresAt] DATETIME2 NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL CONSTRAINT [DF_UserRefreshTokens_CreatedAt] DEFAULT (GETUTCDATE()),
        [CreatedByIp] NVARCHAR(100) NULL,
        [RevokedAt] DATETIME2 NULL,
        [RevokedByIp] NVARCHAR(100) NULL,
        [ReplacedByToken] NVARCHAR(500) NULL,
        [IsExpired] AS (CASE WHEN GETUTCDATE() >= [ExpiresAt] THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END),
        [IsActive] AS (CASE WHEN [RevokedAt] IS NULL AND GETUTCDATE() < [ExpiresAt] THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END),
        CONSTRAINT [PK_UserRefreshTokens] PRIMARY KEY CLUSTERED ([TokenId] ASC),
        CONSTRAINT [UQ_UserRefreshTokens_Token] UNIQUE NONCLUSTERED ([Token] ASC)
    );
    CREATE NONCLUSTERED INDEX [IX_UserRefreshTokens_UserId] ON [dbo].[UserRefreshTokens] ([UserId] ASC);
END;
GO

-- =================================================================================
-- SYSTEM SEED DATA
-- =================================================================================

-- 9. Seed Mandatory AIAgents Directory
MERGE INTO [dbo].[AIAgents] AS Target
USING (VALUES
    ('SRE_Agent', 'AI DevOps/SRE Agent', 'Monitors platform performance and detects API latency degradations.'),
    ('Backup_Agent', 'AI Backup & Recovery Agent', 'Verifies database backups and storage folders.'),
    ('SEO_Agent', 'AI SEO Agent', 'Optimizes Persian canonical URLs and tracks indexing status.'),
    ('GEO_SEO_Agent', 'AI GEO / Local SEO Agent', 'Generates city landing layouts and colloquial search maps.'),
    ('Listing_Intelligence', 'AI Listing Intelligence Agent', 'Evaluates listing titles and computes media quality scores.'),
    ('Listing_Editor', 'AI Listing Editor Agent', 'Recommends optimized Persian wording for listing copies.'),
    ('Image_Intelligence', 'AI Image Intelligence Agent', 'Identifies blurred images and enforces photo orders.'),
    ('Duplicate_Detection', 'AI Duplicate Detection Agent', 'Runs perceptual hash checks across image databases.'),
    ('Moderation_Agent', 'AI Moderation Agent', 'Filters telephone numbers and inappropriate support chat keywords.'),
    ('Ranking_Intelligence', 'AI Ranking Agent', 'Weights listing prominence based on Ladder/Pin and user engagement.'),
    ('Analytics_Agent', 'AI Analytics Agent', 'Aggregates Google Analytics and Search Console performance reports.'),
    ('Customer_Assistant', 'AI Customer Assistant', 'Converts Persian customer queries into database search criteria.'),
    ('Host_Assistant', 'AI Host Assistant', 'Acts as a dynamic seasonal price-optimizer advisor.'),
    ('Admin_Copilot', 'AI Admin Copilot', 'Enables conversational bulk moderation capabilities for admins.'),
    ('Content_Agent', 'AI Content Agent', 'Coordinates CMS content discovery and keyword indexing.'),
    ('Blog_Agent', 'AI Blog Agent', 'Produces travel guides and localized guide content for the blog.'),
    ('News_Agent', 'AI News Agent', 'Generates platform announcements and regional tourism policy news updates.'),
    ('Knowledge_Memory', 'AI KB & Memory Layer', 'Vector-indexes business rule context and legacy decision patterns.')
) AS Source ([AgentId], [AgentName], [Description])
ON Target.[AgentId] = Source.[AgentId]
WHEN MATCHED THEN
    UPDATE SET Target.[AgentName] = Source.[AgentName], Target.[Description] = Source.[Description], Target.[LastActiveAt] = GETUTCDATE()
WHEN NOT MATCHED THEN
    INSERT ([AgentId], [AgentName], [Description]) VALUES (Source.[AgentId], Source.[AgentName], Source.[Description]);
GO

PRINT 'AmlakBashi V10.0 Enterprise Master Database Migration executed successfully. Existing data and sessions are fully preserved.';
GO
