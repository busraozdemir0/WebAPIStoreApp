using Entities.Models;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Repositories.EfCore.Extensions
{
    public static class BookRepositoryExtensions
    {
        // fiyata göre filtreleme işlemi
        public static IQueryable<Book> FilterBooks(this IQueryable<Book> books, uint minPrice, uint maxPrice) =>
            books.Where(book => book.Price >= minPrice && book.Price <= maxPrice);
    
        public static IQueryable<Book> Search(this IQueryable<Book> books, string searchTerm)
        {
            if(string.IsNullOrWhiteSpace(searchTerm)) // arama ifadesi yosa sonuç setini olduğu gibi dönüyoruz
                return books;

            // arama ifadesi varsa lojik işleterek dönüyoruz(Kitap adına göre arama)
            var lowerCaseTerm = searchTerm.Trim().ToLower(); // bir kelime nasıl gelirse gelsin hepsini küçük harflere çevirecektir
            return books.Where(b => b.Title
            .ToLower()
            .Contains(searchTerm));
        }
        // Sıralama için
        public static IQueryable<Book> Sort(this IQueryable<Book> books, string orderByQueryString)
        {
            if(string.IsNullOrWhiteSpace(orderByQueryString)) // eğer parametre boşsa id'ye göre sıralayacak
                return books.OrderBy(b => b.Id);

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Book>(orderByQueryString);

            if (orderQuery is null)
                return books.OrderBy(b => b.Id);

            return books.OrderBy(orderQuery);
        }
    }

}
