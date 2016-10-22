namespace WebJobs.ApplicationInsightsTracer.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Azure.WebJobs.Host.Bindings;
    using Microsoft.Azure.WebJobs.Host.Protocols;
    using Tracing;

    public class AITracerBinding : IBinding
    {
        private readonly ParameterInfo _parameter;
        private readonly IList<ITracer> _additionalTracers;
        private readonly TelemetryConfiguration _config;

        public AITracerBinding(TelemetryConfiguration config, ParameterInfo parameter, ITracer webJobTracer)
        {
            _parameter = parameter;
            _additionalTracers = new List<ITracer> { webJobTracer };
            _config = config;
        }

        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            AITracer aiTracer = value as AITracer;

            return this.BindInternalAsync(aiTracer);
        }

        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return this.BindInternalAsync();
        }

        private Task<IValueProvider> BindInternalAsync(AITracer aiTracer = null)
        {
            if (aiTracer == null)
            {
                // use the to list to create a copy of the internal list
                aiTracer = new AITracer(_config, additionalTracers: _additionalTracers.ToList());
            }

            return Task.FromResult<IValueProvider>(new AITracerValueProvider(aiTracer, _config.InstrumentationKey));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor
            {
                Name = _parameter.Name,
                DisplayHints = new ParameterDisplayHints
                {
                    Description = "AI Tracer"
                }
            };
        }

        public bool FromAttribute => false;
    }
}