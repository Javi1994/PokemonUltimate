using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PokemonUltimate.BattleSimulator.Logging;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.BattleSimulator.Forms
{
    /// <summary>
    /// Separate form for displaying battle logs in real-time.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.8: Interactive Battle Simulator
    /// **Documentation**: See `docs/features/6-development-tools/6.8-interactive-battle-simulator/README.md`
    /// </remarks>
    public partial class BattleLogsForm : Form
    {
        private RichTextBox txtLogs = null!;
        private Button btnClearLogs = null!;
        private CheckBox checkAutoScroll = null!;
        private ComboBox comboLogFilter = null!;
        private UIBattleLogger? _logger;

        public BattleLogsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form
            this.Text = "Battle Logs";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 400);

            // Main Panel
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Header Panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(5)
            };

            var lblLogs = new Label
            {
                Text = "Battle Logs:",
                Location = new Point(10, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            this.btnClearLogs = new Button
            {
                Text = "Clear Logs",
                Location = new Point(120, 10),
                Size = new Size(100, 30),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            this.btnClearLogs.Click += BtnClearLogs_Click;

            this.checkAutoScroll = new CheckBox
            {
                Text = "Auto-scroll",
                Location = new Point(230, 15),
                Checked = true,
                AutoSize = true
            };

            var lblFilter = new Label
            {
                Text = "Filter:",
                Location = new Point(330, 15),
                AutoSize = true
            };
            this.comboLogFilter = new ComboBox
            {
                Location = new Point(380, 13),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            this.comboLogFilter.Items.AddRange(new[] { "All", "Debug", "Info", "Warning", "Error", "Battle Events" });
            this.comboLogFilter.SelectedIndex = 0;
            this.comboLogFilter.SelectedIndexChanged += ComboLogFilter_SelectedIndexChanged;

            headerPanel.Controls.AddRange(new Control[]
            {
                lblLogs, btnClearLogs, checkAutoScroll, lblFilter, comboLogFilter
            });

            // Logs TextBox
            this.txtLogs = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9),
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.LightGreen
            };

            mainPanel.Controls.Add(this.txtLogs);
            mainPanel.Controls.Add(headerPanel);

            // Add main panel to form
            this.Controls.Add(mainPanel);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Sets the logger to display logs from.
        /// </summary>
        public void SetLogger(UIBattleLogger logger)
        {
            // Unsubscribe from previous logger if any
            if (_logger != null)
            {
                _logger.LogAdded -= Logger_LogAdded;
            }

            _logger = logger;
            _logger.LogAdded += Logger_LogAdded;

            // Clear and refresh display
            this.txtLogs.Clear();
            RefreshLogDisplay();
        }

        /// <summary>
        /// Clears the logger reference when form is closing.
        /// </summary>
        public void ClearLogger()
        {
            if (_logger != null)
            {
                _logger.LogAdded -= Logger_LogAdded;
                _logger = null;
            }
        }

        private void BtnClearLogs_Click(object? sender, EventArgs e)
        {
            this.txtLogs.Clear();
            _logger?.Clear();
        }

        private void ComboLogFilter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            RefreshLogDisplay();
        }

        private void Logger_LogAdded(object? sender, UIBattleLogger.LogEntry e)
        {
            // Always invoke on UI thread to avoid cross-thread issues
            if (this.InvokeRequired)
            {
                try
                {
                    this.BeginInvoke(new Action(() => AddLogToUI(e)));
                }
                catch (ObjectDisposedException)
                {
                    // Form is closing, ignore
                }
            }
            else
            {
                AddLogToUI(e);
            }
        }

        private void AddLogToUI(UIBattleLogger.LogEntry entry)
        {
            try
            {
                // Check if form is disposed
                if (this.IsDisposed || this.txtLogs.IsDisposed)
                    return;

                // Check filter
                var filter = this.comboLogFilter.SelectedItem?.ToString() ?? "All";
                if (filter != "All")
                {
                    if (filter == "Debug" && entry.Level != UIBattleLogger.LogLevel.Debug) return;
                    if (filter == "Info" && entry.Level != UIBattleLogger.LogLevel.Info) return;
                    if (filter == "Warning" && entry.Level != UIBattleLogger.LogLevel.Warning) return;
                    if (filter == "Error" && entry.Level != UIBattleLogger.LogLevel.Error) return;
                    if (filter == "Battle Events" && entry.Level != UIBattleLogger.LogLevel.BattleEvent) return;
                }

                // Set color based on level
                Color color = entry.Level switch
                {
                    UIBattleLogger.LogLevel.Debug => Color.Gray,
                    UIBattleLogger.LogLevel.Info => Color.LightGreen,
                    UIBattleLogger.LogLevel.Warning => Color.Yellow,
                    UIBattleLogger.LogLevel.Error => Color.Red,
                    UIBattleLogger.LogLevel.BattleEvent => Color.Cyan,
                    _ => Color.White
                };

                this.txtLogs.SelectionStart = this.txtLogs.TextLength;
                this.txtLogs.SelectionLength = 0;
                this.txtLogs.SelectionColor = color;
                this.txtLogs.AppendText(entry.ToString() + Environment.NewLine);
                this.txtLogs.SelectionColor = this.txtLogs.ForeColor;

                // Auto-scroll if enabled
                if (this.checkAutoScroll.Checked)
                {
                    this.txtLogs.ScrollToCaret();
                }
            }
            catch (ObjectDisposedException)
            {
                // Form is closing, ignore
            }
            catch (InvalidOperationException)
            {
                // Control is being disposed, ignore
            }
        }

        private void RefreshLogDisplay()
        {
            try
            {
                if (_logger == null || this.IsDisposed || this.txtLogs.IsDisposed)
                    return;

                this.txtLogs.Clear();
                var filter = this.comboLogFilter.SelectedItem?.ToString() ?? "All";

                // Make a copy of logs to avoid modification during enumeration
                var logsCopy = _logger.Logs.ToList();

                foreach (var entry in logsCopy)
                {
                    if (filter != "All")
                    {
                        if (filter == "Debug" && entry.Level != UIBattleLogger.LogLevel.Debug) continue;
                        if (filter == "Info" && entry.Level != UIBattleLogger.LogLevel.Info) continue;
                        if (filter == "Warning" && entry.Level != UIBattleLogger.LogLevel.Warning) continue;
                        if (filter == "Error" && entry.Level != UIBattleLogger.LogLevel.Error) continue;
                        if (filter == "Battle Events" && entry.Level != UIBattleLogger.LogLevel.BattleEvent) continue;
                    }

                    AddLogToUI(entry);
                }
            }
            catch (ObjectDisposedException)
            {
                // Form is closing, ignore
            }
            catch (InvalidOperationException)
            {
                // Control is being disposed, ignore
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            ClearLogger();
            base.OnFormClosing(e);
        }
    }
}

