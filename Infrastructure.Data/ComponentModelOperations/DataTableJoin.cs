using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data.Extensions;

namespace Infrastructure.Data.ComponentModelOperations
{
    public class DataTableJoin
    {

        private static int MaxRowCount
        {
            get
            {
                return 10000;
            }
        }

        private static string RowCountErrMsg = "Join/Union/Distinct edilmek istenen tabloda en fazla " + MaxRowCount + " adet kayıt olabilir ! Filtreleme kriterlerini arttırarak kayıt sayısını azaltmayı deneyin.";

        /// <summary>
        /// SQL sorgusundaki INNER JOIN ile aynı işleve sahiptir
        /// </summary>
        /// <param name="pDtLeft">JOIN işleminin solundaki tablo</param>
        /// <param name="pDtRight">JOIN işleminin sağındaki tablo</param>
        /// <param name="pDtLPrmKeyFieldName">Soldaki tabloya ait PrimaryKey kolonunun adı</param>
        /// <param name="pJoinKeyFieldName">JOIN işleminin gerçekleşeceği kolonunun adı</param>
        /// <returns></returns>
        public static DataTable InnerJoin(DataTable pDtLeft, DataTable pDtRight, string pDtLPrmKeyFieldName, string pJoinKeyFieldName)
        {
            if (pDtLeft.Rows.Count > MaxRowCount) throw new ApplicationException(RowCountErrMsg);
            DataTable pDtL = new DataTable(); pDtL = pDtLeft.Copy();
            DataTable pDtR = new DataTable(); pDtR = pDtRight.Copy();

            if (pDtL != null && pDtL.Rows.Count > 0 && pDtR != null && pDtR.Rows.Count > 0)
            {
                SetPrimaryKey(ref pDtL, pDtLPrmKeyFieldName);

                int clmCountL1 = pDtL.Columns.Count;    //pDtR kolonları eklenmeden önce
                SetColumns(ref pDtL, ref pDtR);
                int clmCountL2 = pDtL.Columns.Count;    //pDtR kolonları eklendikten sonra

                for (int i = 0; i < pDtL.Rows.Count; i++)
                {
                    if (pDtL.Rows[i][pJoinKeyFieldName] is DBNull) { pDtL.Rows.RemoveAt(i); i -= 1; continue; }
                    int key = Convert.ToInt32(pDtL.Rows[i][pJoinKeyFieldName].ToString());

                    DataRow[] oDr = pDtR.Select(pJoinKeyFieldName + "=" + key);
                    if (oDr.Count() < 1) { pDtL.Rows.RemoveAt(i); i -= 1; }
                    else
                    {
                        for (int j = 0; j < clmCountL2 - clmCountL1; j++)
                        {
                            DataSet ds = new DataSet(); ds.Merge(oDr);
                            pDtL.Rows[i][clmCountL1 + j] = ds.Tables[0].Rows[0][j];
                        }
                    }
                }
            }
            return pDtL;
        }

        /// <summary>
        /// SQL sorgusundaki LEFT OUTER JOIN ile aynı işleve sahiptir
        /// </summary>
        /// <param name="pDtLeft">JOIN işleminin solundaki tablo</param>
        /// <param name="pDtRight">JOIN işleminin sağındaki tablo</param>
        /// <param name="pDtLPrmKeyFieldName">Soldaki tabloya ait PrimaryKey kolonunun adı</param>
        /// <param name="pJoinKeyFieldName">JOIN işleminin gerçekleşeceği kolonunun adı</param>
        /// <returns></returns>
        public static DataTable LeftOuterJoin(DataTable pDtLeft, DataTable pDtRight, string pDtLPrmKeyFieldName, string pJoinKeyFieldName)
        {
            if (pDtLeft.Rows.Count > MaxRowCount) throw new ApplicationException(RowCountErrMsg);
            DataTable pDtL = new DataTable(); pDtL = pDtLeft.Copy();
            DataTable pDtR = new DataTable(); pDtR = pDtRight.Copy();

            if (pDtL != null && pDtL.Rows.Count > 0 && pDtR != null && pDtR.Rows.Count > 0)
            {
                SetPrimaryKey(ref pDtL, pDtLPrmKeyFieldName);

                int clmCountL1 = pDtL.Columns.Count;    //pDtR kolonları eklenmeden önce
                SetColumns(ref pDtL, ref pDtR);
                int clmCountL2 = pDtL.Columns.Count;    //pDtR kolonları eklendikten sonra

                for (int i = 0; i < pDtL.Rows.Count; i++)
                {
                    int key = pDtL.Rows[i][pJoinKeyFieldName] is DBNull ? 0 : Convert.ToInt32(pDtL.Rows[i][pJoinKeyFieldName].ToString());

                    DataRow[] oDr = pDtR.Select(pJoinKeyFieldName + "=" + key);
                    if (oDr.Count() < 1)
                    {
                        for (int j = 0; j < clmCountL2 - clmCountL1; j++)
                        {
                            DataSet ds = new DataSet(); ds.Merge(oDr);
                            pDtL.Rows[i][clmCountL1 + j] = DBNull.Value;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < clmCountL2 - clmCountL1; j++)
                        {
                            DataSet ds = new DataSet(); ds.Merge(oDr);
                            pDtL.Rows[i][clmCountL1 + j] = ds.Tables[0].Rows[0][j];
                        }
                    }
                }
            }
            return pDtL;
        }

