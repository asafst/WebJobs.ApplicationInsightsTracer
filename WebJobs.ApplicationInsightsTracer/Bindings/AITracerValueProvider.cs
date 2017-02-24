namespace WebJobs.ApplicationInsightsTracer.Bindings
{
    using System;
    using Microsoft.Azure.WebJobs.Host.Bindings;
    using Tracing;

    public class AITracerValueProvider : IValueProvider
    {
        private readonly AIWebJobTracer _aiWebjobTracer;
        private readonly string _instrumentationKey;

        public AITracerValueProvider(AIWebJobTracer aiWebjobTracer, string instrumentationKey)
        {
            _aiWebjobTracer = aiWebjobTracer;
            _instrumentationKey = instrumentationKey;
        }

        public object GetValue()
        {
            return _aiWebjobTracer;
        }

        public string ToInvokeString()
        {
            return _instrumentationKey;
        }

        public Type Type => typeof (AIWebJobTracer);
    }
}