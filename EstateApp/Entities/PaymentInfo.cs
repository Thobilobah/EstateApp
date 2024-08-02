namespace EstateApp.Entities
{
    public class PaymentInfo
    {
        public int id { get; set; }
        public string invoiceRefNo { get; set; }
        public decimal amountPaid { get; set; }
        public string paymentMethod { get; set; }
        public string paymentStatus { get; set; }
        public DateTime paymentDate { get; set; }
    }
}
