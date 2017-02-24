namespace WebJobs.ApplicationInsightsTracer.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Config;
    using global::ApplicationInsightsTracer;
    using global::ApplicationInsightsTracer.Exceptions;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Azure.WebJobs.Host.Bindings;
    using Microsoft.Azure.WebJobs.Host.Protocols;
    using Tracing;

    public class AITracerBindingProvider : IBindingProvider
    {
        private readonly TraceWriter _writer;
        private TelemetryConfiguration _config;

        public AITracerBindingProvider(TelemetryConfiguration config, TraceWriter writer)
        {
            _writer = writer;
            _config = config;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ParameterInfo parameter = context.Parameter;
            if (parameter.ParameterType != typeof(AIWebJobTracer))
            {
                return Task.FromResult<IBinding>(null);
            }

            // if the attribute exists, use the configuration from it
            AITracerConfigurationAttribute aiTracerConfigurationAttribute = parameter.GetCustomAttribute<AITracerConfigurationAttribute>(inherit: false);
            if (aiTracerConfigurationAttribute != null)
            {
                if (aiTracerConfigurationAttribute.AITelemtryConfiguration == null &&
                    string.IsNullOrEmpty(aiTracerConfigurationAttribute.InstrumentationKey))
                {
                    throw new AITracerConfigurationAttributeException("At least one of the configuration properties in the attribute must be set");
                }

                if (aiTracerConfigurationAttribute.AITelemtryConfiguration != null &&
                    !string.IsNullOrEmpty(aiTracerConfigurationAttribute.InstrumentationKey))
                {
                    throw new AITracerConfigurationAttributeException("When telemetry configuration is provided thourgh the attribute, instrumentation key must be null");
                }

                _config = aiTracerConfigurationAttribute.AITelemtryConfiguration ??
                          AITracerFactory.CreateDefaultTelemetryConfiguration(aiTracerConfigurationAttribute.InstrumentationKey);
            }

            return Task.FromResult<IBinding>(new AITracerBinding(_config, context.Parameter, new TraceWriterTracer(_writer)));
        }
    }
}