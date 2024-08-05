using CoreInvoiceSystem.Models;

namespace CoreInvoiceSystem.Interfaces
{
    public interface IInvoiceService
    {
        InvoiceModel CreateInvoice(InvoiceInputModel invoiceInput);
        List<InvoiceResponseModel> GetAllInvoices();
        void PayInvoice(int id, PaymentInputModel paymentInput);
        void ProcessOverdueInvoices(OverdueProcessingInputModel overdueProcessingInput);
    }
}
