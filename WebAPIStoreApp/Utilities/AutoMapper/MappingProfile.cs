using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace WebAPIStoreApp.Utilities.AutoMapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<BookDtoForUpdate, Book>(); // BookDtoForUpdate ifadesi Book entity'sine dönüşecek
            CreateMap<Book, BookDto>(); // Book'dan BookDto'ya geçebilmek için tersini de yazdık

        }
    }
}
