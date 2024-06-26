using System.ComponentModel.DataAnnotations;

namespace ClientAstree.Models
{
    public class AgencyVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}