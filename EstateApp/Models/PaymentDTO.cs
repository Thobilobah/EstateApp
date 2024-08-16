using System.ComponentModel.DataAnnotations;

namespace EstateApp.Models
{
    public class PaymentDTO
    {
        public int id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string email { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal amount { get; set; }

        public DateOnly dateCreated { get; set; }
        public DateOnly dateCompleted { get; set; }
    }
}
