using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Infrastructure.Core.ComponentModel
{
    /// <summary>
    /// Generic tip dönüştürücü sınıf.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericListTypeConverter<T> : TypeConverter
    {
        protected readonly TypeConverter TypeConverter;

        public GenericListTypeConverter()
        {
            TypeConverter = TypeDescriptor.GetConverter(typeof(T));
            if (TypeConverter == null)
                throw new InvalidOperationException("No type converter exists for type " + typeof(T).FullName);
        }

        /// <summary>
        /// Elemanları ',' ile ayrılmış string ifade yi string[] ye çevirir.
        /// </summary>
        /// <param name="input">string[] diziye çevrilmek istenen string ifade.</param>
        /// <returns>Elemanları ',' ile ayrılmış string ifade den string[] döndürür.</returns>
        protected virtual string[] GetStringArray(string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                var result = input.Split(',');
                // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                Array.ForEach(result, s => s.Trim());
                // ReSharper restore ReturnValueOfPureMethodIsNotUsed
                return result;
            }
            return new string[0];
        }
        /// <summary>
        /// Belirtilen context ile verilen type' ın dönüştürülüp dönüştürülemeyeceği değerini verir.
        /// </summary>
        /// <param name="context">Tip tanımlayıcı context.</param>
        /// <param name="sourceType">Dönüştürülecek olan kaynak tip.</param>
        /// <returns>Belirtilen tip tanımlayıcı context kullanılarak verilen kaynak tipten bir object oluşturulup oluşturulamayacağı değerini döndürür.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {

            if (sourceType == typeof(string))
            {
                var items = GetStringArray(sourceType.ToString());
                return items.Any();
            }

            return base.CanConvertFrom(context, sourceType);
        }
        /// <summary>
        /// Belirtilen context ve kültür bilgisi kullanılarak verilen değeri converter tipinde bir nesneye çevirir.
        /// </summary>
        /// <param name="context">Dönüşüm sırasında kullanılan tip tanımlayıcı context.</param>
        /// <param name="culture">Dönüşüm sırasında kullanılan kültür bilgisi.</param>
        /// <param name="value">Dönüştürülecek olan değer.</param>
        /// <returns>Belirtilen context ve kültür bilgisi kullanılarak verilen değeri converter tipinde bir nesneye çevirir.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var valuecast = value as string;
            if (valuecast != null)
            {
                var items = GetStringArray(valuecast);
                var result = new List<T>();
                Array.ForEach(items, s =>
                {
                    var item = TypeConverter.ConvertFromInvariantString(s);
                    if (item != null)
                    {
                        result.Add((T)item);
                    }
                });

                return result;
            }
            return base.ConvertFrom(context, culture, value);
        }
        /// <summary>
        /// Belirtilen context ve kültür bilgisi kullanılarak verilen değeri hedef tipte bir nesneye dönüştürür.
        /// </summary>
        /// <param name="context">Dönüşüm sırasında kullanılan tip tanımlayıcı context.</param>
        /// <param name="culture">Dönüşüm sırasında kullanılan kültür bilgisi.</param>
        /// <param name="value">Dönüştürülecek olan değer.</param>
        /// <param name="destinationType">Hedef tür.</param>
        /// <returns>Belirtilen context ve kültür bilgisi kullanılarak verilen değeri belirtilen tipte bir nesneye çevirir.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var result = string.Empty;
                if (value != null)
                {
                    //we don't use string.Join() because it doesn't support invariant culture
                    for (int i = 0; i < ((IList<T>)value).Count; i++)
                    {
                        var str1 = Convert.ToString(((IList<T>)value)[i], CultureInfo.InvariantCulture);
                        result += str1;
                        //don't add comma after the last element
                        if (i != ((IList<T>)value).Count - 1)
                            result += ",";
                    }
                }
                return result;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
