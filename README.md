# Calculator.IBMMQ

A .NET 9.0 distributed calculator system using **real IBM MQ** for enterprise-grade message-based communication between client and server components.

## 🎯 Overview

This project demonstrates production-ready messaging architecture using **IBM MQ with Docker**, where a client application sends calculation requests to a server application via real IBM MQ queues. The server processes the mathematical operations and sends responses back through IBM MQ.

## 🏗️ Architecture

The solution consists of three main projects:

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

### Calculator.Tests
- **Purpose**: Unit tests for core calculation logic
- **Technology**: xUnit testing framework with Moq
- **Coverage**: Comprehensive tests for all mathematical operations and edge cases

## ✨ Features

- **🔗 Real IBM MQ Integration**: Uses actual IBM MQ client libraries (`IBMMQDotnetClient v9.4.4`)
- **🐳 Docker Support**: Complete Docker Compose setup with IBM MQ container
- **🧮 Mathematical Operations**: Addition, subtraction, multiplication, and division
- **🛡️ Error Handling**: Division by zero protection and comprehensive IBM MQ error reporting
- **🔍 Correlation Tracking**: Request/response correlation for debugging and monitoring
- **⚙️ Configuration**: Flexible configuration using appsettings.json
- **📋 Logging**: Structured logging with configurable levels
- **🧪 Testing**: Comprehensive unit test suite with high coverage

## 🚀 Quick Start

### Prerequisites

- **Docker and Docker Compose** - For running IBM MQ
- **.NET 9.0 SDK** - For running the Calculator applications
- **Git** - For cloning the repository

### Setup Steps

1. **Clone the repository**
   ```bash
   git clone https://github.com/pskumar81/Calculator.IBMMQ.git
   cd Calculator.IBMMQ
   ```

2. **Start IBM MQ with Docker**
   ```bash
   docker-compose up -d ibm-mq
   ```

3. **Run the IBM MQ Setup Script**
   
   This script creates the required queues and configures security automatically.

   **Windows (PowerShell):**
   ```powershell
   .\docker\setup-ibmmq.ps1
   ```

   **Linux/Mac (Bash):**
   ```bash
   chmod +x docker/setup-ibmmq.sh
   ./docker/setup-ibmmq.sh
   ```

   The script will:
   - Create `CALC.REQUEST` and `CALC.RESPONSE` queues
   - Configure channel authentication for `DEV.APP.SVRCONN`
   - Set up proper queue permissions
   - Verify the setup

4. **Run the applications**
   
   **Terminal 1 - Start the Server:**
   ```bash
   cd Calculator.Server
   dotnet run
   ```

   **Terminal 2 - Start the Client:**
   ```bash
   cd Calculator.Client
   dotnet run
   ```

5. **Test the calculator**
   - Choose an operation (Add, Subtract, Multiply, Divide)
   - Enter two numbers
   - See the result returned via IBM MQ!

### Access IBM MQ Web Console

- **URL**: https://localhost:9443/ibmmq/console
- **Username**: `admin`
- **Password**: `passw0rd`
- Monitor queues, messages, and connections in real-time

## 🔧 Configuration

### IBM MQ Connection Settings

The applications connect to IBM MQ using these settings:

```json
{
  "IBMMQ": {
    "QueueManagerName": "CALC_QM",
    "HostName": "localhost",
    "Port": 1414,
    "Channel": "DEV.APP.SVRCONN",
    "RequestQueueName": "CALC.REQUEST",
    "ResponseQueueName": "CALC.RESPONSE"
  }
}
```

**Key Points:**
- **Queue Manager**: `CALC_QM` (created automatically by Docker)
- **Channel**: `DEV.APP.SVRCONN` (IBM MQ development channel with relaxed auth)
- **Port**: `1414` (standard IBM MQ port)
- **Transport**: Uses `TRANSPORT_MQSERIES_MANAGED` for cross-platform compatibility
- **Authentication**: No credentials required for development mode

### Docker Environment Variables

When running in Docker, the `docker-compose.yml` configures:

```yaml
environment:
  - LICENSE=accept
  - MQ_QMGR_NAME=CALC_QM
  - MQ_DEV=true              # Enables development mode
  - MQ_ADMIN_PASSWORD=passw0rd
```

## 🏗️ Architecture

See the [C4 Architecture Diagrams](docs/architecture-c4.md) for detailed system architecture including:
- System Context Diagram
- Container Diagram
- Component Diagrams (Client & Server)
- Deployment Diagram
- Message Flow Documentation

## 🧪 Running Tests

```bash
dotnet test
```

The test suite includes comprehensive unit tests for all calculation operations and edge cases.

## 🐛 Troubleshooting

### IBM MQ Connection Issues

