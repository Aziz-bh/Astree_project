using API.Models;

namespace API.DTOs
{
    public class AutomobileDto : ContractDto
    {
        public VehicleType VehicleType { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int EnginePower { get; set; }
        public string VehicleMake { get; set; }
        public int SeatsNumber { get; set; }
        public float VehicleValue { get; set; }
        public float TrueVehicleValue { get; set; }
        public Guarantees Guarantees { get; set; }
    }
}
