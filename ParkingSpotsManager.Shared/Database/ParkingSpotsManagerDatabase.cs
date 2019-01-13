using ParkingSpotsManager.Shared.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSpotsManager.Shared.Database
{
    public static class ParkingSpotsManagerDatabase
    {
        readonly static string _connectionString = Environment.GetEnvironmentVariable("ConnectionString");

        public static string TestMethod()
        {
            List<User> users = new List<User>();
            using (SqlConnection connection = new SqlConnection(_connectionString)) {
                var query = "select * from user_table";
                var sqlCmd = new SqlCommand(query, connection);

                connection.Open();
                var reader = sqlCmd.ExecuteReader();

                if (reader.HasRows) {
                    while (reader.Read()) {
                        var user = new User() {
                            Email = reader.GetString(1)
                        };
                        users.Add(user);
                    }
                }
            }

            return users[0].Email;
        }
    }
}
