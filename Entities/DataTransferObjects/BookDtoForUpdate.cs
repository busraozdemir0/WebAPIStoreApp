using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public record BookDtoForUpdate(int Id, string Title, decimal Price);  // 2. yöntem => bu şekilde de tanımlandığı anda parametreler gönderilebilir.
    //{
    //    public int Id { get; init; } // record type olarak tanımladığımız için tanımlandığı yerde initialize edilmeli(init)
    //    public string Title { get; init; } // init yaptığımızda readonly olma özelliği kazandırırız.
    //    public decimal Price { get; init; }
    //}
}
