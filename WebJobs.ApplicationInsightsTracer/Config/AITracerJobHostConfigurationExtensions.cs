namespace WebJobs.ApplicationInsightsTracer.Config
{
    using System;
    using System.Runtime.CompilerServices;
    using Bindings;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host.Config;
    using Tracing;

    public static class AITracerJobHostConfigurationExtensions
    {

        public static void UseAITracer(this JobHostConfiguration config)
        {
            UseAITracer(config, AITracer.CreateDefaultTelemetryConfiguration());
        }

        public static void UseAITracer(this JobHostConfiguration config, string instrumentationKey)
        {
            UseAITracer(config, AITracer.CreateDefaultTelemetryConfiguration(instrumentationKey));
        }

        public static void UseAITracer(this JobHostConfiguration config, TelemetryConfiguration telemetryConfiguration)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (telemetryConfiguration == null)
            {
                throw new ArgumentNullException("telemetryConfiguration");
            }

            config.RegisterExtensionConfigProvider(new AITracerExtensionConfig(telemetryConfiguration));
        }
    }
}