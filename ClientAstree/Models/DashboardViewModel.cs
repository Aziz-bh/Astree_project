namespace ClientAstree.Models
{
    public class DashboardViewModel
    {
        public int AutomobileContractsCount { get; set; }
        public int PropertyContractsCount { get; set; }
        public int UsersCount { get; set; }
        public int ComplaintsCount { get; set; }
        public List<string> Sessions { get; set; } = new List<string>();
        public List<string> TotalUsers { get; set; } = new List<string>();
        public List<string> ScreenPageViews { get; set; } = new List<string>();
        public List<string> BounceRates { get; set; } = new List<string>();
        public List<string> EngagementRates { get; set; } = new List<string>();
        public List<string> EventCounts { get; set; } = new List<string>();
        public List<string> Conversions { get; set; } = new List<string>();
        public List<string> Dates { get; set; } = new List<string>();
        public List<string> Countries { get; set; } = new List<string>();
        public List<string> Regions { get; set; } = new List<string>();
        public List<int> RegionUserCounts { get; set; } = new List<int>();
        public List<string> SessionSources { get; set; } = new List<string>();
        public List<string> SessionDefaultChannelGroupings { get; set; } = new List<string>();
        public List<string> PagePaths { get; set; } = new List<string>();
        public List<string> ScreenNames { get; set; } = new List<string>();
        public List<string> EventNames { get; set; } = new List<string>();
        public List<string> DeviceCategories { get; set; } = new List<string>();
        public List<string> Browsers { get; set; } = new List<string>();
    }
}
