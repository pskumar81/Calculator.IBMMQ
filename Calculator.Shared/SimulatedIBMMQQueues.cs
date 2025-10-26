using System.Collections.Concurrent;

namespace Calculator.Shared;

/// <summary>
/// Shared simulation of IBM MQ queues for demo purposes
/// In a real implementation, these would be actual IBM MQ queues
/// </summary>
public static class SimulatedIBMMQQueues
{
    /// <summary>
    /// Simulated request queue
    /// </summary>
    public static readonly ConcurrentQueue<string> RequestQueue = new();

    /// <summary>
    /// Simulated response queue
    /// </summary>
    public static readonly ConcurrentQueue<string> ResponseQueue = new();
}