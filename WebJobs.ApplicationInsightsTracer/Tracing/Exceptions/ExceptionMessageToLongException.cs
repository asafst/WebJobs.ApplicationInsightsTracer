namespace WebJobs.ApplicationInsightsTracer.Tracing.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This exception is a wrapper for another exception with a message string longer than the limit that AI SDK allows.
    /// Currently, AI SDK do not report an exception with too long messages.
    /// </summary>
    [Serializable]
    public class ExceptionMessageToLongException : Exception
    {
        // Constructor form 1 ()
        /// <summary>
        /// Creates a new instance of the <see cref="ExceptionMessageToLongException"/> class with no arguments.
        /// </summary>
        public ExceptionMessageToLongException()
        {
        }

        // Constructor form 2 (message)
        /// <summary>
        /// Creates a new instance of the <see cref="ExceptionMessageToLongException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name='message'>The message that explains the reason for the exception.</param>
        public ExceptionMessageToLongException(string message)
            : base(message)
        {
        }

        // Constructor form 3 (message, exception)
        /// <summary>
        /// Creates a new instance of the <see cref="ExceptionMessageToLongException"/> class
        /// with a specified error message and a reference to an inner exception.
        /// </summary>
        /// <param name='message'>The message that explains the reason for the exception.</param>
        /// <param name='innerException'>The exception that is the cause of the current exception.
        /// If it is not a null reference, the current exception is raised in a catch block
        /// that handles the inner exception.</param>
        public ExceptionMessageToLongException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // Constructor form 4 (info, context)
        /// <summary>
        /// Creates a new instance of the <see cref="ExceptionMessageToLongException"/> class
        /// with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SeraizliationInfo"/> that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual
        /// information about the source or destination.</param>
        protected ExceptionMessageToLongException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
