#!/bin/bash
echo "Setting up IBM MQ queues for Calculator application..."

# Wait for Queue Manager to be available
while ! dspmq -m CALC_QM | grep -q "RUNNING"; do
    echo "Waiting for Queue Manager CALC_QM to be running..."
    sleep 5
done

echo "Queue Manager CALC_QM is running. Creating queues..."

# Create MQ objects using runmqsc
runmqsc CALC_QM << EOF
* Define local queues for Calculator application
DEFINE QLOCAL(CALC.REQUEST) MAXDEPTH(5000) REPLACE
DEFINE QLOCAL(CALC.RESPONSE) MAXDEPTH(5000) REPLACE

* Define dead letter queue
DEFINE QLOCAL(CALC.DEAD.LETTER) MAXDEPTH(5000) REPLACE

* Define server connection channel
DEFINE CHANNEL(CALC.SVRCONN) CHLTYPE(SVRCONN) TRPTYPE(TCP) REPLACE

* Set channel authentication (allow connections)
SET CHLAUTH(CALC.SVRCONN) TYPE(ADDRESSMAP) ADDRESS(*) USERSRC(CHANNEL) CHCKCLNT(ASQMGR) ACTION(REPLACE)

* Set authority for application user
SET AUTHREC PRINCIPAL('app') OBJTYPE(QMGR) AUTHADD(CONNECT,INQ)
SET AUTHREC PROFILE(CALC.*) PRINCIPAL('app') OBJTYPE(QUEUE) AUTHADD(BROWSE,GET,INQ,PUT)

* Display created objects
DISPLAY QLOCAL(CALC.*)
DISPLAY CHANNEL(CALC.SVRCONN)

* Start channel listener
START LISTENER(SYSTEM.DEFAULT.LISTENER.TCP)
EOF

echo "IBM MQ setup completed successfully!"
echo "Queues created:"
echo "  - CALC.REQUEST (for calculation requests)"
echo "  - CALC.RESPONSE (for calculation responses)"
echo "  - CALC.DEAD.LETTER (for failed messages)"
echo "Channel: CALC.SVRCONN"
echo "Port: 1414"