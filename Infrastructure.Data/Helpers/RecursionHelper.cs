using System;
using System.Data;
using System.Text;
using Infrastructure.Core.Extensions;
using Infrastructure.Data.Extensions;
using System.Linq;
using System.Collections.Generic;
using Infrastructure.Data.ComponentModel;
namespace Infrastructure.Data.Helpers
{
    public static class RecursionHelper
    {
        /// <summary>
        ///  Hiyerarsik olarak asagi dogru olanlar
        /// </summary>
        /// <param name="pDataTable">The p data table.</param>
        /// <param name="pRootValue">The p root value.</param>
        /// <param name="pChildColName">Name of the p child col.</param>
        /// <param name="pParentColName">Name of the p parent col.</param>
        /// <param name="pAddNodeLevel">if set to <c>true</c> [p add node level].</param>
        /// <param name="pMaxDepth">The p max depth.</param>
        /// <returns></returns>
        public static DataTable RecursiveSelectChildrenByKey(DataTable pDataTable, string pRootValue, string pChildColName, string pParentColName, bool pAddNodeLevel, int pMaxDepth)
        {
            DataTable dtOut = pDataTable.Clone();
            if (pAddNodeLevel && !dtOut.Columns.Contains("NodeLevel"))
                dtOut.Columns.Add(new DataColumn("NodeLevel", typeof(Int32)));

            DataRow[] drChildren = pDataTable.Select(pChildColName + "='" + pRootValue + "'");
            if (drChildren.Length <= 0)
                return null;
            DataRow drNew = dtOut.LoadDataRow(drChildren[0].ItemArray, true);
            if (pAddNodeLevel)
                drNew["NodeLevel"] = 1;

            GetChildrenDown(pDataTable, pRootValue, pChildColName, pParentColName, 1, dtOut, pAddNodeLevel, pMaxDepth);
            dtOut.AcceptChanges();
            return dtOut;
        }

        

        private static void GetChildrenDown(DataTable pDataTable, string pRootValue, string pChildColName, string pParentColName, int pLevel, DataTable pOutTable, bool pAddNodeLevel, int pMaxDepth)
        {
            if (pMaxDepth < pLevel)
                return;

            DataRow[] drChildren = pDataTable.Select(pParentColName + "='" + pRootValue + "'");

            if (drChildren.Length <= 0)
                return;
            pLevel++;

            foreach (DataRow dr in drChildren)
            {
                DataRow drNew = pOutTable.LoadDataRow(dr.ItemArray, true);
                if (pAddNodeLevel)
                    drNew["NodeLevel"] = pLevel;
                GetChildrenDown(pDataTable, dr[pChildColName].ToString(), pChildColName, pParentColName, pLevel, pOutTable, pAddNodeLevel, pMaxDepth);
            }
        }

        public static DataTable RecursiveSelectChildrenByCustomFilter(DataTable pDataTable, string pRootFilter, string pChildColName, string pParentColName, bool pAddNodeLevel, int pMaxDepth)
        {
            DataTable dtOut = pDataTable.Clone();
            if (pAddNodeLevel && !dtOut.Columns.Contains("NodeLevel"))
                dtOut.Columns.Add(new DataColumn("NodeLevel", typeof(Int32)));

            DataRow[] drChildren = pDataTable.Select(pRootFilter);
            if (drChildren.Length <= 0)
                return null;
            foreach (DataRow drRoot in drChildren)
            {
                DataRow drNew = dtOut.LoadDataRow(drRoot.ItemArray, true);
                if (pAddNodeLevel)
                    drNew["NodeLevel"] = 1;
                GetChildrenDown(pDataTable, drRoot[pChildColName].ToString(), pChildColName, pParentColName, 1, dtOut, pAddNodeLevel, pMaxDepth);
            }
            dtOut.AcceptChanges();
            return dtOut;
        }

        /// <summary>
        /// Hiyerarsik olarak yukari dogru olanlar
        /// </summary>
        /// <param name="pDataTable">The p data table.</param>
        /// <param name="pChildValue">The p child value.</param>
        /// <param name="pChildColName">Name of the p child col.</param>
        /// <param name="pParentColName">Name of the p parent col.</param>
        /// <param name="pAddNodeLevel">if set to <c>true</c> [p add node level].</param>
        /// <param name="pMaxDepth">The p max depth.</param>
        /// <returns></returns>
        public static DataTable RecursiveSelectParentsByKey(DataTable pDataTable, string pChildValue, string pChildColName, string pParentColName, bool pAddNodeLevel, int pMaxDepth)
        {
            DataTable dtOut = pDataTable.Clone();
            if (pAddNodeLevel && !dtOut.Columns.Contains("NodeLevel"))
                dtOut.Columns.Add(new DataColumn("NodeLevel", typeof(Int32)));

            DataRow[] drChildren = pDataTable.Select(pChildColName + "='" + pChildValue + "'");
            if (drChildren.Length <= 0)
                return null;
            DataRow drNew = dtOut.LoadDataRow(drChildren[0].ItemArray, true);
            if (pAddNodeLevel)
                drNew["NodeLevel"] = 1;
            if (drChildren[0][pParentColName] != DBNull.Value)
                GetParentsUp(pDataTable, drChildren[0][pParentColName].ToString(), pChildColName, pParentColName, 1, dtOut, pAddNodeLevel, pMaxDepth);
            dtOut.AcceptChanges();
            return dtOut;
        }

