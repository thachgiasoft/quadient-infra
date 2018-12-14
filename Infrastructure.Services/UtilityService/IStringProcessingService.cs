using System;
using System.Collections.Generic;
using System.ServiceModel;
using Infrastructure.Core.ComponentModel;
using Infrastructure.Core.Extensions;

namespace Infrastructure.Services.UtilityService
{
    [ServiceContract]
    public interface IStringProcessingService
    {
        [OperationContract]
        string Substring(string pParameter, int pStartIndex, int pLength);

        /// <summary>
        /// Verilen bir string değerin son karakterini geri döndürür
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [OperationContract]
        string LastCharacter(string s);

        [OperationContract]
        string ProperCase(string pText);

        /// <summary>
        /// Aralarına ayıraç konularak oluşturulmuş bir diziyi ters çevirmek için kullanılır. Örneğin, a,b,c şeklinde verilen bir dizi, c,b,a şekline dönüştürülür
        /// </summary>
        /// <param name="pDizi">Aralarına ayıraç konularak oluşturulmuş dizi</param>
        /// <param name="pAyirac">Dizi elemanları arasına konulan karakter. Virgül, Slash vs. olabilir.</param>
        /// <returns></returns>
        [OperationContract]
        string DiziyiTersCevir(string pDizi, char pAyirac);

        [OperationContract]
        string RSplit(string pExpression, char pDelimeter);

        [OperationContract]
        string RSplit(string pExpression, string pDelimeter, StringSplitOptions pSplitOption);

        [OperationContract]
        string LSplit(string pExpression, char pDelimeter);

        [OperationContract]
        string LSplit(string pExpression, string pDelimeter, StringSplitOptions pSplitOption);

        [OperationContract]
        string SayiyiLiraKurusOlarakVer(string pSayi);

        [OperationContract]
        string SayiyiYaziyaCevir(long pSayi);

        [OperationContract]
        string RemoveHtml(string pText);

        /// <summary>
        /// Sayi, harf ve karakterden olusan string'inin harf ve karakterlerini cikartarak sadece sayilari return ediyor
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [OperationContract]
        string RemoveNonNumeric(string s);

        /// <summary>
        /// Sayi, harf ve karakterden olusan string'inin sayi ve karakterlerini cikartarak sadece harflari return ediyor
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [OperationContract]
        string RemoveNonAlphabetic(string s);

        [OperationContract]
        string RemoveTurkishCharacters(string s);

        /// <summary>
        /// Türkçe karakterleri kodlayan bir method. Örneğin QueryString ile bir veri gönderirken Türkçe karakter kodlanarak gönderilebilir.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>        string EncodeTurkishCharacters(string s);

        /// <summary>
        /// Kodlanmış Türkçe karakterleri çözümlemek için kullanılır.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [OperationContract]
        string DecodeTurkishCharacters(string s);

        [OperationContract]
        string FckEditorTurkceKarakterDuzelt(string s);

        [OperationContract]
        string SayilarVirgulluGoster(long l);

        /// <summary>
        /// Sayfadaki bir kontrolden (örneğin GridView) bir text alındığında kodlu gelen bazı Türkçe karakterleri düzeltmek için kullanılır
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [OperationContract]
        string HtmlChrCodeCorrection(string s);

        [OperationContract]
        string RemoveInjectionCharacters(string s);

        [OperationContract]
        string RecoverInjectionCharacters(string s);

        [OperationContract]
        string BuildAsString(string[] s);

        [OperationContract]
        string BuildAsString(List<String> s);

        [OperationContract]
        string AddHtmlFontTag(string pValue, string pColor);

        [OperationContract]
        string AddHtmlFontTag(string pValue, string pColor, StringProcessingExtensions.uFontSize pSize, StringProcessingExtensions.uFontWeight pFontWeight);

        [OperationContract]
        string AddHtmlFontTag(string pValue, string pColor, StringProcessingExtensions.uFontSize pSize, StringProcessingExtensions.uFontWeight pFontWeight, StringProcessingExtensions.uFontStyle pFontStyle
                                              , StringProcessingExtensions.uTextDecoration pTextDecoration, StringProcessingExtensions.uTextAlign pAlign, StringProcessingExtensions.uFontName pFontName);

        [OperationContract]
        string ReplaceWholeWord(string pInput, string pOldValue, string pNewValue);

        [OperationContract]
        string Except(string pS1, string pS2);

        [OperationContract]
        List<T> Except<T>(List<T> pS1, List<T> pS2);

        /// <summary>
        /// İstenen özelliklerde rastgele metin üretmek için kullanılır. Üretilen bu metin şifre olarak da kullanılabilir.
        /// </summary>
        /// <param name="pCharacterType">İstenen metnin özelliği</param>
        /// <param name="pLength">İstenen metnin uzunluğu</param>
        /// <returns></returns>
        [OperationContract]
        string GenerateRandomString(CharacterTypes pCharacterType, int pLength);

        /// <summary>
        /// İstenen uzunlukta rastgele sembol dizisi üretmek için kullanılır. Kullanılan semboller: ! # $ % & ( ) ? * +
        /// </summary>
        /// <param name="pLength">İstenen metnin uzunluğ</param>
        /// <returns></returns>
        [OperationContract]
        string GenerateRandomSymbol(int pLength);
    }
}