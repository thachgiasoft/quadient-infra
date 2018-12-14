using System.Collections;
using System.Collections.Generic;
using System.Data;
using Infrastructure.Core.Extensions;
using Infrastructure.Core.Helpers;

namespace Infrastructure.Services.UtilityService
{
    public class ConvertionService : IConvertionService
    {
        public int ConvertCmToInch(double pLength)
        {
            return ConvertionHelper.ConvertCmToInch(pLength);
        }

        public string DataColumnToCommaSeparatedValue(DataTable pDt, string pColumnName)
        {
            return ConvertionHelper.DataColumnToCommaSeparatedValue(pDt, pColumnName);
        }

        public string ListArrayToCommaSeparatedValue<T>(List<T> pValue)
        {
            return ConvertionHelper.ListArrayToCommaSeparatedValue(pValue);
        }

        public string ArrayToCommaSeparatedValue<T>(T[] pValue)
        {
            return ConvertionHelper.ArrayToCommaSeparatedValue(pValue);
        }

        public List<T> DataColumnToListArray<T>(DataTable pDt, string pColumnName)
        {
            return ConvertionHelper.DataColumnToListArray<T>(pDt, pColumnName);
        }

        public List<T> CommaSeparatedValueToListArray<T>(string pValue)
        {
            return ConvertionHelper.CommaSeparatedValueToListArray<T>(pValue);
        }

        public DataTable ListArrayToDataColumn<T>(List<T> pValue, string pColumnName)
        {
            return ConvertionHelper.ListArrayToDataColumn(pValue, pColumnName);
        }

        public DataTable CommaSeparatedValueToDataColumn(string pValue, string pColumnName)
        {
            return ConvertionHelper.CommaSeparatedValueToDataColumn(pValue, pColumnName);
        }

        public DataTable ArrayListToDataColumn(ArrayList pValue, string pColumnName)
        {
            return ConvertionHelper.ArrayListToDataColumn(pValue, pColumnName);
        }

        public ArrayList DataColumnToArrayList(DataTable pDt, string pColumnName)
        {
            return ConvertionHelper.DataColumnToArrayList(pDt, pColumnName);
        }

        public T[] CommaSeparatedValueToArray<T>(string pValue)
        {
            return ConvertionHelper.CommaSeparatedValueToArray<T>(pValue);
        }

        public DataTable EntityListToDataTable<T>(List<T> pList)
        {
            return ConvertionHelper.EntityListToDataTable(pList);
        }

        public DataTable EntityListToDataTable<T>(List<T> pList, string pDefaultColumnType)
        {
            return ConvertionHelper.EntityListToDataTable(pList, pDefaultColumnType);
        }

        public List<T> DataTableToEntityList<T>(DataTable pDt, T pList)
        {
            return ConvertionHelper.DataTableToEntityList(pDt, pList);
        }

        public string ToHtmlTable(DataTable pOdt, string pColumnName)
        {
            return pOdt.ToHtmlTable(pColumnName);
        }

        public string ToHtmlTable(DataTable pOdt, string pColumnName, string pColumnName2)
        {
            return pOdt.ToHtmlTable( pColumnName, pColumnName2);
        }

        public string ToHtmlTable(string pValue)
        {
            return pValue.ToHtmlTable();
        }

        public string ToHtmlTable(DataTable pOdt, string pColumnName, string pUrlColumnName, string pToolTip, string pCssClassName)
        {
            return pOdt.ToHtmlTable( pColumnName, pUrlColumnName, pToolTip, pCssClassName);
        }

        public string ToHtmlTable(DataTable pOdt, string pColumnName, string pQueryStringColumnName, string pUrl, string pToolTip,
                                  string pCssClassName)
        {
            return pOdt.ToHtmlTable(pColumnName, pQueryStringColumnName, pUrl, pToolTip, pCssClassName);
        }
    }
}
