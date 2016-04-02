# Serilog.Enricher.WhenDo

Serilog encricher that adds a fluent API to configure rules for modifying properties.


# Usage

Add as many clauses to your configuration to support the optional rules:

```csharp
var log = 
    new LoggerConfiguration()
        .WriteTo.Trace()
        .Enrich.With<HttpRequestEnricher>()
        // We need to remove the RawUrl property if there is a payment
        // processing error otherwise we may expose the credit card in the logs.
        .Enrich.When().IsExceptionOf<CreditCardPaymentException>().RemovePropertyIfPresent("RawUrl")
        // When the the Special Service fails, log the current endpoint
        .Enrich.When().IsExceptionOf<SpecialServiceException>().AddOrUpdateProperty("SpecialServiceEndpoint", _settings.SpecialServiceEndpoint)
        .CreateLogger();
```