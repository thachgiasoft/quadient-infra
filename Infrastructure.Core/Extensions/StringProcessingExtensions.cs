using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Infrastructure.Core.Security;
using System.Globalization;

namespace Infrastructure.Core.Extensions
{
    public static class StringProcessingExtensions
    {
        public enum uTextAlign { center, justify, left, right }
        public enum uFontSize { large, larger, medium, small, smaller, xlarge, xsmall, xxlarge, xxsmall }
        public enum uFontName { Arial, Verdana, Segoe_UI, Times_New_Roman, Times_New_Roman_TUR, Tahoma, Sans_Serif }
        public enum uFontWeight { bold, bolder, lighter, normal }
        public enum uFontStyle { italic, normal }
        public enum uTextDecoration { none, overline, underline, line_through, blink }

        private const string NewLineHtml = "<br/>";

        public static string Substring(this string pParameter, int pStartIndex, int pLength)
        {
            if (pParameter.Substring(pStartIndex).Length > pLength)
            {
                return pParameter.Substring(pStartIndex, pLength);
            }
            else if (pStartIndex == 0)
            {
                return pParameter;
            }
            else
            {
                return pParameter.Substring(pStartIndex);
            }
        }

        /// <summary>
        /// Verilen bir string değerin son karakterini geri döndürür
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string LastCharacter(this string s)
        {
            s = s.Substring(s.Length - 1, 1);
            return s;
        }

        public static string ProperCase(this string pText)
        {
            string txt = pText.ToLower().Trim().Replace("  ", " ");
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txt)
                    .Replace("Ve ", "ve ")
                    .Replace("Ab ", "AB ");
        }

        /// <summary>
        /// Aralarına ayıraç konularak oluşturulmuş bir diziyi ters çevirmek için kullanılır. Örneğin, a,b,c şeklinde verilen bir dizi, c,b,a şekline dönüştürülür
        /// </summary>
        /// <param name="pDizi">Aralarına ayıraç konularak oluşturulmuş dizi</param>
        /// <param name="pAyirac">Dizi elemanları arasına konulan karakter. Virgül, Slash vs. olabilir.</param>
        /// <returns></returns>
        public static string DiziyiTersCevir(this string pDizi, char pAyirac)
        {
            string[] parca = pDizi.Split(pAyirac);
            StringBuilder sb = new StringBuilder();

            if (parca.Length > 0)
            {
                for (int i = parca.Length - 1; i >= 0; i--)
                {
                    sb.Append(pAyirac + parca[i]);
                }
                return sb.Remove(0, 1).ToString();
            }
            else
                return pDizi;
        }
        /// <summary>
        /// Aralarına ayıraç konularak oluşturulmuş bir diziyi ters çevirmek için kullanılır. Örneğin, a,b,c şeklinde verilen bir dizi, c,b,a şekline dönüştürülür
        /// </summary>
        /// <param name="pDizi">Aralarına ayıraç konularak oluşturulmuş dizi</param>
        /// <param name="pAyirac">Dizi elemanları arasına konulan karakter. Virgül, Slash vs. olabilir.</param>
        /// <returns></returns>
        public static string Invert(this string pDizi, char pAyirac)
        {
            return DiziyiTersCevir(pDizi, pAyirac);
        }
        public static string RSplit(this string pExpression, char pDelimeter)
        {
            string[] parca = pExpression.Split(pDelimeter);
            return parca[parca.Length - 1].ToString();
        }
        public static string RSplit(this string pExpression, string pDelimeter, StringSplitOptions pSplitOption)
        {
            string[] delimeter = { pDelimeter };
            string[] parca = pExpression.Split(delimeter, pSplitOption);
            return parca[parca.Length - 1].ToString();
        }
        public static string LSplit(this string pExpression, char pDelimeter)
        {
            return pExpression.Split(pDelimeter).GetValue(0).ToString();
        }
        public static string LSplit(this string pExpression, string pDelimeter, StringSplitOptions pSplitOption)
        {
            string[] delimeter = { pDelimeter };
            return pExpression.Split(delimeter, pSplitOption).GetValue(0).ToString();
        }

