using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ParkingSpotsManager.Shared.Models
{
    public class Spot
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; }
        public int ParkingId { get; set; }

        [ForeignKey("ParkingId")]
        public Parking Parking { get; set; }
        
        public int? OccupiedBy { get; set; }

        [ForeignKey("OccupiedBy")]
        public User Occupier { get; set; }

        public DateTime? OccupiedAt { get; set; }
        public DateTime? ReleasedAt { get; set; }
    }
}
