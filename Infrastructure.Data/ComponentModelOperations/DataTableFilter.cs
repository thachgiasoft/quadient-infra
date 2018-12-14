using System.Data;
using Infrastructure.Data.Extensions;

namespace Infrastructure.Data.ComponentModelOperations
{
    /// <summary>
    /// Datatable filter islemlerini yoneten sinif.
    /// </summary>
    public class DataTableFilter
    {
        public static DataTable SortDataTable(DataTable pDataTable, string pSortExpression)
        {
            return FilterDataTable(pDataTable, string.Empty, pSortExpression);
        }

        /// <summary>
        /// Verilen datatable icerisinden pTopN ile belirtilen top kayitlari getirir.
        /// </summary>
        /// <param name="pDataTable">Filtrelenecek datatable</param>
        /// <param name="pTopN">Top n kayit sayisi</param>
        /// <returns>Ilgili datatable dan top n kayit dondurur.</returns>
        public static DataTable FilterDataTable(DataTable pDataTable, int pTopN)
        {
            if (pDataTable.Rows.Count > 0)
            {
                if (pDataTable.Rows.Count > pTopN)
                    return GetTopN(pDataTable, pTopN);
                else
                    return pDataTable;
            }
            else
                return pDataTable;
        }

        /// <summary>
        /// Verilen datatable i belirtilen filter ile filtreler.
        /// </summary>
        /// <param name="pDataTable">Filtrelenmek istenen datatable</param>
        /// <param name="pFilterExpression">Filter cumlesi</param>
        /// <returns>Filtrelenmis datatable nesnesini dondurur.</returns>
        public static DataTable FilterDataTable(DataTable pDataTable, string pFilterExpression)
        {
            if (pDataTable.Rows.Count < 1) return pDataTable;
            return DataTableFilterCommonMethod(pDataTable, pFilterExpression, string.Empty, 0);
        }

        /// <summary>
        /// Verilen datatable i belirtilen filter ile filtreler ve top n kaydi getirir.
        /// </summary>
        /// <param name="pDataTable">Filtrelenmek istenen datatable</param>
        /// <param name="pFilterExpression">Filter cumlesi</param>
        /// /// <param name="pTopN">Topn kayit sayisi</param>
        /// <returns>Filtrelenmis datatable nesnesini dondurur.</returns>
        public static DataTable FilterDataTable(DataTable pDataTable, string pFilterExpression, int pTopN)
        {
            if (pDataTable.Rows.Count < 1) return pDataTable;
            return DataTableFilterCommonMethod(pDataTable, pFilterExpression, string.Empty, pTopN);
        }

        /// <summary>
        /// Verilen datatable i belirtilen filter ile filtreler ve sortexpression a gore siralar.
        /// </summary>
        /// <param name="pDataTable">Datatable nesnesi</param>
        /// <param name="pFilterExpression">Filter sql</param>
        /// <param name="pSortExpression">Sort sql</param>
        /// <returns>Filtrelenmis datatable nesnesini dondurur.</returns>
        public static DataTable FilterDataTable(DataTable pDataTable, string pFilterExpression, string pSortExpression)
        {
            if (pDataTable.Rows.Count < 1) return pDataTable;
            return DataTableFilterCommonMethod(pDataTable, pFilterExpression, pSortExpression, 0);
        }

        /// <summary>
        /// Verilen datatable i belirtilen filter ile filtreler ve sortexpression a gore siralayip top n kayit dondurur.
        /// </summary>
        /// <param name="pDataTable">Datatable nesnesi</param>
        /// <param name="pFilterExpression">Filter sql</param>
        /// <param name="pSortExpression">Sort sql</param>
        /// /// <param name="pTopN">Top n kayit sayisi</param>
        /// <returns>Filtrelenmis datatable nesnesini dondurur.</returns>
        public static DataTable FilterDataTable(DataTable pDataTable, string pFilterExpression, string pSortExpression, int pTopN)
        {
            if (pDataTable.Rows.Count < 1) return pDataTable;
            return DataTableFilterCommonMethod(pDataTable, pFilterExpression, pSortExpression, pTopN);
        }

        private static DataTable GetTopN(DataTable pOdt, int pTopN)
        {
            if (pOdt.Rows.Count < 1 || pTopN == 0 || pTopN >= pOdt.Rows.Count) return pOdt;

            DataTable yeniDt = pOdt.Clone();
            for (int i = 0; i < pTopN; i++)
            {
                yeniDt.ImportRow(pOdt.Rows[i]);
            }

            return yeniDt;
        }

        private static DataTable DataTableFilterCommonMethod(DataTable pDt, string pFilterExpression, string pSortExpression, int pTopN)
        {
            lock (pDt)
            {
                DataView dv = new DataView(pDt, pFilterExpression, pSortExpression, DataViewRowState.CurrentRows);
                DataTable oDt = dv.ToTable();

                if (pTopN > 0)
                {
                    if (oDt.Rows.Count > pTopN)
                        return oDt.FilterDataTable(pTopN);
                    else
                        return oDt;
                }

                return oDt;
            }
        }
    }
}
