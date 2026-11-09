# Post-deployment smoke test

1. Run `.\deploy\smoke-test.ps1` and confirm all checks PASS.
2. Open the Vercel site, sign in, and confirm the dashboard loads with no Console errors.
3. Open an employee, then refresh the page (deep link works).
4. As admin: create a test employee, edit it, delete it.
5. Check the newest `logs/api-*.log` in the MonsterASP.NET File Manager:
   - `Hosting environment: Production` after the latest restart
   - No `[ERR]` or `[FTL]` entries from the test
   - Request lines end with `from <your public IP>`