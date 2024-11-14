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
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();
            Vehicle validVehicle = new();
            IDataModel dm = new DataModel(config);
            validVehicle.VIN = "A9T28A798TA98T79A";
            validVehicle.Make = "Chrysler";
            validVehicle.Model = "Voyager";
            validVehicle.Year = 2009;
            validVehicle.Price = 8500.00;
            //create
            int createResult = await dm.CreateVehicle(validVehicle);
            Assert.Equal(1, createResult);
            //read
            List<Vehicle> vehicles = dm.ReadVehicles();
            Assert.NotNull(vehicles);
            Assert.True(vehicles.Any());
            //update
            validVehicle.Year = 2011;
            int updateResult = await dm.UpdateVehicle(validVehicle, validVehicle.VIN);
            Assert.Equal(1, updateResult); 
            //delete
            int deleteResult = await dm.DeleteVehicle(validVehicle);
            Assert.Equal(1, deleteResult);
        }
    }
}
