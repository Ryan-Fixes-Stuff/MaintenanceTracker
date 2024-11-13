using Bunit;
using MaintenanceTracker.Views.Modals;
using MaintenanceTracker.Models;
using Radzen;
namespace MaintenanceTracker.Tests
{
    /// <summary>
    /// ComponentTester gives some examples on testing Blazor-specific components using BUnit inside an XUnit test
    /// Simple SPA like this is less important to component test, but there are many ways you can write these that can become
    /// Very useful for checking null-parameter inserts, etc.
    /// </summary>
    public class ComponentTester : TestContext
    {
        [Fact]
        public void Test1()
        {
            // can now set parameters for Modal manually and test how it works
            // mock up a dialog service (we won't be navigating or showing dialogues here)
            DialogService ds = new(null, null);
            Vehicle testVehicle = new();
            var modalRender = RenderComponent<VehicleEntryModal>(parameters => parameters.Add(p => p.vehicle, testVehicle).Add(p => p.newVehicle, true).Add(p => p.dialogService, ds));

            Assert.NotNull(modalRender);
            Assert.True(modalRender.Instance.newVehicle.Equals(true));
            Assert.False(modalRender.Instance.vehicle.Validate());
            testVehicle.VIN = "A9T28A798TA98T79A";
            testVehicle.Make = "Chrysler";
            testVehicle.Model = "Voyager";
            testVehicle.Year = 2009;
            testVehicle.Price = 8500.00;

            // re-create component with proper vehicle 
            modalRender = RenderComponent<VehicleEntryModal>(parameters => parameters.Add(p => p.vehicle, testVehicle).Add(p => p.newVehicle, true).Add(p => p.dialogService, ds));
            Assert.True(modalRender.Instance.vehicle.Validate());
        }
    }
}