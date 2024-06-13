using Business.Interfaces;

namespace Business.Services
{
    public class Service : IService
    {
        public string hello()
        {
            return ("Hello World");
        }
    }
}