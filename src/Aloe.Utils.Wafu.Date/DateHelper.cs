// <copyright file="DateHelper.cs" company="ted-sharp">
// Copyright (c) ted-sharp. All rights reserved.
// </copyright>

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable ArrangeStaticMemberQualifier
namespace Aloe.Utils.Wafu.Date;

/// <summary>
/// 日付操作を支援するユーティリティクラスです。
/// </summary>
public static class DateHelper
{
    /// <summary>
    /// 日付文字列のパースに使用するフォーマットの配列です。
    /// </summary>
    private static readonly string[] s_formats =
    [
        "yyyyMMdd",
        "yyyyMM",

        "yyyy年MM月dd日",
        "yyyy/MM/dd",
        "yyyy-MM-dd",
        "yyyy.MM.dd",

        "yyyy年M月d日",
        "yyyy/M/d",
        "yyyy-M-d",
        "yyyy.M.d",

        "yyyy年MM月",
        "yyyy/MM",
        "yyyy-MM",
        "yyyy.MM",

        "yyyy年M月",
        "yyyy/M",
        "yyyy-M",
        "yyyy.M",
    ];

    /// <summary>
    /// 和暦カルチャー情報を遅延初期化で保持します。
    /// </summary>
    private static readonly Lazy<CultureInfo> s_jaCulture = new(() =>
    {
        var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        culture.DateTimeFormat.Calendar = new JapaneseCalendar();
        return culture;
    });

    /// <summary>
    /// 日付文字列のパース時に使用するスタイル設定です。
    /// </summary>
    private static readonly DateTimeStyles s_styles =
        // タイムゾーンなしはローカルタイムとみなす
        DateTimeStyles.AssumeLocal
        // 先頭の空白文字を無視
        | DateTimeStyles.AllowLeadingWhite
        // 末尾の空白文字を無視
        | DateTimeStyles.AllowTrailingWhite;

    /// <summary>
    /// 1-2桁の数字や月名が指定された場合は月数(1-12)にパースします。
    /// </summary>
    /// <param name="input">解析する文字列</param>
    /// <param name="month">解析結果の月数</param>
    /// <returns>解析が成功した場合は true、それ以外の場合は false</returns>
    private static bool TryParseMonthName(string input, out int month)
    {
        Span<char> buffer = stackalloc char[input.Length];
        var len = 0;

        foreach (var ch in input)
        {
            switch (ch)
            {
                case '.':
                    // ピリオド除外
                    continue;
                case >= 'Ａ' and <= 'Ｚ':
                    // 全角英字 → 半角英字小文字
                    buffer[len++] = (char)(ch - 'Ａ' + 'a');
                    break;
                case >= '０' and <= '９':
                    // 全角数字 → 半角数字
                    buffer[len++] = (char)(ch - '０' + '0');
                    break;
                case >= 'A' and <= 'Z':
                    // 半角英字 → 小文字
                    buffer[len++] = (char)(ch + 32);
                    break;
                default:
                    buffer[len++] = ch;
                    break;
            }
        }

        var normalized = buffer.Slice(0, len);

        month = normalized switch
        {
            "1" or "01" or "1月" or "一月" or "jan" or "january" or "睦月" or "むつき" or "ムツキ" => 1,
            "2" or "02" or "2月" or "二月" or "feb" or "february" or "如月" or "きさらぎ" or "キサラギ" => 2,
            "3" or "03" or "3月" or "三月" or "mar" or "march" or "弥生" or "やよい" or "ヤヨイ" => 3,
            "4" or "04" or "4月" or "四月" or "apr" or "april" or "卯月" or "うづき" or "ウヅキ" => 4,
            "5" or "05" or "5月" or "五月" or "may" or "皐月" or "さつき" or "サツキ" => 5,
            "6" or "06" or "6月" or "六月" or "jun" or "june" or "水無月" or "みなづき" or "ミナヅキ" => 6,
            "7" or "07" or "7月" or "七月" or "jul" or "july" or "文月" or "ふみづき" or "フミヅキ" => 7,
            "8" or "08" or "8月" or "八月" or "aug" or "august" or "葉月" or "はづき" or "ハヅキ" => 8,
            "9" or "09" or "9月" or "九月" or "sep" or "sept" or "september" or "長月" or "ながつき" or "ナガツキ" => 9,
            "10" or "10月" or "十月" or "oct" or "octo" or "october" or "神無月" or "かんなづき" or "カンナヅキ" => 10,
            "11" or "11月" or "十一月" or "nov" or "novem" or "november" or "霜月" or "しもつき" or "シモツキ" => 11,
            "12" or "12月" or "十二月" or "dec" or "decem" or "december" or "師走" or "しわす" or "シワス" => 12,
            _ => 0,
        };

        return month != 0;
    }

    #region 漢数字

