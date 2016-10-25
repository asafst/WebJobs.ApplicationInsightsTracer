namespace WebJobs.ApplicationInsightsTracer.Tracing
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Exceptions;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;

    /// <summary>
    /// Implementation of the <see cref="ITracer"/> interface that traces to AppInsights.
    /// </summary>
    public class AITracer : ITelemetryClient, ITracer, ITelemetryOperationHandler, IDisposable
    {
        /// <summary>
        /// The internal Application Insights telemetry client
        /// </summary>
        private readonly TelemetryClient _telemetryClient;

        /// <summary>
        /// A list of additional implementations of <see cref="ITracer"/> which will get the traces
        /// </summary>
        private IList<ITracer> _additionalTracers;

        /// <summary>
        /// Custom properties that will be added to all events sent to Application Insights
        /// </summary>
        private readonly IDictionary<string, string> _customProperties;

        /// <summary>
        /// Custom measurements that will be added to the operation request telemetry
        /// </summary>
        private readonly IDictionary<string, double> _operationCustomMeasurements;

        /// <summary>
        /// The internal operation handler (is not null only when an operation starts using the StartOperation method)
        /// </summary>
        private IOperationHolder<RequestTelemetry> _operationHolder;

        /// <summary>
        /// The maximum length of an exception message allowed by AI SDK. 
        /// </summary>
        public const int ExceptionMessageMaxLength = 1024;

        /// <summary>
        /// Initializes a new instance of the <see cref="AITracer"/> class.
        /// </summary>
        /// <param name="telemetryConfiguration">an implementation of the <see cref="TelemetryConfiguration"/></param>
        /// <param name="sessionId">Session id used for tracing</param>
        /// <param name="additionalTracers">a list of adiitional tracers</param>
        public AITracer(TelemetryConfiguration telemetryConfiguration, string sessionId = null, IList<ITracer> additionalTracers = null)
        {
            if (telemetryConfiguration == null)
            {
                telemetryConfiguration = CreateDefaultTelemetryConfiguration();
            }

            _telemetryClient = new TelemetryClient(telemetryConfiguration);
            _telemetryClient.Context.Session.Id = sessionId ?? Guid.NewGuid().ToString();

            _additionalTracers = additionalTracers ?? new List<ITracer>();

            _customProperties = new Dictionary<string, string>();
            _operationCustomMeasurements = new Dictionary<string, double>();
        }

        /// <summary>
        /// Create the <see cref="TelemetryConfiguration"/> with default values.
        /// The instrumentation key is taken from the app settings
        /// </summary>
        /// <param name="instrumentationKey">the application insights instrumentation key</param>
        /// <returns>the default <see cref="TelemetryConfiguration"/></returns>
        public static TelemetryConfiguration CreateDefaultTelemetryConfiguration(string instrumentationKey = null)
        {
            if (instrumentationKey == null)
            {
                instrumentationKey = ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"];
            }
            
            TelemetryConfiguration.Active.InstrumentationKey = instrumentationKey;

            return TelemetryConfiguration.Active;
        }

        /// <summary>
        /// Add a new <see cref="ITracer"/> to the additional tracers list
        /// </summary>
        /// <param name="tracer"></param>
        public void AddTracer(ITracer tracer)
        {
            _additionalTracers.Add(tracer);
        }

        #region Implementation of ITelemetryClient

        public void ReportMetric(string name, double value, IDictionary<string, string> properties = null)
        {
            var metricTelemetry = new MetricTelemetry(name, value);
            this.SetTelemetryProperties(metricTelemetry, properties);
            _telemetryClient.TrackMetric(metricTelemetry);
        }

        public void ReportException(Exception exception)
        {
            // Trace exception
            this.TraceError(exception.ToString());

            // In case the exception message is too long, wrap it within a new exception with a trimmed message 
            if (exception.Message.Length > ExceptionMessageMaxLength)
            {
                // Create the trimmed message for the new wrapper exception
                string prefix = $"Exception of type: {exception.GetType()} could not be tracked becuase of a too long message. original stack trace can be found in traces." +
                                $"{Environment.NewLine}Trimmed message:{Environment.NewLine}";
                string trimmedMessage = exception.Message.Trim().Substring(0, ExceptionMessageMaxLength - prefix.Length);
                string exceptionMessage = $"{prefix}{trimmedMessage}";

                var wrapperException = new ExceptionMessageToLongException(exceptionMessage);

                this.TrackException(wrapperException);
            }

            // Track the original exception (Long messges will be supported by AI in the near future)
            this.TrackException(exception);
        }

        public void TrackRequest(string requestName, TimeSpan elapsedTime)
        {
            var requestTelemetry = new RequestTelemetry(requestName, DateTimeOffset.UtcNow - elapsedTime, elapsedTime, "200", true);
            this.SetTelemetryProperties(requestTelemetry);
            _telemetryClient.TrackRequest(requestTelemetry);
        }

        public void TrackDependency(string dependencyName, string commandName, DateTimeOffset startTime, TimeSpan duration,
            bool success)
        {
            var dependencyTelemetry = new DependencyTelemetry(dependencyName, commandName, startTime, duration, success);
            this.SetTelemetryProperties(dependencyTelemetry);
            _telemetryClient.TrackDependency(dependencyTelemetry);
        }

        public void AddCustomProperty(string key, string value)
        {
            if (_customProperties.ContainsKey(key))
            {
                _customProperties[key] = value;
            }
            else
            {
                _customProperties.Add(key, value);
            }
        }

        #endregion

        #region Implementation of ITelemetryOperationHandler

        public IDisposable StartOperation(string operationName)
        {
            _operationHolder = _telemetryClient.StartOperation<RequestTelemetry>(operationName);
            return this;
        }

        public void MarkOperationAsFailure()
        {
            _operationHolder.Telemetry.ResponseCode = "500";
        }

        public void DispatchOperation()
        {
            // set custom properties
            this.SetTelemetryProperties(_operationHolder.Telemetry);

            // set custom measurements
            foreach (KeyValuePair<string, double> customMeasurement in _operationCustomMeasurements)
            {
                _operationHolder.Telemetry.Metrics[customMeasurement.Key] = customMeasurement.Value;
            }

            _operationHolder.Dispose();
        }

        public void AddOperationCustomMeasurement(string key, double value)
        {
            if (_operationCustomMeasurements.ContainsKey(key))
            {
                _operationCustomMeasurements[key] = value;
            }
            else
            {
                _operationCustomMeasurements.Add(key, value);
            }
        }

        #endregion

        #region Implementation of ITracer

        /// <summary>
        /// Trace <paramref name="message"/> as <see cref="SeverityLevel.Information"/> message.
        /// </summary>
        /// <param name="message">The message to trace</param>
        public void TraceInformation(string message)
        {
            this.Trace(message, SeverityLevel.Information);

            // send to the additional tracers
            foreach (var tracer in _additionalTracers)
            {
                tracer.TraceInformation(message);
            }
        }

        /// <summary>
        /// Trace <paramref name="message"/> as <see cref="SeverityLevel.Error"/> message.
        /// </summary>
        /// <param name="message">The message to trace</param>
        public void TraceError(string message)
        {
            this.Trace(message, SeverityLevel.Error);

            // send to the additional tracers
            foreach (var tracer in _additionalTracers)
            {
                tracer.TraceError(message);
            }
        }

        /// <summary>
        /// Trace <paramref name="message"/> as <see cref="SeverityLevel.Verbose"/> message.
        /// </summary>
        /// <param name="message">The message to trace</param>
        public void TraceVerbose(string message)
        {
            this.Trace(message, SeverityLevel.Verbose);

            // send to the additional tracers
            foreach (var tracer in _additionalTracers)
            {
                tracer.TraceVerbose(message);
            }
        }

        /// <summary>
        /// Trace <paramref name="message"/> as <see cref="SeverityLevel.Warning"/> message.
        /// </summary>
        /// <param name="message">The message to trace</param>
        public void TraceWarning(string message)
        {
            this.Trace(message, SeverityLevel.Warning);

            // send to the additional tracers
            foreach (var tracer in _additionalTracers)
            {
                tracer.TraceWarning(message);
            }
        }


        /// <summary>
        /// Flushes the telemetry channel
        /// </summary>
        public void Flush()
        {
            _telemetryClient.Flush();

            // flush the additional tracers
            foreach (var tracer in _additionalTracers)
            {
                tracer.Flush();
            }
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            this.DispatchOperation();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Traces the specified message to the telemetry client
        /// </summary>
        /// <param name="message">The message to trace</param>
        /// <param name="severityLevel">The message's severity level</param>
        private void Trace(string message, SeverityLevel severityLevel)
        {
            var traceTelemetry = new TraceTelemetry(message, severityLevel);
            _telemetryClient.TrackTrace(traceTelemetry);
        }

        private void SetTelemetryProperties(ISupportProperties telemetry, IDictionary<string, string> properties = null)
        {
            // Add the framework's custom properties
            foreach (KeyValuePair<string, string> customProperty in _customProperties)
            {
                telemetry.Properties[customProperty.Key] = customProperty.Value;
            }

            // And finally, add the user-supplied properties
            if (properties != null)
            {
                foreach (KeyValuePair<string, string> customProperty in properties)
                {
                    telemetry.Properties[customProperty.Key] = customProperty.Value;
                }
            }
        }

        private void TrackException(Exception exception)
        {
            ExceptionTelemetry exceptionTelemetry = new ExceptionTelemetry(exception);
            this.SetTelemetryProperties(exceptionTelemetry);
            _telemetryClient.TrackException(exceptionTelemetry);
        }

        #endregion

    }
}
