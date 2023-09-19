using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EfCore.Extensions
{
    public static class OrderQueryBuilder
    {
        public static String CreateOrderQuery<T>(String orderByQueryString)
        {
            var orderParams = orderByQueryString.Trim().Split(',');  // örneğin önce title'a sonra price'a göre sıralattırmak istenirse

            var propertyInfos = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var orderQueryBuilder = new StringBuilder();

            // title ascending, price descending
            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue; // eğer boşsa bir sonraki elemana geç

                var propertyFromQueryName = param.Split(' ')[0]; // price desc gibi belirtilmişse price ve desc ifadesini parçalıyoruz

                var objectProperty = propertyInfos
                    .FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName,
                    StringComparison.InvariantCultureIgnoreCase));  // küçük büyük harf ayrımını görmezden gelecek.

                if (objectProperty is null)
                    continue;

                var direction = param.EndsWith(" desc") ? "descending" : "ascending"; // ifade  desc ile bitiyorsa descendingtir aksi takdirde ascending.

                orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {direction},");

            }
            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' '); // en sondaki virgülden kurtulmak için

            return orderQuery;
        }
    }
}
