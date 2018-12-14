using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using Infrastructure.Core.ComponentModel;
using Infrastructure.Core.Cryptography;
using Infrastructure.Core.Infrastructure;

namespace Infrastructure.Core.Helpers
{
    public static class ConvertionHelper
    {

        public static string EncodingMeta = @"<META HTTP-EQUIV='Content-Type' CONTENT='text/html; charset=windows-1254'/>
                                              <META HTTP-EQUIV='Content-Type' CONTENT='text/html; charset=ISO-8859-9'/>";

        public const string TableTag = "<table id='tableHtmlData' width='100%' style='height:20px;'>{0}</table>";

        public static int ConvertCmToInch(double pLength)
        {
            if (pLength > 0)
                return Convert.ToInt32(pLength * 28.8);
            return 0;
        }

        /// <summary>
        /// DataTable icerisinde belirtilen kolonu virgulle ayrilmis string formatina cevirir.
        /// </summary>
        /// <param name="pDt">Datatable</param>
        /// <param name="pColumnName">Virgulle ayrilmis formatta donmesi istenilen kolon</param>
        /// <returns>Belirtilen kolon degerlerini a,b,c seklinde dondurur.</returns>
        public static string DataColumnToCommaSeparatedValue(DataTable pDt, string pColumnName)
        {
            if (pDt == null) return string.Empty;
            var sb = new StringBuilder();

            foreach (DataRow item in pDt.Rows)
            {
                sb.Append(item[pColumnName] + ",");
            }
            return sb.ToString().TrimEnd(',');
        }

        /// <summary>
        /// Belirtilen List arrayi virgulle ayrilmis degerlere cevrilir.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="pValue">Array</param>
        /// <returns>a,b,c seklinde string dondurur.</returns>
        public static string ListArrayToCommaSeparatedValue<T>(List<T> pValue)
        {
            if (pValue == null) return string.Empty;
            var sb = new StringBuilder();

            foreach (T t in pValue)
            {
                sb.Append(t + ",");
            }

            return sb.ToString().TrimEnd(',');
        }

        /// <summary>
        /// Belirtilen List array in belirtilen kolon degerlerini virgulle ayrilmis degerlere cevrilir.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="pValue">Array</param>
        /// <param name="columnName">Kolon adi</param>
        /// <returns>a,b,c seklinde string dondurur.</returns>
        public static string ListArrayToCommaSeparatedValue<T>(List<T> pValue,string columnName)
        {
            if (pValue == null) return string.Empty;
            var sb = new StringBuilder();

            foreach (T t in pValue)
            {
                sb.Append(t.GetType().GetProperty(columnName).GetValue(t).ToString() + ",");
            }

            return sb.ToString().TrimEnd(',');
        }

        /// <summary>
        /// Belirtilen arrayi virgulle ayrilmis stringe cevirir.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="pValue">Array</param>
        /// <returns>a,b,c seklinde string dondurur.</returns>
        public static string ArrayToCommaSeparatedValue<T>(T[] pValue)
        {
            if (pValue.Length == 0) return string.Empty;
            var sb = new StringBuilder();

            foreach (T t in pValue)
            {
                sb.Append(t + ",");
            }

            return sb.ToString().TrimEnd(',');
        }

        /// <summary>
        /// Datatable icerisindeki belirtilen kolonu T turunde list e cevirir.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="pDt">Datatable</param>
        /// <param name="pColumnName">Kolon</param>
        /// <returns>List T</returns>
        public static List<T> DataColumnToListArray<T>(DataTable pDt, string pColumnName)
        {
            if (pDt == null) return null;
            var value = new List<T>();
            foreach (DataRow item in pDt.Rows)
            {
                value.Add((T)Convert.ChangeType(item[pColumnName], typeof(T)));
            }
            return value;
        }

        /// <summary>
        /// Virgulle ayrilmis string degeri List T turune donusturur.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="pValue">Virgulle ayrilmis string deger</param>
        /// <returns>List T</returns>
        public static List<T> CommaSeparatedValueToListArray<T>(string pValue)
        {
            if (string.IsNullOrEmpty(pValue)) return null;
            List<T> value = new List<T>();
            string[] parca = pValue.Split(',');

            for (int i = 0; i < parca.Length; i++)
            {
                value.Add((T)Convert.ChangeType(parca[i], typeof(T)));
            }

            return value;
        }

