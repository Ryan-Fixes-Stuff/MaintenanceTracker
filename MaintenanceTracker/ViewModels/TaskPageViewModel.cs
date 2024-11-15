using CommunityToolkit.Mvvm.ComponentModel;
using MaintenanceTracker.Models;
using Radzen;
using Serilog;
namespace MaintenanceTracker.ViewModels
{
    public partial class TaskPageViewModel(IDataModel DM) : ObservableValidator
    {
        public IDataModel dm = DM;

        [ObservableProperty]
        public string taskVIN = string.Empty;

        [ObservableProperty]
        public List<MaintenanceTask> taskList = new();

        [ObservableProperty]
        public List<string> statusOptions = ["New", "Accepted", "Cancelled", "In Progress", "Complete"];

        [ObservableProperty]
        public MaintenanceTask outgoingTask = new();



        #region DataFunctions
        /// <summary>
        /// Pulls all vehicles from local.db.Vehicle, and deletes any that may have maliciously entered the database
        /// </summary>
        /// <returns></returns>
        public Task LoadData()
        {
            TaskList = dm.ReadTasks(TaskVIN);
            // return dummy value, future work could require actual return values
            return Task.CompletedTask;
        }

        /// <summary>
        /// Delete row from Vehicle table by checking to make sure vehicle passed currently exists in the list, then calling
        /// specific IDelayModel function DeleteVehicle. This creates minor separation between VehicleList and database, which
        /// can skip db access time if value being deleted is not held locally
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns>Result Tuple</returns>
        public async Task<(string, NotificationSeverity)> DeleteRow(MaintenanceTask task)
        {
            try
            {
                if (TaskList.Contains(task))
                {
                    //delete from local db, then refresh VehicleList
                    int numRemoved = await dm.DeleteTask(task);
                    if (numRemoved == 1)
                    {
                        await LoadData();
                        Log.Information("{Routine}: Task " + task.VIN + " Successfull removed from system", "DeleteTask");
                        return ("Task Successfully Deleted", NotificationSeverity.Success);
                    }
                    else
                    {
                        await LoadData();
                        Log.Error("{Routine}: Task " + task.TaskName + " for vehicle " + task.VIN + " Unable to be deleted", "DeleteTask");
                        return ($"Unable to remove task {task.TaskName} from vehicle {task.VIN}", NotificationSeverity.Error);
                    }
                }
                else
                {
                    Log.Error("{Routine}: Task " + task.TaskName + " for vehicle " + task.VIN + " Unable to be deleted", "DeleteRow");
                    return ($"Task {task.TaskName} Not Found in DB", NotificationSeverity.Error);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception found in {Routine}: " + e.Message, "DeleteRow");
                return ("Exception thrown when removing Task " + task.TaskName + "for vehicle " + task.VIN, NotificationSeverity.Error);
            }
        }


        public void CancelRowEdit()
        {
            // clear locals, add back old row if you were updating, don't update db
            OutgoingTask = new();
        }

        /// <summary>
        /// Method handles Create/Update cases for database interaction after validating entries.
        /// Only handles single Vehicle instance at a time.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns>Result Tuple</returns>
        public async Task<(string, NotificationSeverity)> RowChangedHandler(MaintenanceTask task)
        {
            // check for validation explicitly
            if (!task.Validate())
            {
                // pop up a results modal
                return ("Please Validate All Entries Before Saving", NotificationSeverity.Warning);
            }
            if (!OutgoingTask.Validate())
            {
                // Row is changed from a fresh insert, no valid vehicle previously in list
                int numInserted = await dm.CreateTask(task);
                if (numInserted == 1)
                {
                    await LoadData();
                    Log.Information("{Routine}: New Task " + task.TaskName + "for vehicle " + task.VIN + " added to list", "RowChangedHandler");
                    return ("New Task Successfully Added", NotificationSeverity.Success);
                }
                else
                {
                    Log.Error("{Routine}: Failed to insert task " + task.VIN + "for vehicle " + task.VIN + " to list", "RowChangedHandler");
                    return ("Failed to Insert Task into Database", NotificationSeverity.Error);
                }
            }
            else
            {
                // check if actual changes exist
                if (task.IsEqual(OutgoingTask))
                {
                    // no need for changes, skip some db hits
                    return ($"No Updates Needed for Task {task.TaskName}", NotificationSeverity.Info);
                }
                // editing exising row, call Update logic
                int numUpdated = await dm.UpdateTask(task, OutgoingTask.VIN, OutgoingTask.TaskName);
                if (numUpdated == 1)
                {
                    // sync list to db
                    await LoadData();
                    string returnMessage = $"Task {task.TaskName} Successfully Updated";
                    OutgoingTask = new();
                    Log.Information("{Routine}: Task " + OutgoingTask.TaskName + " for Vehicle " + OutgoingTask.VIN + " Updated to " + task.TaskName +" for Vehicle " + task.VIN, "RowChangedHandler");
                    return (returnMessage, NotificationSeverity.Success);
                }
                else
                {
                    //fail state logic
                    string returnMessage = $"Task {OutgoingTask.TaskName} Failed to Update";
                    Log.Error("{Routine}: Vehicle " + OutgoingTask.VIN + " Failed to Update For Task " + OutgoingTask.TaskName, "RowChangedHandler");
                    OutgoingTask = new();
                    return (returnMessage, NotificationSeverity.Error);
                }
            }
        }

        #endregion DataFunctions



    }
}