        private static string HataSayiCokBuyuk = "Girilen sayı çok büyük !";
        public static string SayiyiLiraKurusOlarakVer(this string pSayi)
        {
            if (pSayi.Length == 0) return string.Empty;

            string sonuc = string.Empty;
            string sayi = pSayi.ToString();
            string[] parca = sayi.Split(',');
            string lira = parca[0];
            if (lira.Length > 16) return HataSayiCokBuyuk;
            if (!lira.IsInteger()) return string.Empty;
            sonuc = SayiyiYaziyaCevir(Convert.ToInt64(lira)) + "Lira";
            string kurus = string.Empty;
            if (parca.Length > 1)
            {
                kurus = parca[1];
                if (kurus.Length > 16) return HataSayiCokBuyuk;
                if (!kurus.IsInteger()) return string.Empty;
                sonuc = sonuc + SayiyiYaziyaCevir(Convert.ToInt64(kurus)) + "Kuruş";
            }
            return sonuc;
        }
        public static string SayiyiYaziyaCevir(this long pSayi)
        {
            if (pSayi.ToString().Length == 0) return string.Empty;
            if (!pSayi.IsInteger()) return string.Empty;

            string[] b = new string[10];
            string[] y = new string[10];
            string[] m = new string[5];
            string[] v = new string[16];
            string[] c = new string[4];

            b[0] = "";
            b[1] = "Bir";
            b[2] = "İki";
            b[3] = "Üç";
            b[4] = "Dört";
            b[5] = "Beş";
            b[6] = "Altı";
            b[7] = "Yedi";
            b[8] = "Sekiz";
            b[9] = "Dokuz";

            y[0] = "";
            y[1] = "On";
            y[2] = "Yirmi";
            y[3] = "Otuz";
            y[4] = "Kırk";
            y[5] = "Elli";
            y[6] = "Altmış";
            y[7] = "Yetmiş";
            y[8] = "Seksen";
            y[9] = "Doksan";

            m[0] = "Trilyon";
            m[1] = "Milyar";
            m[2] = "Milyon";
            m[3] = "Bin";
            m[4] = "";

            string sayi = pSayi.ToString();
            if (sayi.Length > 16) return HataSayiCokBuyuk;
            bool negatif = false;
            if (sayi.Substring(0, 1) == "-") negatif = true;
            if (negatif) sayi = sayi.Substring(1);
            sayi = sayi.PadLeft(16, '0');

            for (int i = 1; i < 16; i++)
            {
                v[i] = sayi.Substring(i, 1);
            }

            string s = "";
            string e = "";

            for (int i = 0; i < 5; i++)
            {
                c[1] = v[(i * 3) + 1];
                c[2] = v[(i * 3) + 2];
                c[3] = v[(i * 3) + 3];

                if (c[1] == "0") e = "";
                else if (c[1] == "1") e = "Yüz";
                else e = b[Convert.ToInt32(c[1])] + "Yüz";

                e = e + y[Convert.ToInt32(c[2])] + b[Convert.ToInt32(c[3])];

                if (!String.IsNullOrEmpty(e)) e = e + m[i];
                if (i == 3 && (e == "BirBin")) e = "Bin";
                s = s + e;
            }

            if (String.IsNullOrEmpty(s)) s = "Sıfır";
            if (negatif) s = "Eksi" + s;
            return s;
        }

        public static string RemoveHtml(this string pText)
        {
            return Regex.Replace(pText, @"<[^>]*>", string.Empty).Replace("&nbsp;", "").Replace("&quot;", ((char)34).ToString());
        }

        /// <summary>
        /// Sayi, harf ve karakterden olusan string'inin harf ve karakterlerini cikartarak sadece sayilari return ediyor
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveNonNumeric(this string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
                if (Char.IsNumber(s[i]))
                    sb.Append(s[i]);
            return sb.ToString();
        }
        /// <summary>
        /// Sayi, harf ve karakterden olusan string'inin sayi ve karakterlerini cikartarak sadece harflari return ediyor
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveNonAlphabetic(this string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
                if (Char.IsLetter(s[i]))
                    sb.Append(s[i]);
            return sb.ToString();
        }

        public static string RemoveTurkishCharacters(this string s)
        {
            return s.ToLower()
                    .Replace("ü", "u").Replace("ç", "c").Replace("ş", "s")
                    .Replace("ğ", "g").Replace("ı", "i").Replace("ö", "o")
                    .Replace("Ü", "U").Replace("Ç", "C").Replace("Ş", "S")
                    .Replace("Ğ", "G").Replace("İ", "I").Replace("Ö", "O");
        }

