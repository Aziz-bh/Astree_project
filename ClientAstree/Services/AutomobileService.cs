using AutoMapper;
using ClientAstree.Contracts;
using ClientAstree.Models;
using ClientAstree.Services.Base;

namespace ClientAstree.Services
{
    public class AutomobileService: BaseHttpService,IAutomobileService
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IMapper _mapper;
        private readonly IClient _httpclient;
                 public AutomobileService(IMapper mapper, IClient httpclient, ILocalStorageService localStorageService) : base(httpclient, localStorageService)
        {
            this._localStorageService = localStorageService;
            this._mapper = mapper;
            this._httpclient = httpclient;
        }

        public async Task<List<AutomobileVM>> GetMyAutomobileContractsAsync()
        { 
            AddBearerToken();
               var Automobiles = await _client.MycontractsAsync();
               Console.WriteLine($"Automobiles: {Automobiles}");
            
            return _mapper.Map<List<AutomobileVM>>(Automobiles);
           
        }
            public async Task<AutomobileVM> GetAutomobileByIdAsync(long id)
    {
        AddBearerToken();
        var automobile = await _httpclient.AutomobileGETAsync(id);
            if (automobile != null)
    {
        Console.WriteLine($"Automobile: ID={automobile.Id}, Type={automobile.VehicleType}, Make={automobile.VehicleMake}, Model={automobile.Model}, GuaranteesList={string.Join(",", automobile.GuaranteesList)}, EndDate={automobile.EndDate}, EnginePower={automobile.EnginePower}, Guarantees={automobile.Guarantees}, Quota={automobile.Quota}");
    }
    else
    {
        Console.WriteLine("Automobile not found.");
    }
        return _mapper.Map<AutomobileVM>(automobile);
    }

    public async Task<AutomobileVM> CreateAutomobileAsync(AutomobileVM automobile)
    {
        AddBearerToken();
            Console.WriteLine($"Received Model: VehicleMake={automobile.VehicleMake}, Model={automobile.Model}, VehicleType={automobile.VehicleType}, " +
        $"RegistrationNumber={automobile.RegistrationNumber}, RegistrationDate={automobile.RegistrationDate}, StartDate={automobile.StartDate}, " +
        $"EndDate={automobile.EndDate}, EnginePower={automobile.EnginePower}, SeatsNumber={automobile.SeatsNumber}, " +
        $"VehicleValue={automobile.VehicleValue}, TrueVehicleValue={automobile.TrueVehicleValue}, Guarantees={automobile.Guarantees}");
       // var dto = _mapper.Map<AutomobileDto>(automobile);

            var dto = new AutomobileDto
    {
        Model = automobile.Model,
        StartDate = automobile.StartDate,
        EndDate = automobile.EndDate,
        VehicleType = Enum.Parse<VehicleType>(automobile.VehicleType),
        RegistrationNumber = automobile.RegistrationNumber,
        RegistrationDate = automobile.RegistrationDate,
        EnginePower = automobile.EnginePower,
        VehicleMake = automobile.VehicleMake,
        SeatsNumber = automobile.SeatsNumber,
        VehicleValue = automobile.VehicleValue,
        TrueVehicleValue = automobile.TrueVehicleValue,
        Guarantees = Enum.Parse<Base.Guarantees>(automobile.Guarantees)
    };
        var createdDto = await _httpclient.AutomobilePOSTAsync(dto);
        return _mapper.Map<AutomobileVM>(createdDto);
    }

    public async Task UpdateAutomobileAsync(AutomobileVM automobile)
    {
        AddBearerToken();
        var dto = _mapper.Map<AutomobileUpdateDto>(automobile);
        await _httpclient.AutomobilePUTAsync(automobile.Id, dto);
    }

    public async Task DeleteAutomobileAsync(long id)
    {
        AddBearerToken();
        await _httpclient.AutomobileDELETEAsync(id);
    }

        public async Task<List<AutomobileVM>> GetUserAutomobiles(int id)
        {
            AddBearerToken();
            var automobiles = await _client.UserAsync(id);
                        if (automobiles != null)
    {
            foreach (var automobile in automobiles)
            {
                Console.WriteLine($"Automobile: ID={automobile.Id}, Type={automobile.VehicleType}, Make={automobile.VehicleMake}, Model={automobile.Model}, GuaranteesList={string.Join(",", automobile.GuaranteesList)}, EndDate={automobile.EndDate}, EnginePower={automobile.EnginePower}, Guarantees={automobile.Guarantees}, Quota={automobile.Quota}");
            }}

            return _mapper.Map<List<AutomobileVM>>(automobiles);
        }
    }
}