namespace WebJobs.ApplicationInsightsTracer.Tracing
{
    using System;
    using System.Collections.Generic;

    public interface ITelemetryClient
    {
        /// <summary>
        /// Reports a metric.
        /// </summary>
        /// <param name="name">The metric name</param>
        /// <param name="value">The metric value</param>
        /// <param name="properties">Named string values you can use to classify and filter metrics</param>
        void ReportMetric(string name, double value, IDictionary<string, string> properties = null);

        /// <summary>
        /// Reports a runtime exception.
        /// It uses exception and trace entities with same operation id.
        /// </summary>
        /// <param name="exception">The exception to report</param>
        void ReportException(Exception exception);

        /// <summary>
        /// Send information about a request handled by the application.
        /// </summary>
        /// <param name="requestName">The request name.</param>
        /// <param name="elapsedTime">The time taken by the application to handle the request.</param>
        void TrackRequest(string requestName, TimeSpan elapsedTime);

        /// <summary>
        /// Send information about a dependency handled by the application.
        /// </summary>
        /// <param name="dependencyName">The dependency name.</param>
        /// <param name="commandName">The command name</param>
        /// <param name="startTime">The dependency call start time</param>
        /// <param name="duration">The time taken by the application to handle the dependency.</param>
        /// <param name="success">Was the dependency call successful</param>
        void TrackDependency(string dependencyName, string commandName, DateTimeOffset startTime, TimeSpan duration, bool success);

        /// <summary>
        /// Add a custom property to all telemetry events
        /// </summary>
        /// <param name="key">the property key</param>
        /// <param name="value">the property value</param>
        void AddCustomProperty(string key, string value);
    }
}