        /// <summary>
        /// Türkçe karakterleri kodlayan bir method. Örneğin QueryString ile bir veri gönderirken Türkçe karakter kodlanarak gönderilebilir.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string EncodeTurkishCharacters(this string s)
        {
            return s.Replace("ç", "trchrc")
                    .Replace("Ç", "trchrC")
                    .Replace("ğ", "trchrg")
                    .Replace("Ğ", "trchrG")
                    .Replace("ı", "trchri")
                    .Replace("İ", "trchrI")
                    .Replace("ö", "trchro")
                    .Replace("Ö", "trchrO")
                    .Replace("ü", "trchru")
                    .Replace("Ü", "trchrU")
                    .Replace("ş", "trchrs")
                    .Replace("Ş", "trchrS");
        }

        /// <summary>
        /// Kodlanmış Türkçe karakterleri çözümlemek için kullanılır.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string DecodeTurkishCharacters(this string s)
        {
            return s.Replace("trchrc", "ç")
            .Replace("trchrC", "Ç")
            .Replace("trchrg", "ğ")
            .Replace("trchrG", "Ğ")
            .Replace("trchri", "ı")
            .Replace("trchrI", "İ")
            .Replace("trchro", "ö")
            .Replace("trchrO", "Ö")
            .Replace("trchru", "ü")
            .Replace("trchrU", "Ü")
            .Replace("trchrs", "ş")
            .Replace("trchrS", "Ş");
        }

        public static string FckEditorTurkceKarakterDuzelt(this string s)
        {
            return s.Replace("&ccedil;", "ç").Replace("&Ccedil;", "Ç").Replace("&ouml;", "ö").Replace("&Ouml;", "Ö").Replace("&uuml;", "ü").Replace("&Uuml;", "Ü");
        }

        public static string SayilarVirgulluGoster(this long l)
        {
            if (l > 0)
                return string.Format("{0:0,0}", l);
            else
                return l.ToString();
        }

        /// <summary>
        /// Sayfadaki bir kontrolden (örneğin GridView) bir text alındığında kodlu gelen bazı Türkçe karakterleri düzeltmek için kullanılır
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string HtmlChrCodeCorrection(this string s)
        {
            return s.Replace("&#220;", "Ü").Replace("&#252;", "ü").Replace("&#214;", "Ö").Replace("&#246;", "ö").Replace("&#231;", "ç").Replace("&#199;", "Ç").Replace("&nbsp;", " ");
        }

        public static string RemoveInjectionCharacters(this string s)
        {
            return s.Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace(" script ", " &#115;cript ")
            .Replace(" SCRIPT ", " &#083;CRIPT ")
            .Replace(" Script ", " &#083;cript ")
            .Replace(" object ", " &#111;bject ")
            .Replace(" OBJECT ", " &#079;BJECT ")
            .Replace(" Object ", " &#079;bject ")
            .Replace(" applet ", " &#097;pplet ")
            .Replace(" APPLET ", " &#065;PPLET ")
            .Replace(" Applet ", " &#065;pplet ")
            .Replace(" embed ", " &#101;mbed ")
            .Replace(" EMBED ", " &#069;MBED ")
            .Replace(" Embed ", " &#069;mbed ")
            .Replace(" event ", " &#101;vent ")
            .Replace(" EVENT ", " &#069;VENT ")
            .Replace(" Event ", " &#069;vent ")
            .Replace(" document ", " &#100;ocument ")
            .Replace(" DOCUMENT ", " &#068;OCUMENT ")
            .Replace(" Document ", " &#068;ocument ")
            .Replace(" cookie ", " &#099;ookie ")
            .Replace(" COOKIE ", " &#067;OOKIE ")
            .Replace(" Cookie ", " &#067;ookie ")

            .Replace("document.cookie", "&#068;ocument.cookie")
            .Replace("javascript:", "javascript ")
            .Replace("vbscript:", "vbscript ")
            .Replace("'", "&#39;");
        }

