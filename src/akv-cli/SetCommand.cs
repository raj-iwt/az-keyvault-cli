using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace akv_cli
{
    [Command(Name = "set", Description = "Set Azure Key Vault credentials")]
    class SetCommand
    {
        [Option("--uri", Description = "Azure Key Vault URI")]
        public string KeyVaultUri { get; }

        [Option("--tenant", Description = "Azure Tenant ID")]
        public string TenantId { get; }

        [Option("--client-id", Description = "Azure Client ID")]
        public string ClientId { get; }

        [Option("--client-secret", Description = "Azure Client Secret")]
        public string ClientSecret { get; }

        private void OnExecute()
        {
            var config = new KeyVaultConfig
            {
                KeyVaultUri = KeyVaultUri,
                TenantId = TenantId,
                ClientId = ClientId,
                ClientSecret = ClientSecret
            };

            var configJson = JsonConvert.SerializeObject(config);
            File.WriteAllText("keyvault_config.json", configJson);
            Console.WriteLine("Key Vault credentials have been set.");
        }
    }
}
