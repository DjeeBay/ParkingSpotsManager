using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingSpotsManager.Shared.Models
{
    public class Parking
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Columns { get; set; }
        public int Rows { get; set; }
    }
}
