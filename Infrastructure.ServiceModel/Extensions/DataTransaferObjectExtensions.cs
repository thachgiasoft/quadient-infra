using System.Collections.Generic;
using System.Linq;
using Infrastructure.Core.Helpers;

namespace Infrastructure.ServiceModel.Extensions
{
    /// <summary>
    /// Basit düzeyde yazilmis object to object mapper.
    /// </summary>
    public static class DataTransaferObjectExtensions
    {
        public static TO MapToAnOtherObject<TFrom, TO>(this TFrom from) where TO : new()
        {
            return new List<TFrom> { from }.MapToAnOtherObjectAsList<TFrom, TO>().FirstOrDefault();
        }
        public static IList<TO> MapToAnOtherObjectAsList<TFrom, TO>(this IEnumerable<TFrom> from) where TO : new()
        {
            var list = new List<TO>();

            var properties = GenericTypeHelper.GetProperties<TO>().ToArray();

            foreach (var itemFrom in from)
            {
                var destination = new TO();
                var propertiesTarget = GenericTypeHelper.GetProperties<TFrom>();
                foreach (var propertyInfo in propertiesTarget)
                {
                    var prop =
                        properties.FirstOrDefault(p => p.Name.ToLower().Equals(propertyInfo.Name.ToLower()) && p.PropertyType == propertyInfo.PropertyType);
                    if (prop != null)
                        prop.SetValue(destination, propertyInfo.GetValue(itemFrom));
                }
                list.Add(destination);
            }
            return list;
        }
    }
}
