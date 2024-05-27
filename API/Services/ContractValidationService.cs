using System.Text;
using API.Interfaces;
using API.Models;
using API.Persistence;
using Microsoft.EntityFrameworkCore;

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
        sb.AppendLine($"<h1>Dear {user.FirstName} {user.LastName},</h1>");
        sb.AppendLine("<p>We would like to inform you that your contract has expired.</p>");
        sb.AppendLine("<p>Here are the details of your contract:</p>");
        sb.AppendLine("<ul>");
        sb.AppendLine($"<li>Contract ID: {contract.Id}</li>");
        sb.AppendLine($"<li>Contract Type: {contract.ContractType}</li>");
        sb.AppendLine($"<li>Start Date: {contract.StartDate.ToString("yyyy-MM-dd")}</li>");
        sb.AppendLine($"<li>End Date: {contract.EndDate.ToString("yyyy-MM-dd")}</li>");
        sb.AppendLine("</ul>");
        sb.AppendLine("<p>Please contact us if you have any questions or need further assistance.</p>");
        sb.AppendLine("<p>Best regards,</p>");
        sb.AppendLine("<p>Astree assurance</p>");
        return sb.ToString();
    }
}
}