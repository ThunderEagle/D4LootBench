using D4Loot.App.ViewModels;

namespace D4Loot.App;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new JsonEditorViewModel();
    }
}
