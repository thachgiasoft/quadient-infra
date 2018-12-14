using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Infrastructure.Core.Cryptography;
using Infrastructure.Core.Helpers;
using Infrastructure.Core.Infrastructure;

namespace Infrastructure.Core.Extensions
{
    public static class ConvertionExtensions
    {

        /// <summary>
        /// DataTable olarak gönderilen bir verinin istenen kolonunu Html tablo olarak geri döndürür
        /// </summary>
        public static string ToHtmlTable(this DataTable pOdt, string pColumnName)
        {
            string htmlResult = string.Empty;
            int i = 0;

            foreach (DataRow row in pOdt.Rows)
            {
                string bgColor = ConvertionHelper.GetHtmlTableBgColor(i++);
                htmlResult += string.Format("<tr><td {1}>{0}</td></tr>", row[pColumnName], bgColor);
            }
            htmlResult = string.Format(ConvertionHelper.TableTag, htmlResult);
            ConvertionHelper.SetHtmlTableBgColorsToDefault();
            return htmlResult;
        }
        /// <summary>
        /// DataTable olarak gönderilen bir verinin istenen 2 kolonu birleştirip Html tablo olarak geri döndürür
        /// </summary>
        public static string ToHtmlTable(this DataTable pOdt, string pColumnName, string pColumnName2)
        {
            string htmlResult = string.Empty;
            int i = 0;

            foreach (DataRow row in pOdt.Rows)
            {
                string bgColor = ConvertionHelper.GetHtmlTableBgColor(i++);

                string column2 = string.Format("{0}  ", row[pColumnName2]);


                htmlResult += string.Format("<tr><td {1}>{0}</td></tr>", column2 + row[pColumnName], bgColor);

            }
            htmlResult = string.Format(ConvertionHelper.TableTag, htmlResult);
            ConvertionHelper.SetHtmlTableBgColorsToDefault();
            return htmlResult;
        }
        /// <summary>
        /// Virgüllerle ayrışmıi bir string değeri Html tablo olarak geri döndürür
        /// </summary>
        public static string ToHtmlTable(this string pValue)
        {
            string htmlResult = string.Empty;
            int i = 0;
            string[] parca = pValue.Split(',');

            foreach (string item in parca)
            {
                string bgColor = ConvertionHelper.GetHtmlTableBgColor(i++);
                htmlResult += string.Format("<tr><td {1}>{0}</td></tr>", item, bgColor);
            }
            htmlResult = string.Format(ConvertionHelper.TableTag, htmlResult);
            ConvertionHelper.SetHtmlTableBgColorsToDefault();
            return htmlResult;
        }

        public static string ToHtmlTable(this DataTable pOdt, string pColumnName, string pUrlColumnName, string pToolTip, string pCssClassName)
        {
            string htmlResult = string.Empty;
            int i = 0;
            string css = string.Empty;
            if (!String.IsNullOrEmpty(pCssClassName)) css = string.Format("class='{0}'", pCssClassName);

            foreach (DataRow row in pOdt.Rows)
            {
                string bgColor = ConvertionHelper.GetHtmlTableBgColor(i++);
                string adi = row[pColumnName].ToString();
                string link = row[pUrlColumnName].ToString();

                htmlResult += string.Format("<tr><td {0}><a {4} title='{3}' href='{1}'>{2}</a></td></tr>", bgColor, link, adi, pToolTip, css);
            }
            htmlResult = string.Format(ConvertionHelper.TableTag, htmlResult);
            ConvertionHelper.SetHtmlTableBgColorsToDefault();
            return htmlResult;
        }

