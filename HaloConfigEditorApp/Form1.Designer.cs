#nullable enable

namespace HaloConfigEditorApp;

partial class Form1
{
    private TableLayoutPanel _rootPanel = null!;
    private TabControl _tabControl = null!;
    private TableLayoutPanel _buttonPanel = null!;
    private FlowLayoutPanel _saveButtonsPanel = null!;
    private Button _resetButton = null!;
    private Button _saveButton = null!;
    private Button _saveAndLaunchButton = null!;
    private Button _saveAndExitButton = null!;

    private Panel _infoPanel = null!;
    private Label _infoTitle = null!;
    private Panel _infoBodyHost = null!;
    private RichTextBox _infoBody = null!;

    private TabPage _tabDisplay = null!;
    private TabPage _tabAudio = null!;
    private TabPage _tabInput = null!;
    private TabPage _tabControls = null!;
    private TabPage _tabGame = null!;
    private TabPage _tabPaths = null!;
    private TabPage _tabNetwork = null!;
    private TabPage _tabDiscord = null!;
    private TabPage _tabUpdate = null!;
    private TabPage _tabDebug = null!;

    private FlowLayoutPanel _panelDisplay = null!;
    private FlowLayoutPanel _panelAudio = null!;
    private FlowLayoutPanel _panelInput = null!;
    private FlowLayoutPanel _panelControls = null!;
    private FlowLayoutPanel _panelGame = null!;
    private FlowLayoutPanel _panelPaths = null!;
    private FlowLayoutPanel _panelNetwork = null!;
    private FlowLayoutPanel _panelDiscord = null!;
    private FlowLayoutPanel _panelUpdate = null!;
    private FlowLayoutPanel _panelDebug = null!;

