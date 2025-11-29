using Microsoft.AspNetCore.Http;

namespace NexusProcure.Core.DTOs;

public class UploadProfilePictureRequest
{
    public IFormFile File { get; set; }
}
