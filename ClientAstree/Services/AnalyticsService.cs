using Google.Apis.AnalyticsData.v1beta;
using Google.Apis.AnalyticsData.v1beta.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientAstree.Services
{
    public class AnalyticsService
    {
        private readonly string _serviceAccountEmail;
        private readonly string _privateKey;
        private readonly string _propertyId;

        public AnalyticsService()
        {
            _serviceAccountEmail = "google-analytics@astreeclient.iam.gserviceaccount.com";
            _privateKey = "-----BEGIN PRIVATE KEY-----\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQCiZ6yLjJJ57X1h\ntt+pHiUiBr8x8MIh1p58SAFiG+7UzkVRWH7s5qM+YQbwheUWLxJywOj/XQuEyEtQ\nfOJWRE+YZBII8GJeBBZFqYqhudfLsHLsCihXXdu8ffHianDkgnfMMImmupAX4H7R\nkMlhu4xv6XD6TCOfis9nX+Xs/k0N/ByYQnszjf7B71nF+GkAUVgRkswcV1vmPHXy\nKn5Ib1cbF1ixPPU33ggFwUDYdWp5NSBuWCuAKRHIp6cIz46lT1/NikP6YuxegnnH\n8kY1KhwoXd2TetdbvnN3wwuomVZUOz6bmCL3S+DBHY6Y9PCt8cbBgao2cJej89Uq\nvhAPdOYNAgMBAAECggEAIofVnZRFfqw6yQB3nL8+mPYDSAyPUl+OMhGjTadvyAqV\nnX6p+hft5LD/pzto4qcejoyq3cT8Un692L685ye+HsJEPWdTGvdWfvhV9B4OHo6F\nWN+uBSl9M545xHDqUQPWkfFOC0fGpHUEse1NjDyy1wa1EVsIBf+fKrUur43Ad3Tp\nKM55bBgbEv0QxrQ6qEnTLn9H4eeaGdY49JlwVc4OQ570qGp94mkexTALFw8JI4Z5\nxEdzEdNPaXZKc0wlGuPQfgva3KshQlPYs/Me+wHShVygZWcBQsWiqjwt3hcH/Wdp\neGfGTAT5yWqeGsxVrVOkUPBHaWlFODxhhD9fGF5V4QKBgQDRO6YqeC8G7TWUnrAi\nuN3F5tcEIXUofLFaPg29aBli++gr+o6o9qRzPmy7uY2toDwt8Shf24ao1Zqmty7H\niAFQop8e0JXwB6Z1Op7quus9Aj6SImMnglolP8/Gw6WeFAnG3FDzkyiOPLB/i7Af\nifLKJQ/HrFRrA9Lvjx2dhCMsUQKBgQDGtIQ+iJT8pIG0QXiV2wy8cYLUynTHcW/w\nufAl8Y1Quuq3eicTrlfmeMa9oU4VJlxqTemU7V67ypxr2LtFgsqfDp6qZOlmGLl6\nfiKu0Xf9GbyuFoDvaPo85X3taacGlrdmR3HB+4P4VS9dyQA1WucxXScYu3+6XOCO\nABET7Pv6/QKBgQC3KLpLTy4LqDdRRKD3pFEvXuY2jXtb19wBHXU1qx0eE1qotQuR\nSWeoIqtYohQuO6iypvsq/qcgwOxCqPTgYmSUS+dzODEpD+ldkdVvsfUfUQmpL0UR\nXwC+vt+KJdajTPNKC68pqlbEPMhcvtGMqlvJLSxlJKw5if1rDCNmsjeAIQKBgQCH\nu0FmcefLmBfdaKeVPdc6VhN3hYA1yKAdgPWxBYpxbeztPey5fesfTLr9R9VNjzGr\nj+MRzv9aKfGlnZt2xPvhoJvzaxEfo533NXg9kGp1IoKHLGoQn9XniR/276efUY6f\nuAfV6Rfhp1a+qpGQ4LJyPt54/4Mla1IczaDcypHphQKBgQCaBGjoWyi8Ll3w4BAW\nL2kWOQJElMhJ61NYdK+21NJYqREz1wBuCbELO1HPFhZjASvD1KdwNFFLXcxKC1t5\ntqinVZwjbfIt13FIlPVT/ALOSz5HSSfPtKBfPDdgMBxsDOs75OadWQLybVyJoSte\npvtjiDtYHzWAAmHWZ62zI7Iq+w==\n-----END PRIVATE KEY-----\n";
            _propertyId = "443850222";
        }

        public async Task<RunReportResponse> GetReport()
        {
            var credential = GoogleCredential.FromServiceAccountCredential(
                new ServiceAccountCredential(
                    new ServiceAccountCredential.Initializer(_serviceAccountEmail)
                    {
                        Scopes = new[] { AnalyticsDataService.Scope.AnalyticsReadonly }
                    }.FromPrivateKey(_privateKey)
                ));

            var analyticsDataService = new AnalyticsDataService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "GA4 Reporting"
            });

            var dateRange = new DateRange { StartDate = "30daysAgo", EndDate = "today" };
            var metrics = new List<Metric>
            {
                new Metric { Name = "sessions" },
                new Metric { Name = "totalUsers" },
                new Metric { Name = "screenPageViews" },
                new Metric { Name = "bounceRate" },
                new Metric { Name = "engagementRate" },
                new Metric { Name = "eventCount" },
                new Metric { Name = "conversions" }
            };

            var dimensions = new List<Dimension>
            {
                new Dimension { Name = "date" },
                new Dimension { Name = "country" },
                new Dimension { Name = "region" },
                new Dimension { Name = "sessionSource" },
                new Dimension { Name = "sessionDefaultChannelGrouping" },
                new Dimension { Name = "pagePath" },
                new Dimension { Name = "eventName" },
                new Dimension { Name = "deviceCategory" },
                new Dimension { Name = "browser" }
            };

            var request = new RunReportRequest
            {
                DateRanges = new List<DateRange> { dateRange },
                Metrics = metrics,
                Dimensions = dimensions
            };

            return await analyticsDataService.Properties.RunReport(request, $"properties/{_propertyId}").ExecuteAsync();
        }
    }
}
