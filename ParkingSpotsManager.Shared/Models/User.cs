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

            return await GetFromDataTable(query.ToString());
        }

        public async Task<User> GetByIdAsync(int id)
        {
            //TODO query builder with bindings
            var query = new StringBuilder("SELECT TOP 1 * FROM ");
            query.Append(TABLE);
            query.Append(" WHERE id = '");
            query.Append(id);
            query.Append("'");

            return await GetFromDataTable(query.ToString());
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

            return await GetFromDataTable(query.ToString());
        }

        public async Task<User> CreateAsync(User user)
        {
            //TODO query builder with bindings
            var query = new StringBuilder("INSERT INTO ");
            query.Append(TABLE);
            query.Append(" (email, password, username) VALUES (");
            query.Append("'");
            query.Append(user.Email);
            query.Append("', '");
            query.Append(user.Password);
            query.Append("', '");
            query.Append(user.Username);
            query.Append("'");
            query.Append(")");

            await ParkingSpotsManagerDatabase.GetDataTableAsync(query.ToString());

            return await GetByLoginAsync(user.Username);
        }

        //public async Task<User> SetTokenAsync(int userID, string token)
        //{
        //    //TODO query builder with bindings
        //    var query = new StringBuilder("UPDATE ");
        //    query.Append(TABLE);
        //    query.Append(" SET token = '");
        //    query.Append(token);
        //    query.Append("', token_expire = '");
        //    query.Append(DateTime.Now.AddDays(1).ToString("yyyy/MM/dd H:mm:ss"));
        //    query.Append("' WHERE id = ");
        //    query.Append(userID);

        //    await ParkingSpotsManagerDatabase.GetDataTableAsync(query.ToString());

        //    return await GetByIdAsync(userID);
        //}

        private async Task<User> GetFromDataTable(string query)
        {
            var dataTable = await ParkingSpotsManagerDatabase.GetDataTableAsync(query.ToString());
            if (dataTable != null && dataTable.Rows.Count > 0) {
                var row = dataTable.Rows[0];
                var user = new User() {
                    Id = dataTable.Columns.Contains("Id") ? int.Parse(row["Id"].ToString()) : 0,
                    Email = dataTable.Columns.Contains("Email") ? row["Email"].ToString() : null,
                    Password = dataTable.Columns.Contains("Password") ? row["Password"].ToString() : null,
                    Username = dataTable.Columns.Contains("Username") ? row["Username"].ToString() : null,
                };
                return user;
            }

            return null;
        }
    }
}
