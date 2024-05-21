using ClientAstree.Contracts;
using ClientAstree.Models;
using ClientAstree.Services.Base;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using iText.Layout.Borders;
using iText.IO.Image;

namespace ClientAstree.Controllers
{
    public class AutomobileContractController: Controller
    {
                private readonly IUserService _userService;
          private readonly IAuthenticationService _authService;
            private readonly IAutomobileService _automobileService;
            private readonly IPropertyService _propertyService;

        public AutomobileContractController(IUserService leaveTypeService,IAuthenticationService authService,IAutomobileService automobileService,IPropertyService propertyService)
        {
            this._userService = leaveTypeService;
            this._authService = authService;
            this._automobileService=automobileService;
            this._propertyService=propertyService;
        }

       [HttpGet]
        public async Task<IActionResult> Index(string searchMake = null, string searchModel = null, string searchVehicleType = null, int pageNumber = 1, int pageSize = 10)
        {
            var contracts = await _automobileService.AutomobileAllAsync() ?? new List<AutomobileVM>();

            // Apply search and filter
            if (!string.IsNullOrEmpty(searchMake))
            {
                contracts = contracts.Where(c => c.VehicleMake?.Contains(searchMake, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            if (!string.IsNullOrEmpty(searchModel))
            {
                contracts = contracts.Where(c => c.Model?.Contains(searchModel, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            if (!string.IsNullOrEmpty(searchVehicleType))
            {
                contracts = contracts.Where(c => c.VehicleType?.Contains(searchVehicleType, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            // Apply pagination
            var pagedContracts = contracts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(contracts.Count / (double)pageSize);

            return View(pagedContracts);
        }



[HttpGet]
public async Task<IActionResult> MyContract(string searchMake = null, string searchModel = null, string searchVehicleType = null, int pageNumber = 1, int pageSize = 9)
{
    var contracts = await _automobileService.GetMyAutomobileContractsAsync() ?? new List<AutomobileVM>();

    // Apply search and filter
    if (!string.IsNullOrEmpty(searchMake))
    {
        contracts = contracts.Where(c => c.VehicleMake?.Contains(searchMake, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
    }

    if (!string.IsNullOrEmpty(searchModel))
    {
        contracts = contracts.Where(c => c.Model?.Contains(searchModel, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
    }

    if (!string.IsNullOrEmpty(searchVehicleType))
    {
        contracts = contracts.Where(c => c.VehicleType?.Contains(searchVehicleType, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
    }

    // Apply pagination
    var pagedContracts = contracts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

    ViewBag.PageNumber = pageNumber;
    ViewBag.PageSize = pageSize;
    ViewBag.TotalPages = (int)Math.Ceiling(contracts.Count / (double)pageSize);

    return View(pagedContracts);
}




        [HttpGet]
        public IActionResult Create()
        {
            return View(new AutomobileVM());
        }

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(AutomobileVM model, IFormCollection form)
{
    var validationErrors = new List<string>();

    try
    {
        // Manual extraction as a fallback or to ensure correct binding
        model.VehicleMake = form["VehicleMake"];
        model.Model = form["Model"];
        model.VehicleType = form["VehicleType"];
        model.RegistrationNumber = form["RegistrationNumber"];
        model.ContractType = form["ContractType"];

        if (DateTimeOffset.TryParse(form["RegistrationDate"], out var registrationDate)) {
            model.RegistrationDate = registrationDate;
        }
        if (DateTimeOffset.TryParse(form["StartDate"], out var startDate)) {
            model.StartDate = startDate;
        }
        if (DateTimeOffset.TryParse(form["EndDate"], out var endDate)) {
            model.EndDate = endDate;
        }

        model.EnginePower = int.TryParse(form["EnginePower"], out var enginePower) ? enginePower : 0;
        model.SeatsNumber = int.TryParse(form["SeatsNumber"], out var seatsNumber) ? seatsNumber : 0;
        model.VehicleValue = float.TryParse(form["VehicleValue"], out var vehicleValue) ? vehicleValue : 0;
        model.TrueVehicleValue = float.TryParse(form["TrueVehicleValue"], out var trueVehicleValue) ? trueVehicleValue : 0;

        var guarantees = form["Guarantees"].Select(int.Parse).ToArray();
        model.Guarantees = guarantees.Sum().ToString();

        // Manual validation
        if (string.IsNullOrWhiteSpace(model.VehicleMake))
        {
            validationErrors.Add("Vehicle Make is required.");
        }
        if (string.IsNullOrWhiteSpace(model.Model))
        {
            validationErrors.Add("Model is required.");
        }
        if (string.IsNullOrWhiteSpace(model.VehicleType))
        {
            validationErrors.Add("Vehicle Type is required.");
        }
        if (string.IsNullOrWhiteSpace(model.RegistrationNumber))
        {
            validationErrors.Add("Registration Number is required.");
        }
        if (model.EndDate <= model.StartDate)
        {
            validationErrors.Add("End date must be later than start date.");
        }
        if (model.VehicleValue <= 0)
        {
            validationErrors.Add("Vehicle value must be greater than zero.");
        }
        if (model.EnginePower <= 0)
        {
            validationErrors.Add("Engine power must be greater than zero.");
        }
        if (model.SeatsNumber <= 0)
        {
            validationErrors.Add("Seats number must be greater than zero.");
        }
        if (model.TrueVehicleValue <= 0)
        {
            validationErrors.Add("True vehicle value must be greater than zero.");
        }
        if (string.IsNullOrWhiteSpace(model.Guarantees))
        {
            validationErrors.Add("Guarantees are required.");
        }

        // Check if there are any validation errors
        if (!validationErrors.Any())
        {
            var createdAutomobile = await _automobileService.CreateAutomobileAsync(model);
            if (createdAutomobile != null)
            {
                return RedirectToAction(nameof(ContractDetails), new { id = createdAutomobile.Id });
            }
            validationErrors.Add("Failed to create automobile contract.");
        }
    }
    catch (Exception ex)
    {
        validationErrors.Add($"An error occurred while creating the automobile contract: {ex.Message}");
    }

    // Pass validation errors to the view
    ViewBag.ValidationErrors = validationErrors;
    return View(model);
}

[HttpGet]
public async Task<IActionResult> ContractDetails(long id)
{
    var contract = await _automobileService.GetAutomobileByIdAsync(id);
    if (contract == null)
    {
        return NotFound("Automobile contract not found.");
    }
    return View(contract);
}


[HttpGet]
public async Task<IActionResult> DownloadPdf(long id)
{
    var automobile = await _automobileService.GetAutomobileByIdAsync(id);
    if (automobile == null)
    {
        return NotFound("Automobile not found.");
    }

    var userProfile = await _userService.ProfileAsync();

    using (var stream = new MemoryStream())
    {
        var writer = new PdfWriter(stream);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);
        document.SetMargins(20, 20, 20, 20);

        // Fetch the QR code image from the URL
        using (var client = new HttpClient())
        {
            var qrImageBytes = await client.GetByteArrayAsync($"https://localhost:7166/api/Automobile/{id}/qr");
            var qrImageData = ImageDataFactory.Create(qrImageBytes);
            var qrImage = new Image(qrImageData).ScaleAbsolute(100, 100);
            
            // Add header with company logo, title, and QR code
            var headerTable = new Table(new float[] { 1, 1, 1 }).UseAllAvailableWidth();
            var logo = ImageDataFactory.Create("wwwroot/img/logo1.png"); // Adjust path as necessary
            var logoImage = new Image(logo).ScaleAbsolute(100, 120); // Adjust size as necessary

            headerTable.AddCell(new Cell().Add(logoImage)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetBorder(Border.NO_BORDER));
            headerTable.AddCell(new Cell().Add(new Paragraph("Devis Automobile"))
                .SetFontSize(18)
                .SetBold()
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(Border.NO_BORDER));
            headerTable.AddCell(new Cell().Add(qrImage)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetBorder(Border.NO_BORDER));
            document.Add(headerTable);
        }

        document.Add(new Paragraph("\n"));

        // Add user details
        var userDetailsTable = new Table(new float[] { 1, 1 }).UseAllAvailableWidth();
        userDetailsTable.SetMarginBottom(10);
        userDetailsTable.AddCell(CreateHeaderCell("L'assuré(e)", 2));

        userDetailsTable.AddCell(CreateDetailCell("Nom"));
        userDetailsTable.AddCell(CreateDetailCell(userProfile.LastName));

        userDetailsTable.AddCell(CreateDetailCell("Prénom"));
        userDetailsTable.AddCell(CreateDetailCell(userProfile.FirstName));

        userDetailsTable.AddCell(CreateDetailCell("CIN"));
        userDetailsTable.AddCell(CreateDetailCell(userProfile.CIN));

        userDetailsTable.AddCell(CreateDetailCell("Mobile"));
        userDetailsTable.AddCell(CreateDetailCell(userProfile.PhoneNumber));

        userDetailsTable.AddCell(CreateDetailCell("Email"));
        userDetailsTable.AddCell(CreateDetailCell(userProfile.Email));

        userDetailsTable.AddCell(CreateDetailCell("Adresse"));
        userDetailsTable.AddCell(CreateDetailCell(userProfile.Nationality));

        userDetailsTable.AddCell(CreateDetailCell("Date de naissance"));
        userDetailsTable.AddCell(CreateDetailCell(userProfile.BirthDate?.ToString("dd-MM-yyyy")));

        document.Add(userDetailsTable);

        // Add vehicle details
        var vehicleDetailsTable = new Table(new float[] { 1, 1 }).UseAllAvailableWidth();
        vehicleDetailsTable.SetMarginBottom(10);
        vehicleDetailsTable.AddCell(CreateHeaderCell("Données du véhicule", 2));

        vehicleDetailsTable.AddCell(CreateDetailCell("Usage"));
        vehicleDetailsTable.AddCell(CreateDetailCell(automobile.VehicleType));

        vehicleDetailsTable.AddCell(CreateDetailCell("Marque"));
        vehicleDetailsTable.AddCell(CreateDetailCell(automobile.VehicleMake));

        vehicleDetailsTable.AddCell(CreateDetailCell("Modèle"));
        vehicleDetailsTable.AddCell(CreateDetailCell(automobile.Model));

        vehicleDetailsTable.AddCell(CreateDetailCell("Immatriculation"));
        vehicleDetailsTable.AddCell(CreateDetailCell(automobile.RegistrationNumber));

        vehicleDetailsTable.AddCell(CreateDetailCell("Valeur vénale (DT)"));
        vehicleDetailsTable.AddCell(CreateDetailCell(automobile.VehicleValue.ToString()));

        vehicleDetailsTable.AddCell(CreateDetailCell("Valeur à neuf (DT)"));
        vehicleDetailsTable.AddCell(CreateDetailCell(automobile.TrueVehicleValue.ToString()));

        vehicleDetailsTable.AddCell(CreateDetailCell("Première mise en circulation"));
        vehicleDetailsTable.AddCell(CreateDetailCell(automobile.RegistrationDate.ToString("dd-MM-yyyy")));

        vehicleDetailsTable.AddCell(CreateDetailCell("Puissance fiscale"));
        vehicleDetailsTable.AddCell(CreateDetailCell(automobile.EnginePower.ToString()));

        vehicleDetailsTable.AddCell(CreateDetailCell("Nombre de places"));
        vehicleDetailsTable.AddCell(CreateDetailCell(automobile.SeatsNumber.ToString()));

        document.Add(vehicleDetailsTable);

        // Add guarantees and capital details
        var guaranteesTable = new Table(new float[] { 2, 1 }).UseAllAvailableWidth();
        guaranteesTable.SetMarginBottom(10);
        guaranteesTable.AddCell(CreateHeaderCell("Garanties et capitaux", 2));

        guaranteesTable.AddCell(CreateDetailCell("Garantie"));
        guaranteesTable.AddCell(CreateDetailCell(""));

        foreach (var guarantee in automobile.GuaranteesList)
        {
            guaranteesTable.AddCell(CreateDetailCell(guarantee));
            guaranteesTable.AddCell(CreateDetailCell("")); // Placeholder for values if needed
        }
        document.Add(guaranteesTable);

        // Add insurance details
        var insuranceDetailsTable = new Table(new float[] { 1, 1 }).UseAllAvailableWidth();
        insuranceDetailsTable.SetMarginBottom(10);
        insuranceDetailsTable.AddCell(CreateHeaderCell("Prime Assurance T.T.C", 2));
        insuranceDetailsTable.AddCell(CreateDetailCell(automobile.Quota.ToString("F3") + " Dinars"));
        document.Add(insuranceDetailsTable);

        // Add footer
        var footer = new Paragraph("Cette offre n’est donnée qu’à titre indicatif et n’engage nullement la compagnie. La garantie ne peut être considérée comme acquise qu’après la souscription du contrat d’assurance et le paiement de la prime y afférente.")
            .SetFontSize(10)
            .SetTextAlignment(TextAlignment.CENTER);
        document.Add(footer);

        // Close the document to finish writing the PDF content
        document.Close();

        // Convert the MemoryStream to a byte array
        var pdfBytes = stream.ToArray();

        // Return the PDF as a file result
        var fileName = $"Automobile_Contract_{automobile.Id}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }
}

private Cell CreateHeaderCell(string content, int colspan = 1)
{
    return new Cell(1, colspan).Add(new Paragraph(content))
        .SetBold()
        .SetBackgroundColor(new DeviceRgb(0, 46, 95)) // Set the background color to blue
        .SetFontColor(ColorConstants.WHITE) // Set the font color to white
        .SetBorderRadius(new BorderRadius(5)) // Set the border radius to soften the edges
        .SetTextAlignment(TextAlignment.LEFT)
        .SetFontSize(12)
        .SetPadding(5)
        .SetBorder(Border.NO_BORDER);
}

private Cell CreateDetailCell(string content)
{
    return new Cell().Add(new Paragraph(content))
        .SetBorder(Border.NO_BORDER)
        .SetFontSize(10)
        .SetPadding(5);
}



[HttpGet]
public async Task<IActionResult> Update(long id)
{
    var automobile = await _automobileService.GetAutomobileByIdAsync(id);
    if (automobile == null)
    {
        return NotFound("Automobile not found.");
    }
     Console.WriteLine("ModelState is invalid  garanteees: " +automobile.Guarantees);
     foreach(var auto in automobile.GuaranteesList){
         Console.WriteLine("ModelState is invalid  garanteees: " +automobile.Guarantees);
     }
    return View(automobile);
}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Update(AutomobileVM model, IFormCollection form)
{
    try
    {
        model.Id = long.Parse(form["Id"]);
        model.VehicleMake = form["VehicleMake"];
        model.Model = form["Model"];
        model.VehicleType = form["VehicleType"];
        if(model.VehicleType=="0"){
            //  model.VehicleType= VehicleType.Personal;
              model.VehicleType= "Personal";
        }else{
            // model.VehicleType= VehicleType.Business;
            model.VehicleType= "Business";
        }
        model.RegistrationNumber = form["RegistrationNumber"];
        model.RegistrationDate = DateTimeOffset.TryParse(form["RegistrationDate"], out var regDate) ? regDate : model.RegistrationDate;
        model.StartDate = DateTimeOffset.TryParse(form["StartDate"], out var startDate) ? startDate : model.StartDate;
        model.EndDate = DateTimeOffset.TryParse(form["EndDate"], out var endDate) ? endDate : model.EndDate;
        model.EnginePower = int.TryParse(form["EnginePower"], out var enginePower) ? enginePower : model.EnginePower;
        model.SeatsNumber = int.TryParse(form["SeatsNumber"], out var seatsNumber) ? seatsNumber : model.SeatsNumber;
        model.VehicleValue = float.TryParse(form["VehicleValue"], out var vehicleValue) ? vehicleValue : model.VehicleValue;
        model.TrueVehicleValue = float.TryParse(form["TrueVehicleValue"], out var trueVehicleValue) ? trueVehicleValue : model.TrueVehicleValue;
        var guarantees = form["Guarantees"].Select(x => int.Parse(x)).Sum();
        model.Guarantees = guarantees.ToString();

        // Additional server-side validation
        if (model.EndDate <= model.StartDate)
        {
            ModelState.AddModelError("", "End date must be later than start date.");
        }
        if (model.VehicleValue <= 0)
        {
            ModelState.AddModelError("", "Vehicle value must be greater than zero.");
        }

      var c =  await _automobileService.GetAutomobileByIdAsync(model.Id);

if(model.Guarantees == "0"){
    model.Guarantees = c.Guarantees;
}


await _automobileService.UpdateAutomobileAsync(model);
            return RedirectToAction(nameof(Index));
        
    }
    catch (Exception ex)
    {
        ModelState.AddModelError("", $"An error occurred while updating the automobile: {ex.Message}");
    }

    return View(model);
}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(long id)
{
    await _automobileService.DeleteAutomobileAsync(id);
    return RedirectToAction("Index"); // Redirect to the listing page after deletion
}



    }
}