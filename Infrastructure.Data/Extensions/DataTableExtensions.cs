using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data.ComponentModelOperations;

namespace Infrastructure.Data.Extensions
{
   public static class DataTableExtensions
    {
        public static DataTable SortDataTable(this DataTable pDataTable, string pSortExpression)
        {
            return DataTableFilter.SortDataTable(pDataTable, pSortExpression);
        }

        public static DataTable FilterDataTable(this DataTable pDataTable, int pTopN)
        {
            return DataTableFilter.FilterDataTable(pDataTable, pTopN);
        }

        public static DataTable FilterDataTable(this DataTable pDataTable, string pFilterExpression)
        {
            return DataTableFilter.FilterDataTable(pDataTable, pFilterExpression);
        }
        public static DataTable FilterDataTable(this DataTable pDataTable, string pFilterExpression, int pTopN)
        {
            return DataTableFilter.FilterDataTable(pDataTable, pFilterExpression, pTopN);
        }

        public static DataTable FilterDataTable(this DataTable pDataTable, string pFilterExpression, string pSortExpression)
        {
            return DataTableFilter.FilterDataTable(pDataTable, pFilterExpression, pSortExpression);
        }
        public static DataTable FilterDataTable(this DataTable pDataTable, string pFilterExpression, string pSortExpression, int pTopN)
        {
            return DataTableFilter.FilterDataTable(pDataTable, pFilterExpression, pSortExpression, pTopN);
        }

        #region [DATATABLE JOIN] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>
        /// SQL sorgusundaki INNER JOIN ile aynı işleve sahiptir
        /// </summary>
        /// <param name="pDtLeft">JOIN işleminin solundaki tablo</param>
        /// <param name="pDtRight">JOIN işleminin sağındaki tablo</param>
        /// <param name="pDtLPrmKeyFieldName">Soldaki tabloya ait PrimaryKey kolonunun adı</param>
        /// <param name="pJoinKeyFieldName">JOIN işleminin gerçekleşeceği kolonunun adı</param>
        /// <returns></returns>
        public static DataTable InnerJoin(this DataTable pDtLeft, DataTable pDtRight, string pDtLPrmKeyFieldName, string pJoinKeyFieldName)
        {
            return DataTableJoin.InnerJoin(pDtLeft, pDtRight, pDtLPrmKeyFieldName, pJoinKeyFieldName);
        }

        /// <summary>
        /// SQL sorgusundaki LEFT OUTER JOIN ile aynı işleve sahiptir
        /// </summary>
        /// <param name="pDtLeft">JOIN işleminin solundaki tablo</param>
        /// <param name="pDtRight">JOIN işleminin sağındaki tablo</param>
        /// <param name="pDtLPrmKeyFieldName">Soldaki tabloya ait PrimaryKey kolonunun adı</param>
        /// <param name="pJoinKeyFieldName">JOIN işleminin gerçekleşeceği kolonunun adı</param>
        /// <returns></returns>
        public static DataTable LeftOuterJoin(this DataTable pDtLeft, DataTable pDtRight, string pDtLPrmKeyFieldName, string pJoinKeyFieldName)
        {
            return DataTableJoin.LeftOuterJoin(pDtLeft, pDtRight, pDtLPrmKeyFieldName, pJoinKeyFieldName);
        }

        /// <summary>
        /// SQL sorgusundaki RIGHT OUTER JOIN ile aynı işleve sahiptir
        /// </summary>
        /// <param name="pDtLeft">JOIN işleminin solundaki tablo</param>
        /// <param name="pDtRight">JOIN işleminin sağındaki tablo</param>
        /// <param name="pDtRPrmKeyFieldName">Sağdaki tabloya ait PrimaryKey kolonunun adı</param>
        /// <param name="pJoinKeyFieldName">JOIN işleminin gerçekleşeceği kolonunun adı</param>
        /// <returns></returns>
        public static DataTable RightOuterJoin(this DataTable pDtLeft, DataTable pDtRight, string pDtRPrmKeyFieldName, string pJoinKeyFieldName)
        {
            return DataTableJoin.RightOuterJoin(pDtLeft, pDtRight, pDtRPrmKeyFieldName, pJoinKeyFieldName);
        }

