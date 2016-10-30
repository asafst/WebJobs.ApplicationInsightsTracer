# WebJobs SDK Application Insights Extension

This extension adds a binding for sending traces and telemetry (Requrests, Exceptions, Dependencies) to [Application Insights](https://azure.microsoft.com/en-us/services/application-insights/) .

```C#
public void SimpleAITracerBinding([WebHookTrigger] Message m, AITracer aiTracer)
{
    aiTracer.TraceInformation("Function started!");
}
```

See here for more info about [Azure WebJobs SDK Extensions](https://github.com/Azure/azure-webjobs-sdk-extensions).

## Installation

You can obtain it [through Nuget](https://www.nuget.org/packages/WebJobs.ApplicationInsightsTracer/) with:
```
Install-Package WebJobs.ApplicationInsightsTracer
```

Or **clone** this repo and reference it.

## Usage

The Application Insights binding returns a `AITracer` object implements an interface for sending tracers and telemetry messages to Application Insights endpoint.

A full example with different telemetry options:

```C#
public void FullAITracerBinding([WebHookTrigger] Message m, AITracer aiTracer)
{
    // Create a request operation to wrap all the current trigger telemetry under a single group (i.e. Operation)
        aiTracer.StartOperation("Test Operation");

        try
        {
            // Simple trace
            aiTracer.TraceInformation("Function started!");
            throw new Exception("Test Failure");
        }
        catch (Exception e)
        {
            // Report the exception to see full exception details in the Application Insights portal (including full Stack Trace)
            aiTracer.ReportException(e);

            // Mark the operation as failure to see it in failed requests section
            aiTracer.MarkOperationAsFailure();
        }
        finally
        {
            // Eventually, close the operation for this job
            aiTracer.DispatchOperation();

            // Remeber to flush the telemetry buffer before finising the job
            aiTracer.Flush();
        }
}
```

You can enable the Extension via the `JobHostConfiguration` object.
```C#
var config = new JobHostConfiguration();
// Use the AI Tracer extension with default configuration
// The Instrumentation key can be also taken from the app settings
config.UseAITracer("<IKEY_HERE>");
```

Or you can create a new [`TelemetryConfiguration`](https://github.com/Microsoft/ApplicationInsights-dotnet/blob/37cec526194b833f7cd676f25eafd985dd88d3fa/src/Core/Managed/Shared/Extensibility/TelemetryConfiguration.cs) from the Applciation Insights SDK and pass it as a parameter.
```C#
var config = new JobHostConfiguration();
var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
telemetryConfiguration.InstrumentationKey = "<IKEY_HERE>";
config.UseAITracer(telemetryConfiguration);
```

### Using the AITracer on different functions

If your scenario includes creating a unique AITracer for each function that sends telemetry to a different Ikey, this can achieved using the AITracerparameter attributes.

```C#
public void SimpleAITracerBinding([WebHookTrigger] Message m,
    [AITracerConfiguration(InstrumentationKey = "<IKEY_HERE>")] AITracer aiTracer)
{
    aiTracer.TraceInformation("Function started!");
}
```

This creates a default `TelemetryConfiguration` with the new Ikey. The `AITracerConfiguration` can also take a `TelemetryConfiguration` as a parameter instead of using the default one. 

## Notes

This is currently still in development. Not for production use.

## License

[MIT](LICENSE)
