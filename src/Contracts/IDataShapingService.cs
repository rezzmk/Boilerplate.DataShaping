using System.Collections.Generic;
using System.Dynamic;

namespace Boilerplate.DataShaping.Contracts {
    public interface IDataShapingService<T> {
        IEnumerable<ExpandoObject> Shape(IEnumerable<T> records, string properties);
        ExpandoObject Shape(T record, string properties);
    }
}