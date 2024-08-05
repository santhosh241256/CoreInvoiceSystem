using CoreInvoiceSystem.Exceptions;
using CoreInvoiceSystem.Interfaces;
using CoreInvoiceSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreInvoiceSystem.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IDatastore _invoiceStore;

        /// <summary>
        /// constructor with database object injected to it through Dependency Injection
        /// </summary>
        /// <param name="invoiceStore"></param>
        public InvoiceService(IDatastore invoiceStore)
        {
            _invoiceStore = invoiceStore;
        }

        /// <summary>
        /// Creates an Invoice in the InvoiceDatabase for the Amount and Due date input provided by the User
        /// </summary>
        /// <param name="invoiceInput"></param>
        /// <returns></returns>
        public InvoiceModel CreateInvoice(InvoiceInputModel invoiceInput)
        {
            if (invoiceInput.Amount <= 0)
            {
                throw new PaymentAmountMismatchException($"Invalid Input");
            }

            var invoice = new InvoiceModel(invoiceInput.Amount, invoiceInput.DueDate);
            return _invoiceStore.Create(invoice);
        }

        /// <summary>
        /// Returns all the Invoice from the Invoice Database to the API
        /// </summary>
        /// <returns></returns>
        public List<InvoiceResponseModel> GetAllInvoices()
        {
            var invoices = _invoiceStore.GetAll();
            if (!invoices.Any())
            {
                throw new InvoiceNotFoundException($"Invoice not found.");
            }

            List<InvoiceResponseModel> AllInvoices = new List<InvoiceResponseModel>();

            foreach (var invoice in invoices)
            {
                var invoiceResponse = new InvoiceResponseModel
                {
                    InvoiceId = invoice.InvoiceId,
                    Amount = invoice.Amount,
                    PaidAmount = invoice.PaidAmount,
                    DueDate = invoice.DueDate.ToString("yyyy-MM-dd"),
                    Status = invoice.Status.ToString()
                };
                AllInvoices.Add(invoiceResponse);
            }

            return AllInvoices;
        }

        /// <summary>
        /// Updates the Invoice with Payment amount and Status of the Invoice based on the Payment amount provided in the API. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="paymentInput"></param>
        public void PayInvoice(int id, PaymentInputModel paymentInput)
        {
            InvoiceModel invoice = ValidateInput(id, paymentInput);

            if ((invoice.Amount - invoice.PaidAmount) != paymentInput.PaymentAmount)
            {
                invoice.Status = InvoiceModel.InvoiceStatus.Pending;
            }
            else
            {
                invoice.Status = InvoiceModel.InvoiceStatus.Paid;
            }

            invoice.IsPaid = true;
            invoice.PaidAmount += paymentInput.PaymentAmount;
            _invoiceStore.Update(invoice);
        }

        private InvoiceModel ValidateInput(int id, PaymentInputModel paymentInput)
        {
            var invoice = _invoiceStore.GetById(id);
            if (invoice == null)
            {
                throw new InvoiceNotFoundException($"Invoice with ID {id} not found.");
            }
            else if (invoice.Status == InvoiceModel.InvoiceStatus.Paid && invoice.Amount == invoice.PaidAmount)
            {
                throw new PaymentAmountMismatchException($"Payment cannot be made as the Invoice with ID {id} is fully paid.");
            }
            else if (paymentInput.PaymentAmount < 0)
            {
                throw new PaymentAmountMismatchException($"Payment amount should be greater than 0.");
            }
            else if((invoice.Amount - invoice.PaidAmount) < paymentInput.PaymentAmount)
            {
                throw new PaymentAmountMismatchException($"Payment amount is greater than the invoice amount.");
            }

            return invoice;
        }

        /// <summary>
        /// Process the Over Due Invoices by a Late fee charge on the Invoice Balance and generate a new invoice using the Lead Days.
        /// The Status of the old invoice gets changed to either Void or Paid based on the Payment they have made previously on the invoice
        /// </summary>
        /// <param name="overdueProcessingInput"></param>
        public void ProcessOverdueInvoices(OverdueProcessingInputModel overdueProcessingInput)
        {
            if (overdueProcessingInput.LateFee < 0)
            {
                throw new InvalidInputException($"LateFee is Less than Zero.");
            }
            else if (overdueProcessingInput.OverdueDays < 0)
            {
                throw new InvalidInputException($"OverDueDays is less than Zero. This will create backdated invoices.");
            }

            var overdueInvoices = _invoiceStore.GetAll()
            .Where(invoices => invoices.DueDate < DateTime.Now && invoices.Status == InvoiceModel.InvoiceStatus.Pending)
            .ToList();

            foreach (var invoice in overdueInvoices)
            {
                var invoiceBalance = invoice.Amount - invoice.PaidAmount;

                if (invoiceBalance > 0)
                {
                    var remainingAmount = invoiceBalance + overdueProcessingInput.LateFee;
                    if (remainingAmount > 0 && invoice.IsPaid)
                    {
                        invoice.Status = InvoiceModel.InvoiceStatus.Paid;
                    }
                    else
                    {
                        invoice.Status = InvoiceModel.InvoiceStatus.Void;
                    }

                    _invoiceStore.Update(invoice);

                    InvoiceModel newInvoice = new InvoiceModel(remainingAmount, DateTime.Now.AddDays(overdueProcessingInput.OverdueDays));
                    _invoiceStore.Create(newInvoice);
                }
            }
        }
    }
}
