using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;

namespace Infrastructure.Services.UtilityService
{
    [ServiceContract]
    public interface IConvertionService
    {
        [OperationContract]
        int ConvertCmToInch(double pLength);
        [OperationContract]
        string DataColumnToCommaSeparatedValue(DataTable pDt, string pColumnName);
        string ListArrayToCommaSeparatedValue<T>(List<T> pValue);

        string ArrayToCommaSeparatedValue<T>(T[] pValue);

        List<T> DataColumnToListArray<T>(DataTable pDt, string pColumnName);

        List<T> CommaSeparatedValueToListArray<T>(string pValue);

        DataTable ListArrayToDataColumn<T>(List<T> pValue, string pColumnName);
        [OperationContract]
        DataTable CommaSeparatedValueToDataColumn(string pValue, string pColumnName);
        [OperationContract]
        DataTable ArrayListToDataColumn(ArrayList pValue, string pColumnName);

        [OperationContract]
        ArrayList DataColumnToArrayList(DataTable pDt, string pColumnName);

        T[] CommaSeparatedValueToArray<T>(string pValue);

        /// <summary>
        /// Her bir Item'ı kendi içinde birden fazla Item içeren nesnelerden oluşan List objesinin DataTable'a dönüştürülmesi için kullanılır.
        /// Örneğin; KISI tablosuna ait 'Kisi' isimli bir entity objesinden herbirisi KISI tablosundaki bir satırı gösterir ve bu nesnelerden
        /// oluşan bir 'List' bu method aracılığı ile DataTable nesnesine dönüştürülür.
        /// </summary>
        DataTable EntityListToDataTable<T>(List<T> pList);

        DataTable EntityListToDataTable<T>(List<T> pList, string pDefaultColumnType);

        List<T> DataTableToEntityList<T>(DataTable pDt, T pList);

        /// <summary>
        /// DataTable olarak gönderilen bir verinin istenen kolonunu Html tablo olarak geri döndürür
        /// </summary>
        [OperationContract]
        string ToHtmlTable(DataTable pOdt, string pColumnName);

        /// <summary>
        /// DataTable olarak gönderilen bir verinin istenen 2 kolonu birleştirip Html tablo olarak geri döndürür
        /// </summary>
        [OperationContract]
        string ToHtmlTable(DataTable pOdt, string pColumnName, string pColumnName2);

        /// <summary>
        /// Virgüllerle ayrışmıi bir string değeri Html tablo olarak geri döndürür
        /// </summary>
        [OperationContract]
        string ToHtmlTable(string pValue);
        [OperationContract]
        string ToHtmlTable(DataTable pOdt, string pColumnName, string pUrlColumnName, string pToolTip,
                           string pCssClassName);
        [OperationContract]
        string ToHtmlTable(DataTable pOdt, string pColumnName, string pQueryStringColumnName, string pUrl,
                                         string pToolTip, string pCssClassName);

    }
}