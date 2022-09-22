using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace key_vault_console_app
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string secretName = "azureClientId";
            var keyVaultName = "vaultNameavazkey01";
            var kvUri = $"https://{keyVaultName}.vault.azure.net";
            

            //$Env: AZURE_CLIENT_ID = "generated-app-ID"
            //$Env: AZURE_CLIENT_SECRET = "random-password"
            //$Env: AZURE_TENANT_ID = "tenant-ID"

            // including AZURE_CLIENT_ID, AZURE_CLIENT_SECRET, and AZURE_TENANT_ID.
            // Create a new secret client using the default credential from Azure.Identity using environment variables previously set,
            var cred = new DefaultAzureCredential();
          
            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

            var secretValue = "SECRET_VALUE"; ;

            Console.Write($"Creating a secret in {keyVaultName} called '{secretName}' with the value '{secretValue}' ...");
            await client.SetSecretAsync(secretName, secretValue);
            Console.WriteLine(" done.");

            Console.WriteLine("Forgetting your secret.");
            secretValue = string.Empty;
            //Console.WriteLine($"Your secret is '{secretValue}'.");

            Console.WriteLine($"Retrieving your secret from {keyVaultName}.");
            var secret = await client.GetSecretAsync(secretName);
            Console.WriteLine($"Your secret is '{secret.Value.Value}'.");

            Console.Write($"Deleting your secret from {keyVaultName} ...");
            DeleteSecretOperation operation = await client.StartDeleteSecretAsync(secretName);
            // You only need to wait for completion if you want to purge or recover the secret.
            await operation.WaitForCompletionAsync();
            Console.WriteLine(" done.");

            Console.Write($"Purging your secret from {keyVaultName} ...");
            await client.PurgeDeletedSecretAsync(secretName);
            Console.WriteLine(" done.");
        }
    }
}
