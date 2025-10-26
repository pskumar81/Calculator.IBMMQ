# Calculator.IBMMQ - C4 Architecture Diagram

## System Context Diagram (Level 1)

```mermaid
C4Context
    title System Context Diagram - Calculator.IBMMQ

    Person(user, "User", "Sends calculation requests")
    
    System(calculator, "Calculator System", "Distributed calculator using IBM MQ for message-based communication")
    
    System_Ext(ibmmq, "IBM MQ", "Message Queue Manager (CALC_QM) running in Docker")
    
    Rel(user, calculator, "Sends calculation requests", "CLI")
    Rel(calculator, ibmmq, "Sends/Receives messages", "IBM MQ Protocol")
```

## Container Diagram (Level 2)

```mermaid
C4Container
    title Container Diagram - Calculator.IBMMQ

    Person(user, "User", "Initiates calculations")

    Container_Boundary(calculator, "Calculator System") {
        Container(client, "Calculator Client", ".NET 9.0 Console App", "Sends calculation requests and receives results")
        Container(server, "Calculator Server", ".NET 9.0 Background Service", "Processes calculation requests")
    }

    System_Ext(ibmmq, "IBM MQ Queue Manager", "Message broker (CALC_QM)")
    ContainerDb(reqQueue, "CALC.REQUEST", "IBM MQ Queue", "Stores calculation requests")
    ContainerDb(resQueue, "CALC.RESPONSE", "IBM MQ Queue", "Stores calculation responses")

    Rel(user, client, "Enters calculations", "Console")
    Rel(client, reqQueue, "Sends requests", "JSON/MQ")
    Rel(reqQueue, server, "Consumes requests", "JSON/MQ")
    Rel(server, resQueue, "Sends responses", "JSON/MQ")
    Rel(resQueue, client, "Receives responses", "JSON/MQ")
    
    Rel(reqQueue, ibmmq, "Managed by")
    Rel(resQueue, ibmmq, "Managed by")
```

## Component Diagram (Level 3) - Calculator Server

```mermaid
C4Component
    title Component Diagram - Calculator Server

    Container_Boundary(server, "Calculator.Server") {
        Component(bgService, "Calculator Server Background Service", "HostedService", "Orchestrates message consumption")
        Component(consumer, "IBM MQ Consumer Service", "Service", "Consumes messages from request queue")
        Component(calculator, "Calculator Service", "Service", "Performs arithmetic operations")
        Component(mqConnection, "IBM MQ Connection Service", "Service", "Manages IBM MQ connection and queue operations")
        
        ComponentDb(models, "Models", "POCOs", "CalculationRequest, CalculationResponse, IBMMQConfiguration")
    }

    ContainerDb(reqQueue, "CALC.REQUEST", "IBM MQ Queue")
    ContainerDb(resQueue, "CALC.RESPONSE", "IBM MQ Queue")
    System_Ext(ibmmq, "IBM MQ (Docker)", "Queue Manager")

    Rel(bgService, consumer, "Starts/Stops")
    Rel(consumer, mqConnection, "Uses")
    Rel(consumer, calculator, "Calls")
    Rel(consumer, models, "Deserializes/Serializes")
    
    Rel(mqConnection, reqQueue, "Get messages", "IBM.WMQ")
    Rel(mqConnection, resQueue, "Put messages", "IBM.WMQ")
    Rel(reqQueue, ibmmq, "Managed by")
    Rel(resQueue, ibmmq, "Managed by")
```

## Component Diagram (Level 3) - Calculator Client

