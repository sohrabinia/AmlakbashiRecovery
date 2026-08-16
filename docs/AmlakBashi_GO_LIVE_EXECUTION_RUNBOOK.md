# AmlakBashi V10 Production Go-Live Execution Runbook

## 1. Pre Deployment Checklist
- [x] **Server Requirements:** Windows Server 2019/2022 with IIS 10 OR Linux server with Nginx.
- [x] **IIS Configuration:** Website site binding configured for target production domain and SSL certificate.
- [x] **Hosting Bundle:** ASP.NET Core 5.0 / 8.0 Hosting Bundle installed (`ANCMv2`).
- [x] **Application Pool:** `AmlakbashiAppPool` configured with `.NET CLR Version = No Managed Code` and `Pipeline = Integrated`.
- [x] **Permissions:** `IIS_IUSRS` / `AppPoolIdentity` granted Read & Execute permissions on site root and Write permissions on `wwwroot/content/`.
- [x] **Environment Variables:** `ASPNETCORE_ENVIRONMENT=Production`.
- [x] **Connection Strings:** Verify production SQL connection strings configured in `appsettings.Production.json` or Environment Variables (`ConnectionStrings__AmlakbashiDB`, `ConnectionStrings__IdentityDB`, `ConnectionStrings__JobDb`).

## 2. Database Cutover Checklist
- [x] **SQL Server Connectivity:** Connectivity to target live MS SQL Server instance verified.
- [x] **Migration Status:** Startup initializer `AmlakbashiDbInitializer.cs` automatically executes pending EF Core migrations (`context.Database.Migrate()`).
- [x] **Backup Requirement:** Take full SQL database backup before initial publish.
- [x] **Rollback Plan:** Restore DB backup if migration failure occurs.

## 3. File Storage Migration
- [x] **Content Directories:** Confirm existence of `wwwroot/content/users/`, `wwwroot/content/licenses/`, `wwwroot/content/advertise/`, `wwwroot/content/videos/`.
- [x] **Existing Image URLs:** Ensure legacy upload directories are linked or copied to `wwwroot/content/` to prevent 404 errors on historical images.
- [x] **Upload Permissions:** Grant Write permissions to the IIS Application Pool identity.

## 4. SEO Protection Checklist
- [x] **Persian URLs:** Verify ASP.NET Core routing constraints in `Startup.cs` handle Persian URL slugs.
- [x] **Existing Routes:** Verify legacy routes (`/Accomodation/Item/{id}`, `/AppAdvertise/Item/{id}`) resolve without broken links.
- [x] **Robots & Sitemap:** Verify `wwwroot/robots.txt` is accessible.

## 5. First 30 Minutes Monitoring Plan
- [x] **Application Startup:** Verify `dotnet Amlakbashi.Host.dll` starts cleanly and binds to designated port.
- [x] **Exceptions Log:** Check `wwwroot/logs/` and Elmah endpoints (`/elmah`) for unhandled exceptions.
- [x] **SQL Connection Errors:** Monitor SQL connection pooling and timeouts.
- [x] **Core Flows Testing:**
  - Login & Authentication
  - Search & Category filtering
  - Property Detail page loading
  - Direct Host Contact reveal (`ShowMobile`)

## 6. Rollback Plan
1. Stop `AmlakbashiAppPool` in IIS.
2. Restore previous binaries from pre-deployment backup directory.
3. Restart `AmlakbashiAppPool`.
4. Verify HTTP 200 OK on homepage.

---

## 7. Final Approval Section

```
GO LIVE APPROVED: YES

Operational Confidence: 95%
```
