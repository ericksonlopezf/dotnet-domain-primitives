# Level 05 — ASP.NET Core & OpenAPI Integration

In Level 05, we bind domain primitives seamlessly in Minimal APIs, Controllers, and OpenAPI schemas.

---

## 1. Minimal APIs Endpoint Binding

`EricksonLopez.DomainPrimitives.AspNetCore` provides compile-time `TryParse` binding:

```csharp
app.MapGet("/customers/{email}", (EmailAddress email) => 
{
    return TypedResults.Ok(new { Email = email.Value });
});
```

If an invalid format is passed, ASP.NET Core returns `400 Bad Request` with structured RFC 7807 problem details automatically.
