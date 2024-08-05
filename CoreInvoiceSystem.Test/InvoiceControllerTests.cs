using System;
using CoreInvoiceSystem.Controllers;
using CoreInvoiceSystem.Exceptions;
using CoreInvoiceSystem.Interfaces;
using CoreInvoiceSystem.Models;
using CoreInvoiceSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace CoreInvoiceSystem.Tests
{
    public class InvoiceControllerTests
    {
        private InvoiceController _controller;
        private Mock<IInvoiceService> _invoiceServiceMock;

        [SetUp]
        public void Setup()
        {
            _invoiceServiceMock = new Mock<IInvoiceService>();
            _controller = new InvoiceController(_invoiceServiceMock.Object);

            // Set up the controller context with valid ModelState
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.ControllerContext = controllerContext;
        }

        [Test]
        public void CreateInvoice_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Amount", "Required");

            var invoiceInput = new InvoiceInputModel();

            // Act
            var result = _controller.CreateInvoice(invoiceInput);
            var badRequestResult = result.Result as BadRequestObjectResult;
            var responseMessage = badRequestResult.Value as ResponseMessage;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(responseMessage.Message, Is.EqualTo("Invalid input data."));
        }

        [Test]
        public void CreateInvoice_ShouldReturnCreated_WhenInvoiceIsCreated()
        {
            // Arrange
            var invoiceInput = new InvoiceInputModel { Amount = 100, DueDate = DateTime.Now.AddDays(30) };
            var createdInvoice = new InvoiceModel(invoiceInput.Amount, invoiceInput.DueDate) { InvoiceId = 1 };

            _invoiceServiceMock.Setup(s => s.CreateInvoice(invoiceInput)).Returns(createdInvoice);

            // Act
            var result = _controller.CreateInvoice(invoiceInput);
            var createdResult = result.Result as CreatedAtActionResult;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<CreatedAtActionResult>());
            Assert.That(createdResult.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
        }

        [Test]
        public void CreateInvoice_ShouldReturnBadRequest_WhenPaymentAmountMismatchExceptionIsThrown()
        {
            // Arrange
            var invoiceInput = new InvoiceInputModel { Amount = 0, DueDate = DateTime.Now.AddDays(30) };

            _invoiceServiceMock
                .Setup(s => s.CreateInvoice(invoiceInput))
                .Throws(new PaymentAmountMismatchException("Invalid Input"));

            // Act
            var result = _controller.CreateInvoice(invoiceInput);
            var badRequestResult = result.Result as ObjectResult;
            var responseMessage = badRequestResult.Value as ResponseMessage;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(responseMessage.Message, Is.EqualTo("Invalid Input"));
        }

        [Test]
        public void CreateInvoice_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var invoiceInput = new InvoiceInputModel { Amount = 100, DueDate = DateTime.Now.AddDays(30) };

            _invoiceServiceMock
                .Setup(s => s.CreateInvoice(invoiceInput))
                .Throws(new Exception("Some error"));

            // Act
            var result = _controller.CreateInvoice(invoiceInput);
            var internalServerErrorResult = result.Result as ObjectResult;
            var responseMessage = internalServerErrorResult.Value as ResponseMessage;

            //Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            Assert.That(internalServerErrorResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(responseMessage.Message, Is.EqualTo("An error occurred while creating the invoice."));
        }

        [Test]
        public void GetAllInvoices_ShouldReturnOk_WhenInvoicesExist()
        {
            // Arrange
            var invoices = new List<InvoiceResponseModel>
            {
                new InvoiceResponseModel { InvoiceId = 1, Amount = 100, PaidAmount = 0, DueDate = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd") },
                new InvoiceResponseModel { InvoiceId = 2, Amount = 200, PaidAmount = 0, DueDate = DateTime.UtcNow.AddDays(60).ToString("yyyy-MM-dd") }
            };
            _invoiceServiceMock.Setup(s => s.GetAllInvoices()).Returns(invoices);

            // Act
            var result = _controller.GetAllInvoices();
            var okResult = result.Result as OkObjectResult;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());            
            Assert.That(okResult.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
            Assert.That(okResult.Value, Is.EqualTo(invoices));
        }

        [Test]
        public void GetAllInvoices_ShouldReturnNotFound_WhenNoInvoicesExist()
        {
            // Arrange
            _invoiceServiceMock
                .Setup(s => s.GetAllInvoices())
                .Throws(new InvoiceNotFoundException("Invoice not found."));

            // Act
            var result = _controller.GetAllInvoices();
            var notFoundResult = result.Result as NotFoundObjectResult;
            var responseMessage = notFoundResult.Value as ResponseMessage;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());            
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(responseMessage.Message, Is.EqualTo("Invoice not found."));
        }

        [Test]
        public void GetAllInvoices_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _invoiceServiceMock
                .Setup(s => s.GetAllInvoices())
                .Throws(new Exception("Some error"));

            // Act
            var result = _controller.GetAllInvoices();
            var internalServerErrorResult = result.Result as ObjectResult;
            var responseMessage = internalServerErrorResult.Value as ResponseMessage;

            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());            
            Assert.That(internalServerErrorResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(responseMessage.Message, Is.EqualTo("An error occurred while retrieving invoices."));
        }

        [Test]
        public void PayInvoice_ShouldReturnNoContent_WhenPaymentIsSuccessful()
        {
            // Arrange
            var paymentInput = new PaymentInputModel { PaymentAmount = 100 };
            _invoiceServiceMock.Setup(s => s.PayInvoice(It.IsAny<int>(), paymentInput));

            // Act
            var result = _controller.PayInvoice(1, paymentInput);
            var noContentResult = result as NoContentResult;

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
            Assert.That(noContentResult.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
        }

        [Test]
        public void PayInvoice_ShouldReturnNotFound_WhenInvoiceNotFound()
        {
            // Arrange
            var paymentInput = new PaymentInputModel { PaymentAmount = 100 };
            _invoiceServiceMock
                .Setup(s => s.PayInvoice(It.IsAny<int>(), paymentInput))
                .Throws(new InvoiceNotFoundException("Invoice not found."));

            // Act
            var result = _controller.PayInvoice(1, paymentInput);
            var notFoundResult = result as NotFoundObjectResult;
            var responseMessage = notFoundResult.Value as ResponseMessage;

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
            Assert.That(responseMessage.Message, Is.EqualTo("Invoice not found."));
        }

        [Test]
        public void PayInvoice_ShouldReturnBadRequest_WhenPaymentAmountMismatch()
        {
            // Arrange
            var paymentInput = new PaymentInputModel { PaymentAmount = 100 };
            _invoiceServiceMock
                .Setup(s => s.PayInvoice(It.IsAny<int>(), paymentInput))
                .Throws(new PaymentAmountMismatchException("Payment amount mismatch."));

            // Act
            var result = _controller.PayInvoice(1, paymentInput);
            var badRequestResult = result as BadRequestObjectResult;
            var responseMessage = badRequestResult.Value as ResponseMessage;

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(responseMessage.Message, Is.EqualTo("Payment amount mismatch."));
        }

        [Test]
        public void PayInvoice_ShouldReturnBadRequest_WhenInvalidInput()
        {
            // Arrange
            var paymentInput = new PaymentInputModel { PaymentAmount = 100 };
            _invoiceServiceMock
                .Setup(s => s.PayInvoice(It.IsAny<int>(), paymentInput))
                .Throws(new InvalidInputException("Invalid input."));

            // Act
            var result = _controller.PayInvoice(1, paymentInput);
            var badRequestResult = result as BadRequestObjectResult;
            var responseMessage = badRequestResult.Value as ResponseMessage;

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(responseMessage.Message, Is.EqualTo("Invalid input."));
        }

        [Test]
        public void PayInvoice_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var paymentInput = new PaymentInputModel { PaymentAmount = 100 };
            _invoiceServiceMock
                .Setup(s => s.PayInvoice(It.IsAny<int>(), paymentInput))
                .Throws(new Exception("Some error"));

            // Act
            var result = _controller.PayInvoice(1, paymentInput);
            var internalServerErrorResult = result as ObjectResult;
            var responseMessage = internalServerErrorResult.Value as ResponseMessage;

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            Assert.That(internalServerErrorResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(responseMessage.Message, Is.EqualTo("An error occurred while processing the payment."));
        }

        [Test]
        public void ProcessOverdueInvoices_ShouldReturnNoContent_WhenProcessingIsSuccessful()
        {
            // Arrange
            var overdueProcessingInput = new OverdueProcessingInputModel { LateFee = 50, OverdueDays = 10 };
            _invoiceServiceMock.Setup(s => s.ProcessOverdueInvoices(overdueProcessingInput));

            // Act
            var result = _controller.ProcessOverdueInvoices(overdueProcessingInput); 
            var noContentResult = result as NoContentResult;

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());            
            Assert.That(noContentResult.StatusCode, Is.EqualTo(StatusCodes.Status204NoContent));
        }

        [Test]
        public void ProcessOverdueInvoices_ShouldReturnBadRequest_WhenInvalidInput()
        {
            // Arrange
            var overdueProcessingInput = new OverdueProcessingInputModel { LateFee = 50, OverdueDays = 10 };
            _invoiceServiceMock
                .Setup(s => s.ProcessOverdueInvoices(overdueProcessingInput))
                .Throws(new InvalidInputException("Invalid input."));

            // Act
            var result = _controller.ProcessOverdueInvoices(overdueProcessingInput);
            var badRequestResult = result as BadRequestObjectResult;
            var responseMessage = badRequestResult.Value as ResponseMessage;

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            Assert.That(((dynamic)badRequestResult.Value).Message, Is.EqualTo("Invalid input."));
        }

        [Test]
        public void ProcessOverdueInvoices_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var overdueProcessingInput = new OverdueProcessingInputModel { LateFee = 50, OverdueDays = 10 };
            _invoiceServiceMock
                .Setup(s => s.ProcessOverdueInvoices(overdueProcessingInput))
                .Throws(new Exception("Some error"));

            // Act
            var result = _controller.ProcessOverdueInvoices(overdueProcessingInput);
            var internalServerErrorResult = result as ObjectResult;
            var responseMessage = internalServerErrorResult.Value as ResponseMessage;

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            
            Assert.That(internalServerErrorResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(responseMessage.Message, Is.EqualTo("An error occurred while processing overdue invoices."));
        }
    }    
}
