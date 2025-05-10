// <copyright file="DateOnlyExtensions.cs" company="ted-sharp">
// Copyright (c) ted-sharp. All rights reserved.
// </copyright>

// ReSharper disable ArrangeStaticMemberQualifier
namespace Aloe.Utils.Wafu.Date;

/// <summary>
/// DateOnly型の拡張メソッドを提供するクラス
/// </summary>
public static class DateOnlyExtensions
{
    /// <summary>
    /// DateOnlyをDateTimeに変換します。時刻は最小値（00:00:00）が設定されます。
    /// </summary>
    /// <param name="date">変換するDateOnly値</param>
    /// <returns>変換されたDateTime値</returns>
    public static DateTime ToDateTime(this DateOnly date)
    {
        return date.ToDateTime(TimeOnly.MinValue);
    }

    /// <summary>
    /// 文字列をDateOnlyに変換します。変換できない場合は今日の日付を返します。
    /// </summary>
    /// <param name="dateString">変換する日付文字列</param>
    /// <returns>変換されたDateOnly値。変換できない場合は今日の日付</returns>
    public static DateOnly ToDateOrToday(this string dateString)
    {
        if (String.IsNullOrWhiteSpace(dateString))
        {
            return DateHelper.GetToday();
        }

        if (DateHelper.TryParseEx(dateString, out var date))
        {
            return date;
        }

        return DateHelper.GetToday();
    }
}
