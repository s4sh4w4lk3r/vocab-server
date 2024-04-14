namespace Vocab.Infrastructure.Configuration
{
    public class KeycloakConfiguration
    {
        public required string MetadataAddress { get; init; }
        public required string[] ValidIssuers { get; set; }
    }
}
