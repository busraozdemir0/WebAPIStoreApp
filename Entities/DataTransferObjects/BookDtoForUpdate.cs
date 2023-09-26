using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public record BookDtoForUpdate:BookDtoForManipulation
    {
        [Required]
        public int Id { get; init; } // record type olarak tanımladığımız için tanımlandığı yerde initialize edilmeli(init)
        [Required(ErrorMessage ="CategoryId is required")]
        public int CategoryId { get; init; }
    }
}
