using System;
using System.Collections.Generic;
using System.Linq;
using CoreInvoiceSystem.Exceptions;
using CoreInvoiceSystem.Interfaces;
using CoreInvoiceSystem.Models;
using CoreInvoiceSystem.Services;
using Moq;
using NUnit.Framework;

namespace CoreInvoiceSystem.Tests
{
    public class InvoiceServiceTests
    {
        private InvoiceService _invoiceService;
        private Mock<IDatastore> _datastoreMock;

        [SetUp]
        public void Setup()
        {
            _datastoreMock = new Mock<IDatastore>();
            _invoiceService = new InvoiceService(_datastoreMock.Object);
        }

        [Test]
        public void CreateInvoice_ShouldReturnCreatedInvoice()
        {
            // Arrange
            var invoiceInput = new InvoiceInputModel
            {
                Amount = 100,
                DueDate = DateTime.Now.AddDays(30)
            };
            var expectedInvoice = new InvoiceModel(100, invoiceInput.DueDate);
            _datastoreMock.Setup(ds => ds.Create(It.IsAny<InvoiceModel>())).Returns(expectedInvoice);

            // Act
            var result = _invoiceService.CreateInvoice(invoiceInput);

            // Assert
            Assert.That(result.InvoiceId, Is.EqualTo(expectedInvoice.InvoiceId));
            Assert.That(result.Amount, Is.EqualTo(expectedInvoice.Amount));
            Assert.That(result.DueDate, Is.EqualTo(expectedInvoice.DueDate));
        }

        [Test]
        public void CreateInvoice_ShouldThrowExceptionForInvalidAmount()
        {
            // Arrange
            var invoiceInput = new InvoiceInputModel
            {
                Amount = 0,
                DueDate = DateTime.Now.AddDays(30)
            };

            // Act & Assert
            var ex = Assert.Throws<PaymentAmountMismatchException>(() => _invoiceService.CreateInvoice(invoiceInput));
            Assert.That(ex.Message, Is.EqualTo("Invalid Input"));
        }

        [Test]
        public void GetAllInvoices_ShouldThrowExceptionWhenNoInvoices()
        {
            // Arrange
            _datastoreMock.Setup(ds => ds.GetAll()).Returns(new List<InvoiceModel>());

            // Act & Assert
            var ex = Assert.Throws<InvoiceNotFoundException>(() => _invoiceService.GetAllInvoices());
            Assert.That(ex.Message, Is.EqualTo("Invoice not found."));
        }

        [Test]
        public void GetInvoice_ShouldReturnAllInvoices()
        {
            var invoice1 = new InvoiceModel(100, DateTime.Now.AddDays(30))
            {
                InvoiceId = 1,
                Amount = 100,
                IsPaid = true,
                PaidAmount = 100,
                DueDate = DateTime.Now.AddDays(30),
                Status = InvoiceModel.InvoiceStatus.Paid
            };

            var invoice2 = new InvoiceModel(200, DateTime.Now.AddDays(60))
            {
                InvoiceId = 2,
                Amount = 200,
                IsPaid = false,
                PaidAmount = 0,
                DueDate = DateTime.Now.AddDays(60),
                Status = InvoiceModel.InvoiceStatus.Pending
            };


            var invoices = new List<InvoiceModel> { invoice1, invoice2 };
            _datastoreMock.Setup(ds => ds.GetAll()).Returns(invoices);

            // Act
            var result = _invoiceService.GetAllInvoices();

            // Assert
            Assert.That(invoices.Count, Is.EqualTo(result.Count));
            Assert.That(result[0].InvoiceId, Is.EqualTo(invoice1.InvoiceId));
            Assert.That(result[0].Amount, Is.EqualTo(invoice1.Amount));
            Assert.That(result[0].PaidAmount, Is.EqualTo(invoice1.PaidAmount));
            Assert.That(result[0].DueDate, Is.EqualTo(invoice1.DueDate.ToString("yyyy-MM-dd")));
            Assert.That(result[0].Status, Is.EqualTo(invoice1.Status.ToString()));

            Assert.That(result[1].InvoiceId, Is.EqualTo(invoice2.InvoiceId));
            Assert.That(result[1].Amount, Is.EqualTo(invoice2.Amount));
            Assert.That(result[1].PaidAmount, Is.EqualTo(0)); // Unpaid
            Assert.That(result[1].DueDate, Is.EqualTo(invoice2.DueDate.ToString("yyyy-MM-dd")));
            Assert.That(result[1].Status, Is.EqualTo(invoice2.Status.ToString()));
        }

        [Test]
        public void PayInvoice_ShouldUpdateInvoiceStatus()
        {
            // Arrange
            var invoice = new InvoiceModel(100, DateTime.Now.AddDays(30));
            _datastoreMock.Setup(ds => ds.GetById(It.IsAny<int>())).Returns(invoice);
            var paymentInput = new PaymentInputModel { PaymentAmount = 100 };

            // Act
            _invoiceService.PayInvoice(1, paymentInput);

            // Assert
            Assert.That(invoice.Status, Is.EqualTo(InvoiceModel.InvoiceStatus.Paid));
            Assert.That(invoice.PaidAmount, Is.EqualTo(100));
            _datastoreMock.Verify(ds => ds.Update(invoice), Times.Once);
        }

        [Test]
        public void PayInvoice_ShouldThrowExceptionWhenInvoiceNotFound()
        {
            // Arrange
            int id = 1;
            var paymentInput = new PaymentInputModel { PaymentAmount = 100 };
            _datastoreMock.Setup(ds => ds.GetById(id)).Returns((InvoiceModel)null);

            // Act & Assert
            var ex = Assert.Throws<InvoiceNotFoundException>(() => _invoiceService.PayInvoice(id, paymentInput));
            Assert.That(ex.Message, Is.EqualTo($"Invoice with ID {id} not found."));
        }

