namespace EstateApp.Entities
{
    public class InvoiceInfo
    {
        public int id { get; set; }
        public int userId { get; set; }
        public DateTime dateIssue { get; set; }
        public DateTime dueDate { get; set; }
        public decimal amount { get; set; }
        public string refNo { get; set; }
        public string feeType { get; set; }
    }
}
