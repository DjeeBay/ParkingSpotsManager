using Newtonsoft.Json;
using ParkingSpotsManager.Shared.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSpotsManager.Shared.Database
{
    public static class ParkingSpotsManagerDatabase
    {
        private static string ConnectionString = Environment.GetEnvironmentVariable("SQLCONNSTR_ParkingSpotsManagerConnectionString");

        public static async Task<DataTable> GetDataTableAsync(string query)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString)) {
                var sqlCmd = new SqlCommand(query, connection);

                await connection.OpenAsync();
                var dataReader = await sqlCmd.ExecuteReaderAsync();
                var dataTable = new DataTable();
                dataTable.Load(dataReader);

                return dataTable;
            }
        }
    }
}
