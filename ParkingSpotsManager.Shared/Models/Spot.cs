using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingSpotsManager.Shared.Models
{
    public class Spot
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParkingId { get; set; }
        public int OccupiedBy { get; set; }
    }
}
