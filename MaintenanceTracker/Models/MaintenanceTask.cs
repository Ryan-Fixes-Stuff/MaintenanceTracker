using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MaintenanceTracker.Models
{
    public class MaintenanceTask
    {
        [Required]
        [MaxLength(17), MinLength(17)]
        [RegularExpression("[A-HJ-NPR-Z0-9]{17}", ErrorMessage = "17 Character Valid VIN is required")]
        public string VIN { get; set; } = string.Empty;

        [Required]
        [MaxLength(40), MinLength(1)]
        public string TaskName { get; set; } = string.Empty;

        [Required]
        [MaxLength(15), MinLength(1)]
        public string Status { get; set;} = string.Empty;

        [Required]
        public DateTime InsertDatetime { get; set; }


        public MaintenanceTask()
        {
            Status = string.Empty;
            VIN = string.Empty;
            TaskName = string.Empty;
            InsertDatetime = DateTime.Now;
        }
           public object Shallowcopy()
        {
            return this.MemberwiseClone();
        }

        public bool IsEqual(MaintenanceTask task)
        {
            return task.VIN.Equals(VIN) && task.Status.Equals(Status) && task.TaskName.Equals(TaskName) && task.InsertDatetime.Equals(InsertDatetime);
        }

        public bool Validate()
        {
            List<string> invalidMembers = new();
            List<ValidationResult> vr = new List<ValidationResult>();
            var res = Validator.TryValidateObject(this, new(this, null), vr, true);
            return res;
        }
    }
}
