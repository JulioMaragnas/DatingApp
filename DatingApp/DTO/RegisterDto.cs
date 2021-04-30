using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.DTO
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}
