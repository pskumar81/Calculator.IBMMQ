#!/bin/bash

# IBM MQ Initialization Script for Calculator Application
# This script runs after the MQ container starts and sets up the required queues

echo "===========================================" 
echo "IBM MQ Calculator Setup Script"
echo "==========================================="

# Wait for Queue Manager to be fully available
echo "Waiting for Queue Manager CALC_QM to be available..."
until dspmq -m CALC_QM | grep -q "RUNNING"; do
    echo "  Queue Manager not ready yet, waiting 5 seconds..."
    sleep 5
done

echo "✅ Queue Manager CALC_QM is running!"

# Setup MQ objects
echo "🔧 Setting up IBM MQ objects..."

runmqsc CALC_QM << 'EOF'
* Calculator Application MQ Setup
* ================================

* Define local queues for calculator requests and responses
DEFINE QLOCAL(CALC.REQUEST) MAXDEPTH(5000) MAXMSGL(4194304) REPLACE
DEFINE QLOCAL(CALC.RESPONSE) MAXDEPTH(5000) MAXMSGL(4194304) REPLACE

* Define dead letter queue for failed messages  
DEFINE QLOCAL(CALC.DEAD.LETTER) MAXDEPTH(5000) MAXMSGL(4194304) REPLACE

* Define server connection channel for calculator applications
DEFINE CHANNEL(CALC.SVRCONN) CHLTYPE(SVRCONN) TRPTYPE(TCP) MAXINST(10) MAXINSTC(5) REPLACE

* Set channel authentication to allow connections
SET CHLAUTH(CALC.SVRCONN) TYPE(ADDRESSMAP) ADDRESS(*) USERSRC(CHANNEL) CHCKCLNT(ASQMGR) ACTION(REPLACE)

* Create application user and set permissions
SET AUTHREC PRINCIPAL('app') OBJTYPE(QMGR) AUTHADD(CONNECT,INQ)
SET AUTHREC PROFILE(CALC.*) PRINCIPAL('app') OBJTYPE(QUEUE) AUTHADD(BROWSE,GET,INQ,PUT)
SET AUTHREC PROFILE(SYSTEM.DEFAULT.*) PRINCIPAL('app') OBJTYPE(QUEUE) AUTHADD(GET)

* Start the listener on default port 1414
START LISTENER(SYSTEM.DEFAULT.LISTENER.TCP)

* Display created objects for verification
DISPLAY QLOCAL(CALC.*)
DISPLAY CHANNEL(CALC.SVRCONN)  
DISPLAY LISTENER(SYSTEM.DEFAULT.LISTENER.TCP)

* Show queue manager status
DISPLAY QMGR
EOF

echo ""
echo "✅ IBM MQ setup completed successfully!"
echo ""
echo "📋 Configuration Summary:"
echo "  Queue Manager: CALC_QM"
echo "  Queues:"
echo "    - CALC.REQUEST (for calculation requests)"
echo "    - CALC.RESPONSE (for calculation responses)" 
echo "    - CALC.DEAD.LETTER (for failed messages)"
echo "  Channel: CALC.SVRCONN"
echo "  Port: 1414"
echo "  User: app / passw0rd"
echo ""
echo "🌐 IBM MQ Web Console:"
echo "  URL: https://localhost:9443/ibmmq/console"
echo "  Username: admin"
echo "  Password: passw0rd"
echo ""
echo "🚀 Calculator applications can now connect to IBM MQ!"