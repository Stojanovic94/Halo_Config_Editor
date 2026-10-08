#nullable enable

using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace HaloConfigEditorApp;

public partial class Form1 : Form
{
    private readonly Dictionary<string, Dictionary<string, string>> _settings = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _sectionOrder = new();
    private readonly Dictionary<string, List<string>> _sectionKeys = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, TextBox> _keyBindTextBoxes = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Button> _keyBindButtons = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<SettingDefinition> _allDefinitions = new();
    private readonly List<(FlowLayoutPanel Panel, List<Control> Rows)> _tabRows = new();
    private readonly string _configPath;
    private string? _captureSection;
    private string? _captureBindingKey;
    private MouseWheelCaptureFilter? _wheelFilter;

    private bool IsCapturingKey =>
        !string.IsNullOrEmpty(_captureSection) && !string.IsNullOrEmpty(_captureBindingKey);

    public Form1()
    {
        InitializeComponent();
        BuildTabs();
        HookMouseCaptureHandlers(this);
        _configPath = ResolveConfigPath();
        LoadConfig();
        InitializeControlsFromValues();
        Text = $"Halo Config Editor - {Path.GetFileName(_configPath)}";

        _wheelFilter = new MouseWheelCaptureFilter(this);
        Application.AddMessageFilter(_wheelFilter);
        FormClosed += (_, _) =>
        {
            if (_wheelFilter != null)
            {
                Application.RemoveMessageFilter(_wheelFilter);
                _wheelFilter = null;
            }
        };

        // Rows are created before the form is laid out, so their initial
        // widths are based on a placeholder value. Once the form is shown,
        // and on every subsequent resize, we re-stretch them to match the
        // real panel width.
        Shown += (_, _) => RelayoutAllPanels();
        Resize += (_, _) => RelayoutAllPanels();
    }

    private void RelayoutAllPanels()
    {
        foreach (var (panel, rows) in _tabRows)
        {
            var rowWidth = ComputeRowWidth(panel);
            foreach (var row in rows)
            {
                row.Width = rowWidth;
            }
        }
    }

    private static int ComputeRowWidth(FlowLayoutPanel panel)
    {
        var width = panel.ClientSize.Width
                    - panel.Padding.Horizontal
                    - SystemInformation.VerticalScrollBarWidth
                    - 16;

        return width < 400 ? 700 : width;
    }

    private static string GetExecutableDirectory()
    {
        // Environment.ProcessPath is the running .exe. For a single-file
        // publish, that is the bundle itself; for a framework-dependent
        // publish it is the apphost. Either way, this is the folder the
        // user dropped the editor into.
        var processPath = Environment.ProcessPath;
        if (!string.IsNullOrEmpty(processPath))
        {
            var dir = Path.GetDirectoryName(processPath);
            if (!string.IsNullOrEmpty(dir))
            {
                return dir;
            }
        }

        return AppContext.BaseDirectory;
    }


