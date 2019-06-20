using ParkingSpotsManager.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingSpotsManager.Shared.Constants
{
    /// <summary>
    /// Create an APIConstants class with the constants below and the correct associated data.
    /// </summary>
    public static class APIConstantsExample
    {
        public const string ConnectionString = @"";
        public const string CreateAccountUrl = "";
        public const string LoginUrl = "";
        public const string TokenSecretKey = "secret phrase";
        public const string ParkingREST = "";
        public const string SpotREST = "";
        public const string GetUserParkingsUrl = "";
        public const string GetCurrentUser = "";
        public const string SaveUserAccountUrl = "";

        public static string ChangeSpotStatus(int spotID)
        {
            return $"";
        }

        public static string GetInvitableUsersUrl(int parkingID, string search)
        {
            return $"";
        }

        public static string SendInvitationUrl(int parkingID, int userID)
        {
            return $"";
        }

        public static string GetParkingSpotsUrl(int parkingID)
        {
            return $"";
        }

        public static string ChangeParkingUserRoleUrl(int parkingID)
        {
            return $"";
        }

        public static string RemoveParkingUserUrl(int parkingID, int userID)
        {
            return $"";
        }

        public static string LeaveParkingUrl(int parkingID)
        {
            return $"";
        }
    }
}
