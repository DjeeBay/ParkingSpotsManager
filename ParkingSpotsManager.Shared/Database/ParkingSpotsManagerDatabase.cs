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
    public class ParkingSpotsManagerDatabase
    {
        public string ConnectionString
        {
            get => _connectionString;
        }
        private string _connectionString = Environment.GetEnvironmentVariable("ParkingSpotsManagerConnectionString");

        private ParkingSpotsManagerDatabase()
        {
            _connectionString = Environment.GetEnvironmentVariable("SQLCONNSTR_ParkingSpotsManagerConnectionString");
        }

        public static ParkingSpotsManagerDatabase Instance => instance;
        private static ParkingSpotsManagerDatabase instance => new ParkingSpotsManagerDatabase();


        public static async Task<bool> LogIn(string username, string password)
        {
            var client = new HttpClient();
            var result = await client.PostAsync("url", new StringContent("{\"username\":\"test\", \"password\":\"pass\"}"));
            HttpResponseMessage response = await client.GetAsync("url");
            Console.WriteLine(result);
            if (response.IsSuccessStatusCode && response.Content != null) {
                var reader = new JsonTextReader(new StringReader(await response.Content.ReadAsStringAsync()));
                while (reader.Read()) {
                    Console.WriteLine(reader.TokenType);
                    Console.WriteLine(reader.Value);
                }
            }
            return await new Task<bool>(() => true);
        }

        public async Task<DataTable> GetDataTableAsync(string query)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString)) {
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
