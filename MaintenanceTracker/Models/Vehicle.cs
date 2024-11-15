using System.ComponentModel.DataAnnotations;
namespace MaintenanceTracker.Models
{
    public class Vehicle
    {
        [Required]
        [MaxLength(17), MinLength(17)]
        [RegularExpression("[A-HJ-NPR-Z0-9]{17}", ErrorMessage = "17 Character Valid VIN is required")]
        public string VIN { get; set; } = string.Empty;
        [Required]
        [MaxLength(15), MinLength(1)]
        public string Make { get; set; } = string.Empty;
        [Required]
        [MaxLength(20), MinLength(1)]
        public string Model { get; set; } = string.Empty;
        [Required]
        [Range(1900, 2026)]
        public int Year { get; set; } = DateTime.Now.Year;
        [Required]
        [Range(0.0, 200000.0)]
        public double Price { get; set; } = 0.0;

        public Vehicle()
        {
            VIN = string.Empty;
            Make = string.Empty;
            Model = string.Empty;
            Year = DateTime.Now.Year;
            Price = 0.0;
        }

        public object Shallowcopy()
        {
            return this.MemberwiseClone();
        }

        public bool IsEqual(Vehicle vehicle)
        {
            return vehicle.VIN.Equals(VIN) && vehicle.Make.Equals(Make) && vehicle.Model.Equals(Model) && vehicle.Year.Equals(Year) && vehicle.Price.Equals(Price);
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
