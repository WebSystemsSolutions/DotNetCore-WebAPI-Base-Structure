using System;
using System.Collections.Generic;
using System.Text;

namespace SampleProj.Entities.Interfaces
{
    public interface ITrackable
    {
        DateTime Created { get; set; }
        string CreatedByName { get; set; }
        string CreatedByRole { get; set; }
    }
}
