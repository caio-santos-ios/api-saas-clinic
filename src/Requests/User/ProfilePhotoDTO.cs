namespace api_clinic.src.Shared.DTOs
{
    public class ProfilePhotoDTO
    {
        public string Id { get; set; } = string.Empty;
        public IFormFile? Photo { get; set; }
    }
}