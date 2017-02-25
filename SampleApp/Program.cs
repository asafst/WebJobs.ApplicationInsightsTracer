namespace SampleApp
{
    using System;
    using Microsoft.Azure.WebJobs;
    using WebJobs.ApplicationInsightsTracer.Config;
    using WebJobs.ApplicationInsightsTracer.Tracing;
    using WebJobs.ApplicationInsightsTracer;

    public class Program
    {
        static void Main(string[] args)
        {
            JobHostConfiguration config = new JobHostConfiguration();

            // Use the AI Tracer extension with default configuration
            // The Instrumentation key will be taken from the app settings, can also be passed as a parameter
            config.UseApplicationInsightsTracer();

            // additional method to pass the telemetry configuration to the extension
            //var telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            //telemetryConfiguration.InstrumentationKey = "<IKEY_HERE>";
            //config.UseApplicationInsightsTracer(telemetryConfiguration);


            // Use the timer trigger jobs for the example
            config.UseTimers();

            JobHost host = new JobHost(config);
            host.RunAndBlock();
        }

        /// <summary>
        /// A sample function for some AITracer events (trace, exceptions, operations)
        /// </summary>
        public static void AITracerSampleJob([TimerTrigger("00:00:05")] TimerInfo timer, AIWebJobTracer aiTracer)
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
        public static void AITracerAttributeSampleJob([TimerTrigger("00:00:05")] TimerInfo timer, 
            [AITracerConfiguration(InstrumentationKey = "<IKEY_HERE>")] AIWebJobTracer aiTracer)
        {
            // Simple trace
            aiTracer.TraceInformation("Function started!");
        }
    }
}