        public static string RecoverInjectionCharacters(this string s)
        {
            return s.Replace("&lt;", "<")
            .Replace("&gt;", ">")
            .Replace(" &#115;cript ", " script ")
            .Replace(" &#083;CRIPT ", " SCRIPT ")
            .Replace(" &#083;cript ", " Script ")
            .Replace(" &#111;bject ", " object ")
            .Replace(" &#079;BJECT ", " OBJECT ")
            .Replace(" &#079;bject ", " Object ")
            .Replace(" &#097;pplet ", " applet ")
            .Replace(" &#065;PPLET ", " APPLET ")
            .Replace(" &#065;pplet ", " Applet ")
            .Replace(" &#101;mbed ", " embed ")
            .Replace(" &#069;MBED ", " EMBED ")
            .Replace(" &#069;mbed ", " Embed ")
            .Replace(" &#101;vent ", " event ")
            .Replace(" &#069;VENT ", " EVENT ")
            .Replace(" &#069;vent ", " Event ")
            .Replace(" &#100;ocument ", " document ")
            .Replace(" &#068;OCUMENT ", " DOCUMENT ")
            .Replace(" &#068;ocument ", " Document ")
            .Replace(" &#099;ookie ", " cookie ")
            .Replace(" &#067;OOKIE ", " COOKIE ")
            .Replace(" &#067;ookie ", " Cookie ")
            .Replace("&#068;ocument.cookie", "document.cookie")
            .Replace("&#39;", "'");
        }

        public static string BuildAsString(this string[] s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string str in s)
            {
                sb.Append(str + NewLineHtml);
            }
            return sb.ToString();
        }
        public static string BuildAsString(this List<String> s)
        {
            StringBuilder sb = new StringBuilder();
            string[] arr = s.ToArray();
            foreach (string str in arr)
            {
                sb.Append(str + NewLineHtml);
            }
            return sb.ToString();
        }

        public static string AddHtmlFontTag(this string pValue, string pColor)
        {
            return AddHtmlFontTag(pValue, pColor, uFontSize.smaller, uFontWeight.bold, uFontStyle.normal, uTextDecoration.none, uTextAlign.left, uFontName.Arial);
        }
        public static string AddHtmlFontTag(this string pValue, string pColor, uFontSize pSize, uFontWeight pFontWeight)
        {
            return AddHtmlFontTag(pValue, pColor, pSize, pFontWeight, uFontStyle.normal, uTextDecoration.none, uTextAlign.left, uFontName.Arial);
        }
        public static string AddHtmlFontTag(this string pValue, string pColor, uFontSize pSize, uFontWeight pFontWeight, uFontStyle pFontStyle
                , uTextDecoration pTextDecoration, uTextAlign pAlign, uFontName pFontName)
        {
            string size = pSize.ToString().Replace("xx", "xx-").Replace("x", "x-");
            string weight = pFontWeight.ToString();
            string fontStyle = pFontStyle.ToString();
            string decoration = pTextDecoration.ToString().Replace('_', '-');
            string align = pAlign.ToString();
            string fontName = pFontName.ToString().Replace("_", "").Replace("Sans Serif", "Sans-Serif");

            string s = @"<font style=
                        'color: {1}; 
                        font-size: {2}; 
                        font-weight: {3}; 
                        font-style: {4}; 
                        text-decoration: {5};
                        text-align:{6};'
                        font-family: {7};
                     >
                        {0}
                    </font>";

            s = string.Format(s, pValue, pColor, size, weight, fontStyle, decoration, align, fontName);
            return s;
        }

        public static string ReplaceWholeWord(this string pInput, string pOldValue, string pNewValue)
        {
            string oldValue = string.Format(@"\b{0}\b", pOldValue);
            if (!Regex.IsMatch(pInput, oldValue))
                oldValue = string.Format(@"{0}\b", pOldValue);

            string s = Regex.Replace(pInput, oldValue, pNewValue);
            return s;
        }

        public static string Except(this string pS1, string pS2)
        {
            string[] s1 = pS1.Split(',');
            string[] s2 = pS2.Split(',');

            string s = string.Empty;

            foreach (string item in s1)
            {
                if (!s2.Contains(item)) s += string.Format(",{0}", item);
            }
            if (!String.IsNullOrEmpty(s)) s = s.TrimStart(',');
            return s;
        }

        public static List<T> Except<T>(this List<T> pS1, List<T> pS2)
        {
            List<T> s = new List<T>();

            foreach (T item in pS1)
            {
                if (!pS2.Contains(item)) s.Add(item);
            }
            return s;
        }


        public static string Titleize(this string text)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text).ToSentenceCase();
        }

        public static string ToSentenceCase(this string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]));
        }
    }
}
