using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ParkingSpotsManager.Shared.Models
{
    public class Parking
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; }
        public string Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public List<Spot> Spots { get; set; }
        public List<UserParking> UserParkings { get; set; }

        [NotMapped]
        public bool IsCurrentUserAdmin { get; set; }
    }
}
