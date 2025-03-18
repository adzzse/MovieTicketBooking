using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Dtos.Role
{
    public class RoleDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }

    public class CreateRoleDto
    {
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, ErrorMessage = "Role name cannot exceed 50 characters")]
        public string Name { get; set; }
    }
}
