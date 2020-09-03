using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Miku.LanguageEditorNew.Views
{
    public class DevInsertWindow : Window
    {
        public DevInsertWindow()
        {
            this.InitializeComponent();
#if DEBUG
            //this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
