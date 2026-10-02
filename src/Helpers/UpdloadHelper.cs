namespace api_clinic.src.Helpers
{
    public class UploadHelper()
    {
        private readonly string ApiUri = Environment.GetEnvironmentVariable("API_URI") ?? "";

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            try
            {
                return "";
            }
            catch
            {
                return "";
            }
        }

        public async Task<string> SaveFileV1Async(IFormFile file)
        {
            try
            {
                string uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadDirectory)) Directory.CreateDirectory(uploadDirectory);
                string filePath = Path.Combine(uploadDirectory, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return $"{ApiUri}/uploads/{file.FileName}";
            }
            catch
            {
                return "";
            }
        }
    }
}
