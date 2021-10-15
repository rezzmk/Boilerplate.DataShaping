using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Boilerplate.DataShaping.Contracts;

namespace Boilerplate.DataShaping.Services {
    public class DataShapingService<T> : IDataShapingService<T> where T : class {
        private PropertyInfo[] PropertyInfos { get; }

        public DataShapingService() => PropertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        public IEnumerable<ExpandoObject> Shape(IEnumerable<T> records, string properties) => Fetch(records, ParsePropertiesString(properties));
        public ExpandoObject Shape(T record, string properties) => FetchDataForRecord(record, ParsePropertiesString(properties));

        private IEnumerable<PropertyInfo> ParsePropertiesString(string properties) {
            if (string.IsNullOrEmpty(properties)) return PropertyInfos.ToList();

            var parsedProperties = properties.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return parsedProperties.Select(
                property => PropertyInfos.FirstOrDefault(
                    x => x.Name.Equals(
                        property.Trim(), 
                        StringComparison.CurrentCultureIgnoreCase)
                    ))
                .Where(propertyInfo => propertyInfo != null).ToList();
        }

        private IEnumerable<ExpandoObject> Fetch(IEnumerable<T> records, IEnumerable<PropertyInfo> wantedProperties) {
            var enumeratedList = wantedProperties?.ToList();
            return enumeratedList == null 
                ? null 
                : records.Select(record => FetchDataForRecord(record, enumeratedList)).ToList();
        }

        private ExpandoObject FetchDataForRecord(T record, IEnumerable<PropertyInfo> wantedProperties) {
            var shapedObject = new ExpandoObject();
            foreach (var property in wantedProperties) {
                var objectPropertyValue = property.GetValue(record); 
                shapedObject.TryAdd(property.Name, objectPropertyValue);
            } 
            return shapedObject;
        }
    }
}