using System.Text;
using API.Interfaces;
using API.Models;
using API.Persistence;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;

namespace API.Services
{
    public class ContractValidationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ContractValidationService> _logger;
        private readonly IEmailService _emailService;

        public ContractValidationService(IServiceScopeFactory scopeFactory, ILogger<ContractValidationService> logger, IEmailService emailService)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _emailService = emailService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Contract validation service running at: {time}", DateTimeOffset.Now);
                
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AstreeDbContext>();

                    var currentDate = DateTime.UtcNow.Date;

                    var expiredAutomobiles = await context.Automobiles
                        .Include(c => c.User)
                        .Where(c => c.EndDate < currentDate && c.Validated)
                        .ToListAsync();

                    var expiredProperties = await context.Properties
                        .Include(c => c.User)
                        .Where(c => c.EndDate < currentDate && c.Validated)
                        .ToListAsync();

                    foreach (var automobile in expiredAutomobiles)
                    {
                        automobile.Validated = false;
                        context.Automobiles.Update(automobile);
                        await NotifyUserAsync(automobile.User, automobile);
                    }

                    foreach (var property in expiredProperties)
                    {
                        property.Validated = false;
                        context.Properties.Update(property);
                        await NotifyUserAsync(property.User, property);
                    }

                    await context.SaveChangesAsync();

                    // Generate report of validated contracts
                    await GenerateValidatedContractsReportAsync(context);
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task NotifyUserAsync(User user, Contract contract)
        {
            if (user == null)
            {
                _logger.LogWarning("User not found for contract ID {ContractId}", contract.Id);
                return;
            }

            string subject = "Your contract has expired";
            string body = BuildEmailBody(user, contract);
            await _emailService.SendEmailAsync(user.Email, subject, body);
        }

        private string BuildEmailBody(User user, Contract contract)
        {
            var sb = new StringBuilder();

            // Inline CSS for styling
            sb.AppendLine("<div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>");

            // Header
            sb.AppendLine($"<h1 style='color: #007bff;'>Dear {user.FirstName} {user.LastName},</h1>");
            sb.AppendLine("<p>We hope this email finds you well.</p>");
            sb.AppendLine("<p>We would like to inform you that your contract has expired.</p>");

            // Main Content
            sb.AppendLine("<p>Here are the details of your contract:</p>");
            sb.AppendLine("<ul style='list-style-type: none; padding: 0;'>");
            sb.AppendLine($"<li><strong>Contract ID:</strong> {contract.Id}</li>");
            sb.AppendLine($"<li><strong>Contract Type:</strong> {contract.ContractType}</li>");
            sb.AppendLine($"<li><strong>Start Date:</strong> {contract.StartDate:yyyy-MM-dd}</li>");
            sb.AppendLine($"<li><strong>End Date:</strong> {contract.EndDate:yyyy-MM-dd}</li>");
            sb.AppendLine("</ul>");

            // Call to Action (Optional)
            sb.AppendLine("<p style='margin-top: 20px;'><a href='https://localhost:7054/' style='display: inline-block; padding: 10px 20px; background-color: #007bff; color: #fff; text-decoration: none; border-radius: 5px;'>Visit our website</a></p>");

            // Footer
            sb.AppendLine("<p>Please contact us if you have any questions or need further assistance.</p>");
            sb.AppendLine("<p>Best regards,</p>");
            sb.AppendLine("<p style='color: #007bff;'>Astree Assurance</p>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }

        private async Task GenerateValidatedContractsReportAsync(AstreeDbContext context)
        {
            var allAutomobiles = await context.Automobiles
                .Include(a => a.User)
                .ToListAsync();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Reports", "ValidatedContractsReport.xlsx");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("ValidatedContracts");

                // Adding headers
                worksheet.Cells[1, 1].Value = "Id";
                worksheet.Cells[1, 2].Value = "ContractType";
                worksheet.Cells[1, 3].Value = "StartDate";
                worksheet.Cells[1, 4].Value = "EndDate";
                worksheet.Cells[1, 5].Value = "Quota";
                worksheet.Cells[1, 6].Value = "UserId";
                worksheet.Cells[1, 7].Value = "VehicleType";
                worksheet.Cells[1, 8].Value = "RegistrationNumber";
                worksheet.Cells[1, 9].Value = "RegistrationDate";
                worksheet.Cells[1, 10].Value = "EnginePower";
                worksheet.Cells[1, 11].Value = "VehicleMake";
                worksheet.Cells[1, 12].Value = "Model";
                worksheet.Cells[1, 13].Value = "SeatsNumber";
                worksheet.Cells[1, 14].Value = "VehicleValue";
                worksheet.Cells[1, 15].Value = "TrueVehicleValue";
                worksheet.Cells[1, 16].Value = "Guarantees";
                worksheet.Cells[1, 17].Value = "Valid"; // New column for validity

                // Adding data rows
                int row = 2;
                foreach (var automobile in allAutomobiles)
                {
                    worksheet.Cells[row, 1].Value = automobile.Id;
                    worksheet.Cells[row, 2].Value = automobile.ContractType.ToString();
                    worksheet.Cells[row, 3].Value = automobile.StartDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 4].Value = automobile.EndDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 5].Value = automobile.Quota;
                    worksheet.Cells[row, 6].Value = automobile.UserId;
                    worksheet.Cells[row, 7].Value = automobile.VehicleType.ToString();
                    worksheet.Cells[row, 8].Value = automobile.RegistrationNumber;
                    worksheet.Cells[row, 9].Value = automobile.RegistrationDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 10].Value = automobile.EnginePower;
                    worksheet.Cells[row, 11].Value = automobile.VehicleMake;
                    worksheet.Cells[row, 12].Value = automobile.Model;
                    worksheet.Cells[row, 13].Value = automobile.SeatsNumber;
                    worksheet.Cells[row, 14].Value = automobile.VehicleValue;
                    worksheet.Cells[row, 15].Value = automobile.TrueVehicleValue;
                    worksheet.Cells[row, 16].Value = automobile.Guarantees.ToString();
                    worksheet.Cells[row, 17].Value = automobile.Validated ? 1 : 0; // Validity column
                    row++;
                }

                // Styling the header
                using (var range = worksheet.Cells[1, 1, 1, 17])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // AutoFit columns
                worksheet.Cells.AutoFitColumns();

                // Save the Excel package
                var reportsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
                if (!Directory.Exists(reportsDirectory))
                {
                    Directory.CreateDirectory(reportsDirectory);
                }
                var fileInfo = new FileInfo(filePath);
                await package.SaveAsAsync(fileInfo);
            }
        }
    }
}
