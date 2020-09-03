using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Miku.LanguageEditorNew.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            //this.dev;
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
