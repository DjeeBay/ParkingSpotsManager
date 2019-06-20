using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingSpotsManager.Shared.Models
{
    public class UserParking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ParkingId { get; set; }
        public Parking Parking { get; set; }
        public int IsAdmin { get; set; }
    }
}
