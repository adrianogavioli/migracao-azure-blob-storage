using Azure.Storage.Blobs;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MigracaoFotosItens
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var contador = 0;
            IEnumerable<Item> itens;

            using (SqlConnection connection = new SqlConnection(AppSettings.ConnectionString))
            {
                itens = connection.Query<Item>("select que obtem a url da imagem");
            }

            if (itens.Count() == 0)
            {
                Console.WriteLine("Nenhum item foi encontrado.");

                return;
            }

            var blobServiceClient = new BlobServiceClient(AppSettings.ConnectionStorage);

            var containerClient = blobServiceClient.GetBlobContainerClient(AppSettings.ContainerName);

            foreach (var item in itens)
            {
                var fileName = GetFileName(item.Imagem);

                var blobClient = containerClient.GetBlobClient(fileName);

                try
                {
                    await blobClient.UploadAsync(GetImage(item.Imagem), true);
                }
                catch
                {
                    Console.WriteLine($"###ERRO -> {blobClient.Uri}");

                    continue;
                }

                Console.WriteLine(blobClient.Uri);

                contador++;
            }

            Console.WriteLine("------------------------------------------");
            Console.WriteLine($"Total de imagens transferidas: {contador}");
        }

        private static string GetFileName(string filePath)
        {
            return filePath.Split("/")[^1];
        }

        private static Stream GetImage(string imagePath)
        {
            return new WebClient().OpenRead(imagePath);
        }
    }
}