    /// <summary>
    /// 漢数字を数値に変換するためのマッピングです。
    /// </summary>
    private static readonly Dictionary<char, int> s_kanjiNumericMap = new()
    {
        // 通常漢数字
        ['〇'] = 0,
        ['一'] = 1,
        ['二'] = 2,
        ['三'] = 3,
        ['四'] = 4,
        ['亖'] = 4,
        ['五'] = 5,
        ['六'] = 6,
        ['七'] = 7,
        ['八'] = 8,
        ['九'] = 9,

        // 大字（正式表記）
        ['零'] = 0,
        ['壱'] = 1,
        ['壹'] = 1,
        ['弌'] = 1,
        ['弐'] = 2,
        ['貳'] = 2,
        ['貮'] = 2,
        ['弍'] = 2,
        ['参'] = 3,
        ['參'] = 3,
        ['弎'] = 3,
        ['肆'] = 4,
        ['伍'] = 5,
        ['陸'] = 6,
        ['漆'] = 7,
        ['柒'] = 7,
        ['質'] = 7,
        ['捌'] = 8,
        ['玖'] = 9,
    };

    /// <summary>
    /// 漢数字の単位を数値に変換するためのマッピングです。
    /// </summary>
    private static readonly Dictionary<char, int> s_kanjiUnitMap = new()
    {
        ['十'] = 10,
        ['拾'] = 10,
        ['什'] = 10,
        ['廿'] = 20,
        ['卄'] = 20,
        ['丗'] = 30,
        ['卅'] = 30,
        ['卌'] = 40,
        ['百'] = 100,
        ['佰'] = 100,
        ['陌'] = 100,
        ['千'] = 1000,
        ['仟'] = 1000,
        ['阡'] = 1000,
        ['万'] = 10000,
        ['萬'] = 10000,
    };

    /// <summary>
    /// 漢数字の文字配列です。
    /// </summary>
    private static readonly char[] s_kanjiNumbers = s_kanjiNumericMap.Keys
        .Concat(s_kanjiUnitMap.Keys)
        .ToArray();

    /// <summary>
    /// 漢数字を検出するための正規表現です。
    /// </summary>
    private static readonly Regex s_kanjiNumberRegex = new(
        $"[{String.Concat(s_kanjiNumbers.Select(c => Regex.Escape(c.ToString())))}]+",
        RegexOptions.Compiled
    );

    /// <summary>
    /// 漢数字の文字列を数値に変換します。
    /// </summary>
    /// <param name="input">変換する漢数字の文字列</param>
    /// <returns>変換された数値。変換に失敗した場合は -1</returns>
    private static int ParseKanjiNumber(string input)
    {
        if (String.IsNullOrEmpty(input))
        {
            return -1;
        }

        // 単位付き変換（例：拾一 → 11、参拾 → 30）
        var total = 0;
        var current = 0;
        var hasUnit = false;

        foreach (var ch in input)
        {
            if (s_kanjiNumericMap.TryGetValue(ch, out var digit))
            {
                current = digit;
            }
            else if (s_kanjiUnitMap.TryGetValue(ch, out var unit))
            {
                if (unit >= 20 && current == 0)
                {
                    // 単体で20, 30, 40などの漢字（廿, 卅, 卌 など）
                    total += unit;
                }
                else
                {
                    if (current == 0) current = 1;
                    total += current * unit;
                }
                current = 0;
                hasUnit = true;
            }
            else
            {
                return -1;
            }
        }

        if (hasUnit)
        {
            return total + current;
        }

        // 単位が含まれていなければ、単なる連結数字として扱う
        Span<char> buffer = stackalloc char[input.Length];
        var i = 0;

        foreach (var ch in input)
        {
            if (s_kanjiNumericMap.TryGetValue(ch, out var digit))
            {
                buffer[i++] = (char)('0' + digit);
            }
            else
            {
                return -1;
            }
        }

        return Int32.TryParse(buffer[..i], out var result) ? result : -1;
    }

    /// <summary>
    /// 文字列内の複合漢数字を数値に置換します。
    /// </summary>
    /// <param name="input">置換対象の文字列</param>
    /// <returns>漢数字が数値に置換された文字列</returns>
    private static string ReplaceComplexKanjiNumbers(string input)
    {
        return s_kanjiNumberRegex.Replace(input, match =>
        {
            var value = ParseKanjiNumber(match.Value);
            return value >= 0 ? value.ToString() : match.Value;
        });
    }

    #endregion 漢数字

