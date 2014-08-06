using System;
using System.Globalization;

namespace MediaSync.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToShortMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dateTime.Month);
        }
    }
}
