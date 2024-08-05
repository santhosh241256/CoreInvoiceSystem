namespace CoreInvoiceSystem.Exceptions
{
    public class InvoiceNotFoundException : Exception
    {
        /// <summary>
        /// Constructure for the Exception class inherited with base class
        /// </summary>
        /// <param name="message"></param>
        public InvoiceNotFoundException(string message) : base(message)
        {
        }
    }

    public class PaymentAmountMismatchException : Exception
    {
        /// <summary>
        /// Constructure for the Exception class inherited with base class
        /// </summary>
        /// <param name="message"></param>
        public PaymentAmountMismatchException(string message) : base(message)
        {
        }
    }

    public class InvalidInputException : Exception
    {
        /// <summary>
        /// Constructure for the Exception class inherited with base class
        /// </summary>
        /// <param name="message"></param>
        public InvalidInputException(string message) : base(message)
        {
        }
    }

    public class ResponseMessage
    {
        public string Message { get; set; }
    }
}
