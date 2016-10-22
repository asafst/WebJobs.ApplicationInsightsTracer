namespace WebJobs.ApplicationInsightsTracer.Bindings
{
    using System;
    using Microsoft.Azure.WebJobs.Host.Bindings;
    using Tracing;

    public class AITracerValueProvider : IValueProvider
    {
        private readonly AITracer _aiTracer;
        private readonly string _instrumentationKey;

        public AITracerValueProvider(AITracer aiTracer, string instrumentationKey)
        {
            _aiTracer = aiTracer;
            _instrumentationKey = instrumentationKey;
        }

        public object GetValue()
        {
            return _aiTracer;
        }

        public string ToInvokeString()
        {
            return _instrumentationKey;
        }

        public Type Type => typeof (AITracer);
    }
}