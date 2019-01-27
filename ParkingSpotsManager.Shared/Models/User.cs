using ParkingSpotsManager.Shared.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSpotsManager.Shared.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AuthToken { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        private const string TABLE = "user_table";

        public async Task<User> GetByLoginAsync(string login)
        {
            //TODO query builder with bindings
            var query = new StringBuilder("SELECT TOP 1 * FROM ");
            query.Append(TABLE);
            query.Append(" WHERE username = ");
            query.Append("'");
            query.Append(login);
            query.Append("'");

            var dataTable = await ParkingSpotsManagerDatabase.GetDataTableAsync(query.ToString());
            if (dataTable != null && dataTable.Rows.Count > 0) {
                var row = dataTable.Rows[0];
                var user = new User() {
                    Id = dataTable.Columns.Contains("Id") ? int.Parse(row["Id"].ToString()) : 0,
                    Email = dataTable.Columns.Contains("Email") ? row["Email"].ToString() : null,
                    Password = dataTable.Columns.Contains("Password") ? row["Password"].ToString() : null
                };
                return user;
            }

            return null;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            //TODO query builder with bindings
            var query = new StringBuilder("SELECT TOP 1 * FROM ");
            query.Append(TABLE);
            query.Append(" WHERE email = ");
            query.Append("'");
            query.Append(email);
            query.Append("'");

            var dataTable = await ParkingSpotsManagerDatabase.GetDataTableAsync(query.ToString());
            if (dataTable != null && dataTable.Rows.Count > 0) {
                var row = dataTable.Rows[0];
                var user = new User() {
                    Id = dataTable.Columns.Contains("Id") ? int.Parse(row["Id"].ToString()) : 0,
                    Email = dataTable.Columns.Contains("Email") ? row["Email"].ToString() : null,
                    Password = dataTable.Columns.Contains("Password") ? row["Password"].ToString() : null
                };
                return user;
            }

            return null;
        }
    }
}
