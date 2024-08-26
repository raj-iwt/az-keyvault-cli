using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace akv_cli
{
    // Create a list command for listing secrets
    [Command(Name = "list", Description = "List secrets in Azure Key Vault")]
    class ListCommand
    {
        private async Task OnExecuteAsync()
        {
            if (!File.Exists("keyvault_config.json"))
            {
                Console.WriteLine("Key Vault credentials have not been set. Use the 'set' command to provide credentials.");
                return;
            }

            var configContent = File.ReadAllText("keyvault_config.json");
            var keyVaultConfig = JsonConvert.DeserializeObject<KeyVaultConfig>(configContent);

            var clientSecretCredential = new ClientSecretCredential(keyVaultConfig.TenantId, keyVaultConfig.ClientId, keyVaultConfig.ClientSecret);
            var client = new SecretClient(new Uri(keyVaultConfig.KeyVaultUri), clientSecretCredential);

            try
            {
                await foreach (var secret in client.GetPropertiesOfSecretsAsync())
                {
                    Console.WriteLine(secret.Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing secrets: {ex.Message}");
            }
        }
    }
}
