using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    // 2. versiyon için
    //[ApiVersion("2.0",Deprecated = true)] // Deprecated = true ile bu versiyonu yayından kaldıracağımızın bilgisini veriyoruz.
    [ApiController]
    //[Route("api/{v:apiversion}/books")]  // URL ile versiyon tanımlama
    [Route("api/books")]
    public class BooksV2Controller:ControllerBase
    {
        private readonly IServiceManager _manager;

        public BooksV2Controller(IServiceManager manager)
        {
            _manager = manager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBookAsync()
        {
            var books = await _manager.BookService.GetAllBooksAsync(false);
            var booksV2 = books.Select(b => new  // versiyon 2'de sadece Title ve Id gelsin
            {
                Title = b.Title,
                Id = b.Id
            });
            return Ok(booksV2);
        }
    }
}
