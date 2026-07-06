using ImageServer.dto;

namespace ImageServer.service
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool ValidateClient(string clientId, string clientSecret)
        {
            var clients = _configuration.GetSection("Clients").Get<List<ClientCredentials>>() ?? new List<ClientCredentials>();
            return clients.Any(c => c.ClientId == clientId && c.ClientSecret == clientSecret);
        }
    }
}
