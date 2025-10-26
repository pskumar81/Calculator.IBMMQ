# Calculator.IBMMQ

A .NET 9.0 distributed calculator system using **real IBM MQ** for enterprise-grade message-based communication between client and server components.

## 🎯 Overview

This project demonstrates production-ready messaging architecture using **IBM MQ with Docker**, where a client application sends calculation requests to a server application via real IBM MQ queues. The server processes the mathematical operations and sends responses back through IBM MQ.

## 🏗️ Architecture

The solution consists of four main projects:

### Calculator.Server
- **Purpose**: Background service that processes calculation requests
- **Technology**: .NET 9.0 Background Service with **real IBM MQ consumer**
- **Features**: 
  - Connects to real IBM MQ Queue Manager
  - Listens to IBM MQ request queue (`CALC.REQUEST`)
  - Performs mathematical operations (Add, Subtract, Multiply, Divide)
  - Sends responses via IBM MQ response queue (`CALC.RESPONSE`)
  - Comprehensive IBM MQ error handling and logging

### Calculator.Client
- **Purpose**: Interactive console application for sending calculation requests
- **Technology**: .NET 9.0 Console Application with **real IBM MQ producer**
- **Features**:
  - Interactive menu for mathematical operations
  - Real IBM MQ connection and messaging
  - Request/response correlation tracking
  - Timeout handling for server responses
  - User-friendly error reporting

### Calculator.Shared
- **Purpose**: Shared models and utilities
- **Technology**: .NET 9.0 Class Library
- **Features**: Common data models and IBM MQ simulation for testing

### Calculator.Tests
- **Purpose**: Unit tests for core calculation logic
- **Technology**: xUnit testing framework with Moq
- **Coverage**: Comprehensive tests for all mathematical operations and edge cases

## ✨ Features

- **🔗 Real IBM MQ Integration**: Uses actual IBM MQ client libraries (`IBM.MQSeries.Client`)
- **🐳 Docker Support**: Complete Docker Compose setup with IBM MQ container
- **🧮 Mathematical Operations**: Addition, subtraction, multiplication, and division
- **🛡️ Error Handling**: Division by zero protection and comprehensive IBM MQ error reporting
- **🔍 Correlation Tracking**: Request/response correlation for debugging and monitoring
- **⚙️ Configuration**: Flexible configuration for different environments (Local, Docker)
- **📋 Logging**: Structured logging with configurable levels
- **🧪 Testing**: Comprehensive unit test suite with high coverage

## 🚀 Quick Start with Docker

### Prerequisites

- Docker and Docker Compose
- .NET 9.0 SDK (for local development)

### Option 1: Full Docker Setup (Recommended)

1. **Clone the repository**
   ```bash
   git clone https://github.com/pskumar81/Calculator.IBMMQ.git
   cd Calculator.IBMMQ
   ```

2. **Start the entire system with Docker Compose**
   ```bash
   docker-compose up --build
   ```

   This will:
   - Start IBM MQ container with Queue Manager `CALC_QM`
   - Create necessary queues (`CALC.REQUEST`, `CALC.RESPONSE`)
   - Build and start the Calculator Server
   - Build and start the Calculator Client

3. **Access IBM MQ Web Console**
   - URL: https://localhost:9443/ibmmq/console
   - Username: `admin`
   - Password: `passw0rd`

### Option 2: IBM MQ in Docker + Local .NET Applications

1. **Start only IBM MQ with Docker**
   ```bash
   docker-compose up ibm-mq
   ```

2. **Wait for IBM MQ to be ready** (check container logs)
   ```bash
   docker-compose logs -f ibm-mq
   ```

3. **Run applications locally**
   ```bash
   # Terminal 1: Start the server
   cd Calculator.Server
   dotnet run

   # Terminal 2: Start the client
   cd Calculator.Client
   dotnet run
   ```

## 🔧 Configuration

### IBM MQ Settings

The applications use different configurations for Local vs Docker environments:

