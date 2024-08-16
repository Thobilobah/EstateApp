namespace EstateApp.Models
{
    public class Payment
    {
        public int id { get; set; }
        public string email { get; set; }
        public decimal amount { get; set; }
        public DateOnly dateCreated { get; set; }
        public DateOnly dateCompleted { get; set; }
    }
}