        /// <summary>
        /// SQL sorgusundaki RIGHT OUTER JOIN ile aynı işleve sahiptir
        /// </summary>
        /// <param name="pDtLeft">JOIN işleminin solundaki tablo</param>
        /// <param name="pDtRight">JOIN işleminin sağındaki tablo</param>
        /// <param name="pDtRPrmKeyFieldName">Sağdaki tabloya ait PrimaryKey kolonunun adı</param>
        /// <param name="pJoinKeyFieldName">JOIN işleminin gerçekleşeceği kolonunun adı</param>
        /// <returns></returns>
        public static DataTable RightOuterJoin(DataTable pDtLeft, DataTable pDtRight, string pDtRPrmKeyFieldName, string pJoinKeyFieldName)
        {
            if (pDtRight.Rows.Count > MaxRowCount) throw new ApplicationException(RowCountErrMsg);
            DataTable dt = new DataTable();
            dt = LeftOuterJoin(pDtRight, pDtLeft, pDtRPrmKeyFieldName, pJoinKeyFieldName);
            int clmCountL = pDtLeft.Columns.Count;
            int clmCountR = pDtRight.Columns.Count;

            for (int i = 0; i < clmCountR; i++)
            {
                dt.Columns[0].SetOrdinal(clmCountL + clmCountR - 1);
            }
            return dt;
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
        public static DataTable FullOuterJoin(DataTable pDtLeft, DataTable pDtRight, string pDtLPrmKeyFieldName, string pDtRPrmKeyFieldName, string pJoinKeyFieldName)
        {
            if (pDtLeft.Rows.Count > MaxRowCount || pDtRight.Rows.Count > MaxRowCount) throw new ApplicationException(RowCountErrMsg);
            DataTable oDt1 = LeftOuterJoin(pDtLeft, pDtRight, pDtLPrmKeyFieldName, pJoinKeyFieldName);
            DataTable oDt2 = RightOuterJoin(pDtLeft, pDtRight, pDtRPrmKeyFieldName, pJoinKeyFieldName);
            DataTable oDt = Union(oDt1, oDt2);
            return oDt;
        }

        /// <summary>
        /// SQL sorgusundaki UNION ile aynı işleve sahiptir
        /// </summary>
        /// <param name="pDtLeft">UNION işleminin solundaki tablo</param>
        /// <param name="pDtRight">UNION işleminin sağındaki tablo</param>
        /// <returns></returns>
        public static DataTable Union(DataTable pDtLeft, DataTable pDtRight)
        {
            DataTable oDt = UnionAll(pDtLeft, pDtRight);
            return SelectDistinct(oDt, GetColumnsInArray(oDt));
        }

        /// <summary>
        /// SQL sorgusundaki UNION ALL ile aynı işleve sahiptir
        /// </summary>
        /// <param name="pDtLeft">UNION işleminin solundaki tablo</param>
        /// <param name="pDtRight">UNION işleminin sağındaki tablo</param>
        /// <returns></returns>
        public static DataTable UnionAll(DataTable pDtLeft, DataTable pDtRight)
        {
            if (pDtLeft.Rows.Count > MaxRowCount || pDtRight.Rows.Count > MaxRowCount) throw new ApplicationException(RowCountErrMsg);
            DataTable table = new DataTable("Union");

            DataColumn[] newcolumns = new DataColumn[pDtLeft.Columns.Count];
            for (int i = 0; i < pDtLeft.Columns.Count; i++)
            {
                newcolumns[i] = new DataColumn(pDtLeft.Columns[i].ColumnName, pDtLeft.Columns[i].DataType);
            }
            table.Columns.AddRange(newcolumns);
            table.BeginLoadData();
            foreach (DataRow row in pDtLeft.Rows)
            {
                table.LoadDataRow(row.ItemArray, true);
            }
            foreach (DataRow row in pDtRight.Rows)
            {
                table.LoadDataRow(row.ItemArray, true);
            }
            table.EndLoadData();
            return table;
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
        public static void Union(DataTable pDtLeft, DataTable pDtRight, LoadOption pLoadOption, string pDtLeftPKColumnName, bool pAcceptChanges)
        {
            if (pDtLeft == null || pDtRight == null) return;

            lock (pDtLeft)
            {
                if (!String.IsNullOrEmpty(pDtLeftPKColumnName))
                    pDtLeft.SetPrimaryKey(pDtLeftPKColumnName);

                pDtLeft.BeginLoadData();
                foreach (DataRow row in pDtRight.Rows)
                {
                    pDtLeft.LoadDataRow(row.ItemArray, pLoadOption);
                }
                if (pAcceptChanges) pDtLeft.AcceptChanges();
                pDtLeft.EndLoadData();
            }
        }

        /// <summary>
        /// SQL sorgusundaki DISTINCT ile aynı işleve sahiptir. Verilen DataTable objesindeki tüm kolonlara göre distinct işlemi yapar
        /// </summary>
        /// <param name="pDtSource">Üzerinde Distinct işlemine yapılacak kaynak DataTable</param>
        /// <returns></returns>
        public static DataTable SelectDistinct(DataTable pDtSource)
        {
            return SelectDistinct(pDtSource, GetColumnsInArray(pDtSource));
        }

        /// <summary>
        /// SQL sorgusundaki DISTINCT ile aynı işleve sahiptir. string array olarak verilen kolonlara göre distinct işlemi yapar
        /// </summary>
        /// <param name="pDtSource">Üzerinde Distinct işlemine yapılacak kaynak DataTable</param>
        /// <param name="pColumns">Distinct işlemi yapılacak kolon isimlerinin listesi</param>
        /// <returns></returns>
        public static DataTable SelectDistinct(DataTable pDtSource, params string[] pColumns)
        {
            if (pDtSource.Rows.Count > MaxRowCount) throw new ApplicationException(RowCountErrMsg);
            DataTable Result = new DataTable();

            if (pDtSource != null)
            {
                DataView DView = pDtSource.DefaultView;
                Result = DView.ToTable(true, pColumns);
            }
            return Result;
        }

        public static void SetPrimaryKey(ref DataTable pDtL, string pDtLPrmKeyFieldName)
        {
            string[] primaryKeys = pDtLPrmKeyFieldName.Split(',');
            int arrCount = primaryKeys.Count();

            DataColumn[] keys = new DataColumn[arrCount];
            for (int i = 0; i < arrCount; i++)
            {
                if (!pDtL.Columns.Contains(primaryKeys[i]))
                    EventLog.WriteEntry("Application", string.Format("{0} adlı kolon yok!", primaryKeys[i]), EventLogEntryType.Error);
                keys[i] = pDtL.Columns[primaryKeys[i]];
            }
            if (arrCount > 0)
                pDtL = RemoveDuplicateRows(pDtL, primaryKeys[0]);
            pDtL.PrimaryKey = keys;
        }

        public static DataTable RemoveDuplicateRows(DataTable dTable, string colName)
        {
            var hTable = new Hashtable();
            var duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
                dTable.Rows.Remove(dRow);

            //Datatable which contains unique records will be return as output.
            return dTable;
        }

        private static void SetColumns(ref DataTable pDtL, ref DataTable pDtR)
        {
            string clmCap = string.Empty;
            string clmCapOrj = string.Empty;
            int suffix = 2;

            for (int i = 0; i < pDtR.Columns.Count; i++)
            {
                clmCapOrj = pDtR.Columns[i].Caption;
                clmCap = clmCapOrj;

                for (int ii = 0; ii < pDtL.Columns.Count; ii++)
                {
                    if (pDtL.Columns[ii].Caption == clmCap)
                    {
                        clmCap = clmCapOrj + suffix++;
                        ii = 0;
                    }
                }

                pDtL.Columns.Add(clmCap, pDtR.Columns[i].DataType);
            }
        }

        private static string[] GetColumnsInArray(DataTable pDt)
        {
            string[] columns = new string[pDt.Columns.Count];
            for (int i = 0; i < pDt.Columns.Count; i++)
            {
                columns[i] = pDt.Columns[i].Caption;
            }
            return columns;
        }

    }
}
