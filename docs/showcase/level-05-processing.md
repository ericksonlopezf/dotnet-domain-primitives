# Level 5 — Processing

This level covers background processing, high-performance concurrency, and safe boundary reconstruction using `System.Threading.Channels`.

---

## 1. Producer/Consumer Pattern with `Channel<T>`

In decoupled event-driven architectures, asynchronous messages transport raw serialized payloads. When crossing into consumer worker processes, domain integrity must be re-asserted via `TryCreate`:

```csharp
using System.Threading.Channels;
using EricksonLopez.DomainPrimitives;

public class OrderEventChannel
{
    private readonly Channel<RawOrderPayload> _channel = Channel.CreateBounded<RawOrderPayload>(new BoundedChannelOptions(1000)
    {
        FullMode = BoundedChannelFullMode.Wait
    });

    public ChannelWriter<RawOrderPayload> Writer => _channel.Writer;
    public ChannelReader<RawOrderPayload> Reader => _channel.Reader;
}

public record RawOrderPayload(string RawOrderId, string RawEmail, decimal RawAmount);
```

---

## 2. Background Worker with Safe Ingestion Boundary

The worker processes items concurrently, rejecting malformed messages without throwing exceptions:

```csharp
public class OrderProcessingWorker : BackgroundService
{
    private readonly ChannelReader<RawOrderPayload> _reader;

    public OrderProcessingWorker(ChannelReader<RawOrderPayload> reader) => _reader = reader;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var item in _reader.ReadAllAsync(stoppingToken))
        {
            // Safe reconstruction at consumer perimeter
            if (!OrderId.TryCreate(Guid.Parse(item.RawOrderId), out var orderId, out var idError))
            {
                // Route to Dead-Letter or audit log
                continue;
            }

            if (!CustomerEmail.TryCreate(item.RawEmail, out var email, out var emailError))
            {
                // Log structured anomaly
                continue;
            }

            // Beyond this point, state is demonstrably valid
            await ProcessValidOrderAsync(orderId, email, item.RawAmount, stoppingToken);
        }
    }

    private Task ProcessValidOrderAsync(OrderId id, CustomerEmail email, decimal amount, CancellationToken ct)
    {
        // Safe domain business logic
        return Task.CompletedTask;
    }
}
```

---

## 3. Concurrency and Thread Safety

Because domain primitives are declared as `readonly record struct`, instances are deeply immutable and safe for concurrent multi-threaded consumption without locks, synchronization primitives, or defensive copying.

---

## Showcase Reference
- Executable project: [`21-BackgroundProcessing`](file:///d:/DevData/ericksonlopez.dev/dotnet-domain-primitives/samples/OfficialSample/21-BackgroundProcessing/Program.cs)
