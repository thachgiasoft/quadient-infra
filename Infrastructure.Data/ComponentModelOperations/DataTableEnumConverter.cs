using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data.ComponentModel;
using Infrastructure.Data.Extensions;

namespace Infrastructure.Data.ComponentModelOperations
{
    public static class DataTableEnumConverter
    {
        public static DataTable GetEnumAsDataTable(Array pEnum)
        {
            return GetEnumAsDataTable(pEnum, 0);
        }

        public static DataTable GetEnumAsDataTable(Array pEnum, SiralamaYonu pSortDirection)
        {
            DataTable oDt = new DataTable();
            oDt.Columns.Add("Text");
            oDt.Columns.Add("Value");
            oDt.Columns.Add("Description");
            oDt.TableName = "ENUM";

            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = oDt.Columns["Value"];
            oDt.PrimaryKey = PrimaryKeyColumns;

            foreach (var item in pEnum)
            {
                string itemText = item.ToString();
                string itemValue = Convert.ToInt32(item).ToString();
                string itemDescription = "";
                FieldInfo fi = item.GetType().GetField(item.ToString());
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                itemDescription = (attributes.Length > 0) ? attributes[0].Description : item.ToString();

                DataRow oRw = oDt.NewRow();
                oRw["Text"] = itemText;
                oRw["Value"] = itemValue;
                oRw["Description"] = itemDescription;
                oDt.Rows.Add(oRw);
            }

            if (pSortDirection != 0) oDt = oDt.FilterDataTable(string.Empty, string.Format("Text {0}", pSortDirection.ToString()));
            return oDt;
        }

     
    }
}
