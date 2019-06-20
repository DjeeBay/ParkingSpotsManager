using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ParkingSpotsManager.Shared.Libraries
{
    public class UserAccount
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool IsPasswordSet { get; set; } = false;

        public string Password { get; set; } = "";
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
