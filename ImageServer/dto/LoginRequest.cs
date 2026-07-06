namespace ImageServer.dto
{
    public class LoginRequest
    {
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
    }

}
