using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Infrastructure.Core.Extensions;
using Infrastructure.Core.Helpers;

namespace Infrastructure.Data.Extensions
{
    ///
    //http://mapperextensions.codeplex.com DataTableExtensions sinifi ve bagimliliklari alinmistir
    /// 

    public static class MappingExtensions
    {
        /// <summary>
        ///   Sets the target property value.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "targetProperty">The target property.</param>
        /// <param name = "targetValue">The target value.</param>
        /// <param name = "destination">The destination.</param>
        private static void SetValue<T>(Aliases aliases, PropertyInfo targetProperty, object targetValue, T destination)
        {
            if (!targetProperty.PropertyType.IsPrimitive && !targetProperty.PropertyType.Equals(typeof(string)))
            {
                if (targetProperty.PropertyType.IsArray)
                {
                    //nullable check
                    if (targetValue != DBNull.Value)
                    {
                        //byte array behaviour
                        if (targetProperty.PropertyType == typeof(byte[]))
                        {
                            byte[] sourceArray = ((byte[])targetValue);

                            Array array = Array.CreateInstance(targetProperty.PropertyType.GetElementType(),
                                                               sourceArray.Length);

                            for (int i = 0; i < sourceArray.Length; i++)
                            {
                                array.SetValue(sourceArray[i], i);
                            }

                            targetProperty.SetValue(destination, array, null);
                        }
                        //other type of array behaviour will be implemented if its necessary..
                    }
                }
                else if (aliases != null && aliases.ContainsKey(targetProperty.Name))
                {
                    var obj = Activator.CreateInstance(targetProperty.PropertyType);
                    var prop = targetProperty.PropertyType.GetProperty(aliases[targetProperty.Name]);
                    if (prop != null)
                        prop.SetValue(obj, targetValue, null);
                    targetProperty.SetValue(destination, obj, null);
                }
                else if (targetValue != DBNull.Value)
                    targetProperty.SetValue(destination, targetValue, null);
            }
            else
                if (targetValue != DBNull.Value)
                    targetProperty.SetValue(destination, targetValue, null);
        }

        /// <summary>
        ///   Gets the name of the column.
        /// </summary>
        /// <param name = "targetPropertyName">Name of the target property.</param>
        /// <param name = "aliases">The aliases.</param>
        /// <returns></returns>
        private static string GetColumnName(string targetPropertyName, Aliases aliases)
        {
            if (aliases != null && aliases.Keys.Any(k => k == targetPropertyName))
            {
                return aliases[targetPropertyName];
            }

            return targetPropertyName;
        }

        /// <summary>
        ///   Gets the source property
        /// </summary>
        private static object GetColumnValue(Aliases aliases, PropertyInfo targetProperty,
                                             Exclusions exclusions, DataRow sourceRow, bool ignoreMissingField)
        {
            object value = new NoValue();

            if (aliases != null && aliases.Keys.Contains(targetProperty.Name))
            {
                //Excluded properties should not be mapped... even if aliased
                if (!(exclusions != null && GenericTypeHelper.ContainsExclusion(aliases[targetProperty.Name], exclusions)))
                    value = sourceRow[aliases[targetProperty.Name]];
            }
            else
            {
                // If target property is already aliased then do not match on source property of the same name... this will cause a double mapping
                if (aliases == null || (!aliases.Values.Contains(targetProperty.Name)))
                {
                    //do not map excluded properties
                    if (!(exclusions != null && GenericTypeHelper.ContainsExclusion(targetProperty.Name, exclusions)))
                    {
                        if (sourceRow.Table.Columns.Contains(targetProperty.Name))
                            value = sourceRow[targetProperty.Name];
                        else if (ignoreMissingField)
                            return value;
                        else throw new MissingFieldException(string.Format("{0} field does not exist", targetProperty.Name));
                    }
                }
            }
            return value;
        }

        public static object MapToObjectList(this DataTable source, Type typeOfList)
        {
            MethodInfo method = typeof(MappingExtensions).GetMethod("MapToObjectList", new Type[] { typeof(DataTable), typeof(Boolean) });
            MethodInfo generic = method.MakeGenericMethod(typeOfList);
            var obj = generic.Invoke(generic, new object[] { source, true });
            method = null;
            generic = null;
            return obj;
        }

        /// <summary>
        ///   Maps data table to object list.
        /// </summary>
        /// <typeparam name = "T">The destination object type.  Must have a default constructor.</typeparam>
        /// <param name = "source">The source data table.</param>
        /// <param name="ignoreMissingField"></param>
        /// <returns></returns>
        public static IEnumerable<T> MapToObjectList<T>(this DataTable source, bool ignoreMissingField = true) where T : new()
        {
            return MapToObjectList<T>(source, null, null, ignoreMissingField);
        }

