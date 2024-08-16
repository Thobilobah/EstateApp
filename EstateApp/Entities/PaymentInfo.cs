namespace EstateApp.Entities
{
    public class PaymentInfo
    {
        public int id { get; set; }
        public string email { get; set; }
        public decimal amount { get; set; } 
        public DateOnly dateCreated { get; set; }
        public DateOnly dateCompleted { get; set; }
        public string reference { get; set; }
        public string message { get; set; }
        public string authorization_url { get; set; }
        public string access_code { get; set; }
        public string status { get; set; }
    
        
    }
}
