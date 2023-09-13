using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPIStoreApp.Models;

namespace WebAPIStoreApp.Repositories.Config
{
    public class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // eğer bu tablo oluştuğunda veri yoksa aşağıdaki kayıtları varsayılan olarak tabloya kaydet.
            builder.HasData(
                    new Book { Id = 1, Title = "Hacivat ve Karagöz", Price = 75 },
                    new Book { Id = 2, Title = "Mesnevi", Price = 175 },
                    new Book { Id = 3, Title = "Devlet", Price = 350 }
                );
        }
    }
}