    /// <summary>
    /// 会計年度の月を解析して日付に変換します。
    /// </summary>
    /// <param name="s">解析する文字列</param>
    /// <param name="date">解析結果の日付</param>
    /// <returns>解析が成功した場合は true、それ以外の場合は false</returns>
    private static bool TryParseFiscalMonth(string s, out DateOnly date)
    {
        // 1-2桁の数字なら今年とみなす、月名も解釈する
        if (TryParseMonthName(s, out var month))
        {
            var fiscalYear = DateTime.Today.Year;

            var currentMonth = DateTime.Today.Month;
            //var currentMonth = 2;
            //var currentMonth = 4;

            // 4月以降に 1, 2, 3 が入力された場合は来年とする
            // 2025/4 に 1 が入力された場合は 2026/1 とする
            if (4 <= currentMonth && month <= 3)
            {
                fiscalYear++;
            }

            // 3月以前で今月以前の 1, 2, 3 が入力された場合は来年とする
            // 2025/2 に 1 が入力された場合は 2026/1 とする
            // 2025/2 に 3 が入力された場合は 2025/3 とする
            if (currentMonth <= 3 && month <= 3 && month < currentMonth)
            {
                fiscalYear++;
            }

            date = new DateOnly(fiscalYear, month, 1);
            return true;
        }

        date = DateOnly.MinValue;
        return false;
    }

    /// <summary>
    /// 日付文字列を解析して DateOnly 型に変換します。
    /// </summary>
    /// <param name="dateString">解析する日付文字列</param>
    /// <param name="date">解析結果の DateOnly 値</param>
    /// <returns>解析が成功した場合は true、それ以外の場合は false</returns>
    public static bool TryParseEx(string dateString, out DateOnly date)
    {
        if (String.IsNullOrWhiteSpace(dateString))
        {
            date = default;
            return false;
        }

        // 1-2桁の数字なら今年度とみなす
        if (TryParseFiscalMonth(dateString, out date))
        {
            return true;
        }

        // まずは想定フォーマットでパース
        var inv = CultureInfo.InvariantCulture;
        // DateOnly だとパースできないので、DateTime を使用
        if (DateTime.TryParseExact(dateString, s_formats, inv, s_styles, out var dateTime))
        {
            date = DateOnly.FromDateTime(dateTime);
            return true;
        }

        // 全角→半角など
        dateString = dateString.Normalize(NormalizationForm.FormKC);

        // 漢数字が含まれる場合は変換する
        if (dateString.IndexOfAny(s_kanjiNumbers) >= 0)
        {
            // 複合漢数字（拾→10）を数値化
            dateString = ReplaceComplexKanjiNumbers(dateString);
        }

        // その後和暦フォーマットでパース
        // DateOnly だとパースできないので、DateTime を使用
        if (DateTime.TryParse(dateString, s_jaCulture.Value, s_styles, out dateTime))
        {
            date = DateOnly.FromDateTime(dateTime);
            return true;
        }

        date = DateOnly.MinValue;
        return false;
    }

    /// <summary>
    /// 今日の日付を DateOnly 型で取得します。
    /// </summary>
    /// <returns>今日の日付</returns>
    public static DateOnly GetToday()
    {
        return DateOnly.FromDateTime(DateTime.Today);
    }

    /// <summary>
    /// 今月の初日（1日）を DateOnly 型で取得します。
    /// </summary>
    /// <returns>今月の初日</returns>
    public static DateOnly GetFirstDate()
    {
        var today = DateTime.Today;
        return new DateOnly(today.Year, today.Month, 1);
    }

    /// <summary>
    /// 今月の初日（1日）を DateTime 型で取得します。
    /// </summary>
    /// <returns>今月の初日</returns>
    public static DateTime GetFirstDateTime()
    {
        var today = DateTime.Today;
        return new DateTime(today.Year, today.Month, 1);
    }

    /// <summary>
    /// 指定された年月の初日（1日）を DateOnly 型で取得します。
    /// </summary>
    /// <param name="year">年</param>
    /// <param name="month">月</param>
    /// <returns>指定された年月の初日</returns>
    public static DateOnly GetFirstDate(int year, int month)
    {
        return new DateOnly(year, month, 1);
    }

    /// <summary>
    /// 指定された年月の初日（1日）を DateTime 型で取得します。
    /// </summary>
    /// <param name="year">年</param>
    /// <param name="month">月</param>
    /// <returns>指定された年月の初日</returns>
    public static DateTime GetFirstDateTime(int year, int month)
    {
        return new DateTime(year, month, 1);
    }

    /// <summary>
    /// 指定された日付の月末日を DateOnly 型で取得します。
    /// </summary>
    /// <param name="date">基準となる日付</param>
    /// <returns>指定された日付の月末日</returns>
    public static DateOnly GetEndDate(DateOnly date)
    {
        return new DateOnly(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
    }

    /// <summary>
    /// 指定された日付の月末日を DateTime 型で取得します。
    /// </summary>
    /// <param name="date">基準となる日付</param>
    /// <returns>指定された日付の月末日</returns>
    public static DateTime GetEndDateTime(DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
    }
}