        private static void GetParentsUp(DataTable pDataTable, string pRootValue, string pChildColName, string pParentColName, int pLevel, DataTable pOutTable, bool pAddNodeLevel, int pMaxDepth)
        {
            if (pMaxDepth < pLevel)
                return;

            DataRow[] drChildren = pDataTable.Select(pChildColName + "='" + pRootValue + "'");

            if (drChildren.Length <= 0)
                return;
            pLevel++;

            foreach (DataRow dr in drChildren)
            {
                DataRow drNew = pOutTable.LoadDataRow(dr.ItemArray, true);
                if (pAddNodeLevel)
                    drNew["NodeLevel"] = pLevel;
                if (dr[pParentColName] != DBNull.Value)
                    GetParentsUp(pDataTable, dr[pParentColName].ToString(), pChildColName, pParentColName, pLevel, pOutTable, pAddNodeLevel, pMaxDepth);
            }
            
        }


        public static List<T> RecursiveSelectParentsByKey<T>(List<T> source, string valueToSearch, string columnName, string parentColName, int maxDepth) where T : BaseHierarchyData
        {
            if (maxDepth <= 0 || source == null || source.Count == 0)
            {
                return null;
            }
            List<T> outList = new List<T>();
            var next = source.FirstOrDefault(p => p.GetType().GetProperty(columnName).GetValue(p).ToString() == valueToSearch);
            if (next == null)
            {
                return null;
            }
            var level = 1;
            next.NodeLevel = level;

            outList.Add(next);
            
            while (level < maxDepth && next != null)
            {
                var val = next.GetType().GetProperty(parentColName).GetValue(next);
                if (val == null)
                {
                    break;
                }
                valueToSearch = val.ToString();
                next = source.FirstOrDefault(p => p.GetType().GetProperty(columnName).GetValue(p).ToString() == valueToSearch);
                if (next == null)
                {
                    break;
                }
                
                level++;
                next.NodeLevel = level;
                outList.Add(next);
            }



            return outList;
        }

        public static void RecursiveSelectChildrenByKey<T>(List<T> source, string valueToSearch, string columnName, string parentColName, int maxDepth, ref List<T> retList, int level = 0) where T : BaseHierarchyData
        {
            if (level == 0)
            {
                retList = new List<T>();
            }
            if (maxDepth <= 0 || source == null || source.Count == 0 || level == maxDepth)
            {
                return;
            }
            List<T> list = null;
            if (level == 0)
            {
                list = source.Where(p => p.GetType().GetProperty(columnName).GetValue(p) != null && p.GetType().GetProperty(columnName).GetValue(p).ToString().Equals(valueToSearch)).ToList();
            }
            else
            {
                list = source.Where(p => p.GetType().GetProperty(parentColName).GetValue(p) != null && p.GetType().GetProperty(parentColName).GetValue(p).ToString().Equals(valueToSearch)).ToList();
            }
            
            if (list == null || list.Count == 0)
            {
                return;
            }
            level++;

            foreach (var item in list)
            {
                item.NodeLevel = level;
               
                retList.Add(item);

                var val = item.GetType().GetProperty(columnName).GetValue(item);
                if (val != null)
                {
                    RecursiveSelectChildrenByKey(source, val.ToString(), columnName, parentColName, maxDepth, ref retList, level);
                }
            }


        }

        /// <summary>
        /// Benzersiz Key değeri verilen recursive bir tablodan, istenen kolona ait bilgiler, verilen topKey değerine kadar alınarak path şeklinde döndürülür
        /// </summary>
        /// <param name="pDataSource">Verinin sorgulanacağı kaynak</param>
        /// <param name="pKey">Benzersiz Key değeri. Bu değere göre sorgulama yapılır.</param>
        /// <param name="pTopKey">Recursive işlemin nereye kadar yapılacağını gösteren benzersiz key değeri</param>
        /// <param name="pKeyName">Benzersiz Key bilgisinin alınacağı kolonun adı</param>
        /// <param name="pUpKeyName">Üst key değerinin sorgulanacağı kolon adı</param>
        /// <param name="pPathColumnName">Path olarak alınmak istenen kolon adı</param>
        /// <returns></returns>
        public static string GetRecursivePath(DataTable pDataSource, int pKey, int? pTopKey, string pKeyName, string pUpKeyName, string pPathColumnName)
        {
            return GetRecursivePath(pDataSource, pKey, pTopKey, pKeyName, pUpKeyName, pPathColumnName, true, true);
        }

