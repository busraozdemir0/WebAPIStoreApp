using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace WebAPIStoreApp.Utilities.AutoMapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<BookDtoForUpdate, Book>().ReverseMap(); // BookDtoForUpdate ifadesi Book entity'sine dönüşecek - ReverseMap ile tersi de mümkün olacak
            CreateMap<Book, BookDto>(); // Book'dan BookDto'ya geçebilmek için tersini de yazdık
            CreateMap<BookDtoForInsertion, Book>(); 
            CreateMap<UserForRegistrationDto, User>(); 
        }
    }
}