**Error 2035 (MQRC_NOT_AUTHORIZED)**
- **Cause**: Insufficient permissions to access queues
- **Solution**: Re-run the setup script: `.\docker\setup-ibmmq.ps1` (Windows) or `./docker/setup-ibmmq.sh` (Linux/Mac)
- **Alternative**: The script grants permissions to your current Windows user. If connecting as a different user, update the script with the correct username.

**Error 2059 (MQRC_Q_MGR_NOT_AVAILABLE)**
- **Cause**: IBM MQ container not running or not ready
- **Solution**: 
  ```bash
  docker ps  # Check if calculator-ibm-mq is running and healthy
  docker logs calculator-ibm-mq  # Check for startup errors
  ```

**Error 2085 (MQRC_UNKNOWN_OBJECT_NAME)**
- **Cause**: Queues don't exist
- **Solution**: Run the setup script to create queues

### Fresh Setup

If you encounter persistent issues, do a complete cleanup and restart:

**Windows (PowerShell):**
```powershell
# Stop and remove everything including volumes
docker-compose down -v

# Start IBM MQ
docker-compose up -d ibm-mq

# Wait 30 seconds for MQ to initialize
Start-Sleep -Seconds 30

# Run setup script
.\docker\setup-ibmmq.ps1

# Start applications
cd Calculator.Server; dotnet run
```

**Linux/Mac (Bash):**
```bash
# Stop and remove everything including volumes
docker-compose down -v

# Start IBM MQ
docker-compose up -d ibm-mq

# Wait for MQ to initialize
sleep 30

# Run setup script
./docker/setup-ibmmq.sh

# Start applications
cd Calculator.Server && dotnet run
```

## 📋 What Gets Created

### On First Setup

When you run the setup script, it creates:

1. **Queues**:
   - `CALC.REQUEST` - Server consumes calculation requests from here
   - `CALC.RESPONSE` - Client receives results from here

2. **Security Configuration**:
   - Channel authentication for `DEV.APP.SVRCONN`
   - Queue permissions for your user account
   - Connection authentication set to OPTIONAL for development

3. **Docker Volume**:
   - `calculatoribmmq_mq-data` - Persists queue manager data
   - Queues and configuration survive container restarts
   - Only deleted with `docker-compose down -v`

### On Container Restart

- ✅ Queue Manager automatically starts
- ✅ Queues are loaded from the volume (no recreation needed)
- ✅ Channel `DEV.APP.SVRCONN` exists (created by `MQ_DEV=true`)
- ⚠️ **Permissions may need to be reapplied** - Run the setup script if you get error 2035

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

## 🔮 Future Improvements

This section outlines potential enhancements to make the system more production-ready and robust.

### 🔴 High Priority

#### 1. Retry Logic & Resilience
**Current State**: No retry mechanism for IBM MQ connection failures  
**Issue**: If IBM MQ is temporarily unavailable, the application fails immediately  
**Recommendation**:
- Implement **Polly** library for retry policies with exponential backoff
- Add circuit breaker pattern for MQ connections
- Implement automatic reconnection logic in `IBMMQConnectionService`
- Handle transient failures gracefully

**Example Implementation**:
```csharp
services.AddSingleton<IAsyncPolicy>(provider =>
{
    return Policy
        .Handle<MQException>()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
            onRetry: (exception, timeSpan, retry, ctx) =>
            {
                logger.LogWarning($"Retry {retry} after {timeSpan.TotalSeconds}s due to: {exception.Message}");
            });
});
```

#### 2. Health Checks
**Current State**: No health check endpoints  
**Missing**: No way to monitor if server is processing messages or if MQ connection is healthy  
**Recommendation**:
- Add ASP.NET Core Health Checks with custom MQ health check
- Expose `/health` endpoint showing:
  - IBM MQ connection status
  - Queue depths and capacity
  - Last message processed timestamp
  - Service availability and uptime
- Integrate with monitoring tools (Azure Monitor, Prometheus, etc.)

**Example Implementation**:
```csharp
services.AddHealthChecks()
    .AddCheck<IBMMQHealthCheck>("ibmmq")
    .AddCheck<QueueDepthHealthCheck>("queue_depth");

app.MapHealthChecks("/health");
```

### 🟡 Medium Priority

#### 3. Metrics & Monitoring
**Current State**: Only basic logging, no metrics collection  
**Recommendation**:
- Add **Application Insights** or **Prometheus** metrics:
  - Messages processed per second
  - Average processing time per operation
  - Error rates and failure types
  - Queue depths over time
  - Connection failures and retry counts
- Implement distributed tracing with correlation IDs
- Add performance counters for system resources

