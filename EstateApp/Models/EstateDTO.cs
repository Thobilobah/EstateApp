using System.ComponentModel.DataAnnotations;

namespace EstateApp.Models
{
    public class EstateDTO
    {
        public int id { get; set; }
        [Required(ErrorMessage = "roleId is required")]
        public string roleId { get; set; }
        [Required(ErrorMessage = "first name is required")]
        public string fName { get; set; }
        [Required(ErrorMessage = "last name is required")]
        public string lName { get; set; }
        public int houseId { get; set; }
        public int apartmentId { get; set; }

    }
}
