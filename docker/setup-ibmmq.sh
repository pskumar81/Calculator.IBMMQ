#!/bin/bash

# IBM MQ Setup Script for Calculator Application
# This script creates the required queues and sets up permissions

echo "==========================================="
echo "IBM MQ Calculator Setup Script"
echo "==========================================="
echo ""

# Wait for MQ to be ready
echo "Waiting for IBM MQ to be ready..."
sleep 5

# Create queues
echo -e "\nCreating CALC.REQUEST queue..."
echo "DEFINE QLOCAL(CALC.REQUEST) MAXDEPTH(5000) MAXMSGL(4194304) REPLACE" | docker exec -i calculator-ibm-mq runmqsc CALC_QM

echo -e "\nCreating CALC.RESPONSE queue..."
echo "DEFINE QLOCAL(CALC.RESPONSE) MAXDEPTH(5000) MAXMSGL(4194304) REPLACE" | docker exec -i calculator-ibm-mq runmqsc CALC_QM

# Configure security
echo -e "\nConfiguring channel authentication..."
docker exec -i calculator-ibm-mq runmqsc CALC_QM << 'EOF'
SET CHLAUTH(DEV.APP.SVRCONN) TYPE(ADDRESSMAP) ADDRESS(*) USERSRC(CHANNEL) ACTION(REPLACE)
ALTER AUTHINFO(SYSTEM.DEFAULT.AUTHINFO.IDPWOS) AUTHTYPE(IDPWOS) CHCKCLNT(OPTIONAL)
REFRESH SECURITY TYPE(CONNAUTH)
EOF

# Set queue permissions
echo -e "\nSetting queue permissions..."
user=$(whoami)
docker exec -i calculator-ibm-mq runmqsc CALC_QM << EOF
SET AUTHREC OBJTYPE(QMGR) PRINCIPAL('$user') AUTHADD(ALL)
SET AUTHREC PROFILE('CALC.**') OBJTYPE(QUEUE) PRINCIPAL('$user') AUTHADD(ALL)
SET AUTHREC PROFILE('SYSTEM.**') OBJTYPE(QUEUE) PRINCIPAL('$user') AUTHADD(ALL)
EOF

# Verify setup
echo -e "\nVerifying queue creation..."
echo "DISPLAY QLOCAL(CALC.*)" | docker exec -i calculator-ibm-mq runmqsc CALC_QM

echo -e "\n==========================================="
echo "IBM MQ Setup Complete!"
echo "==========================================="
echo -e "\nYou can now run:"
echo "  1. Calculator.Server: cd Calculator.Server && dotnet run"
echo "  2. Calculator.Client: cd Calculator.Client && dotnet run"
