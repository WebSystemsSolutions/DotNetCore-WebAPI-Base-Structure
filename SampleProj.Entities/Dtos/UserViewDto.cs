using SampleProj.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleProj.Entities.Dtos
{
    public class UserViewDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public State Status { get; set; }
    }
}
