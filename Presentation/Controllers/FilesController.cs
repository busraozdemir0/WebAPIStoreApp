using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController:ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if(!ModelState.IsValid) // istek geçersiz ise
            {
                return BadRequest();
            }
            // folder
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Media");  // Dosya Media adlı klasöre kaydolacak
            if(!Directory.Exists(folder)) // yol yoksa oluşturacak
            {
                Directory.CreateDirectory(folder); 
            }

            // path
            var path = Path.Combine(folder, file?.FileName);

            // stream
            using(var stream=new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            // response body
            return Ok(new {
                file=file.FileName,
                path=path,
                size=file.Length
            });
                
        }
        // Bir dosyanın indirilebilmesi için (Dosya adını yazıp istek atıyoruz)
        [HttpGet("download")]
        public async Task<IActionResult> Download(string fileName)
        {
            // File Path
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Media", fileName);  //adı girilen dosyayı Media klasörü içerisinde arayacak

            // ContentType : (MIME)
            var provider = new FileExtensionContentTypeProvider();
            if(provider.TryGetContentType(fileName, out var contentType)) // ilgili tipi belirleyebilirse true dönecek
            {
                contentType = "application/octet-stream";  // ilgili dosyanın indirilebilmesini sağlıyoruz
            }

            // Read
            var bytes=await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, contentType, Path.GetFileName(filePath));
        }
    }
}
