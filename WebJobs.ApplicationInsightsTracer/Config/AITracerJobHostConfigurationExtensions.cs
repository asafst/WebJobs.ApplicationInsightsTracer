namespace WebJobs.ApplicationInsightsTracer.Config
{
    using System;
    using System.Runtime.CompilerServices;
    using Bindings;
    using global::ApplicationInsightsTracer;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host.Config;
    using Tracing;

    public static class AITracerJobHostConfigurationExtensions
    {

        public static void UseApplicationInsightsTracer(this JobHostConfiguration config)
        {
            UseApplicationInsightsTracer(config, AITracerFactory.GetActiveTelemetryConfiguration());
        }

        public static void UseApplicationInsightsTracer(this JobHostConfiguration config, string instrumentationKey)
        {
            UseApplicationInsightsTracer(config, AITracerFactory.GetActiveTelemetryConfiguration(instrumentationKey));
        }

        public static void UseApplicationInsightsTracer(this JobHostConfiguration config, TelemetryConfiguration telemetryConfiguration)
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