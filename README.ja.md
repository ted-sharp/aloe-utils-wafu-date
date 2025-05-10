# Aloe.Utils.Wafu.Date

[![English](https://img.shields.io/badge/Language-English-blue)](./README.md)
[![日本語](https://img.shields.io/badge/言語-日本語-blue)](./README.ja.md)

[![NuGet Version](https://img.shields.io/nuget/v/Aloe.Utils.Wafu.Date.svg)](https://www.nuget.org/packages/Aloe.Utils.Wafu.Date)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Aloe.Utils.Wafu.Date.svg)](https://www.nuget.org/packages/Aloe.Utils.Wafu.Date)
[![License](https://img.shields.io/github/license/ted-sharp/aloe-utils-wafu-date.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)

`Aloe.Utils.Wafu.Date` は、和風な日付処理をサポートするための軽量ユーティリティです。

## 主な機能

* 和暦（明治、大正、昭和、平成、令和）のサポート
* 和暦と西暦の相互変換
* 和暦の年号と年の取得
* 和暦の日付文字列のフォーマット

## 対応環境

* .NET 9 以降

## Install

NuGet パッケージマネージャーからインストールします：

```cmd
Install-Package Aloe.Utils.Wafu.Date
```

あるいは、.NET CLI で：

```cmd
dotnet add package Aloe.Utils.Wafu.Date
```

## Usage

```csharp
using Aloe.Utils.Wafu.Date;

// 和暦の取得
var date = new DateTime(2024, 3, 15);
var japaneseEra = date.GetJapaneseEra(); // "令和"
var japaneseYear = date.GetJapaneseYear(); // 6

// 和暦の日付文字列
var formattedDate = date.ToJapaneseDateString(); // "令和6年3月15日"
```

## ライセンス

MIT License

## 貢献

バグ報告や機能要望は、GitHub Issues でお願いします。プルリクエストも歓迎します。

