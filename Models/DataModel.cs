using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Serilog;
namespace MaintenanceTracker.Models
{
    public class DataModel
    {
        private static string _dbConnString = "Default";
        public static List<Vehicle> ReadVehicles()
        {
            try
            {
                using (IDbConnection con = new SqliteConnection(_dbConnString))
                {
                    var res = con.Query<Vehicle>("SELECT * FROM Vehicle");
                    return res.ToList();
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception thrown in {Routine} when reading list of vehicles: " + e.Message, "ReadVehicles");
                return [];
            }
        }

        public static async Task<int> CreateVehicle(Vehicle vehicle)
        {
            try
            {
                using (IDbConnection con = new SqliteConnection(_dbConnString))
                {
                    int res = await con.ExecuteAsync("INSERT INTO VEHICLE (VIN, Make, Model, Year, Price) VALUES (@VIN, @Make, @Model, @Year, @Price)", vehicle);
                    return res;
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception thrown in {Routine} for vehicle  " + vehicle.VIN + ": " + e.Message, "CreateVehicle");
                return -1;
            }
        }

        public static async Task<int> DeleteVehicle(Vehicle vehicle)
        {
            try
            {
                using (IDbConnection con = new SqliteConnection(_dbConnString))
                {
                    return await con.ExecuteAsync("DELETE FROM VEHICLE WHERE [VIN] = @VIN", vehicle);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception thrown in {Routine}: " + e.Message, "DeleteVehicle");
                return -1;
            }
        }

        public static async Task<int> UpdateVehicle(Vehicle vehicle, string previousVIN)
        {
            try
            {
                using (IDbConnection con = new SqliteConnection(_dbConnString))
                {
                    int res = await con.ExecuteAsync("UPDATE VEHICLE SET [VIN] = @VIN, [MAKE] = @Make, [MODEL] = @Model, [YEAR] = @Year, [PRICE] = @Price where [VIN] = '" + previousVIN + "'", vehicle);
                    return res;
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception thrown in {Routine} for vehicle  " + vehicle.VIN + ": " + e.Message, "UpdateVehicle");
                return -1;
            }
        }

        public DataModel(IConfiguration config)
        {
            _dbConnString = config.GetConnectionString("Default") ?? "error";
        }
    }
}
