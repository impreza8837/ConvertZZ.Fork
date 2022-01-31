===== Fork flier268's ConvertZZ =====

# ConvertZZ

## 前言
經常下載簡體文件的朋友們應該都有這個困擾吧，文件下載下來，常常看到的都是一堆亂碼
亂碼的原因是因為兩國的慣用編碼不同，簡體使用GBK，而繁體使用BIG5，當GBK的文字「直接」看做BIG5時，就會出現亂碼

一直以來，我慣用的簡繁轉換程式'ConvertZ'，但一直不太喜歡它的設計，為什麼上面總是要有一條Bar在那邊?為什麼開啟要等好幾秒?為什麼它有時候還會轉換失敗?為什麼作者好像都沒再更新了?
於是我開始計畫，意圖取代舊有的ConvertZ，改善這些問題。

由於是繼承自ConvertZ，因此取名ConvertZZ

## Feature
* 仿造自ConvertZ，因此功能近似ConvertZ，修正ConvertZ的部分BUG
* 具有懸浮球，使用自定的手勢，使用各種快捷功能
* 漂亮的UI
* ID3 tag轉換

## Require
* .Net 4.7.2以上
