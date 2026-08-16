# AmlakBashi V10 Monetization Architecture & Ranking System

## Executive Summary
This document defines the monetization models, ranking score algorithms, and promotion mechanisms in AmlakBashi V10.

## Monetization Pillars

### 1. Nardeban (Listing Bump)
- **Mechanism**: Updates `LastModifiedDate` to bring the listing to the top of standard date-sorted lists without altering fundamental property attributes.
- **Scoring Integration**: Increases short-term visibility in search filters while preserving `ResidenceScore` and `AmlakbashiScore` Integrity.

### 2. Featured Listings (Featured / VIP Listings)
- **Ranking Logic**: Evaluated via `ResidenceScore` (composite score) and `AmlakbashiScore` (admin-assigned quality score).
- **Default Order**: In category/search listings, results order by `ResidenceScore` descending, ensuring promoted listings appear prominently.

### 3. Credit & Wallet System Integration
- **Database Mapping**: Direct mapping to `CreditTransactions` (`[Table("WalletTransactions")]`) in `Amlakbashi.Data`.
- **Payment Flow**: Host wallet balances are credited via payment gateways (Saman, Pasargad) or direct host settlement transfers (Sheba/Paya).

## Score Preservation Guarantee
- `ResidenceScore` and `AmlakbashiScore` remain unchanged during lead generation transitions.
- The shift to Contact Mode (`ShowMobile`) enhances lead volume while maintaining transparent, score-based listing placement.
