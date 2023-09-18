using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestFeatures
{
    public class PagedList<T>:List<T>
    {
        public MetaData MetaData { get; set; }
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            MetaData = new MetaData() // new'leyerek referans aldırmazsak referans hatasıyla karşılaşırız
            {
                TotalCount=count,
                PageSize=pageSize,
                CurrentPage=pageNumber,
                TotalPage=(int)Math.Ceiling(count/(double)pageSize)
            };
            AddRange(items);
        }

        public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source
                .Skip((pageNumber - 1) * pageSize)  // sayfa numarası x sayfada olan kayıt sayısını çarptıktan sonra bu kayıtları atla ve pageSize(sayfada olan kayıt sayısı) kadar kayıt al/göster.
                .Take(pageSize)
                .ToList();

            return new PagedList<T>(items,count,pageNumber,pageSize);
        }
    }
}
