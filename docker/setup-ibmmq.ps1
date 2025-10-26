# IBM MQ Setup Script for Calculator Application
# This script creates the required queues and sets up permissions

Write-Host "===========================================" -ForegroundColor Cyan
Write-Host "IBM MQ Calculator Setup Script" -ForegroundColor Cyan
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host ""

# Wait for MQ to be ready
Write-Host "Waiting for IBM MQ to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Create queues
Write-Host "`nCreating CALC.REQUEST queue..." -ForegroundColor Yellow
$command1 = "DEFINE QLOCAL(CALC.REQUEST) MAXDEPTH(5000) MAXMSGL(4194304) REPLACE"
$command1 | docker exec -i calculator-ibm-mq runmqsc CALC_QM

Write-Host "`nCreating CALC.RESPONSE queue..." -ForegroundColor Yellow
$command2 = "DEFINE QLOCAL(CALC.RESPONSE) MAXDEPTH(5000) MAXMSGL(4194304) REPLACE"
$command2 | docker exec -i calculator-ibm-mq runmqsc CALC_QM

# Configure security
Write-Host "`nConfiguring channel authentication..." -ForegroundColor Yellow
$securityCommands = @"
SET CHLAUTH(DEV.APP.SVRCONN) TYPE(ADDRESSMAP) ADDRESS(*) USERSRC(CHANNEL) ACTION(REPLACE)
ALTER AUTHINFO(SYSTEM.DEFAULT.AUTHINFO.IDPWOS) AUTHTYPE(IDPWOS) CHCKCLNT(OPTIONAL)
REFRESH SECURITY TYPE(CONNAUTH)
"@
$securityCommands | docker exec -i calculator-ibm-mq runmqsc CALC_QM

# Set queue permissions
Write-Host "`nSetting queue permissions..." -ForegroundColor Yellow
$user = $env:USERNAME
$permissionCommands = @"
SET AUTHREC OBJTYPE(QMGR) PRINCIPAL('$user') AUTHADD(ALL)
SET AUTHREC PROFILE('CALC.**') OBJTYPE(QUEUE) PRINCIPAL('$user') AUTHADD(ALL)
SET AUTHREC PROFILE('SYSTEM.**') OBJTYPE(QUEUE) PRINCIPAL('$user') AUTHADD(ALL)
"@
$permissionCommands | docker exec -i calculator-ibm-mq runmqsc CALC_QM

# Verify setup
Write-Host "`nVerifying queue creation..." -ForegroundColor Yellow
$verifyCommand = "DISPLAY QLOCAL(CALC.*)"
$verifyCommand | docker exec -i calculator-ibm-mq runmqsc CALC_QM

Write-Host "`n===========================================" -ForegroundColor Green
Write-Host "IBM MQ Setup Complete!" -ForegroundColor Green
Write-Host "===========================================" -ForegroundColor Green
Write-Host "`nYou can now run:" -ForegroundColor Cyan
Write-Host "  1. Calculator.Server: cd Calculator.Server; dotnet run" -ForegroundColor White
Write-Host "  2. Calculator.Client: cd Calculator.Client; dotnet run" -ForegroundColor White
