using ParkingSpotsManager.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingSpotsManager.Shared.Constants
{
    public static class APIConstants
    {
        public const string ConnectionString = @"Filename=./psm.db";
        public const string CreateAccountUrl = "http://psm.djeebay.net/api/account/CreateUser";
        public const string LoginUrl = "http://psm.djeebay.net/api/account/Login";
        public const string TokenSecretKey = "UltimateSecretKeyOfParkingSpotsManager";
        public const string ParkingREST = "http://psm.djeebay.net/api/parkings";
        public const string SpotREST = "http://psm.djeebay.net/api/spots";
        public const string GetUserParkingsUrl = "http://psm.djeebay.net/api/parkings/GetUserParkings";
        public const string GetCurrentUser = "http://psm.djeebay.net/api/users/me";
        public const string SaveUserAccountUrl = "http://psm.djeebay.net/api/account/UpdateUser";

        public static string ChangeSpotStatus(int spotID)
        {
            return $"http://psm.djeebay.net/api/spots/{spotID}/ChangeStatus";
        }

        public static string GetInvitableUsersUrl(int parkingID, string search)
        {
            return $"http://psm.djeebay.net/api/users/GetInvitableUsers/{parkingID}/{search}";
        }

        public static string SendInvitationUrl(int parkingID, int userID)
        {
            return $"http://psm.djeebay.net/api/parkings/SendInvitation/{parkingID}/{userID}";
        }

        public static string GetParkingSpotsUrl(int parkingID)
        {
            return $"http://psm.djeebay.net/api/spots/GetParkingSpots/{parkingID}";
        }

        public static string ChangeParkingUserRoleUrl(int parkingID)
        {
            return $"http://psm.djeebay.net/api/parkings/ChangeUserRole/{parkingID}";
        }

        public static string RemoveParkingUserUrl(int parkingID, int userID)
        {
            return $"http://psm.djeebay.net/api/parkings/RemoveUser/{parkingID}/{userID}";
        }

        public static string LeaveParkingUrl(int parkingID)
        {
            return $"http://psm.djeebay.net/api/parkings/Leave/{parkingID}";
        }
    }
}
