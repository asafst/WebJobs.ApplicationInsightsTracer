namespace WebJobs.ApplicationInsightsTracer.Tracing
{
    using System;
    using System.Collections.Generic;
    using global::ApplicationInsightsTracer;
    using Microsoft.Azure.WebJobs.Host;

    public class TraceWriterTracer : ITracer
    {
        private readonly TraceWriter _writer;

        public TraceWriterTracer(TraceWriter writer)
        {
            _writer = writer;
        }

        public void TraceInformation(string message)
        {
            _writer.Info(message);
        }

        public void TraceError(string message)
        {
            _writer.Error(message);
        }

        public void TraceVerbose(string message)
        {
            _writer.Verbose(message);
        }

        public void TraceWarning(string message)
        {
            _writer.Warning(message);
        }

        public void Flush()
        {
            _writer.Flush();
        }

        public void TrackCustomMetric(string name, double value, IDictionary<string, string> properties = null, int? count = null, double? max = null,
            double? min = null, DateTime? timestamp = null)
        {
            _writer.Info($"Metric: name-{name}, value-{value}");
        }

        public void TrackCustomEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            _writer.Info($"Event: name={eventName}");
        }

        public void ReportException(Exception exception)
        {
            _writer.Info($"Exception: {exception}");
        }

        public void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data,
            DateTimeOffset startTime, TimeSpan duration, string resultCode, bool success)
        {
            _writer.Info($"Dependency: name={dependencyName}, target={target}, data={data}, duration={duration}, success={success}");
        }

        public void AddCustomProperty(string key, string value)
        {
            // do nothing
        }

        public void AddCustomProperties(IDictionary<string, string> properties)
        {
            // do nothing
        }
    }
}