        /// <summary>
        /// SQL sorgusundaki FULL OUTER JOIN ile aynı işleve sahiptir
        /// </summary>
        /// <param name="pDtLeft">JOIN işleminin solundaki tablo</param>
        /// <param name="pDtRight">JOIN işleminin sağındaki tablo</param>
        /// <param name="pDtLPrmKeyFieldName">Soldaki tabloya ait PrimaryKey kolonunun adı</param>
        /// <param name="pDtRPrmKeyFieldName">Sağdaki tabloya ait PrimaryKey kolonunun adı</param>
        /// <param name="pJoinKeyFieldName">JOIN işleminin gerçekleşeceği kolonunun adı</param>
        /// <returns></returns>
        public static DataTable FullOuterJoin(this DataTable pDtLeft, DataTable pDtRight, string pDtLPrmKeyFieldName, string pDtRPrmKeyFieldName, string pJoinKeyFieldName)
        {
            return DataTableJoin.FullOuterJoin(pDtLeft, pDtRight, pDtLPrmKeyFieldName, pDtRPrmKeyFieldName, pJoinKeyFieldName);
        }

        /// <summary>
        /// SQL sorgusundaki UNION ile aynı işleve sahiptir
        /// </summary>
        /// <param name="pDtLeft">JOIN işleminin solundaki tablo</param>
        /// <param name="pDtRight">JOIN işleminin sağındaki tablo</param>
        /// <returns></returns>
        public static DataTable Union(this DataTable pDtLeft, DataTable pDtRight)
        {
            return DataTableJoin.Union(pDtLeft, pDtRight);
        }

        /// <summary>
        /// pDtRight parametresi ile gelen veri kaynağındaki veriler, pDtLeft parametresi ile gelen veri kaynağındaki verilere eklenir.
        /// Bu ekleme sırasında, verilen pLoadOption parametresine göre; 
        /// pDtLeft veri kaynağında zaten var olan veriler aynen korunur ya da pDtRight veri kaynağındaki veri ile değiştirilir. 
        /// Zaten var olan verinin korunması isteniyor ise, pLoadOption parametresi "PreserveChanges" verilmelidir.
        /// Güncellenmesi isteniyor ise pLoadOption parametresi "Upsert" veya "OverwriteChanges" olarak verilmelidir.
        /// </summary>
        /// <param name="pDtLeft">Verilerin ekleneceği ana veri kaynağı</param>
        /// <param name="pDtRight">Verilerin alınacağı ikinci veri kaynağı</param>
        /// <param name="pLoadOption">Veri ekleme (yükleme) seçeneği</param>
        /// <param name="pDtLeftPKColumnName">pDtLeft için tanımlanacak olan PrimaryKey kolon adı. 
        /// pDtLeft'de zaten var olan bir verinin pDtRight'deki veri ile güncellenmesi için pDtLeft için bir PrimaryKey tanımlanmalıdır.
        /// PK değeri boş verilirse iki tablo olduğu gibi birleştirilir (UnionAll yapar). </param>
        public static void Union(this DataTable pDtLeft, DataTable pDtRight, LoadOption pLoadOption, string pDtLeftPKColumnName, bool pAcceptChanges)
        {
            DataTableJoin.Union(pDtLeft, pDtRight, pLoadOption, pDtLeftPKColumnName, pAcceptChanges);
        }

        /// <summary>
        /// SQL sorgusundaki UNION ALL ile aynı işleve sahiptir
        /// </summary>
        /// <param name="pDtLeft">JOIN işleminin solundaki tablo</param>
        /// <param name="pDtRight">JOIN işleminin sağındaki tablo</param>
        /// <returns></returns>
        public static DataTable UnionAll(this DataTable pDtLeft, DataTable pDtRight)
        {
            return DataTableJoin.UnionAll(pDtLeft, pDtRight);
        }

        /// <summary>
        /// SQL sorgusundaki DISTINCT ile aynı işleve sahiptir. Verilen DataTable objesindeki tüm kolonlara göre distinct işlemi yapar
        /// </summary>
        /// <param name="pDtSource">Üzerinde Distinct işlemine yapılacak kaynak DataTable</param>
        /// <returns></returns>
        public static DataTable SelectDistinct(this DataTable pDtSource)
        {
            return DataTableJoin.SelectDistinct(pDtSource);
        }

        /// <summary>
        /// SQL sorgusundaki DISTINCT ile aynı işleve sahiptir. string array olarak verilen kolonlara göre distinct işlemi yapar
        /// </summary>
        /// <param name="pDtSource">Üzerinde Distinct işlemine yapılacak kaynak DataTable</param>
        /// <param name="pColumns">Distinct işlemi yapılacak kolon isimlerinin listesi</param>
        /// <returns></returns>
        public static DataTable SelectDistinct(this DataTable pDtSource, params string[] pColumns)
        {
            return DataTableJoin.SelectDistinct(pDtSource, pColumns);
        }

        public static void SetPrimaryKey(this DataTable pDtL, string pDtLPrmKeyFieldName)
        {
            DataTableJoin.SetPrimaryKey(ref pDtL, pDtLPrmKeyFieldName);
        }

        #endregion
    }
}
