# <img src="https://raw.githubusercontent.com/CaptiveAire/Serilog.Enricher.WhenDo/master/Serilog.Enricher.WhenDo.png" width="32" height="32" /> Serilog.Enricher.WhenDo

[![NuGet version](https://badge.fury.io/nu/Serilog.Enricher.WhenDo.svg)](https://badge.fury.io/nu/Serilog.Enricher.WhenDo) ![Build Status](https://ci.appveyor.com/api/projects/status/u7qvdcryijag4ura/branch/master?svg=true)

Serilog enricher that adds a fluent API to configure rules for modifying properties on the fly.

# Usage

Add multiple clauses to your configuration to support your optional rules:

```csharp
var log = 
    new LoggerConfiguration()
        .WriteTo.Trace()
        .Enrich.With<HttpRequestEnricher>()
        // We need to remove the RawUrl property if there is a payment
        // processing error otherwise we may expose the credit card in the logs.
        .Enrich.When().IsExceptionOf<CreditCardPaymentException>().RemovePropertyIfPresent("RawUrl")
        // When the the Special Service fails, log the current endpoint
        .Enrich.When().IsExceptionOf<SpecialServiceException>().AddOrUpdateProperty("SpecialServiceEndpoint", _settings.SpecialServiceEndpoint, true)
        // If one of the two possible properties is there, remove "UnnecessaryProperty"
        .Enrich.When().HasProperty("PossibleProperty", "PossiblePropertyOther").RemovePropertyIfPresent("UnnecessaryProperty")
        .CreateLogger();
```

### Icon
Copyright "Solution" by Arthur Shlain from the Noun Project
