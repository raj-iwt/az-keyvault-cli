using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Validation;
using Newtonsoft.Json;

namespace akv_cli
{
    [Command(Name = "akv", Description = "Azure Key Vault Command Line Tool")]
    [Subcommand(typeof(SetCommand))]
    class Program
    {
        [Option("--secret-name", Description = "Name of the secret to retrieve")]
        public string SecretName { get; }

        private async Task OnExecuteAsync()
        {
            if (!File.Exists("keyvault_config.json"))
            {
                Console.WriteLine("Key Vault credentials have not been set. Use the 'set' command to provide credentials.");
                return;
            }

            var configContent = File.ReadAllText("keyvault_config.json");
            var keyVaultConfig = JsonConvert.DeserializeObject<KeyVaultConfig>(configContent);

            if (string.IsNullOrEmpty(SecretName))
            {
                Console.WriteLine("Please provide a secret name using --secret-name to retrieve the secret.");
                return;
            }

            var clientSecretCredential = new ClientSecretCredential(keyVaultConfig.TenantId, keyVaultConfig.ClientId, keyVaultConfig.ClientSecret);
            var client = new SecretClient(new Uri(keyVaultConfig.KeyVaultUri), clientSecretCredential);

            try
            {
                KeyVaultSecret secret = await client.GetSecretAsync(SecretName);
                Console.WriteLine($"Secret: {secret.Value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving secret: {ex.Message}");
            }
        }

        public static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);
    }

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
