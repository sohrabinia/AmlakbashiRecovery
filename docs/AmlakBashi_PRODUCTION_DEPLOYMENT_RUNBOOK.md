# AmlakBashi V10 Production Deployment Runbook

## 1. Server Requirements
- **OS:** Windows Server 2019+ (IIS 10) OR Linux (Ubuntu 20.04/22.04 LTS)
- **Runtime:** .NET 8.0 Runtime & ASP.NET Core Hosting Bundle (or .NET 5.0 runtime with legacy libssl1.1 on Linux)
- **Database:** MS SQL Server 2016+

## 2. Required Environment Variables
```env
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__AmlakbashiDB=Server=YOUR_SQL_SERVER;Database=AmlakbashiDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;
ConnectionStrings__IdentityDB=Server=YOUR_SQL_SERVER;Database=IdentityDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;
ConnectionStrings__JobDb=Server=YOUR_SQL_SERVER;Database=JobDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;
```

## 3. Database Preparation
1. Ensure MS SQL Server instance is reachable from the hosting server.
2. Grant `db_owner` or `db_datareader/db_datawriter/db_ddladmin` permissions to the connection string user.
3. EF Core migrations execute automatically on application startup via `AmlakbashiDbInitializer.cs`.

## 4. Build Commands
```bash
# Clean artifacts
dotnet clean Amlakbashi.sln

# Publish host application in Release mode
dotnet publish Amlakbashi.Host/Amlakbashi.Host.csproj -c Release -o ./publish
```

## 5. Deployment Steps (IIS / Kestrel)
1. Stop the target IIS Web Site or systemd service (`sudo systemctl stop amlakbashi`).
2. Backup existing binaries in target deployment folder.
3. Copy contents of `./publish` directory to the target web root folder (e.g., `C:\inetpub\amlakbashi`).
4. Verify media directories exist: `wwwroot/content/users/`, `wwwroot/content/licenses/`, `wwwroot/content/advertise/`, `wwwroot/content/videos/`.
5. Start IIS Web Site or systemd service (`sudo systemctl start amlakbashi`).

## 6. Rollback Procedure
1. Stop IIS Web Site or systemd service.
2. Restore target web root directory from the pre-deployment backup.
3. Restart IIS Web Site / systemd service.

## 7. Smoke Tests
- Navigation: `GET /` -> Returns HTTP 200 (Home page).
- Property Details: `GET /Accomodation/Item/{id}` -> Returns HTTP 200 with "نمایش شماره تماس میزبان" button.
- Lead Generation API: `POST /Accomodation/ShowMobile` -> Returns host contact details.
