namespace Brupper.Data.Azure;

public interface IConfiguration
{
    string SasToken { get; set; }
    string TablesSasToken { get; set; }
    string BlobSasToken { get; set; }
}
