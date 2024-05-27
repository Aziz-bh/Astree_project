using AutoMapper;
using ClientAstree.Contracts;
using ClientAstree.Models;
using ClientAstree.Services.Base;

namespace ClientAstree.Services
{
    public class PropertyService: BaseHttpService,IPropertyService
    {
                private readonly ILocalStorageService _localStorageService;
        private readonly IMapper _mapper;
        private readonly IClient _httpclient;
            public PropertyService(IMapper mapper, IClient httpclient, ILocalStorageService localStorageService) : base(httpclient, localStorageService)
        {
            this._localStorageService = localStorageService;
            this._mapper = mapper;
            this._httpclient = httpclient;
        }

        public async Task<List<PropertyVM>> GetMyPropertyContractsAsync()
        {
             AddBearerToken();
               var Propertys = await _client.Mycontracts2Async();
            
            return _mapper.Map<List<PropertyVM>>(Propertys);
        }
            public async Task<PropertyVM> GetPropertyByIdAsync(long id)
    {
        AddBearerToken();
        var property = await _httpclient.PropertyGETAsync(id);
        return _mapper.Map<PropertyVM>(property);
    }

    public async Task<PropertyVM> CreatePropertyAsync(PropertyVM property)
    {
        AddBearerToken();
        // var dto = _mapper.Map<PropertyDto>(property);
               var dto = new PropertyDto
        {
            StartDate = property.StartDate,
            EndDate = property.EndDate,
            Location = property.Location,
            Type = Enum.Parse<PropertyType>(property.Type), // Convert string to enum; ensure this matches the enum definition
            YearOfConstruction = property.YearOfConstruction,
            PropertyValue = property.PropertyValue,
            Coverage = Enum.Parse<Coverage>(property.Coverage), // Convert string to enum; adjust if Coverage is not an enum
        };
        var createdDto = await _httpclient.PropertyPOSTAsync(dto);
        return _mapper.Map<PropertyVM>(createdDto);
    }

public async Task UpdatePropertyAsync(PropertyVM property)
{
    AddBearerToken();

    bool typeParsed = int.TryParse(property.Type, out int type);
    bool coverageParsed = int.TryParse(property.Coverage, out int coverage);

    var updateDto = new PropertyUpdateDto
    {
        StartDate = property.StartDate,
        EndDate = property.EndDate,
        Location = property.Location,
            Type = (int)Enum.Parse<PropertyType>(property.Type), // Convert string to enum; ensure this matches the enum definition
        YearOfConstruction = property.YearOfConstruction,
        PropertyValue = property.PropertyValue,
Coverage = (int)Enum.Parse<Coverage>(property.Coverage), 
    };

    // Make the PUT request using the mapped UpdateDto
    await _httpclient.PropertyPUTAsync(property.Id, updateDto);
}


    public async Task DeletePropertyAsync(long id)
    {
        AddBearerToken();
        await _httpclient.PropertyDELETEAsync(id);
    }

        public async Task<List<PropertyVM>> GetUserPropertys(int id)
        {
                       AddBearerToken();
            var Propertys = await _client.User2Async(id);
                        if (Propertys != null)
    {
            foreach (var Property in Propertys)
            {
                Console.WriteLine($"Property: ID={Property.Id}, Type={Property.Type}, Make={Property.Location}, Model={Property.YearOfConstruction}, Quota={Property.Quota}");
            }}

            return _mapper.Map<List<PropertyVM>>(Propertys);
        }

        public async Task<List<PropertyVM>> PropertyAllAsync()
        {
                         AddBearerToken();
               var Propertys = await _client.PropertyAllAsync();
            
            return _mapper.Map<List<PropertyVM>>(Propertys);
        }

        public async Task<List<PropertyVM>> GetAllValidatedPropertiesAsync()
        {
                         AddBearerToken();
               var Propertys = await _client.Validated3Async();
            
            return _mapper.Map<List<PropertyVM>>(Propertys);
        }

        public async Task<List<PropertyVM>> GetAllUnvalidatedPropertiesAsync()
        {
                         AddBearerToken();
               var Propertys = await _client.Unvalidated2Async();
            
            return _mapper.Map<List<PropertyVM>>(Propertys);
        }

        public async Task<List<PropertyVM>> GetUserValidatedPropertiesAsync()
        {
                         AddBearerToken();
               var Propertys = await _client.Validated4Async();
            
            return _mapper.Map<List<PropertyVM>>(Propertys);
        }

        public async Task Validate2Async(long id)
        {
                    AddBearerToken();
        await _httpclient.Validate2Async(id);
        }

        public async Task Unvalidate2Async(long id)
        {
                    AddBearerToken();
        await _httpclient.Unvalidate2Async(id);
        }

    }
}