        /// <summary>
        /// Benzersiz Key değeri verilen recursive bir tablodan, istenen kolona ait bilgiler, verilen topKey değerine kadar alınarak path şeklinde döndürülür
        /// </summary>
        /// <param name="pDataSource">Verinin sorgulanacağı kaynak</param>
        /// <param name="pKey">Benzersiz Key değeri. Bu değere göre sorgulama yapılır.</param>
        /// <param name="pTopKey">Recursive işlemin nereye kadar yapılacağını gösteren benzersiz key değeri</param>
        /// <param name="pKeyName">Benzersiz Key bilgisinin alınacağı kolonun adı</param>
        /// <param name="pUpKeyName">Üst key değerinin sorgulanacağı kolon adı</param>
        /// <param name="pPathColumnName">Path olarak alınmak istenen kolon adı</param>
        /// <param name="pAddBottomLevelItem">Bu seçenek true olursa, pKey değeri için elde edilen değer de path'e eklenir</param>
        /// <param name="pAddTopLevelItem">Bu seçenek true olursa, pTopKey değeri için elde edilen değer de path'e eklenir</param>
        /// <returns></returns>
        public static string GetRecursivePath(DataTable pDataSource, int pKey, int? pTopKey, string pKeyName, string pUpKeyName, string pPathColumnName, bool pAddBottomLevelItem, bool pAddTopLevelItem)
        {
            DataTable oDt = pDataSource;
            if (oDt.Rows.Count > 0)
            {
                oDt.SetPrimaryKey(pKeyName);
                StringBuilder path = new StringBuilder();
                int topKey = Convert.ToInt32(pTopKey);
                int key = pKey;
                string pathItem = string.Empty;

                do
                {
                    DataRow row = oDt.Rows.Find(key);
                    if (row == null) break;
                    pathItem = row[pPathColumnName] != null ? row[pPathColumnName].ToString() : string.Empty;
                    path.Append(string.Format("/{0}", pathItem));
                    key = int.TryParse(row[pUpKeyName].ToString(), out key) == false ? 0 : Convert.ToInt32(row[pUpKeyName]);

                } while (key != topKey);

                DataRow rw = oDt.Rows.Find(topKey);
                if (rw != null)
                {
                    pathItem = rw[pPathColumnName] != null ? rw[pPathColumnName].ToString() : string.Empty;
                    path.Append(string.Format("/{0}", pathItem));
                }

                string result = path.ToString().TrimStart('/').Invert('/');
                if (!pAddBottomLevelItem) result = result.Replace(result.RSplit('/'), string.Empty);
                if (!pAddTopLevelItem) result = result.Replace(result.LSplit('/'), string.Empty);
                return result.TrimStart('/').TrimEnd('/');
            }
            return string.Empty;
        }


        
        public static string GetRecursivePath<T>(List<T> pDataSource, int pKey, int? pTopKey, string pKeyName, string pUpKeyName, string pPathColumnName, bool pAddBottomLevelItem, bool pAddTopLevelItem)
        {
            var oDt = pDataSource;
            if (oDt.Count > 0)
            {
                
                StringBuilder path = new StringBuilder();
                int topKey = Convert.ToInt32(pTopKey);
                int key = pKey;
                string pathItem = string.Empty;

                do
                {
                    var row = oDt.FirstOrDefault(p => p.GetType().GetProperty(pKeyName).GetValue(p) != null && p.GetType().GetProperty(pKeyName).GetValue(p).Equals(key));
                    if (row == null) break;
                    pathItem = row.GetType().GetProperty(pPathColumnName).GetValue(row) != null ? row.GetType().GetProperty(pPathColumnName).GetValue(row).ToString() : string.Empty;
                    path.Append(string.Format("/{0}", pathItem));
                    key = int.TryParse(row.GetType().GetProperty(pUpKeyName).GetValue(row).ToString(), out key) == false ? 0 : Convert.ToInt32(row.GetType().GetProperty(pUpKeyName).GetValue(row));

                } while (key != topKey);

                var rw = oDt.FirstOrDefault(p => p.GetType().GetProperty(pKeyName).GetValue(p) != null && p.GetType().GetProperty(pKeyName).GetValue(p).Equals(topKey));
                if (rw != null)
                {
                    pathItem = rw.GetType().GetProperty(pPathColumnName).GetValue(rw) != null ? rw.GetType().GetProperty(pPathColumnName).GetValue(rw).ToString() : string.Empty;
                    path.Append(string.Format("/{0}", pathItem));
                }

                string result = path.ToString().TrimStart('/').Invert('/');
                if (!pAddBottomLevelItem) result = result.Replace(result.RSplit('/'), string.Empty);
                if (!pAddTopLevelItem) result = result.Replace(result.LSplit('/'), string.Empty);
                return result.TrimStart('/').TrimEnd('/');
            }
            return string.Empty;
        }
    }
}
