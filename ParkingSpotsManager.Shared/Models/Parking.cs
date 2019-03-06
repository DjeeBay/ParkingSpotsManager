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
        public int? Columns { get; set; }
        public int? Rows { get; set; }

        public List<Spot> Spots { get; set; }
        public List<UserParking> UserParkings { get; set; }

        [NotMapped]
        public bool IsCurrentUserAdmin { get; set; }
    }
}
