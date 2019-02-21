using ParkingSpotsManager.Shared.Models;
using Prism;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Xamarin.Forms;

namespace ParkingSpotsManager.Converters
{
    public class CanUpdateSpotStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null) {
                var spot = value as Spot;
                var handler = new JwtSecurityTokenHandler();
                var token = PrismApplicationBase.Current.Properties["authToken"].ToString();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var userID = jsonToken.Claims.First(claim => claim.Type == "unique_name")?.Value;

                if (spot.IsCurrentUserAdmin || int.Parse(userID) == spot.OccupiedBy || spot.OccupiedBy == null) {
                    return true;
                }
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
