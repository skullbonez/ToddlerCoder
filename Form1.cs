namespace ToddlerCoder;

public partial class Form1 : Form
{
    private readonly bool _kioskMode;
    private readonly List<string> _lines = [];
    private readonly List<Sparkle> _sparkles = [];
    private readonly Rectangle[] _particleButtonBounds = new Rectangle[4];
    private readonly Queue<string> _terminalLines = [];
    private readonly Queue<DiffLine> _diffLines = [];
    private readonly System.Windows.Forms.Timer _paintTimer = new();
    private readonly Random _random = new();
    private readonly Font _codeFont;
    private readonly Font _diffFont;
    private readonly Font _smallFont;
    private readonly Font _tinyFont;
    private readonly Font _titleFont;
    private readonly Brush _appBackgroundBrush = new SolidBrush(Color.FromArgb(13, 17, 23));
    private readonly Brush _panelBrush = new SolidBrush(Color.FromArgb(22, 27, 34));
    private readonly Brush _panelSoftBrush = new SolidBrush(Color.FromArgb(28, 34, 43));
    private readonly Brush _sidebarBrush = new SolidBrush(Color.FromArgb(18, 22, 29));
    private readonly Brush _gutterBrush = new SolidBrush(Color.FromArgb(17, 21, 27));
    private readonly Brush _lineNumberBrush = new SolidBrush(Color.FromArgb(99, 113, 128));
    private readonly Brush _normalCodeBrush = new SolidBrush(Color.FromArgb(214, 223, 231));
    private readonly Brush _keywordBrush = new SolidBrush(Color.FromArgb(111, 211, 187));
    private readonly Brush _stringBrush = new SolidBrush(Color.FromArgb(242, 191, 111));
    private readonly Brush _commentBrush = new SolidBrush(Color.FromArgb(119, 139, 151));
    private readonly Brush _numberBrush = new SolidBrush(Color.FromArgb(169, 205, 255));
    private readonly Brush _accentBrush = new SolidBrush(Color.FromArgb(139, 171, 255));
    private readonly Brush _softTextBrush = new SolidBrush(Color.FromArgb(159, 172, 184));
    private readonly Brush _mutedTextBrush = new SolidBrush(Color.FromArgb(114, 127, 140));
    private readonly Brush _activeBrush = new SolidBrush(Color.FromArgb(39, 48, 61));
    private readonly Brush _plusBackgroundBrush = new SolidBrush(Color.FromArgb(22, 54, 42));
    private readonly Brush _minusBackgroundBrush = new SolidBrush(Color.FromArgb(61, 35, 37));
    private readonly Brush _plusTextBrush = new SolidBrush(Color.FromArgb(151, 235, 178));
    private readonly Brush _minusTextBrush = new SolidBrush(Color.FromArgb(255, 165, 165));
    private readonly Pen _dividerPen = new(Color.FromArgb(47, 57, 69));
    private readonly Pen _softDividerPen = new(Color.FromArgb(34, 42, 52));
    private readonly Pen _cursorPen = new(Color.FromArgb(242, 191, 111), 2);

    private KeyboardGuard? _keyboardGuard;
    private int _scriptIndex;
    private int _typedLength;
    private int _keyCount;
    private int _pulse;
    private int _activeProjectIndex;
    private int _particleOptionIndex;
    private Point _typingParticleOrigin;
    private string _currentBanner = "";
    private int _bannerTicks;
    private float _exitHoldProgress;
    private DateTimeOffset? _exitHoldStartedAt;
    private bool _allowClose;

    private const double ExitHoldSeconds = 3;

    private static readonly ProjectInfo[] Projects =
    [
        new("blocks-bot", "build helper", 42, ["Builder.cs", "Robot.cs", "Blocks.test.cs", "Tower.cs", "StackRules.cs", "BuildSounds.cs", "BlockColors.cs"]),
        new("moon-lights", "soft glow", 68, ["Glow.cs", "Moon.cs", "NightMode.cs", "Stars.cs", "SleepySky.cs", "Dimmer.cs", "Clouds.cs"]),
        new("snack-timer", "very important", 17, ["Timer.cs", "Crackers.cs", "Milk.cs", "SnackBell.cs", "TinyPlate.cs", "Napkin.cs", "Refill.cs"]),
        new("train-builder", "tiny engine", 84, ["Track.cs", "Engine.cs", "Tunnel.cs", "Signals.cs", "Carriages.cs", "Bridge.cs", "Station.cs"]),
        new("button-lab", "tap tests", 31, ["Buttons.cs", "Beep.cs", "Squish.cs", "Knobs.cs", "Switches.cs", "ButtonTests.cs", "Lights.cs"]),
    ];

    private static readonly ParticleOption[] ParticleOptions =
    [
        new("sunny", ParticleMode.Stars, new[]
        {
            Color.FromArgb(255, 220, 112),
            Color.FromArgb(242, 191, 111),
            Color.FromArgb(255, 243, 181),
        }),
        new("ocean", ParticleMode.Dots, new[]
        {
            Color.FromArgb(118, 198, 255),
            Color.FromArgb(139, 171, 255),
            Color.FromArgb(111, 211, 187),
        }),
        new("garden", ParticleMode.Blocks, new[]
        {
            Color.FromArgb(132, 222, 151),
            Color.FromArgb(111, 211, 187),
            Color.FromArgb(190, 230, 125),
        }),
        new("candy", ParticleMode.Pluses, new[]
        {
            Color.FromArgb(255, 159, 203),
            Color.FromArgb(199, 160, 255),
            Color.FromArgb(255, 196, 222),
        }),
    ];

