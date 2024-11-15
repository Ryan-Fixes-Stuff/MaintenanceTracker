using Bunit;
using MaintenanceTracker.Models;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace MaintenanceTracker.Tests
{
    /// <summary>
    /// ModelTester tests CRUD-specific model functions after generating Valid and Faulty Vehicle objects
    /// </summary>
    public class ModelTester : TestContext
    {
        [Fact]
        public async void CRUDTest()
        {
            // launch a create, read, update, and delete sequentially after generating some Vehicle properties
            // mix in Task CRUD based on test vehicle VIN

            // SETUP
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();
            Vehicle validVehicle = new();
            DataModel dm = new(config);
            validVehicle.VIN = "A9T28A798TA98T79A";
            validVehicle.Make = "Chrysler";
            validVehicle.Model = "Voyager";
            validVehicle.Year = 2009;
            validVehicle.Price = 8500.00;
            // END SETUP

            // START TESTS
            //create vehicle
            int createResult = await dm.CreateVehicle(validVehicle);
            Assert.Equal(1, createResult);
            //read vehicles
            List<Vehicle> vehicles = dm.ReadVehicles();
            Assert.NotNull(vehicles);
            Assert.True(vehicles.Count != 0);
            //update vehicle
            validVehicle.Year = 2011;
            // add task to vehicle
            MaintenanceTask task = new();
            task.VIN = validVehicle.VIN;
            task.TaskName = "Oil Change";
            task.Status = "New";
            int createTaskResult = await dm.CreateTask(task);
            Assert.Equal(1, createTaskResult);
            // update task
            task.Status = "Accepted";
            int updateTaskResult = await dm.UpdateTask(task, validVehicle.VIN, task.TaskName);
            Assert.Equal(1, updateTaskResult);
            //update vehicle then update tasks
            string oldVIN = validVehicle.VIN;
            validVehicle.VIN = "A9T28A798TA98T79B";
            int updateResult = await dm.UpdateVehicle(validVehicle, task.VIN);
            Assert.Equal(1, updateResult);
            // update Tasks accordingly
            List<MaintenanceTask> tasks = dm.ReadTasks(task.VIN);
            int taskVINUpdateResult = 0;
            foreach (MaintenanceTask t in tasks){
                t.VIN = validVehicle.VIN;
                taskVINUpdateResult += await dm.UpdateTask(t, oldVIN, t.TaskName);
            }
            task.VIN = validVehicle.VIN;
            Assert.Equal(1, taskVINUpdateResult);
            // delete Task
            int deleteTaskResult = await dm.DeleteTask(task);
            Assert.Equal(1, deleteTaskResult);
            // delete Vehicle
            int deleteResult = await dm.DeleteVehicle(validVehicle);
            Assert.Equal(1, deleteResult);
            // FINISH TESTS
        }
    }
}
