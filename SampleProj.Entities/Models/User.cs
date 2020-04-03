using SampleProj.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SampleProj.Entities.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Position { get; set; }

        [Required]
        public int RoleId { get; set; }

        public Role Role { get; set; }

        public int CompanyIdID { get; set; }

        [Required]
        public State Status { get; set; }

        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

    }
}
