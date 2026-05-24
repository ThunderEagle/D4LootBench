using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using D4Loot.Core.Models;
using D4Loot.Core.Serialization;
using System.Text.Json;

namespace D4Loot.App.ViewModels;

public partial class RawEditorViewModel : ObservableObject
{
    private readonly Action<FilterRuleset> _onApply;

    [ObservableProperty]
    private string _jsonText;

    [ObservableProperty]
    private string _statusMessage = "Edit JSON directly, then click Apply to update the visual editor.";

    [ObservableProperty]
    private bool _hasError;

    public RawEditorViewModel(string initialJson, Action<FilterRuleset> onApply)
    {
        _jsonText = initialJson;
        _onApply  = onApply;
    }

    [RelayCommand]
    private void Apply()
    {
        if (string.IsNullOrWhiteSpace(JsonText))
        {
            SetStatus("Nothing to apply.", error: true);
            return;
        }
        try
        {
            var ruleset = JsonSerializer.Deserialize<FilterRuleset>(JsonText, FilterJsonOptions.Default)
                          ?? throw new InvalidOperationException("Deserialized to null.");
            _onApply(ruleset);
            SetStatus("Applied to visual editor.", error: false);
        }
        catch (Exception ex)
        {
            SetStatus($"Parse error: {ex.Message}", error: true);
        }
    }

    private void SetStatus(string message, bool error)
    {
        StatusMessage = message;
        HasError      = error;
    }
}
