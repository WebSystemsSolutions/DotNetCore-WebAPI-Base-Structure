using System;
using System.Collections.Generic;
using System.Text;

namespace SampleProj.Entities.Dtos
{
    public class UserNewPasdwordDto
    {
        public int Id { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }
    }
}
