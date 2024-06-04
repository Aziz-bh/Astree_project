namespace ClientAstree.Models
{
    public class DashboardViewModel
    {
        public int AutomobileContractsCount { get; set; }
        public int PropertyContractsCount { get; set; }
        public int UsersCount { get; set; }
        public int ComplaintsCount { get; set; }
        public List<string> Sessions { get; set; }
        public List<string> TotalUsers { get; set; }
        public List<string> ScreenPageViews { get; set; }
        public List<string> BounceRates { get; set; }
        public List<string> EngagementRates { get; set; }
        public List<string> EventCounts { get; set; }
        public List<string> Conversions { get; set; }
        public List<string> Dates { get; set; }
        public List<string> Countries { get; set; }
        public List<string> Regions { get; set; }
        public List<string> SessionSources { get; set; }
        public List<string> SessionDefaultChannelGroupings { get; set; }
        public List<string> PagePaths { get; set; }
        public List<string> ScreenNames { get; set; }
        public List<string> EventNames { get; set; }
        public List<string> DeviceCategories { get; set; }
        public List<string> Browsers { get; set; }
    }
}
