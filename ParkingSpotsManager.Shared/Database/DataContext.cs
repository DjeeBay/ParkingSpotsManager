using Microsoft.EntityFrameworkCore;
using ParkingSpotsManager.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingSpotsManager.Shared.Database
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Parking> Parkings { get; set; }
        public DbSet<Spot> Spots { get; set; }
        public DbSet<UserParking> UsersParkings { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}
