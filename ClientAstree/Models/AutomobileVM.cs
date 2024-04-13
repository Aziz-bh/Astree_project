using ClientAstree.Services.Base;

namespace ClientAstree.Models
{
    public class AutomobileVM
    {
    public long Id { get; set; }
    public string ContractType { get; set; } // Changed from Enum to String
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public float Quota { get; set; }
    public int UserId { get; set; }
    public string VehicleType { get; set; } // Changed from Enum to String
    public string RegistrationNumber { get; set; }
    public DateTimeOffset RegistrationDate { get; set; }
    public int EnginePower { get; set; }
    public string VehicleMake { get; set; }
    public string Model { get; set; }
    public int SeatsNumber { get; set; }
    public float VehicleValue { get; set; }
    public float TrueVehicleValue { get; set; }
    public string Guarantees { get; set; } // Changed from Enum to String
    public ICollection<string> GuaranteesList { get; set; }
    }
}