        public static string ToHtmlTable(this DataTable pOdt, string pColumnName, string pQueryStringColumnName, string pUrl, string pToolTip, string pCssClassName)
        {
            string htmlResult = string.Empty;
            int i = 0;
            string css = string.Empty;
            if (!String.IsNullOrEmpty(pCssClassName)) css = string.Format("class='{0}'", pCssClassName);

            string fullDataClmnName = pToolTip.Contains("UseAsColumnName") ? pToolTip.Replace("UseAsColumnName", string.Empty) : string.Empty;

            foreach (DataRow row in pOdt.Rows)
            {
                string bgColor = ConvertionHelper.GetHtmlTableBgColor(i++);

                string encryptedData = EngineContext.Current.Resolve<ICoreCryptography>().Encrypt(row[pQueryStringColumnName].ToString());
                string queryString = string.Format("?{0}={1}", pQueryStringColumnName, encryptedData);
                string fullUrl = pUrl + queryString;

                string data = row[pColumnName].ToString();
                string toolTip = !String.IsNullOrEmpty(fullDataClmnName) ? row[fullDataClmnName].ToString() : pToolTip;

                htmlResult += string.Format("<tr><td {0}><a {4} title='{3}' href='{1}'>{2}</a></td></tr>", bgColor, fullUrl, data, toolTip, css);
            }
            htmlResult = string.Format(ConvertionHelper.TableTag, htmlResult);
            ConvertionHelper.SetHtmlTableBgColorsToDefault();
            return htmlResult;
        }
        public static string ToCommaSeparatedValue(this DataTable pDt, string pColumnName)
        {
            return ConvertionHelper.DataColumnToCommaSeparatedValue(pDt, pColumnName);
        }
        public static string ToCommaSeparatedValue<T>(this List<T> pValue)
        {
            return ConvertionHelper.ListArrayToCommaSeparatedValue(pValue);
        }
        public static string ToCommaSeparatedValue<T>(this List<T> pValue, string columnName)
        {
            return ConvertionHelper.ListArrayToCommaSeparatedValue(pValue, columnName);
        }
        public static string ToCommaSeparatedValue<T>(this T[] pValue)
        {
            return ConvertionHelper.ArrayToCommaSeparatedValue(pValue);
        }
        public static List<T> ToListArray<T>(this DataTable pDt, string pColumnName)
        {
            return ConvertionHelper.DataColumnToListArray<T>(pDt, pColumnName);
        }
        public static List<T> ToListArray<T>(this string pValue)
        {
            return ConvertionHelper.CommaSeparatedValueToListArray<T>(pValue);
        }
        public static List<T> ToListArray<T>(this string pValue, char pSeparater)
        {
            if (pSeparater != ',') pValue = pValue.Replace(pSeparater, ',');
            return ConvertionHelper.CommaSeparatedValueToListArray<T>(pValue);
        }

        public static DataTable ToDataColumn<T>(this List<T> pValue, string pColumnName)
        {
            return ConvertionHelper.ListArrayToDataColumn(pValue, pColumnName);
        }
        public static DataTable ToDataColumn(this string pValue, string pColumnName)
        {
            return ConvertionHelper.CommaSeparatedValueToDataColumn(pValue, pColumnName);
        }
        public static DataTable ToDataColumn(this string pValue, string pColumnName, char pSeparater)
        {
            if (pSeparater != ',') pValue = pValue.Replace(pSeparater, ',');
            return ConvertionHelper.CommaSeparatedValueToDataColumn(pValue, pColumnName);
        }
        public static DataTable ToDataColumn(this ArrayList pValue, string pColumnName)
        {
            return ConvertionHelper.ArrayListToDataColumn(pValue, pColumnName);
        }

        public static ArrayList ToArrayList(this DataTable pDt, string pColumnName)
        {
            return ConvertionHelper.DataColumnToArrayList(pDt, pColumnName);
        }

        public static T[] ToArray<T>(this string pValue)
        {
            return ConvertionHelper.CommaSeparatedValueToArray<T>(pValue);
        }

