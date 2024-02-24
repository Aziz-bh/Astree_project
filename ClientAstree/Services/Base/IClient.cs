namespace ClientAstree.Services.Base
{
    public partial interface IClient
    {
        public HttpClient HttpClient { get; }
    }
}