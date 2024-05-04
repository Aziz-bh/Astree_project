
namespace ClientAstree.Services.Base
{
    public partial interface IClient
    {
        public HttpClient HttpClient { get; }

        Task Send2Async(string v, SendMessageDto message);

    }
}