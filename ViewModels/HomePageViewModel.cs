using CommunityToolkit.Mvvm.ComponentModel;
using MaintenanceTracker.Models;
using Microsoft.AspNetCore.Routing;
using Radzen;
using Radzen.Blazor;
using Serilog;
using System.ComponentModel.DataAnnotations;
namespace MaintenanceTracker.ViewModels
{
    public partial class HomePageViewModel : ObservableValidator
    {
        #region Parameters
        [ObservableProperty]
        [MaxLength(10)]
        public List<Vehicle> vehicleList;

        /// <summary>
        /// Represents vehicle potential being updated/deleted from db. Used to compare edited vs original vehicle for update logic
        /// </summary>
        [ObservableProperty]
        public Vehicle outgoingVehicle;

        [ObservableProperty]
        public RadzenDataGrid<Vehicle> dataGrid;

        #endregion Parameters

        public HomePageViewModel()
        {
            vehicleList = [];
            DataGrid = new();
            OutgoingVehicle = new Vehicle();
        }

        #region DataFunctions
        public Task LoadData()
        {
            VehicleList = DataModel.ReadVehicles();
            return Task.CompletedTask;
        }

        public async Task<(string, NotificationSeverity)> DeleteRow(Vehicle vehicle)
        {
            try
            {
                if (VehicleList.Contains(vehicle))
                {
                    //delete from local db, then refresh DataGrid (shouldn't have to via OnPropertyChanged
                    int numRemoved = await DataModel.DeleteVehicle(vehicle);
                    if (numRemoved == 1)
                    {
                        Log.Information("{Routine}: Vehicle " + vehicle.VIN + " Successfull removed from system", "DeleteRow");
                        return ("Vehicle Vehicle Successfully Deleted", NotificationSeverity.Success);
                    } else
                    {
                        Log.Error("{Routine}: Vehicle " + vehicle.VIN + " Unable to be deleted", "DeleteRow");
                        return ($"Unable to remove vehicle {vehicle.VIN}", NotificationSeverity.Error);
                    }
                } else
                {
                    Log.Error("{Routine}: Vehicle " + vehicle.VIN + " Unable to be deleted", "DeleteRow");
                    return ($"Vehicle {vehicle.VIN} Not Found in DB", NotificationSeverity.Error);
                }
            } catch (Exception e)
            {
                Log.Error("Exception found in {Routine}: " + e.Message, "DeleteRow");
                return ("Exception thrown when removing vehicle " + vehicle.VIN, NotificationSeverity.Error);
            }           
        }

        public void CancelRowEdit()
        {
            // clear locals, add back old row if you were updating, don't update db
            OutgoingVehicle = new();
        }

        public async Task<(string, NotificationSeverity)> RowChangedHandler(Vehicle vehicle)
        {
            // check for validation explicitly
            if (!vehicle.Validate())
            {
                // pop up a results modal                
                return ("Please Validate All Entries Before Saving", NotificationSeverity.Warning);
            }
            if (!OutgoingVehicle.Validate())
            {
                // Row is changed from a fresh insert, no valid vehicle previously in list
                int numInserted = await DataModel.CreateVehicle(vehicle);
                if (numInserted == 1)
                {
                    await LoadData();
                    Log.Information("{Routine}: New Vehicle " + vehicle.VIN + " added to list", "RowChangedHandler");
                    return ("New Vehicle Successfully Added", NotificationSeverity.Success);
                } else
                {
                    Log.Error("{Routine}: Failed to insert vehicle " + vehicle.VIN + " to list", "RowChangedHandler");
                    return ("Failed to Insert Vehicle into Database", NotificationSeverity.Error);
                }
            } else
            {
                // check if actual changes exist 
                if (vehicle.IsEqual(OutgoingVehicle))
                {
                    // no need for changes, skip some db hits
                    return ($"No Updates Needed for Vehicle {vehicle.VIN}", NotificationSeverity.Info);
                }
                // editing exising row, call Update logic
                int numUpdated = await DataModel.UpdateVehicle(vehicle, OutgoingVehicle.VIN);
                if (numUpdated == 1)
                {
                    // sync list to db 
                    await LoadData();
                    string returnMessage = $"Vehicle {OutgoingVehicle.VIN} Successfully Updated";
                    OutgoingVehicle = new();
                    Log.Information("{Routine}: Vehicle " + OutgoingVehicle.VIN + " Updated to " + vehicle.VIN, "RowChangedHandler");
                    return (returnMessage, NotificationSeverity.Success);
                }
                else
                {
                    //fail state logic, add back pre-update vehicle
                    string returnMessage = $"Vehicle {OutgoingVehicle.VIN} Failed to Update";
                    OutgoingVehicle = new();
                    Log.Error("{Routine}: Vehicle " + OutgoingVehicle.VIN + " Failed to Update", "RowChangedHandler");
                    return (returnMessage, NotificationSeverity.Error);
                }
            }
        }

        #endregion DataFunctions

        #region OnPropertyChangedHandlers
        partial void OnOutgoingVehicleChanged(Vehicle value)
        {
            if (string.IsNullOrWhiteSpace(value.VIN))
            {
                VehicleList.Remove(value);
            }
        }
        #endregion OnPropertyChangedHandlers

        #region ViewFunctions


        #endregion ViewFunctions
    }
}
