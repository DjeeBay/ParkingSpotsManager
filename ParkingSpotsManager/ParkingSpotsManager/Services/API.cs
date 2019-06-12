using ParkingSpotsManager.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingSpotsManager.Services
{
    public class API
    {
        private static string _host = Secrets.ApiHost;

        public static string LoginUrl() => $"{_host}/api/account/Login";
        public static string CreateAccountUrl() => $"{_host}/api/account/CreateUser";
        public static string ParkingREST() => $"{_host}/api/parkings";
        public static string SpotREST() => $"{_host}/api/spots";
        public static string GetUserParkingsUrl() => $"{_host}/api/parkings/GetUserParkings";
        public static string GetCurrentUser() => $"{_host}/api/users/me";
        public static string SaveUserAccountUrl() => $"{_host}/api/account/UpdateUser";

        public static string ChangeSpotStatus(int spotID)
        {
            return $"{_host}/api/spots/{spotID}/ChangeStatus";
        }

        public static string GetInvitableUsersUrl(int parkingID, string search)
        {
            return $"{_host}/api/users/GetInvitableUsers/{parkingID}/{search}";
        }

        public static string GetParkingUserListUrl(int parkingID, string search)
        {
            return $"{_host}/api/parkings/GetUserList/{parkingID}/{search}";
        }

        public static string SendInvitationUrl(int parkingID, int userID)
        {
            return $"{_host}/api/parkings/SendInvitation/{parkingID}/{userID}";
        }

        public static string GetParkingSpotsUrl(int parkingID)
        {
            return $"{_host}/api/spots/GetParkingSpots/{parkingID}";
        }

        public static string ChangeParkingUserRoleUrl(int parkingID)
        {
            return $"{_host}/api/parkings/ChangeUserRole/{parkingID}";
        }

        public static string RemoveParkingUserUrl(int parkingID, int userID)
        {
            return $"{_host}/api/parkings/RemoveUser/{parkingID}/{userID}";
        }

        public static string LeaveParkingUrl(int parkingID)
        {
            return $"{_host}/api/parkings/Leave/{parkingID}";
        }

        public static string SetDefaultOccupier(int spotID, int userID)
        {
            return $"{_host}/api/spots/SetDefaultOccupier/{spotID}/{userID}";
        }
    }
}
