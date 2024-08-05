using CoreInvoiceSystem.Models;

namespace CoreInvoiceSystem.Interfaces
{
    public interface IDatastore
    {
        InvoiceModel Create(InvoiceModel invoice);
        void Update(InvoiceModel invoice);
        InvoiceModel GetById(int id);
        List<InvoiceModel> GetAll();
    }
}