    private static readonly string[] Script =
    [
        "using TinyHands.Playground;",
        "using TinyHands.Review;",
        "",
        "var plan = new ProjectPlan(\"blocks-bot\");",
        "plan.AddStep(\"open workspace\");",
        "plan.AddStep(\"write helpful code\");",
        "plan.AddStep(\"make daddy proud\");",
        "",
        "while (keyboard.IsMashing)",
        "{",
        "    editor.TypeLikeDaddy();",
        "    diff.ShowTinyChanges();",
        "    tests.RunSoftly();",
        "    build.SaveProgress();",
        "}",
        "",
        "if (snackTime.IsReady)",
        "{",
        "    console.WriteLine(\"ship snack timer\");",
        "    project.Status = Status.Ready;",
        "}",
        "",
        "for (var block = 0; block < 10; block++)",
        "{",
        "    tower.Place(block);",
        "    lights.GlowSoftly();",
        "    robot.Wave();",
        "}",
        "",
        "// review notes from the tiny teammate",
        "review.MarkNice(\"gentle colors\");",
        "review.MarkNice(\"good button noises\");",
        "review.Approve();",
        "",
        "await cloud.SendHighFiveAsync();",
        "workspace.Commit(\"tiny coder changes\");",
        "console.WriteLine(\"build succeeded\");",
    ];

    private static readonly string[] TerminalMessages =
    [
        "[ok] saved blocks-bot",
        "[run] drawing calm stars",
        "[test] buttons are working",
        "[build] checking tiny project",
        "[ok] robot wave complete",
        "[run] reviewing changes",
        "[ok] build succeeded",
        "[save] all work tucked in",
    ];

    private static readonly (string Minus, string Plus)[] DiffSnippets =
    [
        ("robot.Speed = Fast;", "robot.Speed = Gentle;"),
        ("screen.Theme = Theme.Bright;", "screen.Theme = Theme.Calm;"),
        ("tower.Blocks = 4;", "tower.Blocks = 10;"),
        ("snack.Ready = false;", "snack.Ready = true;"),
        ("button.Sound = Loud;", "button.Sound = SoftBeep;"),
        ("review.Status = Pending;", "review.Status = Approved;"),
        ("lights.Mode = Flash;", "lights.Mode = Glow;"),
        ("train.Cars = 1;", "train.Cars = 3;"),
    ];

    private static readonly HashSet<string> Keywords =
    [
        "await", "bool", "class", "const", "false", "for", "if", "int", "new",
        "return", "static", "string", "true", "using", "var", "while"
    ];

    public Form1(bool kioskMode)
    {
        _kioskMode = kioskMode;
        InitializeComponent();
        DoubleBuffered = true;
        KeyPreview = true;
        BackColor = Color.FromArgb(13, 17, 23);
        _codeFont = new Font("Consolas", 30f, FontStyle.Regular, GraphicsUnit.Pixel);
        _diffFont = new Font("Consolas", 18f, FontStyle.Regular, GraphicsUnit.Pixel);
        _smallFont = new Font("Segoe UI", 16f, FontStyle.Regular, GraphicsUnit.Pixel);
        _tinyFont = new Font("Segoe UI", 13f, FontStyle.Regular, GraphicsUnit.Pixel);
        _titleFont = new Font("Segoe UI Semibold", 20f, FontStyle.Regular, GraphicsUnit.Pixel);

        _lines.Add("");
        _terminalLines.Enqueue("[ready] workspace opened");
        _terminalLines.Enqueue("[hint] keyboard connected");
        SeedDiff();

        ConfigureWindow();

        _paintTimer.Interval = 120;
        _paintTimer.Tick += (_, _) =>
        {
            _pulse++;
            UpdateSparkles();
            UpdateExitHold();

            if (_bannerTicks > 0)
            {
                _bannerTicks--;
            }

            Invalidate();
        };
        _paintTimer.Start();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        if (_kioskMode)
        {
            Bounds = Screen.FromControl(this).Bounds;
            WindowState = FormWindowState.Maximized;
            _keyboardGuard = new KeyboardGuard();
        }

        Activate();
        Focus();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (TryHandleParentExit(keyData))
        {
            return true;
        }

        HandleMash(keyData);
        return true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (TryHandleParentExit(e.KeyData))
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            return;
        }

