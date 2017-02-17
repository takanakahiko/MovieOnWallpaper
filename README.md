# MovieOnWallpaper

## これはなに
壁紙を動画にするやつです

## コンパイル
` C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /t:winexe program.cs WinAPI.cs /r:WPF\WindowsFormsIntegration.dll /r:WPF\PresentationFramework.dll /r:WPF\PresentationCore.dll /r:System.Xaml.dll /r:WPF\WindowsBase.dll /r:Microsoft.VisualBasic.dll`

## 操作方法

### 起動方法
1. 上記コマンドでコンパイル
2. Program.exeを実行
3. 少し待つとファイル選択画面が表示されるので動画を選ぶ

### その他の操作
タスクトレイにあるアイコンを右クリック後に

- Exitを選択で終了
- MuteAudioを選択で音声ミュート
- LoadVideoを選択で動画を再選択
- [NEW!] LoadURLを選択でWEBページを表示 -> [recommend](https://earth.nullschool.net/jp/#current/wind/surface/level/orthographic=139.26,31.86,706)

## 問題点改善点
- アイコンを専用のやつを作りたい
- 音量を調節するパネルがほしい
- webBrowserコンポーネントにmouseMoveイベントを送りたい(マウスの動きに反応するWebページへの対応)

## 意見要望
http://twitter.com/takanakahiko まで

## プロジェクトサイト(?)
http://takanakahiko.me/MovieOnWallpaper/
