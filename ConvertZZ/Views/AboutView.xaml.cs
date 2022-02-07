using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ConvertZZ.Views {
    /// <summary>
    /// AboutView.xaml 的互動邏輯
    /// </summary>
    public partial class AboutView : Window {
        public AboutView() {
            InitializeComponent();
            Version.Content = "v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e) {
            Process.Start("https://github.com/flier268/ConvertZZ");
        }
    }
}
