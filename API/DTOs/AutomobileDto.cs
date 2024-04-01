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
        public string Model { get; set; }
        public int SeatsNumber { get; set; }
        public float VehicleValue { get; set; }
        public float TrueVehicleValue { get; set; }
         public Guarantees Guarantees { get; set; }
        
        // Add a new property to hold the list of guarantee names
        public List<string> GuaranteesList => GetGuaranteesList(Guarantees);

        private List<string> GetGuaranteesList(Guarantees guarantees)
        {
            var guaranteesList = new List<string>();
            if (guarantees.HasFlag(Guarantees.RC)) guaranteesList.Add(nameof(Guarantees.RC));
            if (guarantees.HasFlag(Guarantees.INC)) guaranteesList.Add(nameof(Guarantees.INC));
            if (guarantees.HasFlag(Guarantees.VOL)) guaranteesList.Add(nameof(Guarantees.VOL));
            if (guarantees.HasFlag(Guarantees.ASST)) guaranteesList.Add(nameof(Guarantees.ASST));
            if (guarantees.HasFlag(Guarantees.TR)) guaranteesList.Add(nameof(Guarantees.TR));

            return guaranteesList;
        }
    }
}
