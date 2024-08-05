namespace CoreInvoiceSystem.Models
{
    public class InvoiceResponseModel
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public string DueDate { get; set; }
        public string Status { get; set; }
    }
}
