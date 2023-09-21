using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    //[ApiVersion("1.0")]
    [ServiceFilter(typeof(LogFilterAttribute))] // controller bazlı loglama
    [ApiController]
    //[Route("api/{v:apiversion}/books")]  // URL ile versiyon tanımlama
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly IServiceManager _manager;

        public BooksController(IServiceManager manager)
        {
            _manager = manager;
        }

        [HttpHead] // Get ile benzer işlemdedir. Burada header'daki ifadeleri(meta ifadeleri) okuruz.
        [HttpGet(Name = "GetAllBooksAsync")]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public async Task<IActionResult> GetAllBooksAsync([FromQuery]BookParameters bookParameters)
        {
            var linkParameters = new LinkParameters()
            {
                BookParameters = bookParameters,
                HttpContext = HttpContext
            };

            // Global hata yönetimi yaptığımız için try-catch bloklarını kaldırdık. Hata olduğunda o hatayı yakalayıp ilgili kodu ve mesajı bize döndürecektir
            var result = await _manager
                .BookService
                .GetAllBooksAsync(linkParameters,false); // değişiklikleri izlememesini tercih ettiğimiz için ef core çalışmasında bir performans artışı gözlemleyeceğiz

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData)); // metadataları headers bölümüne ekleme

            // link üretilebildiyse link ile aksi durumda şekillendirilmiş veriyle dönüş yapılacak.
            return result.linkResponse.HasLinks ? Ok(result.linkResponse.LinkedEntities) : Ok(result.linkResponse.ShapedEntities);

        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBookAsync([FromRoute(Name = "id")] int id)  // id Route'dan gelecek
        {
            var book =await _manager.BookService.GetOneBookByIdAsync(id, false);
            return Ok(book);

        }
        [ServiceFilter(typeof(ValidationFilterAttribute))] // böyle bir attribute yazıp servis kaydını da gerçekleştirdiğimiz için action içerisinde modelstate.isvalid gibi kontroller yapmaya gerek yoktur
        [HttpPost(Name = "CreateOneBookAsync")]
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto)
        {
            var book = await _manager.BookService.CreateOneBookAsync(bookDto);

            return StatusCode(201, book); // 201 kodu => created

        }

        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate bookDto)
        {
            await _manager.BookService.UpdateOneBookAsync(id, bookDto, true);

            return NoContent(); //204

        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneBookAsync([FromRoute(Name = "id")] int id)
        {
           await _manager.BookService.DeleteOneBookAsync(id, false);

            return NoContent();
        }
        [HttpPatch("{id:int}")] // kısmi güncelleme(örneğin bir kaydın sadece başlığını güncellemek gibi)
        public async Task<IActionResult> PartiallyUpdateOneBookAsync([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
        {
            if (bookPatch is null)
                return BadRequest(); // 400 code

            var result = await _manager.BookService.GetOneBookForPatchAsync(id, false);

            bookPatch.ApplyTo(result.bookDtoForUpdate ,ModelState);

            TryValidateModel(result.bookDtoForUpdate);

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            await _manager.BookService.SaveChangesForPatchAsync(result.bookDtoForUpdate, result.book);

            return NoContent(); //204
        }

        [HttpOptions]
        public IActionResult GetBooksOptions()
        {
            Response.Headers.Add("Allow", "GET,PUT,POST,PATCH,DELETE,HEAD,OPTIONS");
            return Ok();
        }
    }
}