#### 4. Dead Letter Queue Handling
**Current State**: DLQ created but not actively utilized  
**Issue**: Failed messages are lost without proper handling  
**Recommendation**:
- Implement automatic DLQ routing for:
  - JSON deserialization failures
  - Business logic exceptions
  - Maximum retry exhaustion
- Add DLQ monitoring and alerting
- Create admin tool for DLQ message inspection and reprocessing
- Log detailed error context with each DLQ message

#### 5. Integration Tests
**Current State**: Only unit tests for `CalculatorService`  
**Missing**: No integration tests with real IBM MQ, no end-to-end tests  
**Recommendation**:
- Add **Testcontainers** for IBM MQ in integration tests
- Test complete message flow: Client → MQ → Server → MQ → Client
- Test error scenarios (timeout, connection loss, invalid messages)
- Add load testing for throughput validation
- Test security and authentication scenarios

**Example Test Structure**:
```csharp
public class IntegrationTests : IAsyncLifetime
{
    private readonly IBMMQContainer _mqContainer = new IBMMQBuilder()
        .WithQueueManager("TEST_QM")
        .Build();

    public async Task InitializeAsync() => await _mqContainer.StartAsync();
    
    [Fact]
    public async Task Should_ProcessCalculationRequest_Successfully()
    {
        // Arrange, Act, Assert with real MQ container
    }
}
```

#### 6. Security Enhancements
**Current State**: Development mode with minimal authentication  
**Issues**: Passwords in config files, no TLS/SSL, overly permissive queue access  
**Recommendation**:
- Use **Azure Key Vault** or **HashiCorp Vault** for secrets management
- Implement TLS/SSL for production MQ connections
- Apply principle of least privilege for queue permissions
- Add user-specific authentication for production channels
- Implement API key or JWT authentication for client applications
- Enable MQ channel security with certificate-based auth

#### 7. Message Schema Versioning
**Current State**: No versioning strategy for messages  
**Issue**: Breaking changes will break client-server compatibility  
**Recommendation**:
- Add `Version` property to `CalculationRequest` and `CalculationResponse`
- Implement backward compatibility handling
- Support multiple message versions simultaneously
- Document API contract changes in release notes
- Use semantic versioning for message schemas

### 🟢 Low Priority

#### 8. Configuration Validation
**Current State**: No validation of configuration at startup  
**Issue**: Runtime failures if configuration is invalid or missing  
**Recommendation**:
- Add `IOptions<T>` validation with data annotations
- Validate configuration at startup using `IValidateOptions<T>`
- Fail fast with clear error messages
- Provide configuration examples and templates

**Example**:
```csharp
[Required]
[MinLength(1)]
public string QueueManagerName { get; set; } = string.Empty;

services.AddOptions<IBMMQConfiguration>()
    .Bind(configuration.GetSection("IBMMQ"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

#### 9. Performance Optimization
**Current State**: Functional but not optimized for high throughput  
**Opportunities**:
- Optimize queue access patterns (reduce open/close operations)
- Implement connection pooling for MQ connections
- Add message batching for high-volume scenarios
- Use async/await patterns more effectively
- Add performance benchmarks and profiling

#### 10. Enhanced Docker Configuration
**Current State**: Basic Docker setup working  
**Recommendations**:
- Add Docker secrets for sensitive configuration
- Implement container resource limits (CPU, memory)
- Add container restart policies
- Use Docker health checks
- Create separate Docker compose files for dev/prod
- Add volume management for persistent data

## 📊 Implementation Priority Matrix

| Priority | Improvement | Effort | Impact | Quick Win |
|----------|------------|--------|--------|-----------|
| 🔴 HIGH | Retry Logic & Resilience | Medium | High | ✅ |
| 🔴 HIGH | Health Checks | Low | High | ✅ |
| 🟡 MEDIUM | Metrics & Monitoring | Medium | Medium | |
| 🟡 MEDIUM | DLQ Handling | Low | High | ✅ |
| 🟡 MEDIUM | Integration Tests | High | High | |
| 🟡 MEDIUM | Security Enhancements | Medium | High | |
| 🟢 LOW | Config Validation | Low | Low | ✅ |
| 🟢 LOW | Message Versioning | Low | Medium | |
| 🟢 LOW | Performance Optimization | Medium | Low | |
| 🟢 LOW | Docker Improvements | Low | Low | |

**Quick Wins** (✅) can be implemented in 1-2 hours and provide immediate value.

## 🚀 Getting Started with Improvements

If you'd like to contribute to these improvements:

1. Review the priority matrix above
2. Pick an improvement that matches your skill level
3. Create an issue describing your implementation plan
4. Fork the repository and create a feature branch
5. Implement with tests and documentation
6. Submit a pull request

We welcome contributions that make this system more production-ready!

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