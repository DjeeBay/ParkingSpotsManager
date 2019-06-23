using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ParkingSpotsManager.Shared.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Username { get; set; }

        [StringLength(255, MinimumLength = 5)]
        [JsonIgnore]
        public string Password { get; set; }
        public string AuthToken { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public List<UserParking> UserParkings { get; set; }
    }
}
