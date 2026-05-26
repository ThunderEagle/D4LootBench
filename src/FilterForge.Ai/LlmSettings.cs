namespace ThunderEagle.FilterForge.Ai;

public enum LlmProviderType { Mock, Ollama, OpenAi, Anthropic }

public sealed class LlmSettings
{
    public LlmProviderType Provider { get; set; } = LlmProviderType.Mock;
    public string BaseUrl   { get; set; } = "http://localhost:11434";
    public string ModelName { get; set; } = "qwen2.5-coder:14b";

    /// <summary>Null for Mock/Ollama. DPAPI-encrypted before storage (Phase 4B).</summary>
    public string? ApiKey { get; set; }

    /// <summary>Path to a batch prompts file. When set, runs all prompts and exits instead of interactive mode.</summary>
    public string? BatchFile { get; set; }
}
