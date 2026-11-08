## Generating a JWT signing key
$bytes = New-Object byte[] 64
[System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
[Convert]::ToBase64String($bytes)

Use a different key per environment. Rotating the key signs everyone out.

## Production checklist
- [ ] appsettings.Production.json created from the example file (never committed)
- [ ] Connection string copied from the MonsterASP.NET control panel
- [ ] New JWT signing key generated for production
- [ ] Cors:AllowedOrigins = Vercel production URL (https, no trailing slash)
- [ ] AllowedHosts = the runasp.net host name
- [ ] VITE_API_BASE_URL set in Vercel to the API URL