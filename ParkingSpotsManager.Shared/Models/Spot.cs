using ParkingSpotsManager.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ParkingSpotsManager.Shared.Models
{
    public class Spot : ISoftDeletable, IWithTimestamps
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

        public int? OccupiedByDefaultBy { get; set; }

        public bool IsOccupiedByDefault { get; set; } = false;

        public DateTime? OccupiedAt { get; set; }
        public DateTime? ReleasedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public bool IsCurrentUserAdmin { get; set; }
        public int? DeletedBy { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
