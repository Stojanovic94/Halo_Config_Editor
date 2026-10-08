#nullable enable

namespace HaloConfigEditorApp;

public readonly record struct SettingBinding(string Section, string Key, string Kind = "");

public sealed record SettingDefinition(
    string Label,
    string Section,
    string Key,
    string Kind,
    string DefaultValue,
    string[]? Options = null,
    int Minimum = 0,
    int Maximum = 1000,
    int DecimalPlaces = 0,
    bool IsKeyBinding = false);