        /// <summary>
        ///   Maps data table to object list.
        /// </summary>
        /// <typeparam name = "T">The destination object type.  Must have a default constructor.</typeparam>
        /// <param name = "source">The source data table.</param>
        /// <param name = "dataTypeTriggers">The generic data triggers (null for no trigger).</param>
        /// <param name = "aliases">Property name aliases ([destination name, source name])  Can be null.</param>
        /// <param name = "exclusions">Source object properties that should not be mapped.  Can be null.</param>
        /// <param name="ignoreMissingField"></param>
        /// <returns></returns>
        public static IEnumerable<T> MapToObjectList<T>(this DataTable source,
                                                        DataTypeTriggers dataTypeTriggers, Aliases aliases,
                                                        Exclusions exclusions, bool ignoreMissingField) where T : new()
        {
            var list = new List<T>();

            var properties = GenericTypeHelper.GetProperties<T>();

            foreach (DataRow row in source.Rows)
            {
                var destination = new T();

                foreach (var targetProperty in properties)
                {
                    var targetValue = GetColumnValue(aliases, targetProperty, exclusions, row, ignoreMissingField);

                    if (!(targetValue is NoValue))
                    {
                        var type = source.Columns[GetColumnName(targetProperty.Name, aliases)].DataType;

                        if (dataTypeTriggers != null && dataTypeTriggers.Keys.Contains(type))
                        {
                            if (targetValue != DBNull.Value)
                                targetValue = dataTypeTriggers[type](targetValue);
                        }

                        SetValue(aliases, targetProperty, targetValue, destination);
                    }
                }
                list.Add(destination);
            }
            return list;
        }


        /// <summary>
        ///   Maps data table to object list.
        /// </summary>
        /// <typeparam name = "T">The destination object type.  Must have a default constructor.</typeparam>
        /// <param name = "source">The source data table.</param>
        /// <param name = "propertyNameTriggers">The column name triggers.  Key is based on the column name.</param>
        /// <param name = "aliases">Property name aliases ([destination name, source name])  Can be null.</param>
        /// <param name = "exclusions">Source object properties that should not be mapped.  Can be null.</param>
        /// <param name="ignoreMissingField">Field yoksa ignore et</param>
        /// <returns></returns>
        public static IEnumerable<T> MapToObjectList<T>(this DataTable source,
                                                        PropertyNameTriggers propertyNameTriggers, Aliases aliases,
                                                        Exclusions exclusions, bool ignoreMissingField) where T : new()
        {

            //if (useparallel)
            //{
            //    var results = new List<T>();
            //    var chunkedList = ConvertionExtensions.BreakIntoChunks<DataRow>(source.AsEnumerable().ToList(), 5000);//biner biner parcala
            //    Parallel.ForEach(chunkedList, t =>
            //    {
            //        var result = Map<T>(t);
            //        lock (results)
            //        {
            //            results.AddRange(result);
            //        }
            //    });
            //    return results;
            //}

            var list = new List<T>();

            var properties = GenericTypeHelper.GetProperties<T>();

            foreach (DataRow row in source.Rows)
            {
                var destination = new T();

                foreach (var targetProperty in properties)
                {
                    var targetValue = GetColumnValue(aliases, targetProperty, exclusions, row, ignoreMissingField);

                    if (!(targetValue is NoValue))
                    {
                        var columnName = GetColumnName(targetProperty.Name, aliases);
                        if (propertyNameTriggers != null && propertyNameTriggers.Keys.Contains(columnName))
                        {
                            if (targetValue != DBNull.Value)
                                targetValue = propertyNameTriggers[columnName](targetValue);
                        }

                        SetValue(aliases, targetProperty, targetValue, destination);
                    }
                }
                list.Add(destination);
            }
            return list;
        }
        //paralel execute icin Test amacli eklendi.
        //private static List<T> Map<T>(List<DataRow> source) where T : new()
        //{
        //    var list = new List<T>();
        //    var propertyNameTriggers = new PropertyNameTriggers();
        //    var properties = GenericTypeHelper.GetProperties<T>();

        //    foreach (DataRow row in source)
        //    {
        //        var destination = new T();

        //        foreach (var targetProperty in properties)
        //        {
        //            var targetValue = GetColumnValue(null, targetProperty, null, row, true);

        //            if (!(targetValue is NoValue))
        //            {
        //                var columnName = GetColumnName(targetProperty.Name, null);
        //                if (propertyNameTriggers != null && propertyNameTriggers.Keys.Contains(columnName))
        //                {
        //                    if (targetValue != DBNull.Value)
        //                        targetValue = propertyNameTriggers[columnName](targetValue);
        //                }

        //                SetValue(targetProperty, targetValue, destination);
        //            }
        //        }
        //        list.Add(destination);
        //    }
        //    return list;
        //}
        /// <summary>
        ///   Maps data table to object list.
        /// </summary>
        /// <typeparam name = "T">The destination object type.  Must have a default constructor.</typeparam>
        /// <param name = "source">The source data table.</param>
        /// <param name = "aliases">Property name aliases ([destination name, source name])  Can be null.</param>
        /// <param name = "exclusions">Source object properties that should not be mapped.  Can be null.</param>
        /// <param name="ignoreMissingField"></param>
        /// <returns></returns>
        public static IEnumerable<T> MapToObjectList<T>(this DataTable source,
                                                        Aliases aliases,
                                                        Exclusions exclusions, bool ignoreMissingField) where T : new()
        {
            return MapToObjectList<T>(source, new PropertyNameTriggers(), aliases, exclusions, ignoreMissingField);
        }

    }

}
