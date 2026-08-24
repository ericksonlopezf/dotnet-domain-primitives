# Level 02 — Smart Enums & Domain State Machines

In Level 02, we implement type-safe polymorphic Smart Enums with rich domain behavior.

---

## 1. Defining a Smart Enum

```csharp
using EricksonLopez.DomainPrimitives;

public sealed class OrderStatus : SmartEnum<OrderStatus, int>
{
    public static readonly OrderStatus Pending = new(nameof(Pending), 1);
    public static readonly OrderStatus Paid = new(nameof(Paid), 2);
    public static readonly OrderStatus Shipped = new(nameof(Shipped), 3);
    public static readonly OrderStatus Cancelled = new(nameof(Cancelled), 4);

    private OrderStatus(string name, int value) : base(name, value) { }

    public bool CanTransitionTo(OrderStatus next) => (this, next) switch
    {
        _ when this == Pending && next == Paid => true,
        _ when this == Paid && next == Shipped => true,
        _ when this == Pending && next == Cancelled => true,
        _ => false
    };
}
```
