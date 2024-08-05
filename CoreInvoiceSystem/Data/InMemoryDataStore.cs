// InMemoryDataStore.cs
using CoreInvoiceSystem.Interfaces;
using CoreInvoiceSystem.Models;
using System.Collections.Generic;
using System.Linq;

public class InMemoryDataStore : IDatastore
{
    private readonly List<InvoiceModel> _invoices = new List<InvoiceModel>();
    private int _nextId = 1; // Auto-incrementing InvoiceId

    public InvoiceModel Create(InvoiceModel invoice)
    {
        invoice.InvoiceId = _nextId++;
        _invoices.Add(invoice);
        return invoice;
    }

    public void Update(InvoiceModel invoice)
    {
        var existingInvoice = _invoices.FirstOrDefault(i => i.InvoiceId == invoice.InvoiceId);
        if (existingInvoice != null)
        {
            existingInvoice.Amount = invoice.Amount;
            existingInvoice.IsPaid = invoice.IsPaid;
            existingInvoice.DueDate = invoice.DueDate;
            existingInvoice.Status = invoice.Status;
            existingInvoice.LateFee = invoice.LateFee;
            existingInvoice.OverdueDays = invoice.OverdueDays;
            existingInvoice.PaidAmount = invoice.PaidAmount;
        }
    }

    public InvoiceModel GetById(int id)
    {
        return _invoices.FirstOrDefault(i => i.InvoiceId == id);
    }

    public List<InvoiceModel> GetAll()
    {
        return _invoices;
    }

    public void InitializeSampleData()
    {
        Create(new InvoiceModel(1000, DateTime.Now.AddDays(-5)));
        Create(new InvoiceModel(1500, DateTime.Now.AddDays(-4)));
        Create(new InvoiceModel(2000, DateTime.Now.AddDays(-3)));
        Create(new InvoiceModel(2500, DateTime.Now.AddDays(-2)));
        Create(new InvoiceModel(3000, DateTime.Now.AddDays(-1)));
        Create(new InvoiceModel(3500, DateTime.Now.AddDays(0)));
        Create(new InvoiceModel(4000, DateTime.Now.AddDays(1)));
        Create(new InvoiceModel(4500, DateTime.Now.AddDays(2)));
        Create(new InvoiceModel(5000, DateTime.Now.AddDays(3)));
        Create(new InvoiceModel(5500, DateTime.Now.AddDays(4)));
    }
}
