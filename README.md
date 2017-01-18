# MovieOnWallpaper

## これはなに
壁紙を動画にするやつです

## コンパイル
`C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /t
:winexe program.cs /r:WPF\WindowsFormsIntegration.dll /r:WPF\PresentationFramework.dll /r:WPF\PresentationCore.dll /r:Sy
stem.Xaml.dll /r:WPF\WindowsBase.dll`

## 操作方法

### 起動方法
1. 上記コマンドでコンパイル
2. Program.exeを実行
3. 少し待つとファイル選択画面が表示されるので動画を選ぶ

### その他の操作
タスクトレイにあるアイコンを右クリック後に
* Exitを選択で終了
* MuteAudioを選択で音声ミュート
* LoadVideoを選択で動画を再選択

## 問題点改善点
* ~~いずれはコンソール消したい~~
* ***後輩のPC(Windows7)で起動しなかった***
* アイコンも専用のやつを作りたい
* ~~動画選択をし直す処理を追加したい~~
* 音量を調節するパネルがほしい
* 動画以外のファイルを選択or何も選択しなかった場合の処理を追加すべき

## 意見要望
http://twitter.com/takanakahiko まで

## プロジェクトサイト(?)
http://takanakahiko.me/MovieOnWallpaper/
