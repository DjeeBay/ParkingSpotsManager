using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingSpotsManager.Shared.Interfaces
{
    public interface IWithTimestamps
    {
        DateTime? CreatedAt { get; set; }
        int? CreatedBy { get; set; }
        DateTime? UpdatedAt { get; set; }
        int? UpdatedBy { get; set; }
    }
}
