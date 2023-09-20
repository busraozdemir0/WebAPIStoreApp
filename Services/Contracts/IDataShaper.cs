﻿using Entities.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IDataShaper<T>
    {
        IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString); // liste üzerinde çalışacak
        ShapedEntity ShapeData(T entity, string fieldsString); // tek bir nesne üzerinde çalışacak
    }
}
