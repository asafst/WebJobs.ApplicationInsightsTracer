namespace WebJobs.ApplicationInsightsTracer.Tracing
{
    using System;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;

    public interface ITelemetryOperationHandler
    {
        /// <summary>
        /// Starts a new operation.
        /// </summary>
        /// <param name="operationName">The name of the operation, to be used as OperationName in all telemtry items along the operation</param>
        IDisposable StartOperation(string operationName);

        /// <summary>
        /// Mark operation as failure
        /// </summary>
        void MarkOperationAsFailure();

        /// <summary>
        /// Sends an operation summary telemetry.
        /// </summary>
        void DispatchOperation();

        /// <summary>
        /// Add a custom measurement to the operation summary telemetry.
        /// </summary>
        /// <param name="key">Measurement's key name</param>
        /// <param name="value">Measurement's value</param>
        void AddOperationCustomMeasurement(string key, double value);
    }
}