using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core.ComponentModelOperations
{
    /// <summary>
    /// Enum işlemlerini yöneten sınıf.
    /// </summary>
    public static class EnumOperations
    {
        /// <summary>
        /// Verilen enum için tanım bilgisi döndürür.
        /// DescriptionAttribute e sahip enumlar için Description bilgisi, ilgili attribute ile işaretlenmemiş enum lar için type bilgisi verir.
        /// </summary>
        /// <param name="pEnumItem">Açıklama bilgisi istenen enum türü.</param>
        /// <returns>DescriptionAttribute e sahip enumlar için Description bilgisi, ilgili attribute ile işaretlenmemiş enum lar için type bilgisi döndürür.</returns>
        public static string GetEnumDescription(Enum pEnumItem)
        {
            Type type = pEnumItem.GetType();
            MemberInfo[] memInfo = type.GetMember(pEnumItem.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return pEnumItem.ToString();
        }
        /// <summary>
        /// Verilen enum name i belirtilen tipte bir enum nesnesine çevirir.
        /// </summary>
        /// <typeparam name="T">Enum tipi.</typeparam>
        /// <param name="name">Enum adı bilgisi.</param>
        /// <returns>Verilen enum name için belirtilen tipte bir enum nesnesi döndürür.</returns>
        public static T ToEnum<T>(string name)
        {
            if (Enum.IsDefined(typeof(T), name))
                return (T)Enum.Parse(typeof(T), name);
            else
                return default(T);
        }
        /// <summary>
        /// Belirtilen enum nesnesini istenilen generic tipte döndürür.
        /// </summary>
        /// <typeparam name="T">İstenilen return tipi.</typeparam>
        /// <param name="pEnumItem">Dönüştürülecek enum tipi.</param>
        /// <returns>Belirtilen enum nesnesini istenilen generic tipte döndürür.</returns>
        public static T GetValue<T>(Enum pEnumItem)
        {
            return (T)Convert.ChangeType(pEnumItem, typeof(T));
        }
    }
}
