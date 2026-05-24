using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using D4Loot.App.ViewModels;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Search;

namespace D4Loot.App.Views;

public partial class JsonEditorView : UserControl
{
    private bool _editorChanging;
    private readonly FoldingManager _foldingManager;

    public JsonEditorView()
    {
        InitializeComponent();

        SearchPanel.Install(Editor);

        _foldingManager = FoldingManager.Install(Editor.TextArea);

        DataContextChanged += OnDataContextChanged;
        Editor.TextChanged += OnEditorTextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is JsonEditorViewModel old)
            old.PropertyChanged -= OnViewModelPropertyChanged;

        if (e.NewValue is JsonEditorViewModel vm)
            vm.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(JsonEditorViewModel.JsonText)) return;
        if (_editorChanging) return;

        var vm = (JsonEditorViewModel)sender!;
        Editor.Text = vm.JsonText;
        JsonFoldingStrategy.UpdateFoldings(_foldingManager, Editor.Document);
    }

    private void OnEditorTextChanged(object? sender, EventArgs e)
    {
        if (_editorChanging) return;
        _editorChanging = true;
        if (DataContext is JsonEditorViewModel vm)
            vm.JsonText = Editor.Text;
        _editorChanging = false;

        JsonFoldingStrategy.UpdateFoldings(_foldingManager, Editor.Document);
    }
}

/// <summary>Folding strategy for brace/bracket pairs — covers JSON objects and arrays.</summary>
internal sealed class JsonFoldingStrategy
{
    public static void UpdateFoldings(FoldingManager manager, TextDocument document)
        => manager.UpdateFoldings(CreateFoldings(document.Text), -1);

    private static List<NewFolding> CreateFoldings(string text)
    {
        var result = new List<NewFolding>();
        var stack = new Stack<int>();

        for (var i = 0; i < text.Length; i++)
        {
            switch (text[i])
            {
                case '{' or '[':
                    stack.Push(i);
                    break;
                case '}' or ']' when stack.Count > 0:
                    var start = stack.Pop();
                    if (i > start + 1)
                        result.Add(new NewFolding(start, i + 1));
                    break;
            }
        }

        result.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
        return result;
    }
}

public sealed class BoolToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is true ? Brushes.Red : SystemColors.ControlTextBrush;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
