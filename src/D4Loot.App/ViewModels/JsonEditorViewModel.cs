using System.Text.Json;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using D4Loot.Core.Codec;
using D4Loot.Core.Models;
using D4Loot.Core.Serialization;

namespace D4Loot.App.ViewModels;

public partial class JsonEditorViewModel : ObservableObject
{
    [ObservableProperty] private string _jsonText = string.Empty;
    [ObservableProperty] private string _statusMessage = "Import a filter code to get started.";
    [ObservableProperty] private bool _hasError;

    [RelayCommand]
    private void PasteCode()
    {
        var code = Clipboard.GetText().Trim();
        if (string.IsNullOrEmpty(code))
        {
            SetStatus("Clipboard is empty.", error: true);
            return;
        }
        TryDecode(code);
    }

    [RelayCommand]
    private void CopyCode()
    {
        if (string.IsNullOrWhiteSpace(JsonText))
        {
            SetStatus("Nothing to encode — editor is empty.", error: true);
            return;
        }
        TryEncode();
    }

    internal void TryDecode(string code)
    {
        try
        {
            var ruleset = FilterCodec.Decode(code);
            JsonText = JsonSerializer.Serialize(ruleset, FilterJsonOptions.Default);
            SetStatus($"Imported \"{ruleset.Name}\" — {ruleset.Rules.Count} rule(s).");
        }
        catch (Exception ex)
        {
            SetStatus($"Decode failed: {ex.Message}", error: true);
        }
    }

    private void TryEncode()
    {
        try
        {
            var ruleset = JsonSerializer.Deserialize<FilterRuleset>(JsonText, FilterJsonOptions.Default)
                ?? throw new InvalidOperationException("JSON deserialized to null.");
            var code = FilterCodec.Encode(ruleset);
            Clipboard.SetText(code);
            SetStatus("Filter code copied to clipboard.");
        }
        catch (Exception ex)
        {
            SetStatus($"Encode failed: {ex.Message}", error: true);
        }
    }

    private void SetStatus(string message, bool error = false)
    {
        StatusMessage = message;
        HasError = error;
    }
}
