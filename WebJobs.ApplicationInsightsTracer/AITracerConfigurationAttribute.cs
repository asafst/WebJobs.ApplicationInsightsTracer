namespace WebJobs.ApplicationInsightsTracer
{
    using Microsoft.ApplicationInsights.Extensibility;
    using System;
    using Tracing;

    /// <summary>
    /// Attribute used to set the <see cref="AITracer"/> configuration for a specific function trigger.
    /// The target parameter must be of type <see cref="AITracer"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class AITracerConfigurationAttribute : Attribute
    {
        public string InstrumentationKey { get; set; }

        public TelemetryConfiguration AITelemtryConfiguration { get; set; }
    }
}