```mermaid
C4Component
    title Component Diagram - Calculator Client

    Person(user, "User")

    Container_Boundary(client, "Calculator.Client") {
        Component(program, "Program", "Entry Point", "Interactive menu for calculations")
        Component(clientService, "Calculator Client Service", "Service", "Sends requests and waits for responses")
        Component(mqConnection, "IBM MQ Connection Service", "Service", "Manages IBM MQ connection and queue operations")
        
        ComponentDb(models, "Models", "POCOs", "CalculationRequest, CalculationResponse")
    }

    ContainerDb(reqQueue, "CALC.REQUEST", "IBM MQ Queue")
    ContainerDb(resQueue, "CALC.RESPONSE", "IBM MQ Queue")
    System_Ext(ibmmq, "IBM MQ (Docker)", "Queue Manager")

    Rel(user, program, "Interacts with")
    Rel(program, clientService, "Calls")
    Rel(clientService, mqConnection, "Uses")
    Rel(clientService, models, "Serializes/Deserializes")
    
    Rel(mqConnection, reqQueue, "Put messages", "IBM.WMQ")
    Rel(mqConnection, resQueue, "Get messages", "IBM.WMQ")
    Rel(reqQueue, ibmmq, "Managed by")
    Rel(resQueue, ibmmq, "Managed by")
```

## Deployment Diagram

```mermaid
C4Deployment
    title Deployment Diagram - Calculator.IBMMQ

    Deployment_Node(devMachine, "Developer Machine", "Windows/Linux/MacOS") {
        Deployment_Node(dotnet, ".NET 9.0 Runtime") {
            Container(client, "Calculator.Client", ".NET Console App", "Sends calculation requests")
            Container(server, "Calculator.Server", ".NET Background Service", "Processes calculations")
        }
    }

    Deployment_Node(docker, "Docker", "Container Platform") {
        Deployment_Node(mqContainer, "IBM MQ Container", "ibmcom/mq:latest") {
            ContainerDb(qmgr, "CALC_QM", "Queue Manager", "Manages message queues")
            ContainerDb(reqQueue, "CALC.REQUEST", "Queue", "Request messages")
            ContainerDb(resQueue, "CALC.RESPONSE", "Queue", "Response messages")
        }
    }

    Rel(client, reqQueue, "Sends requests", "TCP 1414")
    Rel(client, resQueue, "Receives responses", "TCP 1414")
    Rel(server, reqQueue, "Consumes requests", "TCP 1414")
    Rel(server, resQueue, "Sends responses", "TCP 1414")
```

## Technology Stack

### Calculator Client & Server
- **Framework**: .NET 9.0
- **Language**: C#
- **IBM MQ Client**: IBMMQDotnetClient v9.4.4
- **Serialization**: System.Text.Json
- **Logging**: Microsoft.Extensions.Logging

### IBM MQ Infrastructure
- **Container**: Docker (ibmcom/mq:latest)
- **Queue Manager**: CALC_QM
- **Channel**: DEV.APP.SVRCONN
- **Queues**: CALC.REQUEST, CALC.RESPONSE
- **Port**: 1414 (MQ), 9443 (Web Console)

## Message Flow

1. **User** enters calculation (e.g., "5 + 3") in **Calculator.Client**
2. **Client** creates `CalculationRequest` JSON message with:
   - Operand1, Operand2
   - Operation (Add/Subtract/Multiply/Divide)
   - CorrelationId (for response matching)
   - ReplyTo: "CALC.RESPONSE"
3. **Client** puts message on **CALC.REQUEST** queue
4. **Server** gets message from **CALC.REQUEST** queue
5. **Server** performs calculation
6. **Server** creates `CalculationResponse` JSON with result
7. **Server** puts response on **CALC.RESPONSE** queue
8. **Client** gets matching response using CorrelationId
9. **Client** displays result to **User**

## Key Design Decisions

1. **Message-Based Architecture**: Decouples client and server for scalability
2. **JSON Serialization**: Human-readable, easy to debug
3. **Correlation IDs**: Enables request-response matching in async messaging
4. **Docker for IBM MQ**: Easy local development, consistent environment
5. **No Simulation Code**: Production-ready, real IBM MQ integration only
6. **Background Service Pattern**: Server runs continuously processing messages
