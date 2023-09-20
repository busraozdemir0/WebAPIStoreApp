using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class ShapedEntity
    {
        public int Id { get; set; }
        public Entity Entity { get; set; } // referans tipli bir ifade var bu yüzden referans aldırmalıyız

        public ShapedEntity()
        {
            Entity = new Entity();
        }
    }
}
