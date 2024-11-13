
namespace MaintenanceTracker.Models
{
    public interface IDataModel
    {
        Task<int> CreateVehicle(Vehicle vehicle);
        Task<int> DeleteVehicle(Vehicle vehicle);
        List<Vehicle> ReadVehicles();
        Task<int> UpdateVehicle(Vehicle vehicle, string previousVIN);
    }
}