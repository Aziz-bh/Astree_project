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
        var dto = _mapper.Map<PropertyDto>(property);
        var createdDto = await _httpclient.PropertyPOSTAsync(dto);
        return _mapper.Map<PropertyVM>(createdDto);
    }

public async Task UpdatePropertyAsync(PropertyVM property)
{
    AddBearerToken();

    // Map the ViewModel to the UpdateDto instead of the full DTO
    var updateDto = new PropertyUpdateDto
    {
        StartDate = property.StartDate,
        EndDate = property.EndDate,
        Location = property.Location,
        Type = int.Parse(property.Type),
        YearOfConstruction = property.YearOfConstruction,
        PropertyValue = property.PropertyValue,
        Coverage = int.Parse(property.Coverage)
    };

    // Make the PUT request using the mapped UpdateDto
    await _httpclient.PropertyPUTAsync(property.Id, updateDto);
}


    public async Task DeletePropertyAsync(long id)
    {
        AddBearerToken();
        await _httpclient.PropertyDELETEAsync(id);
    }

    }
}