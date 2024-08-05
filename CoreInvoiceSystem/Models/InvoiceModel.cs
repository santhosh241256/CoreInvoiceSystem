namespace CoreInvoiceSystem.Models
{
    public class InvoiceModel
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public DateTime DueDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public decimal LateFee { get; set; }
        public int OverdueDays { get; set; }
        public decimal PaidAmount { get; set; }

        public InvoiceModel(decimal amount, DateTime dueDate)
        {
            Amount = amount;
            DueDate = dueDate;
            IsPaid = false;
            Status = InvoiceStatus.Pending;
            LateFee = 0;
            OverdueDays = 0;
            PaidAmount = 0;
        }
        public enum InvoiceStatus
        {
            Pending,
            Paid,
            Void
        }
    }
}