        /// <summary>
        /// Her bir Item'ı kendi içinde birden fazla Item içeren nesnelerden oluşan List objesinin DataTable'a dönüştürülmesi için kullanılır.
        /// Örneğin; KISI tablosuna ait 'Kisi' isimli bir entity objesinden herbirisi KISI tablosundaki bir satırı gösterir ve bu nesnelerden
        /// oluşan bir 'List' bu method aracılığı ile DataTable nesnesine dönüştürülür.
        /// </summary>
        public static DataTable ToDataTable<T>(this List<T> pList)
        {
            return ConvertionHelper.EntityListToDataTable(pList);
        }
        public static DataTable ToDataTable<T>(this List<T> pList, string pDefaultColumnType)
        {
            return ConvertionHelper.EntityListToDataTable(pList, pDefaultColumnType);
        }

        /// <summary>
        /// Bir DataTable objesini, her bir Item'ı kendi içinde birden fazla Item içeren nesnelerden oluşan List objesine dönüştürmek için kullanılır.
        /// </summary>
        /// <typeparam name="T">Listenin tipi. Örneğin KISI tablosuna ait parametre bilgilerini tutan parametre class'ının ismi</typeparam>
        /// <param name="pDt">List array'e dönüştürülecek olan DataTable objesi</param>
        /// <param name="pList">Tipi T parametresi ile verilen listenin mevcut instance'ı veya yeni bir instance</param>
        /// <returns></returns>
        public static List<T> ToListArray<T>(this DataTable pDt, T pList)
        {
            return ConvertionHelper.DataTableToEntityList(pDt, pList);
        }

        /// <summary>
        /// Break a <see cref="List{T}"/> into multiple chunks. The <paramref name="list="/> is cleared out and the items are moved
        /// into the returned chunks.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list to be chunked.</param>
        /// <param name="chunkSize">The size of each chunk.</param>
        /// <returns>A list of chunks.</returns>
        public static List<List<T>> BreakIntoChunks<T>(List<T> list, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentException("chunkSize must be greater than 0.");
            }

            var retVal = new List<List<T>>();
            int index = 0;
            while (index < list.Count)
            {
                int count = list.Count - index > chunkSize ? chunkSize : list.Count - index;
                retVal.Add(list.GetRange(index, count));

                index += chunkSize;
            }

            return retVal;
        }

        public static byte?[] ToNullableByteArray(this byte[] sourceArray)
        {
            if (sourceArray != null)
            {
                var destinationArray = new byte?[sourceArray.Length];
                for (int i = 0; i < sourceArray.Length; i++)
                {
                    destinationArray[i] = sourceArray[i];
                }
                return destinationArray;
            }
            return null;

        }

        public static byte[] ToByteArray(this byte?[] sourceArray)
        {
            if (sourceArray != null)
            {
                var destinationArray = new byte[sourceArray.Length];
                for (int i = 0; i < sourceArray.Length; i++)
                {
                    destinationArray[i] = sourceArray[i].Value;
                }
                return destinationArray;
            }
            return null;
        }

        public static byte[] StreamToByteArray(this Stream pStream)
        {
            byte[] buffer = new byte[pStream.Length];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = pStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }



        public static IList<T> ToList<T>(this DataTable table, Action<DataRow, T> action = null) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            IList<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);

                if (action != null)
                    action((DataRow)row, item);
            }

            return result;
        }

        public static IList<T> ToList<T>(this DataTable table, Dictionary<string, string> mappings, Action<DataRow, T> action = null) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            IList<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties, mappings);
                result.Add(item);

                if (action != null)
                    action((DataRow)row, item);
            }

            return result;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (row.Table.Columns.Contains(property.Name) && row[property.Name] != DBNull.Value)
                    property.SetValue(item, row[property.Name], null);
            }
            return item;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties, Dictionary<string, string> mappings) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (mappings.ContainsKey(property.Name) && row[mappings[property.Name]] != DBNull.Value)
                    property.SetValue(item, row[mappings[property.Name]], null);
            }
            return item;
        }

    }
}
