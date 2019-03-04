using ParkingSpotsManager.Shared.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace ParkingSpotsManager.Converters
{
    public class DisplayFreeSpotsNumber : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) {
                return "0";
            }
            var spots = value as IList<Spot>;
            var nbOccupied = 0;
            foreach (var spot in spots) {
                if (spot.OccupiedBy != null) {
                    nbOccupied++;
                }
            }

            var nbAvailable = spots.Count - nbOccupied;

            return new StringBuilder(nbAvailable.ToString()).Append(" / ").Append(spots.Count).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