        /// <summary>
        /// Belirtilen list degerini datatable a cevirir.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="pValue">List degeri</param>
        /// <param name="pColumnName">Kolon</param>
        /// <returns>Belirtilen listeyi DataTable olarak dondurur.</returns>
        public static DataTable ListArrayToDataColumn<T>(List<T> pValue, string pColumnName)
        {
            DataTable oDt = new DataTable();
            if (pValue == null) return oDt;
            oDt.Columns.Add(pColumnName);

            for (int i = 0; i < pValue.Count; i++)
            {
                DataRow oRw = oDt.NewRow();
                oRw[0] = pValue[i].ToString();
                oDt.Rows.Add(oRw);
            }

            return oDt;
        }

        /// <summary>
        /// Virgulle ayrilmis string degeri DataTable a cevirir.
        /// </summary>
        /// <param name="pValue">DataTable a cevrilecek string deger</param>
        /// <param name="pColumnName">Kolon adi</param>
        /// <returns>Belirtilen string i DataTable olarak dondurur.</returns>
        public static DataTable CommaSeparatedValueToDataColumn(string pValue, string pColumnName)
        {
            if (string.IsNullOrEmpty(pValue)) return new DataTable();
            DataTable oDt = new DataTable();
            oDt.Columns.Add(pColumnName);
            string[] parca = pValue.Split(',');

            for (int i = 0; i < parca.Length; i++)
            {
                DataRow oRw = oDt.NewRow();
                oRw[0] = parca[i];
                oDt.Rows.Add(oRw);
            }

            return oDt;
        }

        /// <summary>
        /// Belirtilen ArrayList degerini DataTable a cevirir.
        /// </summary>
        /// <param name="pValue">ArrayList</param>
        /// <param name="pColumnName">Kolon adi</param>
        /// <returns>Belirtilen arraylist i DataTable olarak dondurur.</returns>
        public static DataTable ArrayListToDataColumn(ArrayList pValue, string pColumnName)
        {
            if (pValue == null) return new DataTable();

            DataTable oDt = new DataTable();
            oDt.Columns.Add(pColumnName);

            for (int i = 0; i < pValue.Count; i++)
            {
                DataRow oRw = oDt.NewRow();
                oRw[0] = pValue[i].ToString();
                oDt.Rows.Add(oRw);
            }

            return oDt;
        }

        /// <summary>
        /// Belirtilen DataTable i ArrayList e cevirir.
        /// </summary>
        /// <param name="pDt">DataTable</param>
        /// <param name="pColumnName">Kolon</param>
        /// <returns>DataTable in arraylist halini dondurur.</returns>
        public static ArrayList DataColumnToArrayList(DataTable pDt, string pColumnName)
        {
            if (pDt == null) return null;
            ArrayList arrSayfaKeyler = new ArrayList();
            foreach (DataRow row in pDt.Rows)
            {
                arrSayfaKeyler.Add(row[pColumnName]);
            }
            return arrSayfaKeyler;
        }

        /// <summary>
        /// Virgulle ayrilmis string degeri belirtilen T tipinde bir array e cevirir.
        /// </summary>
        /// <typeparam name="T">Type name</typeparam>
        /// <param name="pValue">Cevrilecek string deger</param>
        /// <returns>Verilen string degeri dizi olarak dondurur.</returns>
        public static T[] CommaSeparatedValueToArray<T>(string pValue)
        {
            if (string.IsNullOrEmpty(pValue)) return null;
            string[] parca = pValue.Split(',');
            T[] value = new T[parca.Length];

            for (int i = 0; i < parca.Length; i++)
            {
                value[i] = ((T)Convert.ChangeType(parca[i], typeof(T)));
            }

            return value;
        }

        /// <summary>
        /// Her bir Item'ı kendi içinde birden fazla Item içeren nesnelerden oluşan List objesinin DataTable'a dönüştürülmesi için kullanılır.
        /// Örneğin; KISI tablosuna ait 'Kisi' isimli bir entity objesinden herbirisi KISI tablosundaki bir satırı gösterir ve bu nesnelerden
        /// oluşan bir 'List' bu method aracılığı ile DataTable nesnesine dönüştürülür.
        /// </summary>
        public static DataTable EntityListToDataTable<T>(List<T> pList)
        {
            return EntityListToDataTable<T>(pList, string.Empty);
        }