        HandleMash(e.KeyData);
        e.Handled = true;
        e.SuppressKeyPress = true;
        base.OnKeyDown(e);
    }

    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        e.Handled = true;
        base.OnKeyPress(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        AddSparkles(e.Location);
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.Button == MouseButtons.None)
        {
            return;
        }

        if (e.Button == MouseButtons.Left)
        {
            TrySelectParticleMode(e.Location);
        }

        AddSparkles(e.Location, _random.Next(20, 26));
        Invalidate();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_kioskMode && !_allowClose && e.CloseReason is not CloseReason.TaskManagerClosing and not CloseReason.WindowsShutDown)
        {
            e.Cancel = true;
            return;
        }

        base.OnFormClosing(e);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _keyboardGuard?.Dispose();
        _paintTimer.Dispose();
        _codeFont.Dispose();
        _diffFont.Dispose();
        _smallFont.Dispose();
        _tinyFont.Dispose();
        _titleFont.Dispose();
        _dividerPen.Dispose();
        _softDividerPen.Dispose();
        _cursorPen.Dispose();
        _appBackgroundBrush.Dispose();
        _panelBrush.Dispose();
        _panelSoftBrush.Dispose();
        _sidebarBrush.Dispose();
        _gutterBrush.Dispose();
        _lineNumberBrush.Dispose();
        _normalCodeBrush.Dispose();
        _keywordBrush.Dispose();
        _stringBrush.Dispose();
        _commentBrush.Dispose();
        _numberBrush.Dispose();
        _accentBrush.Dispose();
        _softTextBrush.Dispose();
        _mutedTextBrush.Dispose();
        _activeBrush.Dispose();
        _plusBackgroundBrush.Dispose();
        _minusBackgroundBrush.Dispose();
        _plusTextBrush.Dispose();
        _minusTextBrush.Dispose();
        base.OnFormClosed(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;
        g.Clear(Color.FromArgb(13, 17, 23));
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        int width = ClientSize.Width;
        int height = ClientSize.Height;
        int headerHeight = 50;
        int statusHeight = 32;
        int sidebarWidth = Math.Clamp(width / 5, 210, 300);
        int diffWidth = Math.Clamp((int)(width * 0.38f), 360, 600);

        if (width - sidebarWidth - diffWidth < 390)
        {
            diffWidth = Math.Max(300, width - sidebarWidth - 390);
        }

        Rectangle header = new(0, 0, width, headerHeight);
        Rectangle status = new(0, height - statusHeight, width, statusHeight);
        Rectangle sidebar = new(0, header.Bottom, sidebarWidth, height - headerHeight - statusHeight);
        Rectangle diff = new(width - diffWidth, header.Bottom, diffWidth, height - headerHeight - statusHeight);
        Rectangle center = new(sidebar.Right, header.Bottom, diff.Left - sidebar.Right, height - headerHeight - statusHeight);

        DrawHeader(g, header);
        DrawSidebar(g, sidebar);
        DrawWorkspace(g, center);
        DrawDiffPane(g, diff);
        DrawStatus(g, status);
        DrawSparkles(g);
        DrawBanner(g, width);
    }

    private void ConfigureWindow()
    {
        Text = _kioskMode ? "Toddler Coder" : "Toddler Coder - Debug Windowed";
        StartPosition = FormStartPosition.CenterScreen;

        if (_kioskMode)
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            TopMost = false;
            MinimizeBox = false;
            MaximizeBox = false;
            ControlBox = false;
            ShowInTaskbar = true;
        }
        else
        {
            FormBorderStyle = FormBorderStyle.Sizable;
            Size = new Size(1280, 800);
            MinimumSize = new Size(980, 620);
        }
    }

    private bool TryHandleParentExit(Keys keyData)
    {
        Keys key = keyData & Keys.KeyCode;
        bool parentChord = key == Keys.Q
            && keyData.HasFlag(Keys.Control)
            && keyData.HasFlag(Keys.Shift);

        if (!parentChord)
        {
            return false;
        }

        StartExitHold();
        return true;
    }

    private void StartExitHold()
    {
        _exitHoldStartedAt ??= DateTimeOffset.UtcNow;
    }

    private void UpdateExitHold()
    {
        if (_allowClose)
        {
            return;
        }

        if (!IsExitChordDown())
        {
            _exitHoldStartedAt = null;
            _exitHoldProgress = 0;
            return;
        }

        _exitHoldStartedAt ??= DateTimeOffset.UtcNow;
        double elapsedSeconds = (DateTimeOffset.UtcNow - _exitHoldStartedAt.Value).TotalSeconds;
        _exitHoldProgress = Math.Clamp((float)(elapsedSeconds / ExitHoldSeconds), 0f, 1f);

        if (_exitHoldProgress < 1f)
        {
            return;
        }

        _allowClose = true;
        Close();
    }

    private static bool IsExitChordDown()
    {
        return IsKeyDown(Keys.Q)
            && (IsKeyDown(Keys.ControlKey) || IsKeyDown(Keys.LControlKey) || IsKeyDown(Keys.RControlKey))
            && (IsKeyDown(Keys.ShiftKey) || IsKeyDown(Keys.LShiftKey) || IsKeyDown(Keys.RShiftKey));
    }

    private void HandleMash(Keys keyData)
    {
        Keys key = keyData & Keys.KeyCode;

        if (key is Keys.None or Keys.ControlKey or Keys.ShiftKey or Keys.Menu)
        {
            return;
        }

        _keyCount++;

        int amount = key switch
        {
            Keys.Enter => 12,
            Keys.Space => 8,
            Keys.Back => 3,
            Keys.Tab => 10,
            _ => _random.Next(2, 7)
        };

        AdvanceTyping(amount);
        AddSparkles(GetTypingParticleOrigin(), _random.Next(5, 11));

        if (_keyCount % 4 == 0)
        {
            AddDiffChange();
        }

        if (_keyCount % 7 == 0)
        {
            AddTerminalMessage(TerminalMessages[_random.Next(TerminalMessages.Length)]);
        }

        if (_keyCount % 17 == 0)
        {
            _activeProjectIndex = (_activeProjectIndex + 1) % Projects.Length;
            AddDiffHeader(Projects[_activeProjectIndex]);
        }

        if (_keyCount % 31 == 0)
        {
            _currentBanner = _random.Next(3) switch
            {
                0 => "build succeeded",
                1 => "review approved",
                _ => "project saved"
            };
            _bannerTicks = 28;
        }

        Invalidate();
    }

    private void AdvanceTyping(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            string target = Script[_scriptIndex];

            if (_typedLength >= target.Length)
            {
                MoveToNextLine();
                continue;
            }

            _lines[^1] = target[..(_typedLength + 1)];
            _typedLength++;
        }
    }

    private void MoveToNextLine()
    {
        _scriptIndex = (_scriptIndex + 1) % Script.Length;
        _typedLength = 0;
        _lines.Add("");

        while (_lines.Count > 160)
        {
            _lines.RemoveAt(0);
        }
    }

    private void AddTerminalMessage(string message)
    {
        _terminalLines.Enqueue(message);

        while (_terminalLines.Count > 5)
        {
            _terminalLines.Dequeue();
        }
    }

    private bool TrySelectParticleMode(Point location)
    {
        for (int i = 0; i < _particleButtonBounds.Length && i < ParticleOptions.Length; i++)
        {
            if (!_particleButtonBounds[i].Contains(location))
            {
                continue;
            }

            _particleOptionIndex = i;
            _sparkles.Clear();
            return true;
        }

        return false;
    }

    private void AddSparkles(Point location, int countOverride = 0)
    {
        int count = countOverride > 0 ? countOverride : _random.Next(1, 3);
        ParticleOption selectedOption = ParticleOptions[_particleOptionIndex];

        for (int i = 0; i < count; i++)
        {
            float angle = (float)(_random.NextDouble() * Math.PI * 2);
            float speed = 0.7f + (float)_random.NextDouble() * 1.4f;
            Color color = selectedOption.Palette[_random.Next(selectedOption.Palette.Length)];

            _sparkles.Add(new Sparkle(
                new PointF(
                    location.X + _random.Next(-6, 7),
                    location.Y + _random.Next(-6, 7)),
                new PointF(
                    MathF.Cos(angle) * speed,
                    MathF.Sin(angle) * speed - 0.4f),
                age: 0,
                lifespan: _random.Next(14, 24),
                size: 3.5f + (float)_random.NextDouble() * 4.5f,
                color,
                selectedOption.Mode));
        }

        while (_sparkles.Count > 90)
        {
            _sparkles.RemoveAt(0);
        }
    }

    private void UpdateSparkles()
    {
        for (int i = _sparkles.Count - 1; i >= 0; i--)
        {
            Sparkle sparkle = _sparkles[i];
            sparkle.Age++;
            sparkle.Position = new PointF(
                sparkle.Position.X + sparkle.Velocity.X,
                sparkle.Position.Y + sparkle.Velocity.Y);
            sparkle.Velocity = new PointF(sparkle.Velocity.X * 0.93f, sparkle.Velocity.Y * 0.93f + 0.03f);

            if (sparkle.Age >= sparkle.Lifespan)
            {
                _sparkles.RemoveAt(i);
            }
            else
            {
                _sparkles[i] = sparkle;
            }
        }
    }

    private void SeedDiff()
    {
        AddDiffHeader(Projects[0]);
        _diffLines.Enqueue(new DiffLine(' ', "  while (keyboard.IsMashing)"));
        _diffLines.Enqueue(new DiffLine('-', "      screen.Theme = Theme.Bright;"));
        _diffLines.Enqueue(new DiffLine('+', "      screen.Theme = Theme.Calm;"));
        _diffLines.Enqueue(new DiffLine('+', "      diff.ShowTinyChanges();"));
    }

    private void AddDiffHeader(ProjectInfo project)
    {
        _diffLines.Enqueue(new DiffLine(' ', $"diff --git a/{project.Name}/Builder.cs b/{project.Name}/Builder.cs"));
        _diffLines.Enqueue(new DiffLine(' ', "@@ tiny workspace @@"));
        TrimDiff();
    }

    private void AddDiffChange()
    {
        (string minus, string plus) = DiffSnippets[_random.Next(DiffSnippets.Length)];
        _diffLines.Enqueue(new DiffLine(' ', "  tiny.ChangeSet.Apply();"));
        _diffLines.Enqueue(new DiffLine('-', $"  {minus}"));
        _diffLines.Enqueue(new DiffLine('+', $"  {plus}"));
        TrimDiff();
    }

    private void TrimDiff()
    {
        while (_diffLines.Count > 60)
        {
            _diffLines.Dequeue();
        }
    }

    private void DrawHeader(Graphics g, Rectangle bounds)
    {
        g.FillRectangle(_panelBrush, bounds);
        g.DrawLine(_dividerPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);

        using Brush dotRed = new SolidBrush(Color.FromArgb(239, 112, 112));
        using Brush dotYellow = new SolidBrush(Color.FromArgb(242, 191, 111));
        using Brush dotGreen = new SolidBrush(Color.FromArgb(112, 211, 151));
        using Brush logoBrush = new SolidBrush(Color.FromArgb(36, 45, 57));

        g.FillEllipse(dotRed, 18, 19, 12, 12);
        g.FillEllipse(dotYellow, 38, 19, 12, 12);
        g.FillEllipse(dotGreen, 58, 19, 12, 12);

        Rectangle logo = new(88, 11, 104, 28);
        g.FillRectangle(logoBrush, logo);
        DrawText(g, "tiny codex", _smallFont, _normalCodeBrush, logo, StringAlignment.Center, StringAlignment.Center);

        Rectangle title = new(212, 12, Math.Max(100, bounds.Width - 480), 28);
        DrawText(g, "workspace / little-coder", _titleFont, _normalCodeBrush, title);

        string mode = _kioskMode ? "kid mode" : "debug windowed";
        Rectangle modeBounds = new(bounds.Right - 180, 14, 150, 24);
        DrawText(g, mode, _smallFont, _softTextBrush, modeBounds, StringAlignment.Far);
    }

    private void DrawSidebar(Graphics g, Rectangle bounds)
    {
        g.FillRectangle(_sidebarBrush, bounds);
        g.DrawLine(_dividerPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);

        Rectangle title = new(bounds.Left + 18, bounds.Top + 18, bounds.Width - 36, 24);
        DrawText(g, "Projects", _titleFont, _normalCodeBrush, title);

        Rectangle subtitle = new(bounds.Left + 18, bounds.Top + 45, bounds.Width - 36, 20);
        DrawText(g, "tiny workspaces", _smallFont, _mutedTextBrush, subtitle);

        int y = bounds.Top + 82;
        for (int i = 0; i < Projects.Length; i++)
        {
            DrawProjectItem(g, bounds, Projects[i], i, y);
            y += 72;
        }

        int fileTop = y + 20;
        g.DrawLine(_softDividerPen, bounds.Left + 18, fileTop, bounds.Right - 18, fileTop);
        DrawText(g, "Files", _smallFont, _softTextBrush, new Rectangle(bounds.Left + 18, fileTop + 18, bounds.Width - 36, 22));

        string[] files = Projects[_activeProjectIndex].Files;
        y = fileTop + 50;
        foreach (string file in files)
        {
            if (y + 24 > bounds.Bottom - 12)
            {
                break;
            }

            Rectangle fileBounds = new(bounds.Left + 28, y, bounds.Width - 46, 24);
            DrawText(g, file, _smallFont, _normalCodeBrush, fileBounds);
            y += 26;
        }
    }

    private void DrawProjectItem(Graphics g, Rectangle sidebar, ProjectInfo project, int index, int y)
    {
        Rectangle item = new(sidebar.Left + 10, y, sidebar.Width - 20, 60);
        bool active = index == _activeProjectIndex;

        if (active)
        {
            g.FillRectangle(_activeBrush, item);
        }

        Rectangle nameBounds = new(item.Left + 14, item.Top + 9, item.Width - 28, 20);
        Rectangle detailBounds = new(item.Left + 14, item.Top + 30, item.Width - 28, 18);
        DrawText(g, project.Name, _smallFont, active ? _normalCodeBrush : _softTextBrush, nameBounds);
        DrawText(g, project.Detail, _tinyFont, _mutedTextBrush, detailBounds);

        int progress = Math.Clamp(project.BaseProgress + (active ? _keyCount % 30 : 0), 0, 98);
        Rectangle track = new(item.Left + 14, item.Bottom - 8, item.Width - 28, 3);
        using Brush trackBrush = new SolidBrush(Color.FromArgb(43, 52, 63));
        using Brush fillBrush = new SolidBrush(active ? Color.FromArgb(111, 211, 187) : Color.FromArgb(91, 107, 123));
        g.FillRectangle(trackBrush, track);
        g.FillRectangle(fillBrush, track.Left, track.Top, Math.Max(4, track.Width * progress / 100), track.Height);
    }

    private void DrawWorkspace(Graphics g, Rectangle bounds)
    {
        g.FillRectangle(_appBackgroundBrush, bounds);
        g.DrawLine(_dividerPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom);

        int terminalHeight = Math.Clamp(bounds.Height / 4, 120, 178);
        Rectangle editorHeader = new(bounds.Left, bounds.Top, bounds.Width, 42);
        Rectangle editor = new(bounds.Left, editorHeader.Bottom, bounds.Width, bounds.Height - terminalHeight - editorHeader.Height);
        Rectangle terminal = new(bounds.Left, editor.Bottom, bounds.Width, terminalHeight);

        DrawEditorHeader(g, editorHeader);
        DrawEditor(g, editor);
        DrawTerminal(g, terminal);
    }

    private void DrawEditorHeader(Graphics g, Rectangle bounds)
    {
        g.FillRectangle(_panelBrush, bounds);
        g.DrawLine(_softDividerPen, bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);

        Rectangle tab = new(bounds.Left + 16, bounds.Top + 7, Math.Min(320, bounds.Width - 32), 30);
        using Brush tabBrush = new SolidBrush(Color.FromArgb(31, 39, 50));
        g.FillRectangle(tabBrush, tab);
        DrawText(g, $"{Projects[_activeProjectIndex].Name}/Builder.cs", _smallFont, _normalCodeBrush, Inset(tab, 12, 5, 12, 4));
    }

    private void DrawEditor(Graphics g, Rectangle bounds)
    {
        int gutterWidth = Math.Clamp(bounds.Width / 10, 58, 92);
        Rectangle gutter = new(bounds.Left, bounds.Top, gutterWidth, bounds.Height);
        Rectangle codeArea = new(gutter.Right, bounds.Top, bounds.Width - gutterWidth, bounds.Height);

        g.FillRectangle(_appBackgroundBrush, bounds);
        g.FillRectangle(_gutterBrush, gutter);
        g.DrawLine(_softDividerPen, gutter.Right, gutter.Top, gutter.Right, gutter.Bottom);

        using Region previousClip = g.Clip.Clone();
        g.SetClip(bounds);

        float lineHeight = _codeFont.GetHeight(g) + 8;
        int maxLines = Math.Max(1, (int)((bounds.Height - 24) / lineHeight));
        int start = Math.Max(0, _lines.Count - maxLines);
        float y = bounds.Top + 14;

        for (int i = start; i < _lines.Count; i++)
        {
            string lineNumber = (i + 1).ToString();
            SizeF numberSize = g.MeasureString(lineNumber, _tinyFont);
            g.DrawString(lineNumber, _tinyFont, _lineNumberBrush, gutter.Right - numberSize.Width - 14, y + 5);
            DrawCodeLine(g, _lines[i], codeArea.Left + 20, y);
            y += lineHeight;
        }

        string currentLine = _lines.Count == 0 ? "" : _lines[^1];
        float cursorX = codeArea.Left + 20 + MeasureCode(g, currentLine);
        float cursorY = bounds.Top + 14 + ((_lines.Count - start - 1) * lineHeight);
        _typingParticleOrigin = new Point(
            Math.Clamp((int)cursorX + 4, bounds.Left + 12, bounds.Right - 12),
            Math.Clamp((int)(cursorY + lineHeight / 2f), bounds.Top + 12, bounds.Bottom - 12));

        if ((_pulse / 4) % 2 == 0)
        {
            g.DrawLine(_cursorPen, cursorX + 3, cursorY + 3, cursorX + 3, cursorY + lineHeight - 5);
        }

        g.Clip = previousClip;
    }

    private void DrawCodeLine(Graphics g, string line, float x, float y)
    {
        if (string.IsNullOrEmpty(line))
        {
            return;
        }

        if (line.TrimStart().StartsWith("//", StringComparison.Ordinal))
        {
            g.DrawString(line, _codeFont, _commentBrush, x, y);
            return;
        }

        int i = 0;
        float cursor = x;

        while (i < line.Length)
        {
            char c = line[i];

            if (c == '"')
            {
                int end = line.IndexOf('"', i + 1);
                end = end < 0 ? line.Length - 1 : end;
                string token = line[i..(end + 1)];
                g.DrawString(token, _codeFont, _stringBrush, cursor, y);
                cursor += MeasureCode(g, token);
                i = end + 1;
                continue;
            }

            if (char.IsLetter(c) || c == '_')
            {
                int start = i;
                while (i < line.Length && (char.IsLetterOrDigit(line[i]) || line[i] == '_'))
                {
                    i++;
                }

                string token = line[start..i];
                Brush tokenBrush = Keywords.Contains(token) ? _keywordBrush : _normalCodeBrush;
                g.DrawString(token, _codeFont, tokenBrush, cursor, y);
                cursor += MeasureCode(g, token);
                continue;
            }

            if (char.IsDigit(c))
            {
                int start = i;
                while (i < line.Length && char.IsDigit(line[i]))
                {
                    i++;
                }

                string token = line[start..i];
                g.DrawString(token, _codeFont, _numberBrush, cursor, y);
                cursor += MeasureCode(g, token);
                continue;
            }

            string symbol = c.ToString();
            Brush brush = c is '(' or ')' or '{' or '}' or '[' or ']' ? _accentBrush : _normalCodeBrush;
            g.DrawString(symbol, _codeFont, brush, cursor, y);
            cursor += MeasureCode(g, symbol);
            i++;
        }
    }

    private float MeasureCode(Graphics g, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        using StringFormat format = (StringFormat)StringFormat.GenericTypographic.Clone();
        format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

        if (text.All(char.IsWhiteSpace))
        {
            return g.MeasureString("0", _codeFont, int.MaxValue, format).Width * text.Length;
        }

        return g.MeasureString(text, _codeFont, int.MaxValue, format).Width;
    }

    private void DrawTerminal(Graphics g, Rectangle bounds)
    {
        using Brush terminalBrush = new SolidBrush(Color.FromArgb(9, 13, 18));
        g.FillRectangle(terminalBrush, bounds);
        g.DrawLine(_dividerPen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);

        Rectangle title = new(bounds.Left + 18, bounds.Top + 12, bounds.Width - 36, 20);
        DrawText(g, "output", _smallFont, _softTextBrush, title);

        float y = bounds.Top + 40;
        foreach (string line in _terminalLines)
        {
            g.DrawString(line, _diffFont, _normalCodeBrush, bounds.Left + 20, y);
            y += _diffFont.GetHeight(g) + 5;
        }
    }

    private void DrawDiffPane(Graphics g, Rectangle bounds)
    {
        g.FillRectangle(_panelBrush, bounds);

        Rectangle header = new(bounds.Left, bounds.Top, bounds.Width, 78);
        g.FillRectangle(_panelSoftBrush, header);
        g.DrawLine(_dividerPen, bounds.Left, header.Bottom - 1, bounds.Right, header.Bottom - 1);

        DrawText(g, "Review", _titleFont, _normalCodeBrush, new Rectangle(header.Left + 18, header.Top + 16, header.Width - 36, 24));
        DrawText(g, "live diff", _smallFont, _mutedTextBrush, new Rectangle(header.Left + 18, header.Top + 42, header.Width - 36, 20));

        Rectangle fileBadge = new(bounds.Left + 18, header.Bottom + 14, bounds.Width - 36, 28);
        using Brush badgeBrush = new SolidBrush(Color.FromArgb(33, 41, 52));
        g.FillRectangle(badgeBrush, fileBadge);
        DrawText(g, $"{Projects[_activeProjectIndex].Name}/Builder.cs", _smallFont, _softTextBrush, Inset(fileBadge, 12, 5, 12, 4));

        Rectangle particlePanel = new(bounds.Left + 14, bounds.Bottom - 162, bounds.Width - 28, 146);
        Rectangle diffArea = new(bounds.Left, fileBadge.Bottom + 14, bounds.Width, particlePanel.Top - fileBadge.Bottom - 24);
        using Region previousClip = g.Clip.Clone();
        g.SetClip(diffArea);

        float lineHeight = _diffFont.GetHeight(g) + 7;
        int maxLines = Math.Max(1, (int)((diffArea.Height - 16) / lineHeight));
        DiffLine[] lines = _diffLines.ToArray();
        int start = Math.Max(0, lines.Length - maxLines);
        float y = diffArea.Top + 8;

        for (int i = start; i < lines.Length; i++)
        {
            DrawDiffLine(g, lines[i], diffArea, y, lineHeight);
            y += lineHeight;
        }

        g.Clip = previousClip;
        DrawParticleControls(g, particlePanel);
    }

    private void DrawParticleControls(Graphics g, Rectangle bounds)
    {
        using Brush controlsBrush = new SolidBrush(Color.FromArgb(18, 24, 31));
        using Brush activeButtonBrush = new SolidBrush(Color.FromArgb(45, 58, 72));
        using Brush inactiveButtonBrush = new SolidBrush(Color.FromArgb(28, 35, 44));
        using Pen activeButtonPen = new(Color.FromArgb(111, 211, 187));
        using Pen inactiveButtonPen = new(Color.FromArgb(53, 64, 77));

        g.FillRectangle(controlsBrush, bounds);
        g.DrawRectangle(_softDividerPen, bounds);

        DrawText(g, "mouse trail colors", _smallFont, _softTextBrush, new Rectangle(bounds.Left + 14, bounds.Top + 10, bounds.Width - 28, 22));

        int gap = 10;
        int buttonCount = ParticleOptions.Length;
        int columnCount = 2;
        int buttonWidth = Math.Max(100, (bounds.Width - 28 - gap) / columnCount);
        int buttonHeight = 42;
        int startY = bounds.Top + 44;
        int startX = bounds.Left + 14;

        for (int i = 0; i < buttonCount; i++)
        {
            ParticleOption option = ParticleOptions[i];
            int row = i / columnCount;
            int column = i % columnCount;
            int buttonX = startX + column * (buttonWidth + gap);
            int buttonY = startY + row * (buttonHeight + gap);
            Rectangle button = new(buttonX, buttonY, buttonWidth, buttonHeight);
            _particleButtonBounds[i] = button;

            bool active = i == _particleOptionIndex;
            g.FillRectangle(active ? activeButtonBrush : inactiveButtonBrush, button);
            g.DrawRectangle(active ? activeButtonPen : inactiveButtonPen, button);

            Color iconColor = option.Palette[0];
            using Pen iconPen = new(iconColor, 1.8f);
            using Brush iconBrush = new SolidBrush(iconColor);
            DrawParticleShape(g, option.Mode, new PointF(button.Left + 22, button.Top + button.Height / 2f), 13f, iconPen, iconBrush);

            for (int swatch = 0; swatch < option.Palette.Length; swatch++)
            {
                using Brush swatchBrush = new SolidBrush(option.Palette[swatch]);
                int swatchX = button.Right - 42 + swatch * 11;
                g.FillEllipse(swatchBrush, swatchX, button.Top + 12, 8, 8);
            }

            DrawText(g, option.Label, _smallFont, active ? _normalCodeBrush : _softTextBrush, new Rectangle(button.Left + 42, button.Top + 10, button.Width - 88, 22));
        }
    }

    private void DrawDiffLine(Graphics g, DiffLine line, Rectangle bounds, float y, float lineHeight)
    {
        RectangleF row = new(bounds.Left, y - 1, bounds.Width, lineHeight);
        Brush textBrush = _softTextBrush;

        if (line.Marker == '+')
        {
            g.FillRectangle(_plusBackgroundBrush, row);
            textBrush = _plusTextBrush;
        }
        else if (line.Marker == '-')
        {
            g.FillRectangle(_minusBackgroundBrush, row);
            textBrush = _minusTextBrush;
        }

        RectangleF marker = new(bounds.Left + 14, y + 3, 18, lineHeight);
        RectangleF text = new(bounds.Left + 36, y + 3, bounds.Width - 50, lineHeight);
        g.DrawString(line.Marker.ToString(), _diffFont, textBrush, marker);
        DrawText(g, line.Text, _diffFont, textBrush, text);
    }

    private void DrawStatus(Graphics g, Rectangle bounds)
    {
        using Brush statusBrush = new SolidBrush(Color.FromArgb(28, 49, 58));
        g.FillRectangle(statusBrush, bounds);

        string left = $"keys: {_keyCount}";
        string middle = $"project: {Projects[_activeProjectIndex].Name}";
        string adultNote = _exitHoldProgress > 0
            ? $"Exit hold: {(int)(_exitHoldProgress * 100)}%"
            : "Adults: hold Ctrl+Shift+Q 3s";
        string right = _kioskMode ? "kid mode" : "debug windowed";

        DrawText(g, left, _smallFont, Brushes.White, new Rectangle(bounds.Left + 18, bounds.Top + 7, 130, bounds.Height - 8));
        DrawText(g, middle, _smallFont, Brushes.White, new Rectangle(bounds.Left + 160, bounds.Top + 7, bounds.Width - 570, bounds.Height - 8));
        DrawText(g, adultNote, _tinyFont, _softTextBrush, new Rectangle(bounds.Right - 390, bounds.Top + 9, 210, bounds.Height - 8), StringAlignment.Far);
        DrawText(g, right, _smallFont, Brushes.White, new Rectangle(bounds.Right - 160, bounds.Top + 7, 140, bounds.Height - 8), StringAlignment.Far);
    }

    private void DrawBanner(Graphics g, int width)
    {
        if (_bannerTicks <= 0 || string.IsNullOrWhiteSpace(_currentBanner))
        {
            return;
        }

        int alpha = Math.Clamp(_bannerTicks * 8, 0, 176);
        using Brush bannerBrush = new SolidBrush(Color.FromArgb(alpha, 34, 52, 61));
        using Brush textBrush = new SolidBrush(Color.FromArgb(Math.Clamp(alpha + 45, 0, 255), 242, 247, 250));
        using Font bannerFont = new("Segoe UI Semibold", 30f, FontStyle.Regular, GraphicsUnit.Pixel);

        SizeF size = g.MeasureString(_currentBanner, bannerFont);
        RectangleF box = new((width - size.Width) / 2 - 28, 84, size.Width + 56, 64);
        g.FillRectangle(bannerBrush, box);
        g.DrawString(_currentBanner, bannerFont, textBrush, box.Left + 28, box.Top + 16);
    }

    private void DrawSparkles(Graphics g)
    {
        if (_sparkles.Count == 0)
        {
            return;
        }

        System.Drawing.Drawing2D.SmoothingMode previousSmoothing = g.SmoothingMode;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        foreach (Sparkle sparkle in _sparkles)
        {
            float progress = sparkle.Age / (float)sparkle.Lifespan;
            int alpha = Math.Clamp((int)(190 * (1f - progress)), 0, 190);
            float size = sparkle.Size * (1f - progress * 0.35f);

            using Pen pen = new(Color.FromArgb(alpha, sparkle.Color), Math.Max(1.2f, size / 3f));
            using Brush brush = new SolidBrush(Color.FromArgb(Math.Clamp(alpha + 35, 0, 210), sparkle.Color));

            DrawParticleShape(g, sparkle.Mode, sparkle.Position, size, pen, brush);
        }

        g.SmoothingMode = previousSmoothing;
    }

    private Point GetTypingParticleOrigin()
    {
        if (_typingParticleOrigin != Point.Empty)
        {
            return _typingParticleOrigin;
        }

        return new Point(ClientSize.Width / 2, ClientSize.Height / 2);
    }

    private static void DrawParticleShape(Graphics g, ParticleMode mode, PointF center, float size, Pen pen, Brush brush)
    {
        float half = size / 2f;

        switch (mode)
        {
            case ParticleMode.Dots:
                g.FillEllipse(brush, center.X - half, center.Y - half, size, size);
                break;
            case ParticleMode.Blocks:
                g.FillRectangle(brush, center.X - half, center.Y - half, size, size);
                g.DrawRectangle(pen, center.X - half, center.Y - half, size, size);
                break;
            case ParticleMode.Pluses:
                g.DrawLine(pen, center.X - half, center.Y, center.X + half, center.Y);
                g.DrawLine(pen, center.X, center.Y - half, center.X, center.Y + half);
                break;
            default:
                g.DrawLine(pen, center.X - half, center.Y, center.X + half, center.Y);
                g.DrawLine(pen, center.X, center.Y - half, center.X, center.Y + half);
                g.DrawLine(pen, center.X - half * 0.7f, center.Y - half * 0.7f, center.X + half * 0.7f, center.Y + half * 0.7f);
                g.DrawLine(pen, center.X - half * 0.7f, center.Y + half * 0.7f, center.X + half * 0.7f, center.Y - half * 0.7f);

                if (size > 5f)
                {
                    float dotSize = Math.Max(1.5f, size / 3f);
                    g.FillEllipse(brush, center.X - dotSize / 2f, center.Y - dotSize / 2f, dotSize, dotSize);
                }

                break;
        }
    }

    private static Rectangle Inset(Rectangle rectangle, int left, int top, int right, int bottom)
    {
        return new Rectangle(
            rectangle.Left + left,
            rectangle.Top + top,
            Math.Max(0, rectangle.Width - left - right),
            Math.Max(0, rectangle.Height - top - bottom));
    }

    private static void DrawText(
        Graphics g,
        string text,
        Font font,
        Brush brush,
        RectangleF bounds,
        StringAlignment alignment = StringAlignment.Near,
        StringAlignment lineAlignment = StringAlignment.Near)
    {
        using StringFormat format = new()
        {
            Alignment = alignment,
            LineAlignment = lineAlignment,
            Trimming = StringTrimming.EllipsisCharacter,
            FormatFlags = StringFormatFlags.NoWrap
        };

        g.DrawString(text, font, brush, bounds, format);
    }

    private static bool IsKeyDown(Keys key)
    {
        return (GetAsyncKeyState((int)key) & 0x8000) != 0;
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    private readonly record struct ProjectInfo(string Name, string Detail, int BaseProgress, string[] Files);

    private readonly record struct ParticleOption(string Label, ParticleMode Mode, Color[] Palette);

    private readonly record struct DiffLine(char Marker, string Text);

    private enum ParticleMode
    {
        Stars,
        Dots,
        Blocks,
        Pluses
    }

    private struct Sparkle(PointF position, PointF velocity, int age, int lifespan, float size, Color color, ParticleMode mode)
    {
        public PointF Position = position;
        public PointF Velocity = velocity;
        public int Age = age;
        public int Lifespan = lifespan;
        public float Size = size;
        public Color Color = color;
        public ParticleMode Mode = mode;
    }
}
