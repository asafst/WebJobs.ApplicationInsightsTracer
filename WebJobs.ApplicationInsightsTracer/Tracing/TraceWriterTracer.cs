namespace WebJobs.ApplicationInsightsTracer.Tracing
{
    using Microsoft.Azure.WebJobs.Host;

    public class TraceWriterTracer : ITracer
    {
        private TraceWriter _writer;

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
    }
}