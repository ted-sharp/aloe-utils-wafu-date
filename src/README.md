# Aloe.Utils.Wafu.Date

和風日付文字列を扱うためのユーティリティライブラリです。

## 主な機能

* 和風日付文字列のパース
* 漢数字を含む日付文字列のサポート
* 会計年度の月の解析
* 日付の拡張メソッド
* 時間間隔の日本語表示

## 対応環境

* .NET 9以降
* AOT互換
* トリミング対応

## インストール

NuGetパッケージマネージャーを使用する場合：

```cmd
Install-Package Aloe.Utils.Wafu.Date
```

.NET CLIを使用する場合：

```cmd
dotnet add package Aloe.Utils.Wafu.Date
```

## 使用例

```csharp
using Aloe.Utils.Wafu.Date;

// 和風日付文字列のパース
if (DateHelper.TryParseEx("令和5年4月1日", out var date))
{
    Console.WriteLine(date); // 2023-04-01
}

// 漢数字を含む日付文字列のパース
if (DateHelper.TryParseEx("令和五年四月一日", out var date2))
{
    Console.WriteLine(date2); // 2023-04-01
}

// 会計年度の月の解析
if (DateHelper.TryParseEx("4月", out var date3))
{
    Console.WriteLine(date3); // 2024-04-01
}

// 時間間隔の日本語表示
var span = new TimeSpan(1, 2, 3, 4);
Console.WriteLine(span.ToJaString()); // 1日 2時間 3分 4秒
Console.WriteLine(span.ToApproximateJaString()); // 約1日
```

## ライセンス

MIT License
