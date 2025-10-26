# Calculator.Server

This is the IBM MQ-based calculator server that processes calculation requests from clients.

## Overview

The Calculator Server is a background service that:
- Connects to IBM MQ queue manager
- Listens for calculation requests on a designated queue
- Processes mathematical operations (Add, Subtract, Multiply, Divide)
- Sends responses back to clients via IBM MQ

## Features

- **IBM MQ Integration**: Uses IBM.WMQ library for enterprise-grade messaging
- **Asynchronous Processing**: Background service processes messages continuously
- **Error Handling**: Comprehensive error handling with proper logging
- **Input Validation**: Validates calculation requests and parameters
- **Correlation IDs**: Tracks requests and responses for debugging
- **Configuration**: Flexible configuration for different environments

## Architecture

The server consists of several key components:

### Services
- `CalculatorService`: Core business logic for mathematical operations
- `IBMMQConnectionService`: Manages IBM MQ connections and queue operations
- `IBMMQConsumerService`: Handles message consumption and processing
- `CalculatorServerBackgroundService`: Background service lifecycle management

### Models
- `CalculationRequest`: Represents incoming calculation requests
- `CalculationResponse`: Represents outgoing calculation results
- `CalculationOperation`: Enum defining supported operations
- `IBMMQConfiguration`: Configuration settings for IBM MQ connection

## Configuration

Configure IBM MQ settings in `appsettings.json`:

```json
{
  "IBMMQ": {
    "QueueManagerName": "QM1",
    "HostName": "localhost",
    "Port": 1414,
    "Channel": "DEV.APP.SVRCONN",
    "RequestQueueName": "CALC.REQUEST.QUEUE",
    "ResponseQueueName": "CALC.RESPONSE.QUEUE"
  }
}
```

## Running the Server

### Prerequisites
- .NET 9.0 SDK
- IBM MQ server running and accessible
- Required queues created in IBM MQ

### Local Development
```bash
dotnet run
```

### Docker
```bash
docker build -t calculator-server .
docker run -d calculator-server
```

## Supported Operations

The server supports the following mathematical operations:

- **Add**: Addition of two numbers
- **Subtract**: Subtraction of two numbers
- **Multiply**: Multiplication of two numbers
- **Divide**: Division of two numbers (with division by zero protection)

## Message Format

### Request Format
```json
{
  "operand1": 10,
  "operand2": 5,
  "operation": "Add",
  "correlationId": "unique-request-id",
  "replyTo": "CALC.RESPONSE.QUEUE"
}
```

### Response Format
```json
{
  "result": 15,
  "success": true,
  "errorMessage": null,
  "correlationId": "unique-request-id"
}
```

## Error Handling

The server includes comprehensive error handling for:
- Invalid mathematical operations
- Division by zero
- IBM MQ connection issues
- Message serialization/deserialization errors
- Network timeouts

## Logging

The server uses structured logging with different levels:
- **Information**: Normal operation events
- **Debug**: Detailed processing information
- **Error**: Error conditions and exceptions
- **Trace**: Detailed message flow (Development only)

## Health Monitoring

The service includes logging for:
- Connection status to IBM MQ
- Message processing statistics
- Error rates and types
- Queue status and depth