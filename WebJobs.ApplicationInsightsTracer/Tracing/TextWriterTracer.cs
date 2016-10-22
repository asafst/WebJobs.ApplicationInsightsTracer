namespace WebJobs.ApplicationInsightsTracer.Tracing
{
    using System;
    using System.IO;

    /// <summary>
    /// Implementation of the <see cref="ITracer"/> interface that traces to a (WebJob's) <see cref="TextWriter"/> logger.
    /// </summary>
    public class TextWriterTracer : ITracer
    {
        private readonly TextWriter _logger;

        /// <summary>
        /// Initialized a new instance of the <see cref="TextWriterTracer"/> class.
        /// </summary>
        /// <param name="logger">The logger to send traces to</param>
        public TextWriterTracer(TextWriter logger) 
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            // we keep a synchronized instance since logging can occur from multiple threads
            _logger = TextWriter.Synchronized(logger);
        }

        public void TraceInformation(string message)
        {
            _logger.WriteLine(message);
        }

        public void TraceError(string message)
        {
            _logger.WriteLine($"Error: {message}");
        }

        public void TraceVerbose(string message)
        {
            _logger.WriteLine($"Verbose: {message}");
        }

        public void TraceWarning(string message)
        {
            _logger.WriteLine($"Warning: {message}");
        }

        public void Flush()
        {
            _logger.Flush();
        }
    }
}