#### Local Development (`appsettings.json`)
```json
{
  "IBMMQ": {
    "QueueManagerName": "CALC_QM",
    "HostName": "localhost",
    "Port": 1414,
    "Channel": "CALC.SVRCONN",
    "UserName": "app",
    "Password": "passw0rd",
    "RequestQueueName": "CALC.REQUEST",
    "ResponseQueueName": "CALC.RESPONSE"
  }
}
```

#### Docker Environment (`appsettings.Docker.json`)
```json
{
  "IBMMQ": {
    "QueueManagerName": "CALC_QM",
    "HostName": "calculator-ibm-mq",
    "Port": 1414,
    "Channel": "CALC.SVRCONN",
    "UserName": "app", 
    "Password": "passw0rd",
    "RequestQueueName": "CALC.REQUEST",
    "ResponseQueueName": "CALC.RESPONSE"
  }
}
```

## 🏃‍♂️ Local Development Setup

### Prerequisites

- .NET 9.0 SDK
- Docker (for IBM MQ)

### Setup Steps

1. **Start IBM MQ with Docker**
   ```bash
   docker run --name ibm-mq \
     -e LICENSE=accept \
     -e MQ_QMGR_NAME=CALC_QM \
     -e MQ_APP_PASSWORD=passw0rd \
     -p 1414:1414 \
     -p 9443:9443 \
     -d ibmcom/mq:latest
   ```

2. **Setup IBM MQ Queues**
   ```bash
   docker exec -it ibm-mq runmqsc CALC_QM
   ```
   ```
   DEFINE QLOCAL(CALC.REQUEST) MAXDEPTH(5000)
   DEFINE QLOCAL(CALC.RESPONSE) MAXDEPTH(5000)
   DEFINE CHANNEL(CALC.SVRCONN) CHLTYPE(SVRCONN)
   SET CHLAUTH(CALC.SVRCONN) TYPE(ADDRESSMAP) ADDRESS(*) USERSRC(CHANNEL)
   START LISTENER(SYSTEM.DEFAULT.LISTENER.TCP)
   ```

3. **Restore and Build**
   ```bash
   dotnet restore
   dotnet build
   ```

4. **Run Tests**
   ```bash
   dotnet test
   ```

5. **Start Server**
   ```bash
   cd Calculator.Server
   dotnet run
   ```

6. **Start Client**
   ```bash
   cd Calculator.Client
   dotnet run
   ```

## IBM MQ Setup

### Using Docker (Recommended for Development)

```bash
# Run IBM MQ in Docker
docker run --env LICENSE=accept --env MQ_QMGR_NAME=QM1 --publish 1414:1414 --publish 9443:9443 --detach --env MQ_APP_PASSWORD=passw0rd ibmcom/mq:latest

# Create required queues (optional - queues will be auto-created if they don't exist)
docker exec -it <container_id> runmqsc QM1
DEFINE QLOCAL(CALC.REQUEST.QUEUE)
DEFINE QLOCAL(CALC.RESPONSE.QUEUE)
quit
```

### Local IBM MQ Installation

1. Install IBM MQ Server on your local machine
2. Create a Queue Manager named `QM1`
3. Create the following queues:
   - `CALC.REQUEST.QUEUE`
   - `CALC.RESPONSE.QUEUE`
4. Configure the `DEV.APP.SVRCONN` channel

## Configuration

### IBM MQ Settings

Both server and client applications use similar configuration in their `appsettings.json` files:

```json
{
  "IBMMQ": {
    "QueueManagerName": "QM1",
    "HostName": "localhost",
    "Port": 1414,
    "Channel": "DEV.APP.SVRCONN",
    "RequestQueueName": "CALC.REQUEST.QUEUE",
    "ResponseQueueName": "CALC.RESPONSE.QUEUE",
    "ConnectionTimeout": 30000,
    "ApplicationName": "Calculator.Server"
  }
}
```

### Environment-Specific Configurations

- `appsettings.json` - Default configuration
- `appsettings.Development.json` - Development environment with debug logging
- `appsettings.Docker.json` - Docker environment configuration

