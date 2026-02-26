# Azure Blob Storage - Aplicación de Consola

Aplicación de consola en C# (.NET 10.0) que demuestra operaciones básicas con Azure Blob Storage usando autenticación basada en identidad.

## 📋 Requisitos

- .NET 10.0
- Azure Storage Account
- Autenticación de Azure configurada (Azure CLI o Visual Studio)

## 📦 Dependencias

```xml
<PackageReference Include="Azure.Identity" Version="1.17.1" />
<PackageReference Include="Azure.Storage.Blobs" Version="12.27.0" />
```

## 🔑 Autenticación

La aplicación usa `DefaultAzureCredential` con configuración personalizada:

```csharp
DefaultAzureCredentialOptions options = new()
{
    ExcludeEnvironmentCredential = true,
    ExcludeManagedIdentityCredential = true
};
DefaultAzureCredential credential = new(options);
```

## ⚙️ Configuración

Define la cuenta de almacenamiento mediante variable de entorno:

```bash
export AZURE_STORAGE_ACCOUNT="tu-cuenta-de-almacenamiento"
```

O modifica el valor predeterminado en el código:

```csharp
string accountName = Environment.GetEnvironmentVariable("AZURE_STORAGE_ACCOUNT") ?? "stblob0226092309a";
```

## 🚀 Operaciones Principales

### 1. Crear Cliente de Blob Service

```csharp
string blobServiceEndpoint = $"https://{accountName}.blob.core.windows.net";
BlobServiceClient blobServiceClient = new(new Uri(blobServiceEndpoint), credential);
```

### 2. Crear Contenedor

```csharp
string containerName = "wtblob" + Guid.NewGuid().ToString("N");
BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
```

### 3. Crear y Subir Archivo

```csharp
// Crear archivo local
string fileName = "wtfile" + Guid.NewGuid().ToString("N") + ".txt";
string localFilePath = Path.Combine("./data/", fileName);
await File.WriteAllTextAsync(localFilePath, "Felicitaciones, aprobaste!");

// Subir a blob storage
BlobClient blobClient = containerClient.GetBlobClient(fileName);
await using (FileStream uploadFileStream = File.OpenRead(localFilePath))
{
    await blobClient.UploadAsync(uploadFileStream, overwrite: true);
}
```

### 4. Listar Blobs

```csharp
await foreach (var blobItem in containerClient.GetBlobsAsync())
{
    Console.WriteLine("\t" + blobItem.Name);
}
```

### 5. Descargar y Leer Contenido

```csharp
var content = await blobClient.DownloadContentAsync();
Console.WriteLine(content.Value.Content.ToString());
```

## 🏃 Ejecución

```bash
cd "blob storage"
dotnet run
```

## 📁 Estructura del Proyecto

```
blob-storage/
├── blob-storage.sln
├── blob storage/
│   ├── Program.cs          # Código principal
│   ├── blob storage.csproj # Configuración del proyecto
│   └── data/               # Archivos locales generados
└── README.md
```

## 🔍 Flujo de la Aplicación

1. **Autenticación**: Configura `DefaultAzureCredential`
2. **Conexión**: Conecta al Blob Service
3. **Contenedor**: Crea un contenedor único
4. **Archivo Local**: Genera archivo .txt en `./data/`
5. **Upload**: Sube el archivo al blob storage
6. **Lista**: Muestra todos los blobs del contenedor
7. **Download**: Descarga y muestra el contenido del archivo

## 💡 Notas

- Cada ejecución crea un nuevo contenedor con nombre único (GUID)
- Los archivos locales se almacenan en la carpeta `./data/`
- Se requiere permisos de "Storage Blob Data Contributor" en la cuenta de Azure