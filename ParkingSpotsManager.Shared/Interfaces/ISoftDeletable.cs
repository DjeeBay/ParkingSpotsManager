using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingSpotsManager.Shared.Interfaces
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        int? DeletedBy { get; set; }
    }
}
