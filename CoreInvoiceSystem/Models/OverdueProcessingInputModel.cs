namespace CoreInvoiceSystem.Models
{
    public class OverdueProcessingInputModel
    {
        public decimal LateFee { get; set; }
        public int OverdueDays { get; set; }
    }
}
