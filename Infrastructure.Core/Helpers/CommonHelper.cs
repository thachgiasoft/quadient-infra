using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Infrastructure.Core.ComponentModel;
using Infrastructure.Core.Security;
using Infrastructure.Exceptions;
using Infrastructure.Exceptions.CommonExceptions;

namespace Infrastructure.Core.Helpers
{
    public class CommonHelper
    {

        /// <summary>
        /// Ensures the subscriber email or throw.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static string EnsureSubscriberEmailOrThrow(string email)
        {
            string output = EnsureNotNull(email);
            output = output.Trim();
            output = EnsureMaximumLength(output, 255);

            if (!IsValidEmail(output))
            {
                throw new EmailException("Email is not valid.");
            }

            return output;
        }

        /// <summary>
        /// Verifies that a string is in valid e-mail format
        /// </summary>
        /// <param name="email">Email to verify</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
        public static bool IsValidEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;

            email = email.Trim();
            var result = Regex.IsMatch(email, "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$", RegexOptions.IgnoreCase);
            return result;
        }
        /// <summary>
        /// Generate random digit code
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Result string</returns>
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            var str = string.Empty;
            for (var i = 0; i < length; i++)
                str = String.Concat(str, random.Next(10).ToString(CultureInfo.InvariantCulture));
            return str;
        }

        /// <summary>
        /// Returns an random interger number within a specified rage
        /// </summary>
        /// <param name="min">Minimum number</param>
        /// <param name="max">Maximum number</param>
        /// <returns>Result</returns>
        public static int GenerateRandomInteger(int min = 0, int max = 2147483647)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        /// <summary>
        /// Ensure that a string doesn't exceed maximum allowed length
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="postfix">A string to add to the end if the original string was shorten</param>
        /// <returns>Input string if its lengh is OK; otherwise, truncated input string</returns>
        public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
        {
            if (String.IsNullOrEmpty(str))
                return str;

            if (str.Length > maxLength)
            {
                var result = str.Substring(0, maxLength);
                if (!String.IsNullOrEmpty(postfix))
                {
                    result += postfix;
                }
                return result;
            }
            return str;
        }

        /// <summary>
        /// Ensures that a string only contains numeric values
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Input string with only numeric values, empty string if input is null/empty</returns>
        public static string EnsureNumericOnly(string str)
        {
            if (String.IsNullOrEmpty(str))
                return string.Empty;

            var result = new StringBuilder();
            foreach (char c in str)
            {
                if (Char.IsDigit(c))
                    result.Append(c);
            }
            return result.ToString();
        }

        /// <summary>
        /// Ensure that a string is not null
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Result</returns>
        public static string EnsureNotNull(string str)
        {
            return str ?? string.Empty;
        }

        /// <summary>
        /// Indicates whether the specified strings are null or empty strings
        /// </summary>
        /// <param name="stringsToValidate">Array of strings to validate</param>
        /// <returns>Boolean</returns>
        public static bool AreNullOrEmpty(params string[] stringsToValidate)
        {
            var result = false;
            Array.ForEach(stringsToValidate, str =>
            {
                if (string.IsNullOrEmpty(str)) result = true;
            });
            return result;
        }
        /// <summary>
        /// Sets a property on an object to a valuae.
        /// </summary>
        /// <param name="instance">The object whose property to set.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            Type instanceType = instance.GetType();
            PropertyInfo pi = instanceType.GetProperty(propertyName);
            if (pi == null)
                throw new CoreException("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType);
            if (!pi.CanWrite)
                throw new CoreException("The property '{0}' on the instance of type '{1}' does not have a setter.", propertyName, instanceType);
            if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
                value = To(value, pi.PropertyType);
            pi.SetValue(instance, value, new object[0]);
        }

        public static TypeConverter GetCustomTypeConverter(Type type)
        {
            //we can't use the following code in order to register our custom type descriptors
            //TypeDescriptor.AddAttributes(typeof(List<int>), new TypeConverterAttribute(typeof(GenericListTypeConverter<int>)));
            //so we do it manually here
            if (type == typeof(List<int>))
                return new GenericListTypeConverter<int>();
            if (type == typeof(List<decimal>))
                return new GenericListTypeConverter<decimal>();
            if (type == typeof(List<string>))
                return new GenericListTypeConverter<string>();
            return TypeDescriptor.GetConverter(type);
        }

    
        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <param name="culture">Culture</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value != null)
            {
                var sourceType = value.GetType();

                var destinationConverter = GetCustomTypeConverter(destinationType);
                var sourceConverter = GetCustomTypeConverter(sourceType);
                if (destinationConverter != null && destinationConverter.CanConvertFrom(value.GetType()))
                    return destinationConverter.ConvertFrom(null, culture, value);
                if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                    return sourceConverter.ConvertTo(null, culture, value, destinationType);
                if (destinationType.IsEnum && value is int)
                    return Enum.ToObject(destinationType, (int)value);
                if (!destinationType.IsInstanceOfType(value))
                    return Convert.ChangeType(value, destinationType, culture);
            }
            return value;
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        public static T To<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)To(value, typeof(T));
        }

        /// <summary>
        /// Convert enum for front-end
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Converted string</returns>
        public static string ConvertEnum(string str)
        {
            string result = string.Empty;
            char[] letters = str.ToCharArray();
            foreach (char c in letters)
                if (c.ToString() != c.ToString().ToLower())
                    result += " " + c.ToString();
                else
                    result += c.ToString();
            return result;
        }

        public static bool OneToManyCollectionWrapperEnabled
        {
            get
            {
                bool enabled = !String.IsNullOrEmpty(ConfigurationManager.AppSettings["OneToManyCollectionWrapperEnabled"]) &&
                   Convert.ToBoolean(ConfigurationManager.AppSettings["OneToManyCollectionWrapperEnabled"]);
                return enabled;
            }
        }
        /// <summary>
        /// İstenen özelliklerde rastgele metin üretmek için kullanılır. Üretilen bu metin şifre olarak da kullanılabilir.
        /// </summary>
        /// <param name="pCharacterType">İstenen metnin özelliği</param>
        /// <param name="pLength">İstenen metnin uzunluğu</param>
        /// <returns></returns>
        public static string GenerateRandomString(CharacterTypes pCharacterType, int pLength)
        {
            string pw = string.Empty;
            int length = pLength - 1;

            int rdNumber = 0;
            int minValue = 0;
            int maxValue = 0;

            int dMin = 48;
            int dMax = 57;
            int uLMin = 65;
            int uLMax = 90;
            int lLMin = 97;
            int lLMax = 122;

            var _scrRandom = new SecureRandom();

            do
            {
                switch (pCharacterType)
                {
                    case CharacterTypes.OnlyDigits:
                        minValue = dMin;
                        maxValue = dMax;
                        rdNumber = _scrRandom.Next(minValue, maxValue);
                        break;
                    case CharacterTypes.UpperLetters:
                        minValue = uLMin;
                        maxValue = uLMax;
                        rdNumber = _scrRandom.Next(minValue, maxValue);
                        break;
                    case CharacterTypes.LowerLetters:
                        minValue = lLMin;
                        maxValue = lLMax;
                        rdNumber = _scrRandom.Next(minValue, maxValue);
                        break;
                    case CharacterTypes.UpperLowerLetters:
                        minValue = uLMin;
                        maxValue = lLMax;
                        do
                            rdNumber = _scrRandom.Next(minValue, maxValue);
                        while (!((rdNumber >= uLMin & rdNumber <= uLMax) | (rdNumber >= lLMin & rdNumber <= lLMax)));
                        break;
                    case CharacterTypes.DigitLowerLetters:
                        minValue = dMin;
                        maxValue = lLMax;
                        do
                            rdNumber = _scrRandom.Next(minValue, maxValue);
                        while (!((rdNumber >= dMin & rdNumber <= dMax) | (rdNumber >= lLMin & rdNumber <= lLMax)));
                        break;
                    case CharacterTypes.DigitUpperLetters:
                        minValue = dMin;
                        maxValue = uLMax;
                        do
                            rdNumber = _scrRandom.Next(minValue, maxValue);
                        while (!((rdNumber >= dMin & rdNumber <= dMax) | (rdNumber >= uLMin & rdNumber <= uLMax)));
                        break;
                    case CharacterTypes.DigitUpperLowerLetters:
                        minValue = dMin;
                        maxValue = lLMax;
                        do
                            rdNumber = _scrRandom.Next(minValue, maxValue);
                        while (!((rdNumber >= dMin & rdNumber <= dMax) | (rdNumber >= uLMin & rdNumber <= uLMax) | (rdNumber >= lLMin & rdNumber <= lLMax)));
                        break;
                }

                pw = string.Format("{0}{1}", pw, (char)rdNumber);

            } while (pw.Length <= length);

            return pw;
        }

        /// <summary>
        /// İstenen uzunlukta rastgele sembol dizisi üretmek için kullanılır. Kullanılan semboller: ! # $ % & ( ) ? * +
        /// </summary>
        /// <param name="pLength">İstenen metnin uzunluğ</param>
        /// <returns></returns>
        public static string GenerateRandomSymbol(int pLength)
        {
            string pw = string.Empty;
            string symbol = string.Empty;
            int length = pLength - 1;

            do
            {
                string rdnNo = GenerateRandomString(CharacterTypes.OnlyDigits, 1);
                switch (rdnNo)
                {
                    case "0": symbol = "("; break;
                    case "1": symbol = "$"; break;
                    case "2": symbol = "="; break;
                    case "3": symbol = "%"; break;
                    case "4": symbol = "-"; break;
                    case "5": symbol = "!"; break;
                    case "6": symbol = "*"; break;
                    case "7": symbol = "?"; break;
                    case "8": symbol = "+"; break;
                    case "9": symbol = ")"; break;
                    default: symbol = "!"; break;
                }

                pw = string.Format("{0}{1}", pw, symbol);

            } while (pw.Length <= length);

            return pw;
        }

        /// <summary>
        /// Compares two array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < a1.Length; i++)
            {
                if (!comparer.Equals(a1[i], a2[i])) return false;
            }
            return true;
        }

    }
}