    // -----------------------------------------------------------------------
    // Key capture via ProcessCmdKey: fires BEFORE WinForms turns Tab, arrows,
    // Enter, Escape into dialog navigation. So we can capture them as keys.
    // -----------------------------------------------------------------------
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (IsCapturingKey)
        {
            var keyCode = keyData & Keys.KeyCode;
            var bindingKey = FormatKeyCode(keyCode);
            if (!string.IsNullOrEmpty(bindingKey))
            {
                ApplyCapturedKey(bindingKey);
            }

            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void ApplyCapturedKey(string bindingKey)
    {
        if (string.IsNullOrEmpty(_captureSection) || string.IsNullOrEmpty(_captureBindingKey))
        {
            return;
        }

        var section = _captureSection;
        var settingKey = _captureBindingKey;

        UpdateSetting(section, settingKey, bindingKey);

        if (_keyBindButtons.TryGetValue($"{section}:{settingKey}", out var btn))
        {
            btn.Text = "Change Keybind";
        }

        _captureSection = null;
        _captureBindingKey = null;
    }

    // Nested message filter to catch global mouse wheel messages, which the
    // WinForms MouseDown event cannot see.
    private sealed class MouseWheelCaptureFilter : IMessageFilter
    {
        private const int WM_MOUSEWHEEL = 0x020A;
        private readonly Form1 _form;

        public MouseWheelCaptureFilter(Form1 form) => _form = form;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_MOUSEWHEEL && _form.IsCapturingKey)
            {
                var raw = (long)m.WParam;
                var high = (short)((raw >> 16) & 0xFFFF);
                _form.ApplyCapturedKey(high > 0 ? "Wheel Up" : "Wheel Down");
                return true;
            }

            return false;
        }
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        SaveCurrentConfig();
        MessageBox.Show($"Saved config to:{Environment.NewLine}{_configPath}", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void SaveAndLaunchButton_Click(object? sender, EventArgs e)
    {
        SaveCurrentConfig();
        LaunchGame();
    }

    private void SaveAndExitButton_Click(object? sender, EventArgs e)
    {
        SaveCurrentConfig();
        Close();
    }

    private void ResetButton_Click(object? sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "Reset every setting to its default value?" + Environment.NewLine + Environment.NewLine +
            "This cannot be undone. Unsaved changes will be lost.",
            "Reset to Defaults",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (result != DialogResult.Yes)
        {
            return;
        }

        ResetToDefaults();
    }

    private void ResetToDefaults()
    {
        _captureSection = null;
        _captureBindingKey = null;

        foreach (var definition in _allDefinitions)
        {
            RegisterSection(definition.Section);
            _settings[definition.Section][definition.Key] = definition.DefaultValue;
            if (!_sectionKeys[definition.Section].Contains(definition.Key))
            {
                _sectionKeys[definition.Section].Add(definition.Key);
            }
        }

        InitializeControlsFromValues();

        _infoTitle.Text = "Setting Info";
        _infoBody.Text = "Hover over a setting to see what it does.";
        _infoBody.SelectionStart = 0;
        _infoBody.SelectionLength = 0;
    }

    private void LoadConfig()
    {
        foreach (var section in new[] { "display", "audio", "input", "controls", "game", "paths", "network", "discord", "update", "debug" })
        {
            RegisterSection(section);
        }

        if (!File.Exists(_configPath))
        {
            return;
        }

        string? currentSection = null;
        foreach (var rawLine in File.ReadAllLines(_configPath))
        {
            var trimmed = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
            {
                continue;
            }

            if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
            {
                currentSection = trimmed[1..^1].Trim();
                RegisterSection(currentSection);
                continue;
            }

            var equalIndex = trimmed.IndexOf('=');
            if (equalIndex < 0 || string.IsNullOrWhiteSpace(currentSection))
            {
                continue;
            }

            var key = trimmed[..equalIndex].Trim();
            var value = UnquoteValue(trimmed[(equalIndex + 1)..].Trim());

            RegisterSection(currentSection);
            _settings[currentSection][key] = value;
            if (!_sectionKeys[currentSection].Contains(key))
            {
                _sectionKeys[currentSection].Add(key);
            }
        }
    }

    private void RegisterSection(string section)
    {
        if (string.IsNullOrWhiteSpace(section))
        {
            return;
        }

        if (!_settings.ContainsKey(section))
        {
            _settings[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _sectionOrder.Add(section);
            _sectionKeys[section] = new List<string>();
        }
    }

    private void InitializeControlsFromValues()
    {
        foreach (var control in GetAllControls(this))
        {
            if (control.Tag is not SettingBinding binding)
            {
                continue;
            }

            var value = GetSettingValue(binding.Section, binding.Key);
            switch (control)
            {
                case TextBox textBox:
                    if (!textBox.ReadOnly && !textBox.Multiline)
                    {
                        textBox.Text = value;
                    }
                    break;
                case NumericUpDown numeric:
                    if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var numericValue))
                    {
                        numeric.Value = Math.Clamp(numericValue, numeric.Minimum, numeric.Maximum);
                    }
                    break;
                case ComboBox combo:
                    if (combo.Items.Contains(value))
                    {
                        combo.SelectedItem = value;
                    }
                    else if (!string.IsNullOrEmpty(value))
                    {
                        combo.Items.Add(value);
                        combo.SelectedItem = value;
                    }
                    break;
                case TrackBar trackBar when binding.Kind == "color_alpha":
                    var parsedColor = ParseColorValue(value);
                    trackBar.Value = Math.Clamp((int)parsedColor.A, trackBar.Minimum, trackBar.Maximum);
                    if (trackBar.Parent is FlowLayoutPanel panel)
                    {
                        foreach (Control sibling in panel.Controls)
                        {
                            if (sibling is Label av && av.Name == "alphaValueLabel")
                            {
                                av.Text = parsedColor.A.ToString(CultureInfo.InvariantCulture);
                            }
                        }
                    }
                    break;
                case TrackBar trackBar:
                    if (binding.Kind == "int_slider")
                    {
                        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intVal))
                        {
                            trackBar.Value = Math.Clamp(intVal, trackBar.Minimum, trackBar.Maximum);
                        }
                    }
                    else if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var scaledValue))
                    {
                        var scaled = (int)Math.Round(scaledValue * 100.0);
                        trackBar.Value = Math.Clamp(scaled, trackBar.Minimum, trackBar.Maximum);
                    }

                    UpdateSliderSiblingLabel(trackBar, binding.Kind == "int_slider");
                    break;
                case Button btn when binding.Kind == "color_swatch":
                    var loadedColor = ParseColorValue(value);
                    btn.BackColor = Color.FromArgb(255, loadedColor.R, loadedColor.G, loadedColor.B);
                    break;
            }
        }

        foreach (var pair in _keyBindTextBoxes)
        {
            var sectionAndKey = pair.Key.Split(':', 2);
            if (sectionAndKey.Length == 2)
            {
                pair.Value.Text = GetSettingValue(sectionAndKey[0], sectionAndKey[1]);
            }
        }
    }

    private static void UpdateSliderSiblingLabel(TrackBar trackBar, bool isIntSlider)
    {
        if (trackBar.Parent is not FlowLayoutPanel panel)
        {
            return;
        }

        foreach (Control sibling in panel.Controls)
        {
            if (sibling is Label lbl && sibling != trackBar)
            {
                if (isIntSlider)
                {
                    lbl.Text = trackBar.Value.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    lbl.Text = (trackBar.Value / 100.0).ToString("0.00", CultureInfo.InvariantCulture);
                }

                break;
            }
        }
    }

    private void SaveCurrentConfig()
    {
        var builder = new StringBuilder();
        builder.AppendLine("config_version = 2");

        foreach (var section in _sectionOrder)
        {
            if (!_settings.TryGetValue(section, out var sectionValues) || sectionValues.Count == 0)
            {
                continue;
            }

            builder.AppendLine();
            builder.AppendLine($"[{section}]");

            var keys = _sectionKeys.TryGetValue(section, out var orderedKeys)
                ? orderedKeys
                : sectionValues.Keys.ToList();

            foreach (var key in keys)
            {
                if (!sectionValues.TryGetValue(key, out var value))
                {
                    continue;
                }

                builder.AppendLine($"{key} = {FormatTomlValue(value)}");
            }
        }

        Directory.CreateDirectory(Path.GetDirectoryName(_configPath) ?? Directory.GetCurrentDirectory());
        File.WriteAllText(_configPath, builder.ToString());
    }

    private void UpdateSetting(string section, string key, string value)
    {
        RegisterSection(section);
        _settings[section][key] = value;
        if (!_sectionKeys[section].Contains(key))
        {
            _sectionKeys[section].Add(key);
        }

        if (_keyBindTextBoxes.TryGetValue($"{section}:{key}", out var textBox))
        {
            textBox.Text = value;
        }
    }

    private string GetSettingValue(string section, string key)
    {
        if (_settings.TryGetValue(section, out var values) && values.TryGetValue(key, out var value))
        {
            return value;
        }

        return string.Empty;
    }

    private static string GetInfoText(string section, string key)
    {
        return SettingInfo.Get(section, key);
    }

    private void ShowSettingInfo(string label, string section, string key)
    {
        _infoTitle.Text = label;
        _infoBody.Text = GetInfoText(section, key);
        _infoBody.SelectionStart = 0;
        _infoBody.SelectionLength = 0;
    }

    private void AttachHoverInfo(Control control, string label, string section, string key)
    {
        control.MouseEnter += (_, _) => ShowSettingInfo(label, section, key);
        foreach (Control child in control.Controls)
        {
            AttachHoverInfo(child, label, section, key);
        }
    }

    private static IEnumerable<Control> GetAllControls(Control root)
    {
        foreach (Control child in root.Controls)
        {
            yield return child;
            foreach (var nested in GetAllControls(child))
            {
                yield return nested;
            }
        }
    }

    private static string ResolveConfigPath()
    {
        var exeDir = GetExecutableDirectory();

        // 1. config.toml beside the editor .exe (the drop-in folder).
        var candidate = Path.Combine(exeDir, "config.toml");
        if (File.Exists(candidate))
        {
            return candidate;
        }

        // 2. current working directory (dev: dotnet run from the repo root).
        candidate = Path.Combine(Environment.CurrentDirectory, "config.toml");
        if (File.Exists(candidate))
        {
            return candidate;
        }

        // 3. Walk up from the exe folder (dev: bin/Debug/net8.0-windows).
        var current = new DirectoryInfo(exeDir);
        while (current != null)
        {
            candidate = Path.Combine(current.FullName, "config.toml");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            current = current.Parent;
        }

        // 4. Default: create it next to the editor.
        return Path.Combine(exeDir, "config.toml");
    }

    private void LaunchGame()
    {
        var executable = FindGameExecutable();
        if (string.IsNullOrWhiteSpace(executable))
        {
            MessageBox.Show("Could not find a Halo game executable automatically. Add the game .exe path to the launch folder or set it manually before launching.", "Launch game", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = executable,
                WorkingDirectory = Path.GetDirectoryName(executable) ?? Environment.CurrentDirectory,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unable to launch the game:{Environment.NewLine}{ex.Message}", "Launch failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static string FindGameExecutable()
    {
        var exeDir = GetExecutableDirectory();
        var selfPath = Environment.ProcessPath;

        // 1. Well-known Halo executable names beside the editor.
        foreach (var name in new[] { "halo.exe", "haloce.exe", "halo2.exe", "halo3.exe" })
        {
            var candidate = Path.Combine(exeDir, name);
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        // 2. Any .exe whose name contains "halo" beside the editor,
        //    excluding the editor itself.
        foreach (var file in Directory.EnumerateFiles(exeDir, "*.exe", SearchOption.TopDirectoryOnly))
        {
            if (selfPath != null && string.Equals(file, selfPath, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var name = Path.GetFileNameWithoutExtension(file);
            if (name.Contains("halo", StringComparison.OrdinalIgnoreCase))
            {
                return file;
            }
        }

        // 3. Fallback: the previous broad search.
        var searchRoots = new[]
        {
            Environment.CurrentDirectory,
            AppContext.BaseDirectory,
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Steam"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam")
        };

        foreach (var root in searchRoots.Distinct())
        {
            if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
            {
                continue;
            }

            foreach (var file in Directory.EnumerateFiles(root, "*.exe", SearchOption.AllDirectories))
            {
                if (selfPath != null && string.Equals(file, selfPath, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var name = Path.GetFileNameWithoutExtension(file);
                if (name.Contains("halo", StringComparison.OrdinalIgnoreCase))
                {
                    return file;
                }
            }
        }

        return string.Empty;
    }

    private void HookMouseCaptureHandlers(Control root)
    {
        root.MouseDown += HandleMouseDown;
        foreach (Control child in root.Controls)
        {
            HookMouseCaptureHandlers(child);
        }
    }

    private static Color ParseColorValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Color.FromArgb(16, 16, 16, 150);
        }

        var parts = value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 3)
        {
            var r = int.TryParse(parts[0], out var red) ? red : 16;
            var g = int.TryParse(parts[1], out var green) ? green : 16;
            var b = int.TryParse(parts[2], out var blue) ? blue : 16;
            var a = parts.Length >= 4 && int.TryParse(parts[3], out var alpha) ? alpha : 255;
            return Color.FromArgb(a, r, g, b);
        }

        return Color.FromArgb(16, 16, 16, 150);
    }

    private void HandleMouseDown(object? sender, MouseEventArgs e)
    {
        if (!IsCapturingKey)
        {
            return;
        }

        var mouseKey = e.Button switch
        {
            MouseButtons.Left => "Mouse Left",
            MouseButtons.Right => "Mouse Right",
            MouseButtons.Middle => "Mouse Middle",
            MouseButtons.XButton1 => "Mouse 4",
            MouseButtons.XButton2 => "Mouse 5",
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(mouseKey))
        {
            return;
        }

        ApplyCapturedKey(mouseKey);
    }

    private static string FormatKeyCode(Keys key)
    {
        if (key == Keys.Escape) return "Escape";
        if (key == Keys.Space) return "Space";
        if (key == Keys.Tab) return "Tab";
        if (key == Keys.Enter || key == Keys.Return) return "Enter";
        if (key == Keys.Back) return "Backspace";
        if (key == Keys.Delete) return "Delete";

        if (key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down)
        {
            return key.ToString();
        }

        if (key == Keys.LControlKey || key == Keys.RControlKey || key == Keys.ControlKey) return "Left Ctrl";
        if (key == Keys.LShiftKey || key == Keys.RShiftKey || key == Keys.ShiftKey) return "Left Shift";
        if (key == Keys.LMenu || key == Keys.RMenu || key == Keys.Menu) return "Left Alt";

        return key.ToString();
    }

    private static string UnquoteValue(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.StartsWith('"') && trimmed.EndsWith('"') && trimmed.Length >= 2)
        {
            return trimmed[1..^1].Replace("\\\"", "\"").Replace("\\\\", "\\");
        }

        if (trimmed.StartsWith('\'') && trimmed.EndsWith('\'') && trimmed.Length >= 2)
        {
            return trimmed[1..^1];
        }

        return trimmed;
    }

    private static string FormatTomlValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return "\"\"";
        if (bool.TryParse(value, out var boolValue)) return boolValue ? "true" : "false";
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)) return value;
        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _)) return value;
        if (value.StartsWith('"') && value.EndsWith('"')) return value;
        if (value.StartsWith('\'') && value.EndsWith('\'')) return value;

        return $"\"{value.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
    }

    private void BuildTabs()
    {
        AddDefinitions(new[]
        {
            new SettingDefinition("Fullscreen", "display", "fullscreen", "bool", "true", new[] { "true", "false" }),
            new SettingDefinition("Mode", "display", "mode", "dropdown", "fullscreen", new[] { "fullscreen", "borderless", "windowed" }),
            new SettingDefinition("Resolution", "display", "resolution", "resolution", "native", new[] { "native", "640x480", "800x600", "1024x768", "1280x720", "1280x1024", "1366x768", "1440x900", "1600x900", "1920x1080", "2560x1440", "3840x2160" }),
            new SettingDefinition("Window Size", "display", "window_size", "dropdown", "640x480", new[] { "640x480", "800x600", "1024x768", "1280x720", "1280x1024", "1366x768", "1440x900", "1600x900", "1920x1080", "2560x1440", "3840x2160" }),
            new SettingDefinition("Window Scale", "display", "window_scale", "int", "2", Minimum: 1, Maximum: 10),
            new SettingDefinition("VSync", "display", "vsync", "bool", "true", new[] { "true", "false" }),
            new SettingDefinition("Max FPS", "display", "max_fps", "int", "60", Minimum: -1, Maximum: 300),
            new SettingDefinition("Interpolation", "display", "interpolation", "bool", "true", new[] { "true", "false" }),
            new SettingDefinition("Direct Camera", "display", "direct_camera", "bool", "true", new[] { "true", "false" }),
            new SettingDefinition("High Res HUD", "display", "high_res_hud", "bool", "true", new[] { "true", "false" }),
            new SettingDefinition("High Res Text", "display", "high_res_text", "bool", "true", new[] { "true", "false" }),
            new SettingDefinition("Menus", "display", "menus", "dropdown", "pc", new[] { "pc", "xbox" }),
            new SettingDefinition("Player Names", "display", "player_names", "dropdown", "all", new[] { "all", "allies", "enemies", "none" }),
            new SettingDefinition("Player Name Scale", "display", "player_name_scale", "slider", "1.0", Minimum: 25, Maximum: 400, DecimalPlaces: 2),
            new SettingDefinition("Scoreboard Layout", "display", "scoreboard_team_layout", "dropdown", "teams", new[] { "teams", "score" }),
            new SettingDefinition("Scoreboard Background", "display", "scoreboard_background", "bool", "true", new[] { "true", "false" }),
            new SettingDefinition("Scoreboard Colour", "display", "scoreboard_background_color", "color", "16, 16, 16, 150"),
            new SettingDefinition("Anti Aliasing", "display", "anti_aliasing", "dropdown", "msaa2x", new[] { "off", "fxaa", "smaa", "ssaa2x", "msaa2x", "msaa4x", "msaa8x" }),
            new SettingDefinition("Shadow Resolution", "display", "shadow_resolution", "dropdown", "512", new[] { "128", "256", "512", "1024" }),
            new SettingDefinition("Per Pixel Lighting", "display", "per_pixel_lighting", "bool", "true", new[] { "true", "false" })
        }, _panelDisplay);

        AddDefinitions(new[]
        {
            new SettingDefinition("Enabled", "audio", "enabled", "bool", "true", new[] { "true", "false" }),
            new SettingDefinition("Volume", "audio", "volume", "slider", "0.8", Minimum: 0, Maximum: 100),
            new SettingDefinition("Music Volume", "audio", "music_volume", "slider", "0.8", Minimum: 0, Maximum: 100),
            new SettingDefinition("Effects Volume", "audio", "effects_volume", "slider", "0.8", Minimum: 0, Maximum: 100),
            new SettingDefinition("Reverb", "audio", "reverb", "bool", "true", new[] { "true", "false" }),
            new SettingDefinition("Loose Sounds", "audio", "loose_sounds", "bool", "false", new[] { "true", "false" })
        }, _panelAudio);

        AddDefinitions(new[]
        {
            new SettingDefinition("Mouse Sensitivity", "input", "mouse_sensitivity", "mouse", "1.25", Minimum: 0, Maximum: 200),
            new SettingDefinition("Invert Mouse", "input", "invert_mouse", "bool", "false", new[] { "true", "false" }),
            new SettingDefinition("Mouse Aim Assist", "input", "mouse_aim_assist", "bool", "false", new[] { "true", "false" }),
            new SettingDefinition("Vertical Sensitivity", "input", "mouse_vertical_sensitivity", "mouse_vertical", "0.0", Minimum: 0, Maximum: 100, DecimalPlaces: 2)
        }, _panelInput);

        AddDefinitions(new[]
        {
            new SettingDefinition("Move Forward", "controls", "move_forward", "keybind", "W", IsKeyBinding: true),
            new SettingDefinition("Move Backward", "controls", "move_backward", "keybind", "S", IsKeyBinding: true),
            new SettingDefinition("Move Left", "controls", "strafe_left", "keybind", "A", IsKeyBinding: true),
            new SettingDefinition("Move Right", "controls", "strafe_right", "keybind", "D", IsKeyBinding: true),
            new SettingDefinition("Jump", "controls", "jump", "keybind", "Space", IsKeyBinding: true),
            new SettingDefinition("Crouch", "controls", "crouch", "keybind", "Left Ctrl, C", IsKeyBinding: true),
            new SettingDefinition("Fire", "controls", "fire", "keybind", "Mouse Left", IsKeyBinding: true),
            new SettingDefinition("Throw Grenade", "controls", "throw_grenade", "keybind", "Mouse Right", IsKeyBinding: true),
            new SettingDefinition("Melee", "controls", "melee", "keybind", "F, Mouse 4", IsKeyBinding: true),
            new SettingDefinition("Reload", "controls", "reload", "keybind", "R", IsKeyBinding: true),
            new SettingDefinition("Zoom", "controls", "zoom", "keybind", "Z, Mouse Middle", IsKeyBinding: true),
            new SettingDefinition("Switch Weapon", "controls", "switch_weapon", "keybind", "Tab, 1", IsKeyBinding: true),
            new SettingDefinition("Switch Grenade", "controls", "switch_grenade", "keybind", "G", IsKeyBinding: true),
            new SettingDefinition("Action", "controls", "action", "keybind", "E", IsKeyBinding: true),
            new SettingDefinition("Flashlight", "controls", "flashlight", "keybind", "Q", IsKeyBinding: true),
            new SettingDefinition("Scoreboard", "controls", "scoreboard", "keybind", "F1", IsKeyBinding: true),
            new SettingDefinition("Pause", "controls", "pause", "keybind", "Escape", IsKeyBinding: true)
        }, _panelControls);

        AddDefinitions(new[]
        {
            new SettingDefinition("Console Log", "game", "console_log", "dropdown", "important", new[] { "important", "all", "none" }),
            new SettingDefinition("Language", "game", "language", "dropdown", "", new[] { "", "ja", "de", "fr", "es", "it" }),
            new SettingDefinition("Custom Edition", "game", "custom_edition", "bool", "true", new[] { "true", "false" })
        }, _panelGame);

        AddDefinitions(new[]
        {
            new SettingDefinition("Data", "paths", "data", "string", ""),
            new SettingDefinition("Saves", "paths", "saves", "string", ""),
            new SettingDefinition("Custom Edition Root", "paths", "custom_edition", "string", "")
        }, _panelPaths);

        AddDefinitions(new[]
        {
            new SettingDefinition("Address", "network", "address", "string", ""),
            new SettingDefinition("Broadcast", "network", "broadcast", "string", ""),
            new SettingDefinition("Online", "network", "online", "bool", "true", new[] { "true", "false" }),
            new SettingDefinition("Join From Clipboard", "network", "join_from_clipboard", "bool", "true", new[] { "true", "false" }),
            new SettingDefinition("Tunnel Port", "network", "tunnel_port", "int", "0", Minimum: 0, Maximum: 65535),
            new SettingDefinition("Allow UPnP", "network", "allow_upnp", "bool", "true", new[] { "true", "false" }),
            new SettingDefinition("Public Lobby", "network", "public_lobby", "bool", "true", new[] { "true", "false" }),
            new SettingDefinition("Host Public", "network", "host_public", "bool", "true", new[] { "true", "false" }),
            new SettingDefinition("Co-op Public", "network", "coop_public", "bool", "false", new[] { "true", "false" }),
            new SettingDefinition("Co-op Friendly Fire", "network", "coop_friendly_fire", "dropdown", "on", new[] { "off", "on", "shields_only", "explosives_only" }),
            new SettingDefinition("Co-op Enemies Mode", "network", "coop_enemies_mode", "dropdown", "per_player", new[] { "none", "per_player", "multiplier" }),
            new SettingDefinition("Co-op Enemies", "network", "coop_enemies", "int_slider", "50", Minimum: 25, Maximum: 200),
            new SettingDefinition("Co-op Enemies Multiplier", "network", "coop_enemies_multiplier", "int_slider", "2", Minimum: 2, Maximum: 32),
            new SettingDefinition("Brokers File", "network", "brokers_file", "string", "brokers.txt"),
            new SettingDefinition("STUN Servers", "network", "stun_servers", "string", ""),
            new SettingDefinition("Co-op Player Collisions", "network", "coop_player_collisions", "bool", "true", new[] { "true", "false" })
        }, _panelNetwork);

        AddDefinitions(new[]
        {
            new SettingDefinition("Application ID", "discord", "application_id", "string", "1553978809840050229")
        }, _panelDiscord);

        AddDefinitions(new[]
        {
            new SettingDefinition("Auto Update", "update", "auto", "bool", "true", new[] { "true", "false" })
        }, _panelUpdate);

        AddDefinitions(new[]
        {
            new SettingDefinition("Network Test", "debug", "network_test", "string", ""),
            new SettingDefinition("Network Test Start", "debug", "network_test_start", "float", "15.0", Minimum: 0, Maximum: 300, DecimalPlaces: 2),
            new SettingDefinition("Network Test Kill", "debug", "network_test_kill", "float", "0.0", Minimum: 0, Maximum: 300, DecimalPlaces: 2),
            new SettingDefinition("Network Test Score", "debug", "network_test_score", "int", "0", Minimum: 0, Maximum: 10000),
            new SettingDefinition("Network Test Shoot", "debug", "network_test_shoot", "float", "0.0", Minimum: 0, Maximum: 300, DecimalPlaces: 2),
            new SettingDefinition("Network Test Vehicle", "debug", "network_test_vehicle", "float", "0.0", Minimum: 0, Maximum: 300, DecimalPlaces: 2),
            new SettingDefinition("Network Test Pickup", "debug", "network_test_pickup", "float", "0.0", Minimum: 0, Maximum: 300, DecimalPlaces: 2),
            new SettingDefinition("Pickup Weapon", "debug", "network_test_pickup_weapon", "string", ""),
            new SettingDefinition("Telnet Console", "debug", "telnet_console", "bool", "false", new[] { "true", "false" }),
            new SettingDefinition("Telnet Console Port", "debug", "telnet_console_port", "int", "2323", Minimum: 1, Maximum: 65535),
            new SettingDefinition("Network Latency", "debug", "network_latency", "float", "0.0", Minimum: 0, Maximum: 1000, DecimalPlaces: 2),
            new SettingDefinition("Network Loss", "debug", "network_loss", "float", "0.0", Minimum: 0, Maximum: 100, DecimalPlaces: 2),
            new SettingDefinition("Test Input", "debug", "test_input", "string", ""),
            new SettingDefinition("Update Answer", "debug", "update_answer", "dropdown", "yes", new[] { "yes", "no", "never" }),
            new SettingDefinition("Exit After", "debug", "exit_after", "float", "0.0", Minimum: 0, Maximum: 3600, DecimalPlaces: 2),
            new SettingDefinition("Hidden Window", "debug", "hidden_window", "bool", "false", new[] { "true", "false" }),
            new SettingDefinition("Null Renderer", "debug", "null_renderer", "bool", "false", new[] { "true", "false" }),
            new SettingDefinition("GL Debug", "debug", "gl_debug", "bool", "false", new[] { "true", "false" }),
            new SettingDefinition("Menu Open", "debug", "menu_open", "string", ""),
            new SettingDefinition("GPU Flush Draws", "debug", "gpu_flush_draws", "int", "-1", Minimum: -1, Maximum: 10000),
            new SettingDefinition("GPU Stats", "debug", "gpu_stats", "bool", "false", new[] { "true", "false" }),
            new SettingDefinition("GPU Trace Frame", "debug", "gpu_trace_frame", "int", "-1", Minimum: -1, Maximum: 100000),
            new SettingDefinition("GPU Trace Constants", "debug", "gpu_trace_constants", "bool", "false", new[] { "true", "false" }),
            new SettingDefinition("GPU Skip Vertex Shaders", "debug", "gpu_skip_vertex_shaders", "string", ""),
            new SettingDefinition("GPU Dump Shaders", "debug", "gpu_dump_shaders", "string", ""),
            new SettingDefinition("GPU Debug Expression", "debug", "gpu_debug_expression", "string", ""),
            new SettingDefinition("GPU Debug Texture0", "debug", "gpu_debug_texture0", "bool", "false", new[] { "true", "false" }),
            new SettingDefinition("GPU Debug Flat", "debug", "gpu_debug_flat", "bool", "false", new[] { "true", "false" }),
            new SettingDefinition("Screenshot Directory", "debug", "screenshot_directory", "string", ""),
            new SettingDefinition("Screenshot Every", "debug", "screenshot_every", "int", "0", Minimum: 0, Maximum: 100000),
            new SettingDefinition("Texture Dump Directory", "debug", "texture_dump_directory", "string", ""),
            new SettingDefinition("Texture Log", "debug", "texture_log", "bool", "false", new[] { "true", "false" }),
            new SettingDefinition("Texture No Cache", "debug", "texture_no_cache", "bool", "false", new[] { "true", "false" }),
            new SettingDefinition("Network Corrupt", "debug", "network_corrupt", "float", "0.0", Minimum: 0, Maximum: 100, DecimalPlaces: 2),
            new SettingDefinition("Network Corrupt Stream", "debug", "network_corrupt_stream", "float", "0.0", Minimum: 0, Maximum: 100, DecimalPlaces: 2),
            new SettingDefinition("Network Corrupt After", "debug", "network_corrupt_after", "float", "0.0", Minimum: 0, Maximum: 300, DecimalPlaces: 2)
        }, _panelDebug);
    }

    private void AddDefinitions(IEnumerable<SettingDefinition> definitions, FlowLayoutPanel panel)
    {
        var list = definitions.ToList();
        _allDefinitions.AddRange(list);
        PopulatePanel(panel, list);
    }

    private void PopulatePanel(FlowLayoutPanel panel, IEnumerable<SettingDefinition> definitions)
    {
        panel.SuspendLayout();

        var rowWidth = ComputeRowWidth(panel);
        var rows = new List<Control>();

        foreach (var setting in definitions)
        {
            var row = CreateSettingRow(setting, rowWidth);
            panel.Controls.Add(row);
            rows.Add(row);
        }

        // Bottom spacer: an empty control after the last row so the final
        // setting is not glued to the bottom edge when scrolled all the way
        // down.
        var spacer = new Panel
        {
            Width = rowWidth,
            Height = 28,
            Margin = new Padding(0),
            BackColor = Color.Transparent
        };
        panel.Controls.Add(spacer);
        rows.Add(spacer);

        _tabRows.Add((panel, rows));

        panel.ResumeLayout(true);
    }

    private Control CreateSettingRow(SettingDefinition setting, int rowWidth)
    {
        var row = new TableLayoutPanel
        {
            Width = rowWidth,
            MinimumSize = new Size(rowWidth, 0),
            MaximumSize = new Size(rowWidth, 0),
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            RowCount = 1,
            Margin = new Padding(0, 0, 0, 6),
            Padding = new Padding(0, 5, 0, 5),
            BackColor = Color.Transparent
        };
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        var label = new Label
        {
            Text = setting.Label,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
            Padding = new Padding(0, 0, 16, 0),
            ForeColor = Color.FromArgb(60, 60, 60),
            AutoEllipsis = true,
            Margin = new Padding(0)
        };

        Control editor;
        if (setting.Kind == "keybind")
        {
            var rowPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Margin = new Padding(0),
                Anchor = AnchorStyles.Left,
                Tag = new SettingBinding(setting.Section, setting.Key, setting.Kind)
            };

            var textBox = new TextBox
            {
                Width = 220,
                ReadOnly = true,
                Text = GetSettingValue(setting.Section, setting.Key),
                Margin = new Padding(0)
            };
            _keyBindTextBoxes[$"{setting.Section}:{setting.Key}"] = textBox;

            var button = new Button
            {
                Text = "Change Keybind",
                Width = 150,
                Height = 30,
                Margin = new Padding(8, 0, 0, 0)
            };
            _keyBindButtons[$"{setting.Section}:{setting.Key}"] = button;

            button.Click += (_, _) =>
            {
                _captureSection = setting.Section;
                _captureBindingKey = setting.Key;
                button.Text = "Press a key, mouse button or scroll...";
                ActiveControl = null;
            };

            rowPanel.Controls.Add(textBox);
            rowPanel.Controls.Add(button);
            editor = rowPanel;
        }
        else if (setting.Kind == "mouse" || setting.Kind == "mouse_vertical" || setting.Kind == "slider" || setting.Kind == "int_slider")
        {
            var isIntSlider = setting.Kind == "int_slider";

            var sliderPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0),
                Anchor = AnchorStyles.Left
            };

            int trackMin;
            int trackMax;

            if (isIntSlider)
            {
                trackMin = setting.Minimum;
                trackMax = setting.Maximum;
            }
            else if (setting.Kind == "slider")
            {
                trackMin = setting.Minimum;
                trackMax = setting.Maximum;
            }
            else
            {
                trackMin = 0;
                trackMax = 300;
            }

            var rawValue = GetSettingValue(setting.Section, setting.Key);
            int trackValue;

            if (isIntSlider)
            {
                if (!int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intVal))
                {
                    int.TryParse(setting.DefaultValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out intVal);
                }

                trackValue = Math.Clamp(intVal, trackMin, trackMax);
            }
            else
            {
                if (!double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var sliderValue))
                {
                    double.TryParse(setting.DefaultValue, NumberStyles.Float, CultureInfo.InvariantCulture, out sliderValue);
                }

                trackValue = Math.Clamp((int)Math.Round(sliderValue * 100.0), trackMin, trackMax);
            }

            var trackBar = new TrackBar
            {
                Width = 260,
                Minimum = trackMin,
                Maximum = trackMax,
                Value = trackValue,
                SmallChange = 1,
                LargeChange = 10,
                TickStyle = TickStyle.None,
                Margin = new Padding(0),
                Tag = new SettingBinding(setting.Section, setting.Key, setting.Kind)
            };

            var valueLabel = new Label
            {
                AutoSize = false,
                Width = 56,
                Height = 26,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(10, 0, 0, 0)
            };

            if (isIntSlider)
            {
                valueLabel.Text = trackValue.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                valueLabel.Text = (trackValue / 100.0).ToString("0.00", CultureInfo.InvariantCulture);
            }

            trackBar.Scroll += (_, _) =>
            {
                if (isIntSlider)
                {
                    valueLabel.Text = trackBar.Value.ToString(CultureInfo.InvariantCulture);
                    UpdateSetting(setting.Section, setting.Key, trackBar.Value.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    var normalizedValue = trackBar.Value / 100.0;
                    valueLabel.Text = normalizedValue.ToString("0.00", CultureInfo.InvariantCulture);
                    UpdateSetting(setting.Section, setting.Key, normalizedValue.ToString("0.00", CultureInfo.InvariantCulture));
                }
            };

            sliderPanel.Controls.Add(trackBar);
            sliderPanel.Controls.Add(valueLabel);
            editor = sliderPanel;
        }
        else
        {
            switch (setting.Kind)
            {
                case "bool":
                    var boolCombo = new ComboBox
                    {
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Width = 200,
                        Anchor = AnchorStyles.Left,
                        Tag = new SettingBinding(setting.Section, setting.Key, setting.Kind)
                    };
                    foreach (var item in setting.Options ?? new[] { "true", "false" })
                    {
                        boolCombo.Items.Add(item);
                    }
                    boolCombo.SelectedItem = GetSettingValue(setting.Section, setting.Key);
                    boolCombo.SelectedIndexChanged += (_, _) => UpdateSetting(setting.Section, setting.Key, boolCombo.Text);
                    editor = boolCombo;
                    break;
                case "int":
                    var intBox = new NumericUpDown
                    {
                        Width = 120,
                        Anchor = AnchorStyles.Left,
                        Minimum = setting.Minimum,
                        Maximum = setting.Maximum,
                        Value = decimal.TryParse(GetSettingValue(setting.Section, setting.Key), NumberStyles.Integer, CultureInfo.InvariantCulture, out var intVal) ? intVal : setting.Minimum,
                        DecimalPlaces = 0,
                        Tag = new SettingBinding(setting.Section, setting.Key, setting.Kind)
                    };
                    intBox.ValueChanged += (_, _) => UpdateSetting(setting.Section, setting.Key, intBox.Value.ToString(CultureInfo.InvariantCulture));
                    editor = intBox;
                    break;
                case "float":
                    var floatBox = new NumericUpDown
                    {
                        Width = 120,
                        Anchor = AnchorStyles.Left,
                        Minimum = setting.Minimum,
                        Maximum = setting.Maximum,
                        DecimalPlaces = setting.DecimalPlaces,
                        Increment = setting.DecimalPlaces == 0 ? 1 : 0.1M,
                        Value = decimal.TryParse(GetSettingValue(setting.Section, setting.Key), NumberStyles.Float, CultureInfo.InvariantCulture, out var floatVal) ? floatVal : 0,
                        Tag = new SettingBinding(setting.Section, setting.Key, setting.Kind)
                    };
                    floatBox.ValueChanged += (_, _) => UpdateSetting(setting.Section, setting.Key, floatBox.Value.ToString(CultureInfo.InvariantCulture));
                    editor = floatBox;
                    break;
                case "dropdown":
                    var dropdownBox = new ComboBox
                    {
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Width = 200,
                        Anchor = AnchorStyles.Left,
                        Tag = new SettingBinding(setting.Section, setting.Key, setting.Kind)
                    };
                    foreach (var item in setting.Options ?? Array.Empty<string>())
                    {
                        dropdownBox.Items.Add(item);
                    }
                    var currentDropdownValue = GetSettingValue(setting.Section, setting.Key);
                    if (string.IsNullOrWhiteSpace(currentDropdownValue) || !dropdownBox.Items.Contains(currentDropdownValue))
                    {
                        currentDropdownValue = setting.DefaultValue;
                    }
                    dropdownBox.SelectedItem = currentDropdownValue;
                    dropdownBox.SelectedIndexChanged += (_, _) => UpdateSetting(setting.Section, setting.Key, dropdownBox.Text);
                    editor = dropdownBox;
                    break;
                case "resolution":
                    var resolutionBox = new ComboBox
                    {
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Width = 200,
                        Anchor = AnchorStyles.Left,
                        Tag = new SettingBinding(setting.Section, setting.Key, setting.Kind)
                    };
                    foreach (var item in setting.Options ?? Array.Empty<string>())
                    {
                        resolutionBox.Items.Add(item);
                    }
                    resolutionBox.SelectedItem = string.IsNullOrWhiteSpace(GetSettingValue(setting.Section, setting.Key)) ? "native" : GetSettingValue(setting.Section, setting.Key);
                    resolutionBox.SelectedIndexChanged += (_, _) => UpdateSetting(setting.Section, setting.Key, resolutionBox.Text);
                    editor = resolutionBox;
                    break;
                case "color":
                    var colorPanel = new FlowLayoutPanel
                    {
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                        FlowDirection = FlowDirection.LeftToRight,
                        WrapContents = false,
                        Margin = new Padding(0),
                        Anchor = AnchorStyles.Left
                    };

                    var initialColor = ParseColorValue(GetSettingValue(setting.Section, setting.Key));

                    var colorPreview = new Button
                    {
                        Width = 50,
                        Height = 26,
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.FromArgb(255, initialColor.R, initialColor.G, initialColor.B),
                        Text = string.Empty,
                        Margin = new Padding(0),
                        Tag = new SettingBinding(setting.Section, setting.Key, "color_swatch")
                    };
                    colorPreview.FlatAppearance.BorderColor = Color.FromArgb(140, 140, 140);

                    var alphaLabel = new Label
                    {
                        Text = "Alpha:",
                        AutoSize = true,
                        Height = 26,
                        TextAlign = ContentAlignment.MiddleRight,
                        Margin = new Padding(10, 6, 0, 0)
                    };

                    var alphaTrack = new TrackBar
                    {
                        Width = 110,
                        Minimum = 0,
                        Maximum = 255,
                        Value = Math.Clamp((int)initialColor.A, 0, 255),
                        SmallChange = 1,
                        LargeChange = 10,
                        TickStyle = TickStyle.None,
                        Margin = new Padding(4, 0, 0, 0),
                        Tag = new SettingBinding(setting.Section, setting.Key, "color_alpha")
                    };

                    var alphaValue = new Label
                    {
                        Name = "alphaValueLabel",
                        AutoSize = false,
                        Width = 44,
                        Height = 26,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Margin = new Padding(6, 0, 0, 0),
                        Text = initialColor.A.ToString(CultureInfo.InvariantCulture)
                    };

                    void WriteColorValue()
                    {
                        var rgb = colorPreview.BackColor;
                        var a = alphaTrack.Value;
                        alphaValue.Text = a.ToString(CultureInfo.InvariantCulture);
                        UpdateSetting(setting.Section, setting.Key, $"{rgb.R}, {rgb.G}, {rgb.B}, {a}");
                    }

                    colorPreview.Click += (_, _) =>
                    {
                        using var dialog = new ColorDialog
                        {
                            FullOpen = true,
                            AnyColor = true,
                            Color = colorPreview.BackColor
                        };

                        if (dialog.ShowDialog(this) == DialogResult.OK)
                        {
                            colorPreview.BackColor = dialog.Color;
                            WriteColorValue();
                        }
                    };

                    alphaTrack.Scroll += (_, _) => WriteColorValue();

                    colorPanel.Controls.Add(colorPreview);
                    colorPanel.Controls.Add(alphaLabel);
                    colorPanel.Controls.Add(alphaTrack);
                    colorPanel.Controls.Add(alphaValue);
                    editor = colorPanel;
                    break;
                default:
                    var textBox = new TextBox
                    {
                        Width = 260,
                        Anchor = AnchorStyles.Left,
                        Text = GetSettingValue(setting.Section, setting.Key),
                        Tag = new SettingBinding(setting.Section, setting.Key, setting.Kind)
                    };
                    textBox.TextChanged += (_, _) => UpdateSetting(setting.Section, setting.Key, textBox.Text);
                    editor = textBox;
                    break;
            }
        }

        row.Controls.Add(label, 0, 0);
        row.Controls.Add(editor, 1, 0);

        AttachHoverInfo(row, setting.Label, setting.Section, setting.Key);

        return row;
    }

    private static Color GetContrastingTextColor(Color background)
    {
        var luminance = (0.299 * background.R + 0.587 * background.G + 0.114 * background.B) / 255.0;
        return luminance > 0.55 ? Color.Black : Color.White;
    }
}