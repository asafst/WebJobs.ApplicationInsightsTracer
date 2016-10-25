namespace SampleApp
{
    using System;
    using Microsoft.Azure.WebJobs;
    using WebJobs.ApplicationInsightsTracer.Config;
    using WebJobs.ApplicationInsightsTracer.Tracing;
    using Microsoft.ApplicationInsights.Extensibility;
    using WebJobs.ApplicationInsightsTracer;

    public class Program
    {
        static void Main(string[] args)
        {
            JobHostConfiguration config = new JobHostConfiguration();

            // Use the AI Tracer extension with default configuration
            // The Instrumentation key can be also taken from the app settings
            config.UseAITracer("<IKEY_HERE>");

            var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.InstrumentationKey = "<IKEY_HERE>";
            config.UseAITracer(telemetryConfiguration);


            // Use the timer trigger jobs for the example
            config.UseTimers();

            JobHost host = new JobHost(config);
            host.RunAndBlock();
        }

        /// <summary>
        /// A sample function for some AITracer samples (trace, exceptions, operations)
        /// </summary>
        public static void AITracerSampleJob([TimerTrigger("00:00:05")] TimerInfo timer, AITracer aiTracer)
        {
            // Create a request operation to wrap all the current trigger telemetry under a single group (i.e. Operation)
            aiTracer.StartOperation("Test Operation");

            try
            {
                // Simple trace
                aiTracer.TraceInformation("Function started!");
                throw new Exception("Test Failure");
            }
            catch (Exception e)
            {
                // Report the exception to see full exception details in the Application Insights portal (including full Stack Trace)
                aiTracer.ReportException(e);

                // Mark the operation as failure to see it in failed requests section
                aiTracer.MarkOperationAsFailure();
            }
            finally
            {
                // Eventually, close the operation for this job
                aiTracer.DispatchOperation();

                // Remeber to flush the telemetry buffer before finising the job
                aiTracer.Flush();
            }
        }

        /// <summary>
        /// A sample function for using the AITracerConfiguration Attribute for setting the telemetry configuration for each invocation
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="aiTracer"></param>
        public static void AITracerAttributeSampleJob([TimerTrigger("00:00:05")] TimerInfo timer, 
            [AITracerConfiguration(InstrumentationKey = "<IKEY_HERE>")] AITracer aiTracer)
        {
            // Simple trace
            aiTracer.TraceInformation("Function started!");
        }
    }
}