        public static DataTable EntityListToDataTable<T>(List<T> pList, string pDefaultColumnType)
        {
            DataTable dt = new DataTable();

            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                Type clmType = pDefaultColumnType == string.Empty ? info.PropertyType : Type.GetType(pDefaultColumnType);
                bool nullableMi = false;
                //Can Ekledi
                if (clmType.IsGenericType && clmType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type[] typeArray = info.PropertyType.GetGenericArguments();
                    foreach (Type t in typeArray)
                    {
                        if (t != null)
                        {
                            clmType = t;
                            nullableMi = true;
                            break;
                        }
                    }

                }
                //
                DataColumn colum = new DataColumn(info.Name, clmType);
                if (nullableMi) colum.AllowDBNull = true;
                dt.Columns.Add(colum);
            }
            foreach (T t in pList)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null) == null ? DBNull.Value : info.GetValue(t, null);
                }
                dt.Rows.Add(row);
            }
            string tableName = string.Empty;

            
            tableName = dt.GetHashCode().ToString(CultureInfo.InvariantCulture);
            if (string.Equals(dt.TableName, "Table") || string.IsNullOrEmpty(dt.TableName))
            {
                dt.TableName = tableName;
            }
            return dt;

        }

        public static List<T> DataTableToEntityList<T>(DataTable pDt, T pList)
        {
            if (pDt == null) return null;
            var lists = new List<T>(pDt.Rows.Count);

            foreach (DataRow row in pDt.Rows)
            {
                pList = Activator.CreateInstance<T>();

                for (int i = 0; i < pDt.Columns.Count; i++)
                {
                    string clmName = pDt.Columns[i].ColumnName;
                    if (row[clmName] == DBNull.Value) continue;

                    if (pList.GetType().GetProperty(clmName) == null) continue;

                    string type = pList.GetType().GetProperty(clmName).PropertyType.FullName;

                    if (type.Contains(DataTypes.Int))
                        pList.GetType().GetProperty(clmName).SetValue(pList, Convert.ToInt32(row[clmName]), null);

                    else if (type.Contains(DataTypes.String))
                        pList.GetType().GetProperty(clmName).SetValue(pList, Convert.ToString(row[clmName]), null);

                    else if (type.Contains(DataTypes.Long))
                        pList.GetType().GetProperty(clmName).SetValue(pList, Convert.ToInt64(row[clmName]), null);

                    else if (type.Contains(DataTypes.Short))
                        pList.GetType().GetProperty(clmName).SetValue(pList, Convert.ToInt16(row[clmName]), null);

                    else if (type.Contains(DataTypes.Decimal))
                        pList.GetType().GetProperty(clmName).SetValue(pList, Convert.ToDecimal(row[clmName]), null);

                    else if (type.Contains(DataTypes.Double))
                        pList.GetType().GetProperty(clmName).SetValue(pList, Convert.ToDouble(row[clmName]), null);

                    else if (type.Contains(DataTypes.Byte))
                        pList.GetType().GetProperty(clmName).SetValue(pList, Convert.ToByte(row[clmName]), null);

                    else if (type.Contains(DataTypes.Boolean))
                        pList.GetType().GetProperty(clmName).SetValue(pList, Convert.ToBoolean(row[clmName]), null);

                    else if (type.Contains(DataTypes.DateTime))
                        pList.GetType().GetProperty(clmName).SetValue(pList, Convert.ToDateTime(row[clmName]), null);

                    else continue;
                }
                lists.Add(pList);
            }

            return lists;
        }

        private static string _mHtmlTableBgColor = string.Empty;
        public static string HtmlTableBgColor { get { return _mHtmlTableBgColor; } set { _mHtmlTableBgColor = value; } }
        private static string _mHtmlTableAlterRowBgColor = "#f0f8ff";
        public static string HtmlTableAlterRowBgColor { get { return _mHtmlTableAlterRowBgColor; } set { _mHtmlTableAlterRowBgColor = value; } }

        public static void SetHtmlTableBgColorsToDefault()
        {
            HtmlTableBgColor = string.Empty;
            HtmlTableAlterRowBgColor = "#f0f8ff";
        }

        public static string GetHtmlTableBgColor(int i)
        {
            string bgColor;
            const string bgColorStyle = "style='background-color:{0}'";
            if (i % 2 == 1)
                bgColor = string.Format(bgColorStyle, HtmlTableBgColor);
            else
                bgColor = !String.IsNullOrEmpty(HtmlTableAlterRowBgColor) ? string.Format(bgColorStyle, HtmlTableAlterRowBgColor) : string.Empty;

            return bgColor;
        }

    }
}
