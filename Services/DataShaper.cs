﻿using Entities.Models;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class DataShaper<T> : IDataShaper<T> where T : class
    {
        public PropertyInfo[] Properties { get; set; }
        public DataShaper()
        {
            Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance); // public veya new'lenerek elde edilenler
        }
        public IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString) // list formatında olan nesnelerin şekillenmesi için
        {
            var requiredFields = GetRequiredProperties(fieldsString);
            return FetchData(entities, requiredFields);
        }
        // ExpandoObject => dinamik olarak üretilen herhangi bir nesneye karşılık gelir.
        public ShapedEntity ShapeData(T entity, string fieldsString) // tek bir nesnenin şekilenmesi için
        {
            var requiredProperties = GetRequiredProperties(fieldsString);
            return FetchDataForEntity(entity, requiredProperties);
        }

        private IEnumerable<PropertyInfo> GetRequiredProperties(string fieldsString)
        {
            var requiredFields = new List<PropertyInfo>();
            
            if(!string.IsNullOrWhiteSpace(fieldsString))    // örneğin sadece kitap id'si ve kitap adlarını listelemek isterse veri şekillendirme yapıyoruz
            {
                var fields=fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach(var field in fields)
                {
                    var property = Properties
                        .FirstOrDefault(p => p.Name.Equals(field.Trim(), 
                        StringComparison.InvariantCultureIgnoreCase));  // boşlukları Trim metoduyla yok ediyoruz

                    if (property is null)
                        continue;

                    requiredFields.Add(property);
                }
            }
            else // şekillendirme yoksa
            {
                requiredFields = Properties.ToList();
            }

            return requiredFields;
        }

        private ShapedEntity FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedObject = new ShapedEntity();

            foreach (var property in requiredProperties)
            {
                var objectPropertyValue = property.GetValue(entity);
                shapedObject.Entity.TryAdd(property.Name, objectPropertyValue);
            }

            var objectProperty = entity.GetType().GetProperty("Id");
            shapedObject.Id = (int)objectProperty.GetValue(entity);  // dönüş yapacağımız verinin id ifadesini de şekillendirdik

            return shapedObject;
        }

        private IEnumerable<ShapedEntity> FetchData(IEnumerable<T> entities, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedData = new List<ShapedEntity>();
            foreach(var entity in entities)
            {
                var shapedObject = FetchDataForEntity(entity, requiredProperties);
                shapedData.Add(shapedObject);
            }
            return shapedData;
        }
    }
}
