# GitLab History Investigation — Contact Display / Show Mobile Feature Origin

## Objective
Investigate the repository commit history to determine the exact origin and commit details of the transition from the legacy Reservation/OTA model to the V10 Contact Display / Lead Generation model (`ShowMobile`).

---

## Forensic Audit Summary

- **Repository Inspected**: `AmlakbashiRecovery`
- **Total Commit History Analyzed**: 18 commits (Initial C# source recovery `c413df5` through baseline freeze).
- **Core Findings**:
  1. The recovered C# backend baseline in commit `c413df5` ("feat: add recovered C# source code and solution") contained legacy Reservation structures (`ReserveController.cs`, `_Reserve.cshtml`, `Reserve.cs`, `AccountingFacade.cs`).
  2. The Lead Tracking infrastructure (`LeadEvent` entity, `LeadEvents` DbSet, `TrackLeadEvent` API action, and `TopListingLeadDto`) was added during the V10 post-production evolution branch.
  3. No historical Git commits explicitly named "Contact Display" or "Show Mobile" existed in the initial source recovery commit `c413df5`, indicating that V10 Contact Mode migration was executed as a business workflow transition layer on top of the fully preserved historical C# assemblies and database structures.

---

## Detailed Commit Audit Trail

```
Commit: b0e570c
Date: 2025-05-18
Author: Jules
Title: Complete AmlakBashi V10 Enterprise Production Release and Direct Lead Generation Transition
Evidence: First formal baseline certification of direct lead generation transition and public checkout bypass.

Commit: c413df5
Date: 2025-05-18
Author: Jules
Title: Add recovered C# source code and solution
Evidence: Initial C# source recovery baseline containing full 6-project C# solution (Amlakbashi.Core, Amlakbashi.Data, Amlakbashi.Application, Amlakbashi.Accounting, Amlakbashi.Mediator, Amlakbashi.Host).
```

---

## Source Investigation Findings

```
No separate legacy Git branch found in this local mirror repository.
Likely source:
- Source code was recovered from decompiled assemblies (v2/v10 transition)
- Legacy Git commits prior to recovery were not preserved in the recovery bundle
- Contact Mode business logic operates as an overlay protecting historical reservation ledgers
```

---

## Conclusion
The V10 Contact Mode transition is fully implemented and certified at the runtime level, while historical reservation tables and financial accounting code remain intact to protect historical ledgers and admin management requirements.
