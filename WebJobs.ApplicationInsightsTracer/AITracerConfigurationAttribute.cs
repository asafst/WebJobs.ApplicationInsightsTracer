namespace WebJobs.ApplicationInsightsTracer
{
    using Microsoft.ApplicationInsights.Extensibility;
    using System;
    using WebJobs.ApplicationInsightsTracer.Tracing;

    /// <summary>
    /// Attribute used to set the <see cref="AIWebJobTracer"/> configuration for a specific function trigger.
    /// The target parameter must be of type <see cref="AIWebJobTracer"/>
    /// <remarks>If the <param name="AITelemtryConfiguration"/> is set, the <param name="InstrumentationKey"/> must be null</remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class AITracerConfigurationAttribute : Attribute
    {
        public string InstrumentationKey { get; set; }

        public TelemetryConfiguration AITelemtryConfiguration { get; set; }
    }
}