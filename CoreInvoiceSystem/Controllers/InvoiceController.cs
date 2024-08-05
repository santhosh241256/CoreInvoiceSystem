using CoreInvoiceSystem.Interfaces;
using CoreInvoiceSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using CoreInvoiceSystem.Services;
using CoreInvoiceSystem.Exceptions;

namespace CoreInvoiceSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        /// <summary>
        /// Constructor for the InvoiceContoller class with Invoice service injected with Dependency injector
        /// </summary>
        /// <param name="invoiceService"></param>
        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// Get All Invoices in the InvoiceDatabase in InvoiceResponseModel structure
        /// </summary>
        /// <returns>Returns all the Invoices</returns>
        [HttpGet("invoices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<InvoiceModel>> GetAllInvoices()
        {
            try
            {
                var allInvoices = _invoiceService.GetAllInvoices();
                return Ok(allInvoices);
            }
            catch (InvoiceNotFoundException ex)
            {
                return NotFound(new ResponseMessage { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseMessage { Message = "An error occurred while retrieving invoices." });
            }
        }

        //// <summary>
        /// Create a new invoice
        /// </summary>
        /// <param name="invoiceInput">Invoice to be created</param>
        /// <returns>The ID of the created invoice</returns>
        [HttpPost("invoices")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<int> CreateInvoice(InvoiceInputModel invoiceInput)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseMessage { Message = "Invalid input data." });
                //return BadRequest(new { Message = "Invalid input data.", Errors = ModelState });
            }

            try
            {
                var createdInvoice = _invoiceService.CreateInvoice(invoiceInput);
                var response = new 
                {
                    Message = "Invoice created successfully.",
                    InvoiceId = createdInvoice.InvoiceId
                };
                return CreatedAtAction(nameof(CreateInvoice), new { id = createdInvoice.InvoiceId }, response);
            }
            catch (PaymentAmountMismatchException ex)
            {
                return StatusCode(400, new ResponseMessage { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseMessage { Message = "An error occurred while creating the invoice."});
            }

        }

        /// <summary>
        /// Updates the Invoice based on the payment made by the user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="paymentInput"></param>
        /// <returns>Do not retrun anything</returns> 
        [HttpPost("invoices/{id}/pay")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PayInvoice(int id, [FromBody] PaymentInputModel paymentInput)
        {
            try
            {
                _invoiceService.PayInvoice(id, paymentInput);
                return NoContent();
            }
            catch (InvoiceNotFoundException ex)
            {
                return NotFound(new ResponseMessage { Message = ex.Message });
            }
            catch (PaymentAmountMismatchException ex)
            {
                return BadRequest(new ResponseMessage { Message = ex.Message });
            }
            catch (InvalidInputException ex)
            {
                return BadRequest(new ResponseMessage { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseMessage { Message = "An error occurred while processing the payment."});
            }
        }

        /// <summary>
        /// Process the Overdue Invoices based on the Late Fee and the Lead days provided in the Input
        /// </summary>
        /// <param name="overdueProcessingInput"></param>
        /// <returns>Do not returns anything</returns>
        [HttpPost("invoices/process-overdue")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ProcessOverdueInvoices([FromBody] OverdueProcessingInputModel overdueProcessingInput)
        {
            try
            {
                _invoiceService.ProcessOverdueInvoices(overdueProcessingInput);
                return NoContent();
            }
            catch (InvalidInputException ex)
            {
                return BadRequest(new ResponseMessage { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseMessage { Message = "An error occurred while processing overdue invoices." });
            }
        }
    }
}
