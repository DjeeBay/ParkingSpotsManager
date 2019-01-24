using ParkingSpotsManager.Shared.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSpotsManager.Shared.Models
{
    public class User
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AuthToken { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        private const string TABLE = "user_table";

        private readonly ParkingSpotsManagerDatabase psmDatabaseInstance;

        public User()
        {
            //TODO BaseModel
            psmDatabaseInstance = ParkingSpotsManagerDatabase.Instance;
        }

        public async Task<User> GetByLogin(string login)
        {
            //TODO query builder with bindings
            var query = new StringBuilder("SELECT TOP 1 * FROM ");
            query.Append(TABLE);
            query.Append(" WHERE username = ");
            query.Append("'");
            query.Append(login);
            query.Append("'");

            var dataTable = await psmDatabaseInstance.GetDataTableAsync(query.ToString());
            if (dataTable != null && dataTable.Rows.Count > 0) {
                var row = dataTable.Rows[0];
                var user = new User() {
                    Email = dataTable.Columns.Contains("Email") ? row["Email"].ToString() : null
                };
                return user;
            }

            return null;
        }
    }
}
