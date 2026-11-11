using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Extensions
{
    public static DateTime Format(this DateOnly date) => new DateTime(date.Year, date.Month, date.Day);
    public static DateOnly Format(this DateTime date) => new DateOnly(date.Year, date.Month, date.Day);
    public static short FormatToShort(this bool value) => (short)(value ? 1 : 0);
    public static bool FormatToBool(this short value) => value == 1;
    public static string FormatPeriod(int year, int? periodo) => $"{year}-{periodo ?? 0}";
    public static int FormatPeriodYear(this string Semestre) => Semestre.Split('-').Length > 0 ? int.Parse(Semestre.Split('-')[0]) : 0;
    public static int? FormatPeriodPeriodo(this string Semestre) => Semestre.Split('-').Length > 1 ? int.Parse(Semestre.Split('-')[1]) : null;
}
