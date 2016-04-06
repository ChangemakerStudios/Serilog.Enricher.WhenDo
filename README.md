# <img src="https://raw.githubusercontent.com/CaptiveAire/Serilog.Enricher.WhenDo/master/Serilog.Enricher.WhenDo.png" width="32" height="32" /> Serilog.Enricher.WhenDo

[![NuGet version](https://badge.fury.io/nu/Serilog.Enricher.WhenDo.svg)](https://badge.fury.io/nu/Serilog.Enricher.WhenDo) ![Build Status](https://ci.appveyor.com/api/projects/status/u7qvdcryijag4ura/branch/master?svg=true)

Serilog extra that adds a fluent API to configure rules for modifying properties on the fly and piping events to secondary loggers.

# Usage

##### Property Modification

Add/Remove/Update properties based on the criteria.

```csharp
var log = 
    new LoggerConfiguration()
        .WriteTo.Trace()
        .Enrich.With<HttpRequestEnricher>()
        // We need to remove the RawUrl property if there is a payment
        // processing error otherwise we may expose the credit card in the logs.
        .When().IsExceptionOf<CreditCardPaymentException>().Do().RemovePropertyIfPresent("RawUrl")
        // When the the Special Service fails, log the current endpoint
        .When().IsExceptionOf<SpecialServiceException>().Do().AddOrUpdateProperty("SpecialServiceEndpoint", _settings.SpecialServiceEndpoint, true)
        // If one of the two possible properties is there, remove "UnnecessaryProperty"
        .When().HasProperty("PossibleProperty", "PossiblePropertyOther").Do().RemovePropertyIfPresent("UnnecessaryProperty")
        .CreateLogger();
```

##### Send/Pipe to Another Logger

```csharp
var backupLogger = new LoggerConfiguration().WriteTo.Trace.CreateLogger();

var specialLogger = new LoggerConfiguration().WriteTo.SpecialSink().CreateLogger();

var log = 
    new LoggerConfiguration()
        .WriteTo.Trace()
        // Sends a copy of the event to the backupLogger --
        // both loggers will get the same event.
        .When().FromSourceContext<AccountingService>().Do().SendTo(backupLogger)
        // Pipes all events that match criteria to the specialLogger.
        // "this" logger will not receive the event.
        .When().FromSourceContext<ChattyService>().Do().PipeTo(specialLogger)
        .CreateLogger();
```

### Icon
Copyright "Solution" by Arthur Shlain from the Noun Project
