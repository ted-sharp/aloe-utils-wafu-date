# Aloe.Utils.Wafu.Date

和風な日付処理をサポートするための軽量ユーティリティです。

## 主な機能

* 和風日付文字列のパース
* 漢数字を含む日付文字列のサポート
* 会計年度の月の解析
* 日付の拡張メソッド
* 時間間隔の日本語表示

## 対応環境

* .NET 9 以降
* AOT互換
* トリミング対応

## インストール

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

// 時間間隔の日本語表示
var span = new TimeSpan(1, 2, 3, 4);
Console.WriteLine(span.ToJaString()); // 1日 2時間 3分 4秒
```

## ライセンス

MIT License
