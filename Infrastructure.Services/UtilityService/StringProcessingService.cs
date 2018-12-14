using System;
using System.Collections.Generic;
using Infrastructure.Core.ComponentModel;
using Infrastructure.Core.Extensions;
using Infrastructure.Core.Helpers;

namespace Infrastructure.Services.UtilityService
{
    public class StringProcessingService : IStringProcessingService
    {
        public string Substring(string pParameter, int pStartIndex, int pLength)
        {
            return pParameter.Substring(pStartIndex, pLength);
        }

        public string LastCharacter(string s)
        {
            return s.LastCharacter();
        }

        public string ProperCase(string pText)
        {
            return pText.ProperCase();
        }

        public string DiziyiTersCevir(string pDizi, char pAyirac)
        {
            return pDizi.DiziyiTersCevir(pAyirac);
        }

        public string RSplit(string pExpression, char pDelimeter)
        {
            return pExpression.RSplit(pDelimeter);
        }

        public string RSplit(string pExpression, string pDelimeter, StringSplitOptions pSplitOption)
        {
            return pExpression.RSplit(pDelimeter, pSplitOption);
        }

        public string LSplit(string pExpression, char pDelimeter)
        {
            return pExpression.LSplit(pDelimeter);
        }

        public string LSplit(string pExpression, string pDelimeter, StringSplitOptions pSplitOption)
        {
            return pExpression.LSplit(pDelimeter, pSplitOption);
        }

        public string SayiyiLiraKurusOlarakVer(string pSayi)
        {
            return pSayi.SayiyiLiraKurusOlarakVer();
        }

        public string SayiyiYaziyaCevir(long pSayi)
        {
            return pSayi.SayiyiYaziyaCevir();
        }

        public string RemoveHtml(string pText)
        {
            return pText.RemoveHtml();
        }

        public string RemoveNonNumeric(string s)
        {
            return s.RemoveNonNumeric();
        }

        public string RemoveNonAlphabetic(string s)
        {
            return s.RemoveNonAlphabetic();
        }

        public string RemoveTurkishCharacters(string s)
        {
            return s.RemoveTurkishCharacters();
        }

        public string DecodeTurkishCharacters(string s)
        {
            return s.DecodeTurkishCharacters();
        }

        public string FckEditorTurkceKarakterDuzelt(string s)
        {
            return s.FckEditorTurkceKarakterDuzelt();
        }

        public string SayilarVirgulluGoster(long l)
        {
            return l.SayilarVirgulluGoster();
        }

        public string HtmlChrCodeCorrection(string s)
        {
            return s.HtmlChrCodeCorrection();
        }

        public string RemoveInjectionCharacters(string s)
        {
            return s.RemoveInjectionCharacters();
        }

        public string RecoverInjectionCharacters(string s)
        {
            return s.RecoverInjectionCharacters();
        }

        public string BuildAsString(string[] s)
        {
            return s.BuildAsString();
        }

        public string BuildAsString(List<string> s)
        {
            return s.BuildAsString();
        }

        public string AddHtmlFontTag(string pValue, string pColor)
        {
            return pValue.AddHtmlFontTag(pColor);
        }

        public string AddHtmlFontTag(string pValue, string pColor, StringProcessingExtensions.uFontSize pSize,
                                     StringProcessingExtensions.uFontWeight pFontWeight)
        {
            return pValue.AddHtmlFontTag(pColor, pSize, pFontWeight);
        }

        public string AddHtmlFontTag(string pValue, string pColor, StringProcessingExtensions.uFontSize pSize,
                                     StringProcessingExtensions.uFontWeight pFontWeight,
                                     StringProcessingExtensions.uFontStyle pFontStyle,
                                     StringProcessingExtensions.uTextDecoration pTextDecoration,
                                     StringProcessingExtensions.uTextAlign pAlign,
                                     StringProcessingExtensions.uFontName pFontName)
        {
            return pValue.AddHtmlFontTag(pColor, pSize, pFontWeight, pFontStyle, pTextDecoration, pAlign, pFontName);
        }

        public string ReplaceWholeWord(string pInput, string pOldValue, string pNewValue)
        {
            return pInput.ReplaceWholeWord(pOldValue, pNewValue);
        }

        public string Except(string pS1, string pS2)
        {
            return pS1.Except(pS2);
        }

        public List<T> Except<T>(List<T> pS1, List<T> pS2)
        {
            return pS1.Except(pS2);
        }

        public string GenerateRandomString(CharacterTypes pCharacterType, int pLength)
        {
            return CommonHelper.GenerateRandomString(pCharacterType, pLength);
        }

        public string GenerateRandomSymbol(int pLength)
        {
            return CommonHelper.GenerateRandomSymbol(pLength);
        }
    }
}