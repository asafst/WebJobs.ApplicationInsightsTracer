namespace WebJobs.ApplicationInsightsTracer.Tracing
{
    using System.Collections.Generic;
    using global::ApplicationInsightsTracer;
    using global::ApplicationInsightsTracer.OtherTracers;

    public class AIWebJobTracer : AIAggregatedTracer
    {
        public AIWebJobTracer(IAITracer aiTracer, IReadOnlyCollection<ITracer> additionalTracers) 
            : base(aiTracer, additionalTracers)
        {
        }
    }
}