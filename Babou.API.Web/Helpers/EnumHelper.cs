using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Babou.API.Web.Helpers
{
    public class Enum<T> where T : struct, IConvertible
    {
        /// <summary>
        /// Uses the <see cref="DisplayAttribute"/> as the text for an item and the <see cref="DescriptionAttribute"/> for the value.
        /// If one or the other are absent, converts the <see cref="Enum"/> to it's string value.
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetSelectListItem()
        {
            var items = new List<SelectListItem>();

            var type = typeof(T);

            if (type.IsEnum)
            {
                var values = Enum.GetValues(type);

                foreach (int val in values)
                {
                    var memInfo = type.GetMember(type.GetEnumName(val));

                    var displayValue = memInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
                    var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                    var item = new SelectListItem
                    {
                        Text = val.ToString(),
                        Value = val.ToString()
                    };

                    if (displayValue.Length > 0)
                    {
                        item.Text = ((DisplayAttribute)displayValue[0]).Name;
                    }

                    if (descriptionAttributes.Length > 0)
                    {
                        item.Value = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                    }

                    items.Add(item);
                }
            }
            return items;
        }

        public static List<string> GetListByDescription()
        {
            var items = new List<string>();

            var type = typeof(T);

            if (type.IsEnum)
            {
                var values = Enum.GetValues(type);

                foreach (int val in values)
                {
                    var memInfo = type.GetMember(type.GetEnumName(val));

                    var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                    var enumString = val.ToString();

                    if (descriptionAttributes.Length > 0)
                    {
                        enumString = ((DescriptionAttribute)descriptionAttributes[0]).Description;
                    }

                    items.Add(enumString);
                }
            }
            return items;
        }
    }
}
