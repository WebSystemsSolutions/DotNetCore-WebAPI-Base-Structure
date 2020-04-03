using System;
using System.Collections.Generic;
using System.Text;

namespace SampleProj.Entities.Dtos
{
    public class UserChangePasdwordDto : UserNewPasdwordDto
    {
        public string OldPassword { get; set; }
    }
}
