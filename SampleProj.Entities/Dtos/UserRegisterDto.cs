using SampleProj.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SampleProj.Entities.Dtos
{
    public class UserRegisterDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Position { get; set; }

        [Required]
        public int RoleId { get; set; }

        public State Status { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
