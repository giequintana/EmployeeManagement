# Deployment runbook

## Environments

| Part | Host | URL | Deploys |
|---|---|---|---|
| Frontend | Vercel (Hobby) | https://employee-management-liard-six.vercel.app | Automatic on push to `main` |
| API | MonsterASP.NET (Free) | https://empmanagement-gq.runasp.net | Manual, Visual Studio WebDeploy |
| Database | MonsterASP.NET MSSQL | db78619 | Manual, reviewed idempotent script |

## Release order for changes that touch the database

1. **Database:** apply the migration (must stay compatible with the currently running API).
2. **API:** publish.
3. **Frontend:** push to `main`.

Tag each release: `git tag v1.x.y && git push --tags`.

## Database changes

```powershell
dotnet ef migrations script --idempotent `
  --project src/EmployeeManagement.Infrastructure `
  --startup-project src/EmployeeManagement.API `
  --output deploy/migrations.sql
```

1. Review `deploy/migrations.sql` (only the expected changes; no drops of data you need).
2. Back up first: control panel backup if available, or SSMS **Generate Scripts** (schema and data) on `db78619`.
3. In SSMS, select `db78619` and execute the script. Re-running it is safe.
4. Verify `SELECT MigrationId FROM __EFMigrationsHistory`.

## API deployment

1. `dotnet test` passes.
2. `src/EmployeeManagement.API/appsettings.Production.json` is present locally with production values (never committed).
3. Visual Studio > right-click API > **Publish** (Release, framework-dependent, no EF migrations, keep extra files at destination).
4. Run `.\deploy\smoke-test.ps1`.

## Frontend deployment

Push to `main`. Vercel builds `client/` with `VITE_API_BASE_URL=https://empmanagement-gq.runasp.net`.
Changing environment variables requires a **Redeploy**.

## Rollback

| Part | How |
|---|---|
| Frontend | Vercel > Deployments > previous deployment > **Promote to Production** |
| API | `git checkout <previous tag>`, publish, `git checkout main` |
| Database | Prefer a forward fix. If a migration must be reverted: `dotnet ef migrations script <current> <previous>` generates the Down SQL; review, back up, apply |

## Secret rotation

| Secret | Steps | Effect |
|---|---|---|
| JWT signing key | Generate a new key, update `appsettings.Production.json`, publish | All users signed out |
| Database password | Change in control panel, update connection string, publish | Brief downtime between steps |
| Admin password | Delete the admin row, rerun the seeding command with new `SeedAdmin__*` variables | Admin must use the new password |

## Configuration formats

| Setting | Format |
|---|---|
| `AllowedHosts` | `empmanagement-gq.runasp.net` (host only) |
| `Cors:AllowedOrigins` | `https://employee-management-liard-six.vercel.app` (no trailing slash) |
| `VITE_API_BASE_URL` (Vercel) | `https://empmanagement-gq.runasp.net` (no trailing slash) |
| CSP `connect-src` (`client/vercel.json`) | `https://empmanagement-gq.runasp.net` |

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| HTTP 500.30 | Startup failure (configuration) | Read `logs/startup-*.log`; reproduce locally with `dotnet publish` + `dotnet EmployeeManagement.API.dll` in Production |
| 500.30 "Connection string not configured" | Production settings file not published | Check it's in the site root; check `.csproj` publish settings |
| 400 "Invalid Hostname" | `AllowedHosts` contains scheme, slash, or wrong host | Host name only |
| Requests go to `vercel.app/<api host>` | `VITE_API_BASE_URL` missing `https://` | Fix variable, redeploy |
| CSP or mixed-content block | API URL is `http://` | HTTPS only; certificate via MonsterASP.NET support on the Free plan |
| "blocked by CORS policy" | Vercel URL differs from `Cors:AllowedOrigins` | Update origin, publish API |
| `dotnet ef` "Unable to create a DbContext" | App startup failed in that environment | Run `dotnet run --no-launch-profile` in the same environment to see the real error |
| Slow first request | App or database idle on the free tier | Expected; EF retries cover transient connection errors |