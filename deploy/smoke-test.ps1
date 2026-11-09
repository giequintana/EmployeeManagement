param(
    [string]$ApiBaseUrl = "https://empmanagement-gq.runasp.net",
    [string]$FrontendUrl = "https://employee-management-liard-six.vercel.app",
    [string]$Username = "admin"
)

$script:results = [System.Collections.Generic.List[object]]::new()

function Test-Check {
    param([string]$Name, [scriptblock]$Test)
    try {
        $detail = & $Test
        $script:results.Add([pscustomobject]@{ Check = $Name; Result = 'PASS'; Detail = $detail })
    }
    catch {
        $script:results.Add([pscustomobject]@{ Check = $Name; Result = 'FAIL'; Detail = $_.Exception.Message })
    }
}

function Get-StatusCode([string]$Url, [string[]]$ExtraArgs = @()) {
    curl.exe -s -o NUL -w "%{http_code}" @ExtraArgs $Url
}

function Get-Headers([string]$Url, [string[]]$ExtraArgs = @()) {
    (curl.exe -s -D - -o NUL @ExtraArgs $Url) -join "`n"
}

$apiHost = ([Uri]$ApiBaseUrl).Host
$origin = $FrontendUrl.TrimEnd('/')

Test-Check "HTTP redirects to HTTPS" {
    $code = Get-StatusCode "http://$apiHost/api/departments"
    if ($code -notin '301', '302', '307', '308') { throw "Expected a redirect, got $code" }
    "status $code"
}

Test-Check "API requires authentication" {
    $code = Get-StatusCode "$ApiBaseUrl/api/departments"
    if ($code -ne '401') { throw "Expected 401, got $code" }
    "status 401"
}

Test-Check "API security headers" {
    $headers = Get-Headers "$ApiBaseUrl/api/departments"
    foreach ($name in 'strict-transport-security', 'x-content-type-options', 'content-security-policy', 'no-store') {
        if ($headers -notmatch [regex]::Escape($name)) { throw "Missing $name" }
    }
    "HSTS, nosniff, CSP, no-store"
}

Test-Check "Swagger not exposed" {
    $code = Get-StatusCode "$ApiBaseUrl/swagger/index.html"
    if ($code -eq '200') { throw "Swagger UI is publicly reachable" }
    "status $code"
}

Test-Check "CORS allows the frontend" {
    $headers = Get-Headers "$ApiBaseUrl/api/auth/login" @(
        '-X', 'OPTIONS',
        '-H', "Origin: $origin",
        '-H', 'Access-Control-Request-Method: POST',
        '-H', 'Access-Control-Request-Headers: content-type')
    if ($headers -notmatch "access-control-allow-origin: $([regex]::Escape($origin))") { throw "Origin not allowed" }
    "preflight allowed for $origin"
}

Test-Check "Frontend loads" {
    $code = Get-StatusCode $FrontendUrl
    if ($code -ne '200') { throw "Expected 200, got $code" }
    "status 200"
}

Test-Check "Frontend deep link (SPA rewrite)" {
    $code = Get-StatusCode "$FrontendUrl/employees/1"
    if ($code -ne '200') { throw "Expected 200, got $code" }
    "status 200"
}

Test-Check "Frontend CSP allows only the API" {
    $headers = Get-Headers $FrontendUrl
    if ($headers -notmatch "connect-src[^;]*https://$([regex]::Escape($apiHost))") { throw "connect-src missing the HTTPS API host" }
    if ($headers -match "connect-src[^;]*http://") { throw "connect-src still allows plain HTTP" }
    "connect-src restricted to HTTPS API"
}

Test-Check "Sign in and load dashboard" {
    $securePassword = Read-Host "Password for $Username" -AsSecureString
    $body = @{
        username = $Username
        password = [System.Net.NetworkCredential]::new('', $securePassword).Password
    } | ConvertTo-Json

    $login = Invoke-RestMethod -Method Post -Uri "$ApiBaseUrl/api/auth/login" -ContentType 'application/json' -Body $body
    $summary = Invoke-RestMethod -Uri "$ApiBaseUrl/api/dashboard/summary" -Headers @{ Authorization = "Bearer $($login.accessToken)" }
    "role $($login.user.role); $($summary.totalEmployees) employees in $($summary.totalDepartments) departments"
}

$script:results | Format-Table -AutoSize -Wrap
if ($script:results.Result -contains 'FAIL') { exit 1 }