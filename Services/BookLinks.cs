using Entities.DataTransferObjects;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BookLinks : IBookLinks
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<BookDto> _dataShaper;

        public BookLinks(LinkGenerator linkGenerator, IDataShaper<BookDto> dataShaper)
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }

        public LinkResponse TryGenerateLinks(IEnumerable<BookDto> booksDto, string fields, HttpContext httpContext)
        {
            var shapedBooks = ShapedData(booksDto, fields); // şekillendirilmiş olan verileri oluşturacak
            if (ShouldGenerateLinks(httpContext)) // link oluşturabilirse
                return ReturnLinkedBooks(booksDto, fields, httpContext, shapedBooks); // link ifadelerini üreteceğimiz method
            return ReturnShapedBooks(shapedBooks); // eğer false dönerse şekillendirilmiş veriyi dönsün
        }

        private LinkResponse ReturnLinkedBooks(IEnumerable<BookDto> booksDto, string fields, HttpContext httpContext, List<Entity> shapedBooks)
        {
            var bookDtoList = booksDto.ToList();

            for (int index = 0; index < bookDtoList.Count(); index++)
            {
                var bookLinks = CreateForBook(httpContext, bookDtoList[index], fields);
                shapedBooks[index].Add("Links", bookLinks);
            }

            var bookCollection = new LinkCollectionWrapper<Entity>(shapedBooks);
            CreateForBooks(httpContext, bookCollection);
            return new LinkResponse { HasLinks = true, LinkedEntities = bookCollection };
        }
        private LinkCollectionWrapper<Entity>  CreateForBooks(HttpContext httpContext, LinkCollectionWrapper<Entity> bookCollectionWrapper) 
        { // doğrudan kaynağın kendisidir (tüm kitapların listesini döndürecek olan link üretiyoruz)
            bookCollectionWrapper.Links.Add(new Link() {
                Href = $"/api/{httpContext.GetRouteData().Values["controller"].ToString().ToLower()}",
                Rel="self",
                Method="GET",
            });

            return bookCollectionWrapper;
        }
        private List<Link> CreateForBook(HttpContext httpContext, BookDto bookDto, string fields)
        {
            var links = new List<Link>() // link üretme
            {
                new Link()
                {
                    Href=$"/api/{httpContext.GetRouteData().Values["controller"].ToString().ToLower()}" +
                    $"/{bookDto.Id}",   // Href => her bir kayıt için get olarak link ürettik
                    Rel="self",
                    Method="GET",
                },
                new Link()
                {
                    Href=$"/api/{httpContext.GetRouteData().Values["controller"].ToString().ToLower()}",
                    Rel="create",
                    Method="POST",
                },
            };
            return links;
        }

        private LinkResponse ReturnShapedBooks(List<Entity> shapedBooks)
        {
            return new LinkResponse() { ShapedEntities = shapedBooks };
        }

        private bool ShouldGenerateLinks(HttpContext httpContext)
        {
            var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];
            return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
        }

        private List<Entity> ShapedData(IEnumerable<BookDto> booksDto, string fields)
        {
            return _dataShaper.ShapeData(booksDto, fields)
                 .Select(b => b.Entity).ToList();
        }
    }
}
