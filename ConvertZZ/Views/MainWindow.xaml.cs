﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ConvertZZ.Models;

using static ConvertZZ.Views.Pages.AudioTagsConverView;
using static Fanhuaji_API.Fanhuaji;

namespace ConvertZZ.Views {
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window {
        List<Models.HotKey> hotKeys = new List<Models.HotKey>();
        CancellationTokenSource Cancellation = new CancellationTokenSource();
        public MainWindow() {
            InitializeComponent();
            App.nIcon.MouseClick += NIcon_MouseClick;
            if (0 < App.Settings.PositionX && App.Settings.PositionX < SystemParameters.WorkArea.Width) {
                Left = App.Settings.PositionX;
            }

            if (0 < App.Settings.PositionY && App.Settings.PositionY < SystemParameters.WorkArea.Height) {
                Top = App.Settings.PositionY;
            }

            RegAllHotkey();
            ServerThread();
        }
        private async void ServerThread() {
            while (!Cancellation.IsCancellationRequested) {
                try {
                    using (NamedPipeServerStream pipeServer = new NamedPipeServerStream("ConvertZZ_Pipe", PipeDirection.InOut)) {
                        await pipeServer.WaitForConnectionAsync(Cancellation.Token);
                        Console.WriteLine("Client connected.");
                        StreamString ss = new StreamString(pipeServer);
                        string[] Args = (await ss.ReadStringAsync()).Split('|');

                        DialogHostView DialogHostView = new DialogHostView(Args[0] == "/file" ? Enums.Enum_Mode.Mode.File_FileName : Enums.Enum_Mode.Mode.AutioTag, Args.Skip(1).ToArray());
                        DialogHostView.Show();

                        await ss.WriteStringAsync("ACK");
                        pipeServer.WaitForPipeDrain();
                        pipeServer.Close();
                    }
                } catch (IOException e) {
                    Console.WriteLine("ERROR: {0}", e.Message);
                }
            }
        }
        private void NIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) {
            if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                ContextMenu NotifyIconMenu = (ContextMenu)FindResource("NotifyIconMenu");
                NotifyIconMenu.IsOpen = true;
            }
        }
        void HotkeyAction1(Models.HotKey hotKey) {
            if (App.Settings.HotKey.AutoCopy) {
                ClipBoardHelper.Copy(hotKey.Key, hotKey.KeyModifiers);
            }

            MenuItem_Click(new MenuItem { Uid = App.Settings.HotKey.Feature1.Action, Visibility = Visibility.Hidden }, null);
            if (App.Settings.HotKey.AutoPaste) {
                ClipBoardHelper.Paste();
            }
        }
        void HotkeyAction2(Models.HotKey hotKey) {
            if (App.Settings.HotKey.AutoCopy) {
                ClipBoardHelper.Copy(hotKey.Key, hotKey.KeyModifiers);
            }

            MenuItem_Click(new MenuItem { Uid = App.Settings.HotKey.Feature2.Action, Visibility = Visibility.Hidden }, null);
            if (App.Settings.HotKey.AutoPaste) {
                ClipBoardHelper.Paste();
            }
        }
        void HotkeyAction3(Models.HotKey hotKey) {
            if (App.Settings.HotKey.AutoCopy) {
                ClipBoardHelper.Copy(hotKey.Key, hotKey.KeyModifiers);
            }

            MenuItem_Click(new MenuItem { Uid = App.Settings.HotKey.Feature3.Action, Visibility = Visibility.Hidden }, null);
            if (App.Settings.HotKey.AutoPaste) {
                ClipBoardHelper.Paste();
            }
        }
        void HotkeyAction4(Models.HotKey hotKey) {
            if (App.Settings.HotKey.AutoCopy) {
                ClipBoardHelper.Copy(hotKey.Key, hotKey.KeyModifiers);
            }

            MenuItem_Click(new MenuItem { Uid = App.Settings.HotKey.Feature4.Action, Visibility = Visibility.Hidden }, null);
            if (App.Settings.HotKey.AutoPaste) {
                ClipBoardHelper.Paste();
            }
        }
        private void RegHotkey(Feature feature, Action<Models.HotKey> action) {
            if (!feature.Enable) {
                return;
            }

            KeyModifier keyModifier = KeyModifier.None;
            feature.Modift.Split(',').ToList().ForEach(x => keyModifier = keyModifier | (KeyModifier)Enum.Parse(typeof(KeyModifier), x.Trim()));
            hotKeys.Add(new Models.HotKey((Key)Enum.Parse(typeof(Key), feature.Key), keyModifier, action));
        }
        public void RegAllHotkey() {
            RegHotkey(App.Settings.HotKey.Feature1, HotkeyAction1);
            RegHotkey(App.Settings.HotKey.Feature2, HotkeyAction2);
            RegHotkey(App.Settings.HotKey.Feature3, HotkeyAction3);
            RegHotkey(App.Settings.HotKey.Feature4, HotkeyAction4);
        }
        public void UnRegAllHotkey() {
            hotKeys.ForEach(x => x.Dispose());
            hotKeys.Clear();
        }

        Point pointNow = new Point();
        bool leftDown = false;
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                pointNow = new Point(Left, Top);
                leftDown = true;
                DragMove();
            }
        }
        private void About_Click(object sender, RoutedEventArgs e) {
            AboutView AboutView = new AboutView();
            AboutView.ShowDialog();
        }
        private void Report_Click(object sender, RoutedEventArgs e) {
            ReportView ReportView = new ReportView();
            ReportView.ShowDialog();
        }
        private void Setting_Click(object sender, RoutedEventArgs e) {
            UnRegAllHotkey();
            Topmost = false;
            SettingsView SettingsView = new SettingsView() { Owner = this };
            SettingsView.ShowDialog();
            Topmost = true;
            RegAllHotkey();
        }
        private void Exit_Click(object sender, RoutedEventArgs e) {
            Close();
            App.nIcon.Visible = false;
            App.nIcon.Dispose();
            Environment.Exit(0);
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e) {
            ContextMenu NotifyIconMenu = (ContextMenu)FindResource("NotifyIconMenu");
            e.Handled = true;
            switch (e.ChangedButton) {
                case MouseButton.Left: {
                    if (Left == pointNow.X && Top == pointNow.Y && leftDown) {
                        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                            MenuItem_Click(GetByUid(NotifyIconMenu, App.Settings.QuickStart.LeftClick_Ctrl), null);
                        } else if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) {
                            MenuItem_Click(GetByUid(NotifyIconMenu, App.Settings.QuickStart.LeftClick_Alt), null);
                        } else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
                            MenuItem_Click(GetByUid(NotifyIconMenu, App.Settings.QuickStart.LeftClick_Shift), null);
                        } else {
                            e.Handled = false;
                        }
                    } else {
                        e.Handled = false;
                    }
                }
                break;
                case MouseButton.Right: {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
                        MenuItem_Click(GetByUid(NotifyIconMenu, App.Settings.QuickStart.RightClick_Ctrl), null);
                    } else if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt)) {
                        MenuItem_Click(GetByUid(NotifyIconMenu, App.Settings.QuickStart.RightClick_Alt), null);
                    } else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) {
                        MenuItem_Click(GetByUid(NotifyIconMenu, App.Settings.QuickStart.RightClick_Shift), null);
                    } else {
                        e.Handled = false;
                    }
                }
                break;
                default:
                    e.Handled = false;
                    break;
            }
            leftDown = false;
        }
        DragDropKeyStates dragDropKeyStates;
        private void Window_DragEnter(object sender, DragEventArgs e) {
            //紀錄拖曳進來時的按鍵
            dragDropKeyStates = e.KeyStates;
        }
        private void Window_Drop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                /*
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                //減去輔助鍵，得到現在是左鍵還是右鍵
                dragDropKeyStates -= e.KeyStates;
                Pages.FileConverView FileConverView = new Pages.FileConverView();
                FileConverView.Button_Convert_Click(null, null);
                if (dragDropKeyStates == DragDropKeyStates.LeftMouseButton)
                {
                    switch (e.KeyStates)
                    {
                        case DragDropKeyStates.ControlKey:
                            MenuItem_Click(new MenuItem { Uid = App.Settings.QuickStart.LeftDrop_Ctrl }, null);
                            break;
                        case DragDropKeyStates.ShiftKey:
                            MenuItem_Click(new MenuItem { Uid = App.Settings.QuickStart.LeftDrop_Shift }, null);
                            break;
                        case DragDropKeyStates.AltKey:
                            MenuItem_Click(new MenuItem { Uid = App.Settings.QuickStart.LeftDrop_Alt }, null);
                            break;
                    }
                }
                else if (dragDropKeyStates == DragDropKeyStates.RightMouseButton)
                {
                    switch (e.KeyStates)
                    {
                        case DragDropKeyStates.ControlKey:
                            MenuItem_Click(new MenuItem { Uid = App.Settings.QuickStart.RightDrop_Ctrl }, null);
                            break;
                        case DragDropKeyStates.ShiftKey:
                            MenuItem_Click(new MenuItem { Uid = App.Settings.QuickStart.RightDrop_Shift }, null);
                            break;
                        case DragDropKeyStates.AltKey:
                            MenuItem_Click(new MenuItem { Uid = App.Settings.QuickStart.RightDrop_Alt }, null);
                            break;
                    }
                }*/
            } else if (e.Data.GetDataPresent(DataFormats.UnicodeText)) {
                dragDropKeyStates -= e.KeyStates;
                string s = (string)e.Data.GetData(DataFormats.UnicodeText);
                if (dragDropKeyStates == DragDropKeyStates.LeftMouseButton) {
                    switch (e.KeyStates) {
                        case DragDropKeyStates.ControlKey:
                            MenuItem_Click(new MenuItem { Uid = App.Settings.QuickStart.LeftDrop_Ctrl, ToolTip = s }, null);
                            break;
                        case DragDropKeyStates.ShiftKey:
                            MenuItem_Click(new MenuItem { Uid = App.Settings.QuickStart.LeftDrop_Shift, ToolTip = s }, null);
                            break;
                        case DragDropKeyStates.AltKey:
                            MenuItem_Click(new MenuItem { Uid = App.Settings.QuickStart.LeftDrop_Alt, ToolTip = s }, null);
                            break;
                    }
                } else if (dragDropKeyStates == DragDropKeyStates.RightMouseButton) {
                    switch (e.KeyStates) {
                        case DragDropKeyStates.ControlKey:
                            MenuItem_Click(new MenuItem { Uid = App.Settings.QuickStart.RightDrop_Ctrl, ToolTip = s }, null);
                            break;
                        case DragDropKeyStates.ShiftKey:
                            MenuItem_Click(new MenuItem { Uid = App.Settings.QuickStart.RightDrop_Shift, ToolTip = s }, null);
                            break;
                        case DragDropKeyStates.AltKey:
                            MenuItem_Click(new MenuItem { Uid = App.Settings.QuickStart.RightDrop_Alt, ToolTip = s }, null);
                            break;
                    }
                }
            } else {
                var g = e.Data.GetFormats(true);
                foreach (var h in g) {
                    object ss = e.Data.GetData(h);
                    if (ss != null) {

                    }
                }
                string s = (string)e.Data.GetData(DataFormats.EnhancedMetafile);
            }
        }
        private async void MenuItem_Click(object sender, RoutedEventArgs e) {
            if (sender == null) {
                return;
            }

            if (((MenuItem)sender).Uid == null) {
                return;
            }

            string clip = ClipBoardHelper.GetClipBoard_UnicodeText();
            if (!string.IsNullOrWhiteSpace((string)(((MenuItem)sender).ToolTip))) {
                clip = (string)(((MenuItem)sender).ToolTip);
            }
            StringBuilder sb = new StringBuilder();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Restart();
            try {
                switch (((MenuItem)sender).Uid) {
                    case "1":
                        Visibility = Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                        break;
                    case "a1":
                        if (App.Settings.RecognitionEncoding) {
                            int encodingtype = EncodingAnalyzer.Analyze(clip);
                            if (encodingtype == 0 || encodingtype == 1) {
                                if (MessageBox.Show(this, "編碼似乎已是Big5，繼續轉換?", "警告", MessageBoxButton.YesNo) == MessageBoxResult.No) {
                                    return;
                                }
                            }
                        }
                        clip = await ConvertHelper.ConvertAsync(clip, new Encoding[2] { Encoding.GetEncoding("GBK"), Encoding.GetEncoding("BIG5") }, 1);
                        break;
                    case "a2":
                        if (App.Settings.RecognitionEncoding) {
                            int encodingtype = EncodingAnalyzer.Analyze(clip);
                            if (encodingtype == 2 || encodingtype == 3) {
                                if (MessageBox.Show(this, "編碼似乎已是GBK，繼續轉換?", "警告", MessageBoxButton.YesNo) == MessageBoxResult.No) {
                                    return;
                                }
                            }
                        }
                        clip = await ConvertHelper.ConvertAsync(clip, new Encoding[] { Encoding.GetEncoding("BIG5"), Encoding.GetEncoding("GBK") }, 2);
                        break;
                    case "a3":
                        clip = await ConvertHelper.ConvertAsync(clip, 1);
                        break;
                    case "a4":
                        clip = await ConvertHelper.ConvertAsync(clip, 2);
                        break;
                    case "b1":
                        DialogHostView window_File_FileNameConverter = new DialogHostView(Enums.Enum_Mode.Mode.File_FileName);
                        window_File_FileNameConverter.Show();
                        break;
                    case "b2":
                        DialogHostView window_ClipBoard_Converter = new DialogHostView(Enums.Enum_Mode.Mode.ClipBoard);
                        window_ClipBoard_Converter.Show();
                        break;
                    case "c1":
                        DialogHostView DialogHostView = new DialogHostView(Enums.Enum_Mode.Mode.AutioTag, Format.ID3);
                        DialogHostView.Show();
                        break;
                    case "c2":
                        DialogHostView DialogHostView2 = new DialogHostView(Enums.Enum_Mode.Mode.AutioTag, Format.APE);
                        DialogHostView2.Show();
                        break;
                    case "c3":
                        DialogHostView DialogHostView3 = new DialogHostView(Enums.Enum_Mode.Mode.AutioTag, Format.OGG);
                        DialogHostView3.Show();
                        break;
                    case "za1":
                        foreach (char c in clip) {
                            if ((' ' <= c && c <= '~') || (c == '\r') || (c == '\n')) {
                                if (c == '&') {
                                    sb.Append("&amp;");
                                } else if (c == '<') {
                                    sb.Append("&lt;");
                                } else if (c == '>') {
                                    sb.Append("&gt;");
                                } else {
                                    sb.Append(c.ToString());
                                }
                            } else {
                                sb.Append("&#");
                                sb.Append(Convert.ToInt32(c));
                                sb.Append(";");
                            }
                        }
                        clip = sb.ToString();
                        break;
                    case "za2":
                        foreach (char c in clip) {
                            if ((' ' <= c && c <= '~') || (c == '\r') || (c == '\n')) {
                                if (c == '&') {
                                    sb.Append("&amp;");
                                } else if (c == '<') {
                                    sb.Append("&lt;");
                                } else if (c == '>') {
                                    sb.Append("&gt;");
                                } else {
                                    sb.Append(c.ToString());
                                }
                            } else {
                                sb.Append("&#x");
                                sb.Append(Convert.ToInt32(c).ToString("X"));
                                sb.Append(";");
                            }
                        }
                        clip = sb.ToString();
                        break;
                    case "za3":
                        clip.Replace("&amp;", "&");
                        clip.Replace("&lt;", "<");
                        clip.Replace("&gt;", ">");
                        //以;將文字拆成陣列
                        string[] tmp = clip.Split(';');
                        //檢查最後一個字元是否為【;】，因為有【英文】、【阿拉伯數字】、【&#XXXX;】
                        //若最後一個要處理的字並非HTML UNICODE則不進行處理
                        bool Process_last = clip.Substring(clip.Length - 1, 1).Equals(";");
                        //Debug.WriteLine(tmp.Length + "");
                        for (int i = 0; i < tmp.Length; i++) {
                            //以&#將文字拆成陣列
                            string[] tmp2 = tmp[i].Split(new string[] { "&#" }, StringSplitOptions.RemoveEmptyEntries);
                            if (tmp2.Length == 1) {
                                //如果長度為1則試圖轉換UNICODE回字符，若失敗則使用原本的字元
                                if (i != tmp.Length - 1) {
                                    try {
                                        if (tmp2[0].StartsWith("x")) {
                                            sb.Append(Convert.ToChar(Convert.ToInt32(tmp2[0].Substring(1, tmp2[0].Length - 1), 16)).ToString());
                                        } else {
                                            sb.Append(Convert.ToChar(Convert.ToInt32(int.Parse(tmp2[0]))).ToString());
                                        }
                                    } catch {
                                        sb.Append(tmp2[0]);
                                    }
                                } else {
                                    sb.Append(tmp2[0]);
                                }
                            }
                            if (tmp2.Length == 2) {
                                //若長度為2，則第一項不處理，只處理第二項即可
                                sb.Append(tmp2[0]);
                                var g = Convert.ToInt32(tmp2[1].Substring(1, tmp2[1].Length - 1), 16);
                                if (tmp2[1].StartsWith("x")) {
                                    sb.Append(Convert.ToChar(Convert.ToInt32(tmp2[1].Substring(1, tmp2[1].Length - 1), 16)).ToString());
                                } else {
                                    sb.Append(Convert.ToChar(Convert.ToInt32(tmp2[1])).ToString());
                                }
                            }
                        }
                        clip = sb.ToString();
                        break;
                    case "zb1":
                        //Unicode>GBK
                        clip = Encoding.Default.GetString(Encoding.GetEncoding("GBK").GetBytes(clip));
                        break;
                    case "zb2":
                        clip = Encoding.Default.GetString(Encoding.GetEncoding("BIG5").GetBytes(clip));
                        break;
                    case "zb3":
                        clip = Encoding.Default.GetString(Encoding.GetEncoding("Shift-JIS").GetBytes(clip));
                        break;
                    case "zb4":
                        //GBK>Unicode
                        clip = Encoding.GetEncoding("GBK").GetString(Encoding.Default.GetBytes(clip));
                        break;
                    case "zb5":
                        clip = Encoding.GetEncoding("BIG5").GetString(Encoding.Default.GetBytes(clip));
                        break;
                    case "zb6":
                        clip = Encoding.GetEncoding("Shift-JIS").GetString(Encoding.Default.GetBytes(clip));
                        break;
                    case "zc1":
                        //Shift-JIS>GBK           
                        clip = Encoding.GetEncoding("shift_jis").GetString(Encoding.GetEncoding("GBK").GetBytes(clip));
                        break;
                    case "zc2":
                        clip = Encoding.GetEncoding("shift_jis").GetString(Encoding.GetEncoding("BIG5").GetBytes(clip));
                        break;
                    case "zc3":
                        clip = Encoding.GetEncoding("GBK").GetString(Encoding.GetEncoding("shift_jis").GetBytes(clip));
                        break;
                    case "zc4":
                        clip = Encoding.GetEncoding("BIG5").GetString(Encoding.GetEncoding("shift_jis").GetBytes(clip));
                        break;
                    case "zd1":
                        //hz-gb-2312>GBK
                        clip = Encoding.GetEncoding("hz-gb-2312").GetString(Encoding.GetEncoding("GBK").GetBytes(clip));
                        break;
                    case "zd2":
                        clip = Encoding.GetEncoding("hz-gb-2312").GetString(Encoding.GetEncoding("BIG5").GetBytes(clip));
                        break;
                    case "zd3":
                        clip = Encoding.GetEncoding("GBK").GetString(Encoding.GetEncoding("hz-gb-2312").GetBytes(clip));
                        break;
                    case "zd4":
                        clip = Encoding.GetEncoding("BIG5").GetString(Encoding.GetEncoding("hz-gb-2312").GetBytes(clip));
                        break;
                    case "ze1":
                        clip = ConvertHelper.ConvertSymbol(clip, 0);
                        break;
                    case "ze2":
                        clip = ConvertHelper.ConvertSymbol(clip, 1);
                        break;
                }
                ClipBoardHelper.SetClipBoard_UnicodeText(clip);
                sw.Stop();
                //顯示提示
                switch (((MenuItem)sender).Uid) {
                    case "1":
                    case "b1":
                    case "b2":
                    case "c1":
                    case "c2":
                    case "c3":
                        break;
                    default:
                        if (App.Settings.Prompt && !(((MenuItem)sender).Visibility == Visibility.Hidden && (App.Settings.HotKey.AutoCopy || App.Settings.HotKey.AutoPaste))) {
                            ContextMenu NotifyIconMenu = (ContextMenu)FindResource("NotifyIconMenu");
                            string ItemInfo = ((MenuItem)GetByUid(NotifyIconMenu, ((MenuItem)sender).Uid)).Header.ToString();
                            new Toast(string.Format("轉換完成\r\n耗時：{0} ms", sw.ElapsedMilliseconds)).Show();
                        }
                        break;
                }
            } catch (FanhuajiException fe) {
                Window_MessageBoxEx.ShowDialog(fe.Message, "繁化姬API出現錯誤", "確定");
            }
        }

        public static UIElement GetByUid(DependencyObject rootElement, string uid) {
            foreach (UIElement element in LogicalTreeHelper.GetChildren(rootElement).OfType<UIElement>()) {
                if (element as MenuItem != null) {
                    if ((element as MenuItem).Items.Count > 0) {
                        UIElement resoult = GetByUid(element, uid);
                        if (resoult != null) {
                            return resoult;
                        }
                    }
                }

                if (element.Uid == uid) {
                    return element;
                }

                UIElement resultChildren = GetByUid(element, uid);
                if (resultChildren != null) {
                    return resultChildren;
                }
            }
            return null;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Cancellation.Cancel();
            App.Settings.PositionX = Left;
            App.Settings.PositionY = Top;
            App.Save();
            UnRegAllHotkey();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            if (!App.Settings.AssistiveTouch) {
                Visibility = Visibility.Hidden;
            }
        }
    }

}
