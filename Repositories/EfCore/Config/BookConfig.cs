using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;

namespace Repositories.EfCore.Config
{
    public class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // eğer bu tablo oluştuğunda veri yoksa aşağıdaki kayıtları varsayılan olarak tabloya kaydet.
            builder.HasData(
                    new Book { Id = 1, Title = "Hacivat ve Karagöz", Price = 75, CategoryId=1 },
                    new Book { Id = 2, Title = "Mesnevi", Price = 175, CategoryId = 2 },
                    new Book { Id = 3, Title = "Devlet", Price = 350, CategoryId = 2 }
                );
        }
    }
}
