namespace EstateApp.Models
{
    public class PaymentRepository
    {
        public static List<Payment> Payments { get; set; } = new List<Payment>()
        {
            new Payment { id = 1, email = "payer1@gmail.com", amountPaid = 100.00M },
            new Payment { id = 2, email = "payer2@gmail.com", amountPaid = 200.50M },
            new Payment { id = 3, email = "payer3@gmail.com", amountPaid = 150.75M }
        };
    }
}
