namespace Brupper.Forms.Services.Interfaces
{
    public interface IImageResizer
    {
        byte[] Resize(byte[] imageData, float width, float height, int imageQuality = 50);
    }
}
