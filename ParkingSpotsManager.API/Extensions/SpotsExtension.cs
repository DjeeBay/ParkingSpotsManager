using ParkingSpotsManager.Shared.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingSpotsManager.API.Extensions
{
    public static class SpotsExtension
    {
        public static async Task<int> RestoreDefaultOccupiers(this DataContext dbContext)
        {
            var spotsWithDefaultOccupier = dbContext.Spots.Where(s => s.IsOccupiedByDefault && s.OccupiedByDefaultBy != null).ToArray();

            foreach (var spot in spotsWithDefaultOccupier) {
                spot.OccupiedBy = spot.OccupiedByDefaultBy;
                spot.OccupiedAt = DateTime.Now;
            }

            return await dbContext.SaveChangesAsync();
        }

        public static async Task<int> ResetOccupiers(this DataContext dbContext)
        {
            var spotsWithoutDefaultOccupier = dbContext.Spots.Where(s => !s.IsOccupiedByDefault && s.OccupiedByDefaultBy == null).ToArray();

            foreach (var spot in spotsWithoutDefaultOccupier) {
                spot.OccupiedBy = null;
                spot.ReleasedAt = DateTime.Now;
            }

            return await dbContext.SaveChangesAsync();
        }
    }
}
