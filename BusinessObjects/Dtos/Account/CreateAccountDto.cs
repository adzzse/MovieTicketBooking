using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Dtos.Account
{
    public class CreateAccountDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(15)]
        public string? Phone { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}