    private void InitializeComponent()
    {
        _rootPanel = new TableLayoutPanel();
        _tabControl = new TabControl();
        _buttonPanel = new TableLayoutPanel();
        _saveButtonsPanel = new FlowLayoutPanel();
        _resetButton = new Button();
        _saveButton = new Button();
        _saveAndLaunchButton = new Button();
        _saveAndExitButton = new Button();
        _infoPanel = new Panel();
        _infoTitle = new Label();
        _infoBodyHost = new Panel();
        _infoBody = new RichTextBox();
        _tabDisplay = new TabPage();
        _tabAudio = new TabPage();
        _tabInput = new TabPage();
        _tabControls = new TabPage();
        _tabGame = new TabPage();
        _tabPaths = new TabPage();
        _tabNetwork = new TabPage();
        _tabDiscord = new TabPage();
        _tabUpdate = new TabPage();
        _tabDebug = new TabPage();
        _panelDisplay = new FlowLayoutPanel();
        _panelAudio = new FlowLayoutPanel();
        _panelInput = new FlowLayoutPanel();
        _panelControls = new FlowLayoutPanel();
        _panelGame = new FlowLayoutPanel();
        _panelPaths = new FlowLayoutPanel();
        _panelNetwork = new FlowLayoutPanel();
        _panelDiscord = new FlowLayoutPanel();
        _panelUpdate = new FlowLayoutPanel();
        _panelDebug = new FlowLayoutPanel();
        SuspendLayout();

        // 
        // _rootPanel
        // 
        _rootPanel.ColumnCount = 2;
        _rootPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _rootPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F));
        _rootPanel.Controls.Add(_tabControl, 0, 0);
        _rootPanel.Controls.Add(_infoPanel, 1, 0);
        _rootPanel.Controls.Add(_buttonPanel, 0, 1);
        _rootPanel.Dock = DockStyle.Fill;
        _rootPanel.Padding = new Padding(12);
        _rootPanel.RowCount = 2;
        _rootPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _rootPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
        _rootPanel.SetColumnSpan(_buttonPanel, 2);

        // 
        // _tabControl
        // 
        _tabControl.Controls.Add(_tabDisplay);
        _tabControl.Controls.Add(_tabAudio);
        _tabControl.Controls.Add(_tabInput);
        _tabControl.Controls.Add(_tabControls);
        _tabControl.Controls.Add(_tabGame);
        _tabControl.Controls.Add(_tabPaths);
        _tabControl.Controls.Add(_tabNetwork);
        _tabControl.Controls.Add(_tabDiscord);
        _tabControl.Controls.Add(_tabUpdate);
        _tabControl.Controls.Add(_tabDebug);
        _tabControl.Dock = DockStyle.Fill;

        // 
        // _infoPanel
        // 
        _infoPanel.BackColor = Color.White;
        _infoPanel.BorderStyle = BorderStyle.FixedSingle;
        _infoPanel.Controls.Add(_infoBodyHost);
        _infoPanel.Controls.Add(_infoTitle);
        _infoPanel.Dock = DockStyle.Fill;
        _infoPanel.Margin = new Padding(12, 0, 0, 0);
        _infoPanel.Padding = new Padding(4, 8, 4, 8);

        // 
        // _infoTitle
        // 
        _infoTitle.Dock = DockStyle.Top;
        _infoTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _infoTitle.ForeColor = Color.FromArgb(30, 30, 30);
        _infoTitle.Height = 34;
        _infoTitle.Padding = new Padding(12, 4, 12, 6);
        _infoTitle.Text = "Setting Info";
        _infoTitle.TextAlign = ContentAlignment.MiddleLeft;

        // 
        // _infoBodyHost
        // 
        // Host panel gives us symmetric padding around the text, which
        // RichTextBox itself cannot provide.
        // 
        _infoBodyHost.BackColor = Color.White;
        _infoBodyHost.Controls.Add(_infoBody);
        _infoBodyHost.Dock = DockStyle.Fill;
        _infoBodyHost.Padding = new Padding(10, 6, 10, 10);

        // 
        // _infoBody
        // 
        _infoBody.BackColor = Color.White;
        _infoBody.BorderStyle = BorderStyle.None;
        _infoBody.DetectUrls = false;
        _infoBody.Dock = DockStyle.Fill;
        _infoBody.ForeColor = Color.FromArgb(60, 60, 60);
        _infoBody.ReadOnly = true;
        _infoBody.ScrollBars = RichTextBoxScrollBars.Vertical;
        _infoBody.TabStop = false;
        _infoBody.Text = "Hover over a setting to see what it does.";

        // 
        // _tabDisplay
        // 
        _tabDisplay.AutoScroll = true;
        _tabDisplay.Controls.Add(_panelDisplay);
        _tabDisplay.Location = new Point(4, 29);
        _tabDisplay.Name = "_tabDisplay";
        _tabDisplay.Padding = new Padding(3);
        _tabDisplay.Size = new Size(580, 510);
        _tabDisplay.TabIndex = 0;
        _tabDisplay.Text = "Display";
        _tabDisplay.UseVisualStyleBackColor = true;

        // 
        // _panelDisplay
        // 
        _panelDisplay.AutoScroll = true;
        _panelDisplay.Dock = DockStyle.Fill;
        _panelDisplay.FlowDirection = FlowDirection.TopDown;
        _panelDisplay.Location = new Point(3, 3);
        _panelDisplay.Name = "_panelDisplay";
        _panelDisplay.Padding = new Padding(12);
        _panelDisplay.Size = new Size(574, 504);
        _panelDisplay.TabIndex = 0;
        _panelDisplay.WrapContents = false;

        // 
        // _tabAudio
        // 
        _tabAudio.AutoScroll = true;
        _tabAudio.Controls.Add(_panelAudio);
        _tabAudio.Location = new Point(4, 29);
        _tabAudio.Name = "_tabAudio";
        _tabAudio.Padding = new Padding(3);
        _tabAudio.Size = new Size(580, 510);
        _tabAudio.TabIndex = 1;
        _tabAudio.Text = "Audio";
        _tabAudio.UseVisualStyleBackColor = true;

        // 
        // _panelAudio
        // 
        _panelAudio.AutoScroll = true;
        _panelAudio.Dock = DockStyle.Fill;
        _panelAudio.FlowDirection = FlowDirection.TopDown;
        _panelAudio.Location = new Point(3, 3);
        _panelAudio.Name = "_panelAudio";
        _panelAudio.Padding = new Padding(12);
        _panelAudio.Size = new Size(574, 504);
        _panelAudio.TabIndex = 0;
        _panelAudio.WrapContents = false;

        // 
        // _tabInput
        // 
        _tabInput.AutoScroll = true;
        _tabInput.Controls.Add(_panelInput);
        _tabInput.Location = new Point(4, 29);
        _tabInput.Name = "_tabInput";
        _tabInput.Padding = new Padding(3);
        _tabInput.Size = new Size(580, 510);
        _tabInput.TabIndex = 2;
        _tabInput.Text = "Input";
        _tabInput.UseVisualStyleBackColor = true;

        // 
        // _panelInput
        // 
        _panelInput.AutoScroll = true;
        _panelInput.Dock = DockStyle.Fill;
        _panelInput.FlowDirection = FlowDirection.TopDown;
        _panelInput.Location = new Point(3, 3);
        _panelInput.Name = "_panelInput";
        _panelInput.Padding = new Padding(12);
        _panelInput.Size = new Size(574, 504);
        _panelInput.TabIndex = 0;
        _panelInput.WrapContents = false;

        // 
        // _tabControls
        // 
        _tabControls.AutoScroll = true;
        _tabControls.Controls.Add(_panelControls);
        _tabControls.Location = new Point(4, 29);
        _tabControls.Name = "_tabControls";
        _tabControls.Padding = new Padding(3);
        _tabControls.Size = new Size(580, 510);
        _tabControls.TabIndex = 3;
        _tabControls.Text = "Controls";
        _tabControls.UseVisualStyleBackColor = true;

        // 
        // _panelControls
        // 
        _panelControls.AutoScroll = true;
        _panelControls.Dock = DockStyle.Fill;
        _panelControls.FlowDirection = FlowDirection.TopDown;
        _panelControls.Location = new Point(3, 3);
        _panelControls.Name = "_panelControls";
        _panelControls.Padding = new Padding(12);
        _panelControls.Size = new Size(574, 504);
        _panelControls.TabIndex = 0;
        _panelControls.WrapContents = false;

        // 
        // _tabGame
        // 
        _tabGame.AutoScroll = true;
        _tabGame.Controls.Add(_panelGame);
        _tabGame.Location = new Point(4, 29);
        _tabGame.Name = "_tabGame";
        _tabGame.Padding = new Padding(3);
        _tabGame.Size = new Size(580, 510);
        _tabGame.TabIndex = 4;
        _tabGame.Text = "Game";
        _tabGame.UseVisualStyleBackColor = true;

        // 
        // _panelGame
        // 
        _panelGame.AutoScroll = true;
        _panelGame.Dock = DockStyle.Fill;
        _panelGame.FlowDirection = FlowDirection.TopDown;
        _panelGame.Location = new Point(3, 3);
        _panelGame.Name = "_panelGame";
        _panelGame.Padding = new Padding(12);
        _panelGame.Size = new Size(574, 504);
        _panelGame.TabIndex = 0;
        _panelGame.WrapContents = false;

        // 
        // _tabPaths
        // 
        _tabPaths.AutoScroll = true;
        _tabPaths.Controls.Add(_panelPaths);
        _tabPaths.Location = new Point(4, 29);
        _tabPaths.Name = "_tabPaths";
        _tabPaths.Padding = new Padding(3);
        _tabPaths.Size = new Size(580, 510);
        _tabPaths.TabIndex = 5;
        _tabPaths.Text = "Paths";
        _tabPaths.UseVisualStyleBackColor = true;

        // 
        // _panelPaths
        // 
        _panelPaths.AutoScroll = true;
        _panelPaths.Dock = DockStyle.Fill;
        _panelPaths.FlowDirection = FlowDirection.TopDown;
        _panelPaths.Location = new Point(3, 3);
        _panelPaths.Name = "_panelPaths";
        _panelPaths.Padding = new Padding(12);
        _panelPaths.Size = new Size(574, 504);
        _panelPaths.TabIndex = 0;
        _panelPaths.WrapContents = false;

        // 
        // _tabNetwork
        // 
        _tabNetwork.AutoScroll = true;
        _tabNetwork.Controls.Add(_panelNetwork);
        _tabNetwork.Location = new Point(4, 29);
        _tabNetwork.Name = "_tabNetwork";
        _tabNetwork.Padding = new Padding(3);
        _tabNetwork.Size = new Size(580, 510);
        _tabNetwork.TabIndex = 6;
        _tabNetwork.Text = "Network";
        _tabNetwork.UseVisualStyleBackColor = true;

        // 
        // _panelNetwork
        // 
        _panelNetwork.AutoScroll = true;
        _panelNetwork.Dock = DockStyle.Fill;
        _panelNetwork.FlowDirection = FlowDirection.TopDown;
        _panelNetwork.Location = new Point(3, 3);
        _panelNetwork.Name = "_panelNetwork";
        _panelNetwork.Padding = new Padding(12);
        _panelNetwork.Size = new Size(574, 504);
        _panelNetwork.TabIndex = 0;
        _panelNetwork.WrapContents = false;

        // 
        // _tabDiscord
        // 
        _tabDiscord.AutoScroll = true;
        _tabDiscord.Controls.Add(_panelDiscord);
        _tabDiscord.Location = new Point(4, 29);
        _tabDiscord.Name = "_tabDiscord";
        _tabDiscord.Padding = new Padding(3);
        _tabDiscord.Size = new Size(580, 510);
        _tabDiscord.TabIndex = 7;
        _tabDiscord.Text = "Discord";
        _tabDiscord.UseVisualStyleBackColor = true;

        // 
        // _panelDiscord
        // 
        _panelDiscord.AutoScroll = true;
        _panelDiscord.Dock = DockStyle.Fill;
        _panelDiscord.FlowDirection = FlowDirection.TopDown;
        _panelDiscord.Location = new Point(3, 3);
        _panelDiscord.Name = "_panelDiscord";
        _panelDiscord.Padding = new Padding(12);
        _panelDiscord.Size = new Size(574, 504);
        _panelDiscord.TabIndex = 0;
        _panelDiscord.WrapContents = false;

        // 
        // _tabUpdate
        // 
        _tabUpdate.AutoScroll = true;
        _tabUpdate.Controls.Add(_panelUpdate);
        _tabUpdate.Location = new Point(4, 29);
        _tabUpdate.Name = "_tabUpdate";
        _tabUpdate.Padding = new Padding(3);
        _tabUpdate.Size = new Size(580, 510);
        _tabUpdate.TabIndex = 8;
        _tabUpdate.Text = "Update";
        _tabUpdate.UseVisualStyleBackColor = true;

        // 
        // _panelUpdate
        // 
        _panelUpdate.AutoScroll = true;
        _panelUpdate.Dock = DockStyle.Fill;
        _panelUpdate.FlowDirection = FlowDirection.TopDown;
        _panelUpdate.Location = new Point(3, 3);
        _panelUpdate.Name = "_panelUpdate";
        _panelUpdate.Padding = new Padding(12);
        _panelUpdate.Size = new Size(574, 504);
        _panelUpdate.TabIndex = 0;
        _panelUpdate.WrapContents = false;

        // 
        // _tabDebug
        // 
        _tabDebug.AutoScroll = true;
        _tabDebug.Controls.Add(_panelDebug);
        _tabDebug.Location = new Point(4, 29);
        _tabDebug.Name = "_tabDebug";
        _tabDebug.Padding = new Padding(3);
        _tabDebug.Size = new Size(580, 510);
        _tabDebug.TabIndex = 9;
        _tabDebug.Text = "Debug";
        _tabDebug.UseVisualStyleBackColor = true;

        // 
        // _panelDebug
        // 
        _panelDebug.AutoScroll = true;
        _panelDebug.Dock = DockStyle.Fill;
        _panelDebug.FlowDirection = FlowDirection.TopDown;
        _panelDebug.Location = new Point(3, 3);
        _panelDebug.Name = "_panelDebug";
        _panelDebug.Padding = new Padding(12);
        _panelDebug.Size = new Size(574, 504);
        _panelDebug.TabIndex = 0;
        _panelDebug.WrapContents = false;

        // 
        // _buttonPanel
        // 
        _buttonPanel.ColumnCount = 3;
        _buttonPanel.RowCount = 1;
        _buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _buttonPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _buttonPanel.Controls.Add(_resetButton, 0, 0);
        _buttonPanel.Controls.Add(_saveButtonsPanel, 2, 0);
        _buttonPanel.Dock = DockStyle.Fill;
        _buttonPanel.Margin = new Padding(0);

        // 
        // _resetButton
        // 
        _resetButton.Anchor = AnchorStyles.Left;
        _resetButton.Height = 36;
        _resetButton.Margin = new Padding(0);
        _resetButton.Text = "Reset to Defaults";
        _resetButton.Width = 160;
        _resetButton.Click += ResetButton_Click;

        // 
        // _saveButtonsPanel
        // 
        _saveButtonsPanel.Anchor = AnchorStyles.Right;
        _saveButtonsPanel.AutoSize = true;
        _saveButtonsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _saveButtonsPanel.Controls.Add(_saveAndExitButton);
        _saveButtonsPanel.Controls.Add(_saveAndLaunchButton);
        _saveButtonsPanel.Controls.Add(_saveButton);
        _saveButtonsPanel.FlowDirection = FlowDirection.RightToLeft;
        _saveButtonsPanel.Margin = new Padding(0);
        _saveButtonsPanel.WrapContents = false;

        // 
        // _saveButton
        // 
        _saveButton.Height = 36;
        _saveButton.Margin = new Padding(6);
        _saveButton.Text = "Save";
        _saveButton.Width = 120;
        _saveButton.Click += SaveButton_Click;

        // 
        // _saveAndLaunchButton
        // 
        _saveAndLaunchButton.Height = 36;
        _saveAndLaunchButton.Margin = new Padding(6);
        _saveAndLaunchButton.Text = "Save && Launch Halo";
        _saveAndLaunchButton.Width = 180;
        _saveAndLaunchButton.Click += SaveAndLaunchButton_Click;

        // 
        // _saveAndExitButton
        // 
        _saveAndExitButton.Height = 36;
        _saveAndExitButton.Margin = new Padding(6);
        _saveAndExitButton.Text = "Save && Exit";
        _saveAndExitButton.Width = 130;
        _saveAndExitButton.Click += SaveAndExitButton_Click;

        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(910, 600);
        Controls.Add(_rootPanel);
        Font = new Font("Segoe UI", 10F);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        KeyPreview = true;
        MaximizeBox = false;
        MinimumSize = new Size(700, 500);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Halo Config Editor";
        MouseDown += HandleMouseDown;
        ResumeLayout(false);
    }
}