namespace WebJobs.ApplicationInsightsTracer.Config
{
    using System;
    using System.Configuration;
    using Bindings;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host.Config;


    public class AITracerExtensionConfig : IExtensionConfigProvider
    {
        private readonly TelemetryConfiguration _config;

        public AITracerExtensionConfig(TelemetryConfiguration config)
        {
            _config = config;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Config.RegisterBindingExtensions(new AITracerBindingProvider(_config, context.Trace));
        }
    }
}