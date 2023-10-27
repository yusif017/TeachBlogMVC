namespace WebUI.Helper
{
    public static class FileUpload
    {
        public static async Task<string> SaveFileAsync(this IFormFile file, string WebRootPath)
        {
            //if (!Directory.Exists("/uploads/"))
            //{
            //    Directory.CreateDirectory("/uploads/");
            //}
            var path = "/uploads/" + Guid.NewGuid() + file.FileName;
            // using directive
            using FileStream fileStream = new(WebRootPath + path, FileMode.Create);
            await file.CopyToAsync(fileStream);
            return path;
        }
    }
}
