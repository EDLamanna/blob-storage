using Azure.Identity;
using Azure.Storage.Blobs;

Console.WriteLine("Azure Blob Storage exercise\n");

DefaultAzureCredentialOptions options = new()
{
	ExcludeEnvironmentCredential = true,
	ExcludeManagedIdentityCredential = true
};

await ProcessAsync();

Console.WriteLine("\nPress enter to exit the sample application.");
Console.ReadLine();

async Task ProcessAsync()
{
	string accountName = Environment.GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT") ?? "stblob0226092309a";

	DefaultAzureCredential credential = new(options);
	string blobServiceEndpoint = $"https://{accountName}.blob.core.windows.net";
	BlobServiceClient blobServiceClient = new(new Uri(blobServiceEndpoint), credential);

	string containerName = "wtblob" + Guid.NewGuid().ToString("N");
	Console.WriteLine("Creating container: " + containerName);
	BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);

	string localPath = "./data/";
	Directory.CreateDirectory(localPath);

	string fileName = "wtfile" + Guid.NewGuid().ToString("N") + ".txt";
	string localFilePath = Path.Combine(localPath, fileName);

	await File.WriteAllTextAsync(localFilePath, "Felicitaciones, aprobaste!");
	Console.WriteLine("Local file created: " + localFilePath);

	BlobClient blobClient = containerClient.GetBlobClient(fileName);
	Console.WriteLine("Uploading to Blob storage as blob:\n\t{0}", blobClient.Uri);

	await using (FileStream uploadFileStream = File.OpenRead(localFilePath))
	{
		await blobClient.UploadAsync(uploadFileStream, overwrite: true);
	}

	Console.WriteLine("Blob uploaded successfully.");

	Console.WriteLine("Listing blobs in container...");
	await foreach (var blobItem in containerClient.GetBlobsAsync())
	{
		Console.WriteLine("\t" + blobItem.Name);
	}

	var content = await blobClient.DownloadContentAsync();
	Console.WriteLine("\nContenido del fichero subido al blob:");
	Console.WriteLine(content.Value.Content.ToString());
}
