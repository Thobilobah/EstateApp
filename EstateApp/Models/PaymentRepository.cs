namespace EstateApp.Models
{
    public class PaymentRepository
    {
        public static List<Payment> Payments { get; set; } = new List<Payment>()
        {
            new Payment { id = 1, email = "payer1@gmail.com", amount = 100.00M },
            new Payment { id = 2, email = "payer2@gmail.com", amount = 200.50M },
            new Payment { id = 3, email = "payer3@gmail.com", amount = 150.75M }
        };
    }
}
