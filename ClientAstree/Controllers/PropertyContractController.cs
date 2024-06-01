using ClientAstree.Contracts;
using ClientAstree.Models;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using iText.IO.Image;
namespace ClientAstree.Controllers
{
    public class PropertyContractController: Controller
    {
                private readonly IUserService _userService;
          private readonly IAuthenticationService _authService;
            private readonly IAutomobileService _automobileService;
            private readonly IPropertyService _propertyService;

        public PropertyContractController(IUserService leaveTypeService,IAuthenticationService authService,IAutomobileService automobileService,IPropertyService propertyService)
        {
            this._userService = leaveTypeService;
            this._authService = authService;
            this._automobileService=automobileService;
            this._propertyService=propertyService;
        }

        [HttpGet]
        public async Task<IActionResult> MyContract(string searchLocation = null, string searchType = null, int? searchValue = null, int pageNumber = 1, int pageSize = 9)
        {
            var contracts = await _propertyService.GetMyPropertyContractsAsync() ?? new List<PropertyVM>();

            // Apply search and filter
            if (!string.IsNullOrEmpty(searchLocation))
            {
                contracts = contracts.Where(c => c.Location?.Contains(searchLocation, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            if (!string.IsNullOrEmpty(searchType))
            {
                contracts = contracts.Where(c => c.Type?.Contains(searchType, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            if (searchValue.HasValue)
            {
                contracts = contracts.Where(c => c.PropertyValue == searchValue.Value).ToList();
            }

            // Apply pagination
            var pagedContracts = contracts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(contracts.Count / (double)pageSize);

            return View(pagedContracts);
        }



                [HttpGet]
        public async Task<IActionResult> Index(string searchLocation = null, string searchType = null, int? searchValue = null, int pageNumber = 1, int pageSize = 10)
        {
            var contracts = await _propertyService.PropertyAllAsync() ?? new List<PropertyVM>();

            // Apply search and filter
            if (!string.IsNullOrEmpty(searchLocation))
            {
                contracts = contracts.Where(c => c.Location?.Contains(searchLocation, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            if (!string.IsNullOrEmpty(searchType))
            {
                contracts = contracts.Where(c => c.Type?.Contains(searchType, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            if (searchValue.HasValue)
            {
                contracts = contracts.Where(c => c.PropertyValue == searchValue.Value).ToList();
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
    return View(new PropertyVM());
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(PropertyVM model)
{
    // Manual validation
    var locationParts = model.Location.Split(',');
    if (locationParts.Length != 2 || string.IsNullOrWhiteSpace(locationParts[0]) || string.IsNullOrWhiteSpace(locationParts[1]))
    {
        ModelState.AddModelError("Location", "Address and state are required.");
    }

    if (string.IsNullOrWhiteSpace(model.ContractType))
    {
        ModelState.AddModelError("ContractType", "Contract type is required.");
    }

    if (model.StartDate == default)
    {
        ModelState.AddModelError("StartDate", "Start date is required.");
    }

    if (model.EndDate == default)
    {
        ModelState.AddModelError("EndDate", "End date is required.");
    }
    else if (model.EndDate <= model.StartDate)
    {
        ModelState.AddModelError("EndDate", "End date must be later than start date.");
    }

    if (string.IsNullOrWhiteSpace(model.Type))
    {
        ModelState.AddModelError("Type", "Property type is required.");
    }

    if (model.YearOfConstruction == default)
    {
        ModelState.AddModelError("YearOfConstruction", "Year of construction is required.");
    }

    if (model.PropertyValue <= 0)
    {
        ModelState.AddModelError("PropertyValue", "Property value must be greater than zero.");
    }

    if (string.IsNullOrWhiteSpace(model.Coverage))
    {
        ModelState.AddModelError("Coverage", "Coverage is required.");
    }

    if (ModelState.IsValid)
    {
        var createdProperty = await _propertyService.CreatePropertyAsync(model);
        if (createdProperty != null)
        {
            return RedirectToAction(nameof(Details), new { id = createdProperty.Id });
        }
        ModelState.AddModelError("", "Failed to create property contract");
    }

    return View(model);
}




[HttpGet]
public async Task<IActionResult> Details(long id)
{
    // Fetch validated properties
    var properties = await _propertyService.GetAllValidatedPropertiesAsync() ?? new List<PropertyVM>();
    
    // Log the fetched validated properties
    foreach (var validatedProperty in properties)
    {
        Console.WriteLine($"Validated Property: Id={validatedProperty.Id}");
    }

    // Fetch the specific property by id
    var property = properties.FirstOrDefault(p => p.Id == id) ?? await _propertyService.GetPropertyByIdAsync(id);

    // Check if the property is null
    if (property == null)
    {
        return NotFound("Property not found.");
    }

    // Determine if the property is validated using a loop and if statement
    bool isValidated = false;
    foreach (var validatedProperty in properties)
    {
        if (validatedProperty.Id == id)
        {
            isValidated = true;
            break;
        }
    }

    // Assign the validation status to ViewBag
    ViewBag.Validated = isValidated;

    // Log the validation status
    Console.WriteLine($"Property Id={id} Validated={isValidated}");

    return View(property);
}




[HttpGet]
public async Task<IActionResult> DownloadPdf(long id)
{
    var property = await _propertyService.GetPropertyByIdAsync(id);
    if (property == null)
    {
        return NotFound("Property not found.");
    }

    var userProfile = await _userService.ProfileAsync();
    using (var stream = new MemoryStream())
    {
        var writer = new PdfWriter(stream);
        var pdf = new PdfDocument(writer);
        var document = new Document(pdf);
        document.SetMargins(20, 20, 20, 20);

        // Add header with company logo and title
        var headerTable = new Table(new float[] { 1, 1, 1 }).UseAllAvailableWidth();
        var logo = ImageDataFactory.Create("wwwroot/img/logo1.png");
        var logoImage = new Image(logo).ScaleAbsolute(100, 120);

        // Fetch the QR code image from the URL
        using (var client = new HttpClient())
        {
            var qrImageBytes = await client.GetByteArrayAsync($"https://localhost:7166/api/Property/{id}/qr");
            var qrImageData = ImageDataFactory.Create(qrImageBytes);
            var qrImage = new Image(qrImageData).ScaleAbsolute(100, 100);

            headerTable.AddCell(new Cell().Add(logoImage)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetBorder(Border.NO_BORDER));
            headerTable.AddCell(new Cell().Add(new Paragraph("Devis Propriété"))
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

        // Add property details
        var propertyDetailsTable = new Table(new float[] { 1, 1 }).UseAllAvailableWidth();
        propertyDetailsTable.SetMarginBottom(10);
        propertyDetailsTable.AddCell(CreateHeaderCell("Données de la propriété", 2));

        propertyDetailsTable.AddCell(CreateDetailCell("Location"));
        propertyDetailsTable.AddCell(CreateDetailCell(property.Location));

        propertyDetailsTable.AddCell(CreateDetailCell("Valeur de la propriété (DT)"));
        propertyDetailsTable.AddCell(CreateDetailCell(property.PropertyValue.ToString()));

        propertyDetailsTable.AddCell(CreateDetailCell("Type"));
        propertyDetailsTable.AddCell(CreateDetailCell(property.Type));

        propertyDetailsTable.AddCell(CreateDetailCell("Année de construction"));
        propertyDetailsTable.AddCell(CreateDetailCell(property.YearOfConstruction.ToString("dd-MM-yyyy")));

        propertyDetailsTable.AddCell(CreateDetailCell("Date de début"));
        propertyDetailsTable.AddCell(CreateDetailCell(property.StartDate.ToString("dd-MM-yyyy")));

        propertyDetailsTable.AddCell(CreateDetailCell("Date de fin"));
        propertyDetailsTable.AddCell(CreateDetailCell(property.EndDate.ToString("dd-MM-yyyy")));

        propertyDetailsTable.AddCell(CreateDetailCell("Couverture"));
        propertyDetailsTable.AddCell(CreateDetailCell(property.Coverage));

        propertyDetailsTable.AddCell(CreateDetailCell("Quota"));
        propertyDetailsTable.AddCell(CreateDetailCell(property.Quota.ToString("F3") + " Dinars"));

        document.Add(propertyDetailsTable);

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
        var fileName = $"Property_Contract_{property.Id}.pdf";
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
        public async Task<IActionResult> Update(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
            {
                return NotFound();
            }
            return View(property);
        }

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Update(PropertyVM model)
{
    // Manual validation
    var locationParts = model.Location.Split(',');
    if (locationParts.Length != 2 || string.IsNullOrWhiteSpace(locationParts[0]) || string.IsNullOrWhiteSpace(locationParts[1]))
    {
        ModelState.AddModelError("Location", "Address and state are required.");
    }

    if (string.IsNullOrWhiteSpace(model.ContractType))
    {
        ModelState.AddModelError("ContractType", "Contract type is required.");
    }

    if (model.StartDate == default)
    {
        ModelState.AddModelError("StartDate", "Start date is required.");
    }

    if (model.EndDate == default)
    {
        ModelState.AddModelError("EndDate", "End date is required.");
    }
    else if (model.EndDate <= model.StartDate)
    {
        ModelState.AddModelError("EndDate", "End date must be later than start date.");
    }

    if (string.IsNullOrWhiteSpace(model.Type))
    {
        ModelState.AddModelError("Type", "Property type is required.");
    }

    if (model.YearOfConstruction == default)
    {
        ModelState.AddModelError("YearOfConstruction", "Year of construction is required.");
    }

    if (model.PropertyValue <= 0)
    {
        ModelState.AddModelError("PropertyValue", "Property value must be greater than zero.");
    }

    if (string.IsNullOrWhiteSpace(model.Coverage))
    {
        ModelState.AddModelError("Coverage", "Coverage is required.");
    }

    if (ModelState.IsValid)
    {
        await _propertyService.UpdatePropertyAsync(model);
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    return View(model);
}


        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(long id)
{
    await _propertyService.DeletePropertyAsync(id);
    return RedirectToAction("Index"); // Redirect to the listing page after deletion
}





[HttpPost]
[Route("PropertyContract/ValidatePropertyContract/{id}")]
public async Task<IActionResult> ValidatePropertyContract(long id)
{
    await _propertyService.Validate2Async(id);
    string refererUrl = Request.Headers["Referer"].ToString();
    if (string.IsNullOrEmpty(refererUrl))
    {
        return RedirectToAction("Details", new { id });
    }
    return Redirect(refererUrl); // Redirect to the previous page
}

[HttpPost]
[Route("PropertyContract/UnvalidatePropertyContract/{id}")]
public async Task<IActionResult> UnvalidatePropertyContract(long id)
{
    await _propertyService.Unvalidate2Async(id);
    string refererUrl = Request.Headers["Referer"].ToString();
    if (string.IsNullOrEmpty(refererUrl))
    {
        return RedirectToAction("Details", new { id });
    }
    return Redirect(refererUrl); // Redirect to the previous page
}


[HttpGet]
public async Task<IActionResult> Validated(int pageNumber = 1, int pageSize = 10)
{
    var contracts = await _propertyService.GetAllValidatedPropertiesAsync();

    // Pagination logic
    var totalContracts = contracts.Count;
    var pagedContracts = contracts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

    ViewBag.PageNumber = pageNumber;
    ViewBag.PageSize = pageSize;
    ViewBag.TotalPages = (int)Math.Ceiling(totalContracts / (double)pageSize);

    return View(pagedContracts);
}


        [HttpGet]
        public async Task<IActionResult> Unvalidated()
        {
            var contracts = await _propertyService.GetAllUnvalidatedPropertiesAsync();
            return View(contracts);
        }

        [HttpGet]
        public async Task<IActionResult> MyValidatedContracts()
        {
            var contracts = await _propertyService.GetUserValidatedPropertiesAsync();
            return View(contracts);
        }
    }
}