using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data.ComponentModel;
using Infrastructure.Data.ComponentModelOperations;

namespace Infrastructure.Data.Extensions
{
    public static class DataTableEnumExtensions
    {
        public static DataTable GetEnumAsDataTable(this Array pEnum, SiralamaYonu pSortDirection)
        {
            return DataTableEnumConverter.GetEnumAsDataTable(pEnum, pSortDirection);
        }
    }
}
