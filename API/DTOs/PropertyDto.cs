using API.Models;

namespace API.DTOs
{
    public class PropertyDto : ContractDto
    {
        public string Location { get; set; }
        public PropertyType Type { get; set; }
        public DateTime YearOfConstruction { get; set; }
        public int PropertyValue { get; set; }
        public List<string> CoveragesList => GetCoveragesList(Coverage);
        public Coverage Coverage { get; set; } // Preserve the original property if necessary

        private List<string> GetCoveragesList(Coverage coverages)
        {
            var coveragesList = new List<string>();
            foreach (Coverage coverageValue in Enum.GetValues(typeof(Coverage)))
            {
                if (coverageValue != Coverage.None && coverages.HasFlag(coverageValue))
                {
                    coveragesList.Add(coverageValue.ToString());
                }
            }
            return coveragesList;
        }
    }
}
