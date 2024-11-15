
namespace MaintenanceTracker.Models
{
    public interface IDataModel
    {
        Task<int> CreateVehicle(Vehicle vehicle);
        Task<int> DeleteVehicle(Vehicle vehicle);
        List<Vehicle> ReadVehicles();
        Task<int> UpdateVehicle(Vehicle vehicle, string previousVIN);
        Task<int> DeleteVehicleWithPrejudice(Vehicle vehicle);
        Task<int> CreateTask(MaintenanceTask task);
        Task<int> DeleteTask(MaintenanceTask task);
        List<MaintenanceTask> ReadTasks(string VIN);
        Task<int> UpdateTask(MaintenanceTask task, string previousVIN, string previousTaskName);

    }
}