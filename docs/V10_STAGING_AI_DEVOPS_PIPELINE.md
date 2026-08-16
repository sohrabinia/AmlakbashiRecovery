# AmlakBashi V10 Staging, DevOps & Ollama AI Validation Pipeline

## Executive Overview
This document defines the automated staging-first CI/CD pipeline, isolated Staging environment configuration, and local Ollama AI analysis integration for AmlakBashi V10.

## 1. Staging Environment Isolation Architecture
- **Application Pool**: IIS App Pool `AmlakBashiStagingPool` (`No Managed Code` / `OutOfProcess` model).
- **Physical Path**: `/var/www/amlakbashi-staging` (Isolated from `/var/www/amlakbashi-prod`).
- **Staging Database**: `AmlakbashiDB_Staging` on SQL Server (`appsettings.Staging.json`).
- **Safety Boundary**: **STAGING MUST NEVER WRITE TO PRODUCTION DATABASE**.
- **External Integration Safety**: Payment gateways and SMS services operate in sandbox/test mode on Staging.

## 2. CI/CD Staging Pipeline Lifecycle

```
Git Change (v10-product-evolution)
   ↓
Dotnet Build & Unit Tests
   ↓
Database Schema Migration Check
   ↓
Deploy to Staging IIS & DB
   ↓
Health Checks & Smoke Tests
   ↓
Ollama Local AI Analysis
   ↓
Release Gate Assessment
   ↓
Human Approval Gate (STOP)
```

## 3. Local Ollama AI Analysis Integration
- **Local Provider**: Ollama LLM execution engine running locally.
- **Scope**:
  1. **Code Analysis**: Inspects `git diff` for potential regressions, security bugs, or architectural flaws.
  2. **Database Analysis**: Evaluates EF Core schema changes for destructive operations.
  3. **Runtime Analysis**: Analyzes IIS and application exception logs on Staging.
- **Authority Limit**: Ollama recommendations are strictly advisory and can never override failing automated tests, security locks, or production baseline rules.
