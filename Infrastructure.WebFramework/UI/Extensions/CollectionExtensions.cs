using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Infrastructure.WebFramework.UI.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// İlgili listenin selectlist'ini döner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="textField"></param>
        /// <param name="valueField"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> list, string textField
                                                                  , string valueField) where T : new()
        {
            return ToSelectList<T>(list, textField, valueField, new SelectListItem() { Selected = true, Text = "", Value = "-1" });
        }

        /// <summary>
        /// İlgili listenin selectlist'ini döner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="textField"></param>
        /// <param name="valueField"></param>
        /// <param name="selectedItem"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> list, string textField
                                                                  , string valueField, SelectListItem selectedItem) where T : new()
        {
            try
            {
                var selectList =
                    list.Where(x => x != null
                        && x.GetType().GetProperty(textField).GetValue(x) != null
                        && x.GetType().GetProperty(valueField).GetValue(x) != null)
                        .Select(x =>
                            new SelectListItem
                            {
                                Selected = false,
                                Text = x.GetType().GetProperty(textField).GetValue(x).ToString(),
                                Value = x.GetType().GetProperty(valueField).GetValue(x).ToString()
                            }).ToList();
                if (!selectedItem.Value.Equals("-1"))
                {
                    var selected = selectList.SingleOrDefault(i => i.Value == selectedItem.Value);
                    if (selected != null)
                        selected.Selected = true;
                }
                else
                    selectList.Add(selectedItem);
                return selectList;
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("{0},{1} selectlistinde hata oluştu. Detay: {2}", textField, valueField, e.Message));
            }
        }

        /// <summary>
        /// textField ve additional field'ın birleştirildiği bir select list döner. Örnek: textField: Ad, additonalTextField: Soyad verilirse geriye text field olarak Anakin Skywalker döner.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="textField"></param>
        /// <param name="additionalTextField"></param>
        /// <param name="valueField"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> list
            , string textField, string additionalTextField, string valueField) where T : new()
        {
            try
            {
                return
                    list.Where(x => x != null
                        && x.GetType().GetProperty(textField).GetValue(x) != null
                        && x.GetType().GetProperty(valueField).GetValue(x) != null)
                        .Select(x =>
                            new SelectListItem
                            {
                                Selected = false,
                                Text = String.Format("{0} - {1}"
                                , x.GetType().GetProperty(textField).GetValue(x).ToString()
                                , x.GetType().GetProperty(additionalTextField).GetValue(x).ToString()),
                                Value = x.GetType().GetProperty(valueField).GetValue(x).ToString()
                            });
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("{0},{1} selectlistinde hata oluştu. Detay: {2}", textField, valueField, e.Message));
            }
        }

    }
}
