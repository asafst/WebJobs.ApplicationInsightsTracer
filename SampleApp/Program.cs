using System;

namespace SampleApp
{
    using System.IO;
    using Microsoft.Azure.WebJobs;
    using WebJobs.ApplicationInsightsTracer.Config;
    using WebJobs.ApplicationInsightsTracer.Tracing;

    public class Program
    {
        static void Main(string[] args)
        {
            JobHostConfiguration config = new JobHostConfiguration();

            config.UseTimers();
            config.UseAITracer();

            JobHost host = new JobHost(config);
            host.RunAndBlock();
        }

        public static void TimerJob([TimerTrigger("00:00:05")] TimerInfo timer, AITracer aiTracer)
        {
            Console.WriteLine("Timer job fired!");

            aiTracer.StartOperation("testOperation");

            try
            {
                aiTracer.TraceInformation("Timer job fired - New!");
                throw new Exception("Test Failure");
            }
            catch (Exception e)
            {
                aiTracer.ReportException(e);
                aiTracer.MarkOperationAsFailure();
                //throw;
            }
            finally
            {
                aiTracer.DispatchOperation();
                aiTracer.Flush();
            }
        }
    }
}