## Building and Running

### Building the Solution

```bash
# Clone the repository
git clone https://github.com/pskumar81/Calculator.IBMMQ.git
cd Calculator.IBMMQ

# Build the solution
dotnet build

# Run tests
dotnet test
```

### Running the Applications

#### Start the Server (First)

```bash
cd Calculator.Server
dotnet run
```

The server will:
- Connect to IBM MQ
- Start listening for calculation requests
- Log all operations to the console

#### Start the Client

```bash
cd Calculator.Client
dotnet run
```

The client will:
- Display an interactive menu
- Allow you to perform calculations
- Show results from the server

## Usage Example

1. Start the IBM MQ server (Docker or local installation)
2. Launch the Calculator.Server application
3. Launch the Calculator.Client application
4. Follow the interactive prompts in the client:

```
=== IBM MQ Calculator Client ===
Choose an operation:
1. Add
2. Subtract
3. Multiply
4. Divide
5. Exit
Enter your choice (1-5): 1
Enter first number: 10
Enter second number: 5
Sending request to server...

Addition Result:
10 + 5 = 15
```

## Supported Operations

- **Addition**: Adds two numbers
- **Subtraction**: Subtracts the second number from the first
- **Multiplication**: Multiplies two numbers
- **Division**: Divides the first number by the second (with division by zero protection)

## Message Format

### Request Message
```json
{
  "operand1": 10,
  "operand2": 5,
  "operation": "Add",
  "correlationId": "unique-guid",
  "replyTo": "CALC.RESPONSE.QUEUE"
}
```

### Response Message
```json
{
  "result": 15,
  "success": true,
  "errorMessage": null,
  "correlationId": "unique-guid"
}
```

## Error Handling

The system includes comprehensive error handling for:

- **Division by zero**: Returns error response instead of throwing exception
- **IBM MQ connection issues**: Automatic retry logic and proper error reporting
- **Message serialization errors**: Graceful handling of malformed messages
- **Network timeouts**: Configurable timeout handling for client requests
- **Invalid operations**: Proper validation of input parameters

## Logging

Both applications use structured logging with different levels:

- **Trace**: Detailed message flow (Development only)
- **Debug**: Detailed processing information
- **Information**: Normal operation events
- **Warning**: Non-critical issues
- **Error**: Error conditions and exceptions

## Testing

The solution includes comprehensive unit tests:

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "FullyQualifiedName~CalculatorServiceTests"
```

### Test Coverage

- Mathematical operations (all four basic operations)
- Edge cases (division by zero, negative numbers, decimals)
- Error conditions and validation
- Correlation ID tracking

## Architecture Decisions

### Why IBM MQ?

- **Enterprise-grade messaging**: Proven reliability and scalability
- **Message persistence**: Messages are stored until processed
- **Guaranteed delivery**: Ensures no calculation requests are lost
- **Correlation tracking**: Built-in correlation ID support
- **Security**: Enterprise authentication and authorization features

### Design Patterns Used

- **Publisher-Subscriber**: Client publishes requests, server subscribes to process them
- **Request-Response**: Correlation IDs track request/response pairs
- **Background Service**: Server runs as a long-lived background service
- **Dependency Injection**: Clean separation of concerns and testability
- **Configuration Pattern**: Environment-specific settings management

## Deployment

### Docker Support

Future enhancement will include Docker containers for both client and server applications with docker-compose orchestration.

### Production Considerations

- Configure IBM MQ clustering for high availability
- Implement message persistence for durability
- Set up monitoring and alerting for queue depths
- Configure proper security credentials and SSL/TLS
- Implement dead letter queue handling
- Set up log aggregation and monitoring

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

- **Author**: Pradeep Kumar
- **GitHub**: [@pskumar81](https://github.com/pskumar81)
- **Repository**: [Calculator.IBMMQ](https://github.com/pskumar81/Calculator.IBMMQ)

## Acknowledgments

- IBM MQ team for the excellent messaging platform
- .NET team for the powerful development framework
- xUnit team for the testing framework