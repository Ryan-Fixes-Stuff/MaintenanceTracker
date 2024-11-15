using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Serilog;
namespace MaintenanceTracker.Models
{
    public class DataModel(IConfiguration config) : IDataModel
    {
        private readonly string _dbConnString = config.GetConnectionString("Default") ?? "error";
        #region VehicleFunctions
        public List<Vehicle> ReadVehicles()
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

        public async Task<int> CreateVehicle(Vehicle vehicle)
        {
            if (!vehicle.Validate())
            {
                return -1;
            }
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

        public async Task<int> DeleteVehicle(Vehicle vehicle)
        {
            if (!vehicle.Validate())
            {
                return -1;
            }
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

        public async Task<int> UpdateVehicle(Vehicle vehicle, string previousVIN)
        {
            if (!vehicle.Validate())
            {
                return -1;
            }
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

        /// <summary>
        /// Function called when reading from Database and encountering any non-valid vehicle entries.
        /// Differs from DeleteVehicle as that only applies to valid entries
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public async Task<int> DeleteVehicleWithPrejudice(Vehicle vehicle)
        {
            try
            {
                using (IDbConnection con = new SqliteConnection(_dbConnString))
                {
                    int res = await con.ExecuteAsync("DELETE FROM VEHICLE WHERE [VIN] = @VIN, [MAKE] = @Make, [MODEL] = @Model, [YEAR] = @Year, [PRICE] = @Price", vehicle);
                    return res;
                }
            } catch (Exception e)
            {
                Log.Error("Exception thrown in {Routine} for vehicle  " + vehicle.VIN + ": " + e.Message, "DeleteVehicleWithPrejudice");
                return -1;
            }
        }
        #endregion VehicleFunctions

        #region TaskFunctions
        public async Task<int> CreateTask(MaintenanceTask task)
        {
            if (!task.Validate())
            {
                return -1;
            }
            try
            {
                using (IDbConnection con = new SqliteConnection(_dbConnString))
                {
                    int res = await con.ExecuteAsync("INSERT INTO Tasks (VIN, Taskname, Status, InsertDatetime) VALUES (@VIN, @TaskName, @Status, DATETIME('now'))", task);
                    return res;
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception thrown in {Routine} for task " + task.VIN + ": " + e.Message, "CreateTask");
                return -1;
            }
        }

        public async Task<int> DeleteTask(MaintenanceTask task)
        {
            if (!task.Validate())
            {
                return -1;
            }
            try
            {
                using (IDbConnection con = new SqliteConnection(_dbConnString))
                {
                    return await con.ExecuteAsync("DELETE FROM Tasks WHERE [VIN] = @VIN and [TaskName] = @TaskName", task);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception thrown in {Routine} for vehicle " + task.VIN + ": " + e.Message, "DeleteTask");
                return -1;
            }
        }

        public List<MaintenanceTask> ReadTasks(string VIN)
        {
            try
            {
                using (IDbConnection con = new SqliteConnection(_dbConnString))
                {
                    var res = con.Query<MaintenanceTask>($"SELECT * FROM Tasks where [VIN] = '{VIN}'");
                    return res.ToList();
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception thrown in {Routine} when reading list of Tasks: " + e.Message, "ReadTasks");
                return [];
            }
        }

        public async Task<int> UpdateTask(MaintenanceTask task, string previousVIN, string previousTaskName)
        {
            if (!task.Validate())
            {
                return -1;
            }
            try
            {
                using (IDbConnection con = new SqliteConnection(_dbConnString))
                {
                    int res = await con.ExecuteAsync("UPDATE Tasks SET [VIN] = @VIN, [TaskName] = @TaskName, [Status] = @Status where [VIN] = '" + previousVIN + 
                        "' and [TaskName] = '" + previousTaskName + "'", task);
                    return res;
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception thrown in {Routine} for vehicle  " + task.VIN + ": " + e.Message, "UpdateTask");
                return -1;
            }
        }

        #endregion TaskFunctions
    }
}