        [Test]
        public void PayInvoice_ShouldThrowExceptionWhenPaymentAmountExceeds()
        {
            // Arrange
            int id = 1;
            var paymentInput = new PaymentInputModel { PaymentAmount = 200 };
            var invoice = new InvoiceModel(100, DateTime.Now.AddDays(10))
            {
                InvoiceId = id,
                Amount = 100,
                PaidAmount = 0,
                IsPaid = false,
                Status = InvoiceModel.InvoiceStatus.Pending
            };
            _datastoreMock.Setup(ds => ds.GetById(id)).Returns(invoice);

            // Act & Assert
            var ex = Assert.Throws<PaymentAmountMismatchException>(() => _invoiceService.PayInvoice(id, paymentInput));
            Assert.That(ex.Message, Is.EqualTo("Payment amount is greater than the invoice amount."));
        }

        [Test]
        public void PayInvoice_ShouldMarkInvoiceAsPaidWhenAmountMatches()
        {
            // Arrange
            int id = 1;
            var paymentInput = new PaymentInputModel { PaymentAmount = 100 };
            var invoice = new InvoiceModel(100, DateTime.Now.AddDays(10))
            {
                InvoiceId = id,
                Amount = 100,
                PaidAmount = 0,
                IsPaid = false,
                Status = InvoiceModel.InvoiceStatus.Pending
            };
            _datastoreMock.Setup(ds => ds.GetById(id)).Returns(invoice);

            // Act
            _invoiceService.PayInvoice(id, paymentInput);

            // Assert
            _datastoreMock.Verify(ds => ds.Update(It.Is<InvoiceModel>(inv =>
                inv.InvoiceId == id &&
                inv.PaidAmount == 100 &&
                inv.IsPaid == true &&
                inv.Status == InvoiceModel.InvoiceStatus.Paid
            )), Times.Once);
        }

        [Test]
        public void PayInvoice_ShouldMarkInvoiceAsPendingWhenPartialPayment()
        {
            // Arrange
            int id = 1;
            var paymentInput = new PaymentInputModel { PaymentAmount = 50 };
            var invoice = new InvoiceModel(100, DateTime.Now.AddDays(10))
            {
                InvoiceId = id,
                Amount = 100,
                PaidAmount = 0,
                IsPaid = false,
                Status = InvoiceModel.InvoiceStatus.Pending
            };
            _datastoreMock.Setup(ds => ds.GetById(id)).Returns(invoice);

            // Act
            _invoiceService.PayInvoice(id, paymentInput);

            // Assert
            _datastoreMock.Verify(ds => ds.Update(It.Is<InvoiceModel>(inv =>
                inv.InvoiceId == id &&
                inv.PaidAmount == 50 &&
                inv.IsPaid == true &&
                inv.Status == InvoiceModel.InvoiceStatus.Pending
            )), Times.Once);
        }

        [Test]
        public void ProcessOverdueInvoices_ShouldThrowExceptionWhenLateFeeIsNegative()
        {
            // Arrange
            var overdueProcessingInput = new OverdueProcessingInputModel { LateFee = -10, OverdueDays = 5 };

            // Act & Assert
            var ex = Assert.Throws<InvalidInputException>(() => _invoiceService.ProcessOverdueInvoices(overdueProcessingInput));
            Assert.That(ex.Message, Is.EqualTo("LateFee is Less than Zero."));
        }

        [Test]
        public void ProcessOverdueInvoices_ShouldThrowExceptionWhenOverdueDaysIsNegative()
        {
            // Arrange
            var overdueProcessingInput = new OverdueProcessingInputModel { LateFee = 10, OverdueDays = -5 };

            // Act & Assert
            var ex = Assert.Throws<InvalidInputException>(() => _invoiceService.ProcessOverdueInvoices(overdueProcessingInput));
            Assert.That(ex.Message, Is.EqualTo("OverDueDays is less than Zero. This will create backdated invoices."));
        }

        [Test]
        public void ProcessOverdueInvoices_ShouldUpdateOldInvoices()
        {
            // Arrange
            var overdueProcessingInput = new OverdueProcessingInputModel { LateFee = 10, OverdueDays = 5 };
            var overdueInvoices = new List<InvoiceModel>
            {
                new InvoiceModel(100,DateTime.Now.AddDays(-10)) 
                { 
                    InvoiceId = 1, 
                    Amount = 100, 
                    PaidAmount = 50, 
                    DueDate = DateTime.Now.AddDays(-10), 
                    Status = InvoiceModel.InvoiceStatus.Pending, 
                    IsPaid = true 
                },
                new InvoiceModel (200,DateTime.Now.AddDays(-15))
                { 
                    InvoiceId = 2, 
                    Amount = 200, 
                    PaidAmount = 0, 
                    DueDate = DateTime.Now.AddDays(-15), 
                    Status = InvoiceModel.InvoiceStatus.Pending, 
                    IsPaid = false 
                }
            };

            _datastoreMock.Setup(ds => ds.GetAll()).Returns(overdueInvoices);

            // Act
            _invoiceService.ProcessOverdueInvoices(overdueProcessingInput);

            // Assert
            _datastoreMock.Verify(ds => ds.Update(It.Is<InvoiceModel>(inv => inv.InvoiceId == 1 && inv.Status == InvoiceModel.InvoiceStatus.Paid)), Times.Once);
            _datastoreMock.Verify(ds => ds.Update(It.Is<InvoiceModel>(inv => inv.InvoiceId == 2 && inv.Status == InvoiceModel.InvoiceStatus.Void)), Times.Once);
        }
    }
}
