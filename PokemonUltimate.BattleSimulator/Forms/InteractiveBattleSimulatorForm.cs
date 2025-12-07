using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokemonUltimate.BattleSimulator.Logging;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Effects;
using PokemonUltimate.Combat.Engine;
using PokemonUltimate.Combat.Events;
using PokemonUltimate.Combat.Factories;
using PokemonUltimate.Combat.Helpers;
using PokemonUltimate.Combat.Logging;
using PokemonUltimate.Combat.Providers;
using PokemonUltimate.Combat.Statistics;
using PokemonUltimate.Combat.Validation;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Extensions;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Instances;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.BattleSimulator.Forms
{
    /// <summary>
    /// Interactive Battle Simulator form with two windows: battle configuration and real-time logs.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.8: Interactive Battle Simulator
    /// **Documentation**: See `docs/features/6-development-tools/6.8-interactive-battle-simulator/README.md`
    /// </remarks>
    public partial class InteractiveBattleSimulatorForm : Form
    {
        // Tab Control
        private TabControl tabControl = null!;
        private TabPage tabBattleMode = null!;
        private TabPage tabPokemon = null!;
        private TabPage tabLogs = null!;

        // Logs Tab controls
        private RichTextBox txtLogs = null!;
        private Button btnClearLogs = null!;
        private CheckBox checkAutoScroll = null!;
        private ComboBox comboLogFilter = null!;

        // Battle Mode Tab controls
        private ComboBox comboBattleMode = null!;
        private NumericUpDown numericPlayerSlots = null!;
        private NumericUpDown numericEnemySlots = null!;

        // Pokemon Tab controls - Dynamic lists for multiple slots
        private Panel panelPlayerPokemon = null!;
        private Panel panelEnemyPokemon = null!;
        private List<PokemonSlotControls> playerSlotControls = new List<PokemonSlotControls>();
        private List<PokemonSlotControls> enemySlotControls = new List<PokemonSlotControls>();

        // Helper class to hold controls for each Pokemon slot
        private class PokemonSlotControls
        {
            public GroupBox GroupBox { get; set; } = null!;
            public ComboBox ComboPokemon { get; set; } = null!;
            public NumericUpDown NumericLevel { get; set; } = null!;
            public ComboBox ComboNature { get; set; } = null!;
            public CheckBox CheckPerfectIVs { get; set; } = null!;
        }

        // Control buttons (outside tabs)
        private Button btnStartBattle = null!;
        private Button btnStopBattle = null!;
        private Label lblStatus = null!;

        private UIBattleLogger? _logger;
        private CombatEngine? _engine;
        private Task? _battleTask;
        private bool _isBattleRunning = false;
        private bool _isUpdatingSlots = false; // Flag to prevent recursive updates

        public InteractiveBattleSimulatorForm()
        {
            // Initialize localization (defaults to Spanish)
            LocalizationManager.Initialize(new LocalizationProvider(), "es");

            InitializeComponent();
            LoadPokemonList();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form
            this.Text = "Interactive Battle Simulator";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(900, 550);

            // Main Panel
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            // Tab Control
            this.tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };

            // Tab 1: Battle Mode
            this.tabBattleMode = new TabPage("Battle Mode")
            {
                Padding = new Padding(10),
                UseVisualStyleBackColor = true
            };

            var lblBattleMode = new Label
            {
                Text = "Battle Mode:",
                Location = new Point(10, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            this.comboBattleMode = new ComboBox
            {
                Location = new Point(10, 45),
                Size = new Size(250, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            // LoadBattleModes() will be called after all controls are created

            var lblModeInfo = new Label
            {
                Text = "Select a preset battle mode or configure custom slots.\n" +
                       "Common modes:\n" +
                       "• Singles: 1v1\n" +
                       "• Doubles: 2v2\n" +
                       "• Triples: 3v3\n" +
                       "• Horde: 1v3 or 1v5",
                Location = new Point(280, 20),
                Size = new Size(400, 100),
                AutoSize = false
            };

            var groupSlots = new GroupBox
            {
                Text = "Custom Slots Configuration",
                Location = new Point(10, 85),
                Size = new Size(400, 120),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            var lblPlayerSlots = new Label
            {
                Text = "Player Slots:",
                Location = new Point(10, 25),
                AutoSize = true
            };
            this.numericPlayerSlots = new NumericUpDown
            {
                Location = new Point(10, 45),
                Size = new Size(100, 25),
                Minimum = 1,
                Maximum = 6,
                Value = 1
            };

            var lblEnemySlots = new Label
            {
                Text = "Enemy Slots:",
                Location = new Point(120, 25),
                AutoSize = true
            };
            this.numericEnemySlots = new NumericUpDown
            {
                Location = new Point(120, 45),
                Size = new Size(100, 25),
                Minimum = 1,
                Maximum = 6,
                Value = 1
            };

            groupSlots.Controls.AddRange(new Control[]
            {
                lblPlayerSlots, numericPlayerSlots,
                lblEnemySlots, numericEnemySlots
            });

            this.tabBattleMode.Controls.AddRange(new Control[]
            {
                lblBattleMode, comboBattleMode, lblModeInfo, groupSlots
            });

            // Tab 2: Pokemon Configuration
            this.tabPokemon = new TabPage("Pokemon")
            {
                Padding = new Padding(10),
                UseVisualStyleBackColor = true
            };

            // Inner TabControl for Player/Enemy separation
            var innerTabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };

            // Tab for Player Team
            var tabPlayer = new TabPage("Player Team")
            {
                Padding = new Padding(10),
                UseVisualStyleBackColor = true
            };

            var scrollPanelPlayer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(5)
            };

            var lblPlayerTeam = new Label
            {
                Text = "Player Team Configuration",
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            this.panelPlayerPokemon = new Panel
            {
                Location = new Point(10, 40),
                Width = 900,
                Height = 200,
                AutoSize = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            scrollPanelPlayer.Controls.AddRange(new Control[]
            {
                lblPlayerTeam, panelPlayerPokemon
            });

            tabPlayer.Controls.Add(scrollPanelPlayer);

            // Tab for Enemy Team
            var tabEnemy = new TabPage("Enemy Team")
            {
                Padding = new Padding(10),
                UseVisualStyleBackColor = true
            };

            var scrollPanelEnemy = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(5)
            };

            var lblEnemyTeam = new Label
            {
                Text = "Enemy Team Configuration",
                Location = new Point(10, 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            this.panelEnemyPokemon = new Panel
            {
                Location = new Point(10, 40),
                Width = 900,
                Height = 200,
                AutoSize = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            scrollPanelEnemy.Controls.AddRange(new Control[]
            {
                lblEnemyTeam, panelEnemyPokemon
            });

            tabEnemy.Controls.Add(scrollPanelEnemy);

            // Add inner tabs to inner TabControl
            innerTabControl.TabPages.AddRange(new TabPage[] { tabPlayer, tabEnemy });

            // Add inner TabControl to Pokemon tab
            this.tabPokemon.Controls.Add(innerTabControl);

            // Tab 3: Logs
            this.tabLogs = new TabPage("Logs")
            {
                Padding = new Padding(10),
                UseVisualStyleBackColor = true
            };

            // Header Panel for Logs
            var logsHeaderPanel = new Panel
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
                Size = new Size(100, 30)
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
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.comboLogFilter.Items.AddRange(new[] { "All", "Debug", "Info", "Warning", "Error", "Battle Events" });
            this.comboLogFilter.SelectedIndex = 0;
            this.comboLogFilter.SelectedIndexChanged += ComboLogFilter_SelectedIndexChanged;

            logsHeaderPanel.Controls.AddRange(new Control[]
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

            this.tabLogs.Controls.AddRange(new Control[]
            {
                txtLogs, logsHeaderPanel
            });

            // Add tabs to TabControl
            this.tabControl.TabPages.AddRange(new TabPage[] { tabBattleMode, tabPokemon, tabLogs });

            // Control Buttons Panel (outside tabs, at bottom)
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                Padding = new Padding(10)
            };

            this.btnStartBattle = new Button
            {
                Text = "Start Battle",
                Location = new Point(10, 10),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.LightGreen
            };
            this.btnStartBattle.Click += BtnStartBattle_Click;

            this.btnStopBattle = new Button
            {
                Text = "Stop Battle",
                Location = new Point(140, 10),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.LightCoral,
                Enabled = false
            };
            this.btnStopBattle.Click += BtnStopBattle_Click;

            // Removed Show Logs button - logs are now in a tab

            this.lblStatus = new Label
            {
                Text = "Ready",
                Location = new Point(400, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            buttonPanel.Controls.AddRange(new Control[]
            {
                btnStartBattle, btnStopBattle, lblStatus
            });

            mainPanel.Controls.AddRange(new Control[]
            {
                tabControl, buttonPanel
            });

            // Add main panel to form
            this.Controls.Add(mainPanel);

            this.ResumeLayout(false);

            // Now that all controls are created, set up event handlers and initialize
            LoadBattleModes();

            // Initialize with default slots (1v1) - this will be triggered by LoadBattleModes setting SelectedIndex
            // But we also call it explicitly to ensure initial state
            UpdatePokemonSlots();
        }

        private void LoadPokemonList()
        {
            // This will be called when creating individual combo boxes
        }

        private void LoadPokemonList(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            var pokemonList = PokemonCatalog.All.OrderBy(p => p.Name).ToList();
            foreach (var pokemon in pokemonList)
            {
                comboBox.Items.Add(pokemon.Name);
            }
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        private void LoadBattleModes()
        {
            // Populate battle mode combo box
            this.comboBattleMode.Items.Clear();
            this.comboBattleMode.Items.Add("Singles (1v1)");
            this.comboBattleMode.Items.Add("Doubles (2v2)");
            this.comboBattleMode.Items.Add("Triples (3v3)");
            this.comboBattleMode.Items.Add("Horde (1v3)");
            this.comboBattleMode.Items.Add("Horde (1v5)");
            this.comboBattleMode.Items.Add("Custom");

            // Set up event handlers (controls are now initialized)
            // Handle selection change to update slots
            this.comboBattleMode.SelectedIndexChanged += (s, e) =>
            {
                UpdateSlotsFromBattleMode();
                UpdatePokemonSlots(); // Update Pokemon controls when mode changes
            };

            // Handle manual slot changes
            this.numericPlayerSlots.ValueChanged += (s, e) =>
            {
                if (!_isUpdatingSlots && this.comboBattleMode.SelectedItem?.ToString() == "Custom")
                {
                    UpdatePokemonSlots();
                }
            };

            this.numericEnemySlots.ValueChanged += (s, e) =>
            {
                if (!_isUpdatingSlots && this.comboBattleMode.SelectedItem?.ToString() == "Custom")
                {
                    UpdatePokemonSlots();
                }
            };

            // Set default selection AFTER event handlers are set up
            // Temporarily disable event to avoid double update
            this.comboBattleMode.SelectedIndexChanged -= (EventHandler)((s, e) =>
            {
                UpdateSlotsFromBattleMode();
                UpdatePokemonSlots();
            });
            this.comboBattleMode.SelectedIndex = 0; // Default to Singles
            this.comboBattleMode.SelectedIndexChanged += (s, e) =>
            {
                UpdateSlotsFromBattleMode();
                UpdatePokemonSlots();
            };
        }

        private void UpdateSlotsFromBattleMode()
        {
            if (this.comboBattleMode.SelectedItem == null)
                return;

            var selectedMode = this.comboBattleMode.SelectedItem.ToString();
            bool isCustom = selectedMode == "Custom";

            // Enable/disable manual slot editing based on mode
            this.numericPlayerSlots.Enabled = isCustom;
            this.numericEnemySlots.Enabled = isCustom;

            switch (selectedMode)
            {
                case "Singles (1v1)":
                    this.numericPlayerSlots.Value = 1;
                    this.numericEnemySlots.Value = 1;
                    break;
                case "Doubles (2v2)":
                    this.numericPlayerSlots.Value = 2;
                    this.numericEnemySlots.Value = 2;
                    break;
                case "Triples (3v3)":
                    this.numericPlayerSlots.Value = 3;
                    this.numericEnemySlots.Value = 3;
                    break;
                case "Horde (1v3)":
                    this.numericPlayerSlots.Value = 1;
                    this.numericEnemySlots.Value = 3;
                    break;
                case "Horde (1v5)":
                    this.numericPlayerSlots.Value = 1;
                    this.numericEnemySlots.Value = 5;
                    break;
                case "Custom":
                    // Keep current values, allow manual editing
                    break;
            }
        }

        private void LoadNatureList(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.Items.Add("Random");
            foreach (Nature nature in Enum.GetValues(typeof(Nature)))
            {
                comboBox.Items.Add(nature);
            }
            comboBox.SelectedIndex = 0; // Default to "Random"
        }

        private Nature? GetSelectedNature(ComboBox comboBox)
        {
            if (comboBox.SelectedItem == null || comboBox.SelectedItem.ToString() == "Random")
                return null;

            if (comboBox.SelectedItem is Nature nature)
                return nature;

            if (Enum.TryParse<Nature>(comboBox.SelectedItem.ToString(), out var parsedNature))
                return parsedNature;

            return null;
        }

        private void UpdatePokemonSlots()
        {
            int playerSlots = (int)this.numericPlayerSlots.Value;
            int enemySlots = (int)this.numericEnemySlots.Value;

            // Clear existing controls
            ClearPokemonSlots();

            // Create Player Pokemon controls
            CreatePokemonSlotControls(panelPlayerPokemon, playerSlotControls, playerSlots, "Player");

            // Create Enemy Pokemon controls
            CreatePokemonSlotControls(panelEnemyPokemon, enemySlotControls, enemySlots, "Enemy");
        }

        private void ClearPokemonSlots()
        {
            // Clear Player slots
            foreach (var controls in playerSlotControls)
            {
                panelPlayerPokemon.Controls.Remove(controls.GroupBox);
                controls.GroupBox.Dispose();
            }
            playerSlotControls.Clear();

            // Clear Enemy slots
            foreach (var controls in enemySlotControls)
            {
                panelEnemyPokemon.Controls.Remove(controls.GroupBox);
                controls.GroupBox.Dispose();
            }
            enemySlotControls.Clear();
        }

        private void CreatePokemonSlotControls(Panel parentPanel, List<PokemonSlotControls> slotControlsList, int slotCount, string teamName)
        {
            int yOffset = 0;
            int slotHeight = 80;

            // Calculate available width (accounting for padding and scrollbar)
            int availableWidth = parentPanel.Width > 0 ? parentPanel.Width - 20 : 900;

            for (int i = 0; i < slotCount; i++)
            {
                var controls = new PokemonSlotControls();

                // GroupBox for this slot
                controls.GroupBox = new GroupBox
                {
                    Text = $"{teamName} Slot {i + 1}",
                    Location = new Point(0, yOffset),
                    Width = availableWidth,
                    Height = slotHeight,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };

                // Pokemon ComboBox
                var lblPokemon = new Label
                {
                    Text = "Pokemon:",
                    Location = new Point(10, 25),
                    AutoSize = true
                };
                controls.ComboPokemon = new ComboBox
                {
                    Location = new Point(10, 45),
                    Size = new Size(200, 25),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                LoadPokemonList(controls.ComboPokemon);

                // Level NumericUpDown
                var lblLevel = new Label
                {
                    Text = "Level:",
                    Location = new Point(220, 25),
                    AutoSize = true
                };
                controls.NumericLevel = new NumericUpDown
                {
                    Location = new Point(220, 45),
                    Size = new Size(80, 25),
                    Minimum = 1,
                    Maximum = 100,
                    Value = 50
                };

                // Nature ComboBox
                var lblNature = new Label
                {
                    Text = "Nature:",
                    Location = new Point(310, 25),
                    AutoSize = true
                };
                controls.ComboNature = new ComboBox
                {
                    Location = new Point(310, 45),
                    Size = new Size(150, 25),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                LoadNatureList(controls.ComboNature);

                // Perfect IVs CheckBox
                controls.CheckPerfectIVs = new CheckBox
                {
                    Text = "Perfect IVs",
                    Location = new Point(470, 45),
                    AutoSize = true,
                    Checked = false
                };

                controls.GroupBox.Controls.AddRange(new Control[]
                {
                    lblPokemon, controls.ComboPokemon,
                    lblLevel, controls.NumericLevel,
                    lblNature, controls.ComboNature,
                    controls.CheckPerfectIVs
                });

                parentPanel.Controls.Add(controls.GroupBox);
                slotControlsList.Add(controls);

                yOffset += slotHeight + 10; // Add spacing between slots
            }

            // Update panel height to fit all slots
            parentPanel.Height = yOffset;
        }

        private void BtnStartBattle_Click(object? sender, EventArgs e)
        {
            if (_isBattleRunning)
                return;

            // Validate selections
            if (playerSlotControls.Count == 0 || enemySlotControls.Count == 0)
            {
                MessageBox.Show(
                    "Please configure Pokemon for each team.",
                    "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate all Pokemon are selected
            foreach (var slot in playerSlotControls)
            {
                if (slot.ComboPokemon.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Please select Pokemon for all player slots.",
                        "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            foreach (var slot in enemySlotControls)
            {
                if (slot.ComboPokemon.SelectedItem == null)
                {
                    MessageBox.Show(
                        "Please select Pokemon for all enemy slots.",
                        "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Create logger
            _logger = new UIBattleLogger();

            // Subscribe to log events for the logs tab
            _logger.LogAdded += Logger_LogAdded;

            // Clear and refresh logs display
            this.txtLogs.Clear();
            RefreshLogDisplay();

            // Update UI
            this.btnStartBattle.Enabled = false;
            this.btnStopBattle.Enabled = true;
            this.lblStatus.Text = "Battle running...";
            _isBattleRunning = true;

            // Start battle in background thread to avoid blocking UI
            _battleTask = Task.Run(async () => await RunBattleAsync());
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

        private void BtnStopBattle_Click(object? sender, EventArgs e)
        {
            if (!_isBattleRunning)
                return;

            _isBattleRunning = false;
            this.btnStartBattle.Enabled = true;
            this.btnStopBattle.Enabled = false;
            this.lblStatus.Text = "Battle stopped";

            if (_logger != null)
            {
                _logger.LogInfo("Battle stopped by user");
            }

            // Note: We can't actually stop the battle mid-execution easily
            // The battle will complete but UI won't update when it finishes
            // This is a limitation - proper cancellation would require CancellationToken support in CombatEngine
        }


        private async Task RunBattleAsync()
        {
            try
            {
                // Create player party from slot controls
                var playerParty = new List<PokemonInstance>();
                foreach (var slot in playerSlotControls)
                {
                    var pokemonName = slot.ComboPokemon.SelectedItem?.ToString();
                    if (string.IsNullOrEmpty(pokemonName))
                        continue;

                    var pokemonData = PokemonCatalog.All.FirstOrDefault(p => p.Name == pokemonName);
                    if (pokemonData == null)
                        continue;

                    var level = (int)slot.NumericLevel.Value;
                    var builder = Pokemon.Create(pokemonData, level);

                    var nature = GetSelectedNature(slot.ComboNature);
                    if (nature.HasValue)
                        builder = builder.WithNature(nature.Value);

                    if (slot.CheckPerfectIVs.Checked)
                        builder = builder.WithIVs(new IVSet(31, 31, 31, 31, 31, 31));

                    playerParty.Add(builder.Build());
                }

                // Create enemy party from slot controls
                var enemyParty = new List<PokemonInstance>();
                foreach (var slot in enemySlotControls)
                {
                    var pokemonName = slot.ComboPokemon.SelectedItem?.ToString();
                    if (string.IsNullOrEmpty(pokemonName))
                        continue;

                    var pokemonData = PokemonCatalog.All.FirstOrDefault(p => p.Name == pokemonName);
                    if (pokemonData == null)
                        continue;

                    var level = (int)slot.NumericLevel.Value;
                    var builder = Pokemon.Create(pokemonData, level);

                    var nature = GetSelectedNature(slot.ComboNature);
                    if (nature.HasValue)
                        builder = builder.WithNature(nature.Value);

                    if (slot.CheckPerfectIVs.Checked)
                        builder = builder.WithIVs(new IVSet(31, 31, 31, 31, 31, 31));

                    enemyParty.Add(builder.Build());
                }

                if (playerParty.Count == 0 || enemyParty.Count == 0)
                {
                    throw new InvalidOperationException("Both teams must have at least one Pokemon.");
                }

                // Create AI
                var playerAI = new RandomAI();
                var enemyAI = new RandomAI();

                // Create view (null view for simulation)
                var view = NullBattleView.Instance;

                // Create logger and inject into engine
                // Create engine manually to use custom logger
                var randomProvider = new RandomProvider();
                var battleFieldFactory = new BattleFieldFactory();
                var battleQueueFactory = new BattleQueueFactory();
                var damageContextFactory = new DamageContextFactory();
                var endOfTurnProcessor = new EndOfTurnProcessor(damageContextFactory);
                var battleTriggerProcessor = new BattleTriggerProcessor();
                var accuracyChecker = new AccuracyChecker(randomProvider);
                var damagePipeline = new DamagePipeline(randomProvider);
                var effectProcessorRegistry = new MoveEffectProcessorRegistry(randomProvider, damageContextFactory);
                var stateValidator = new BattleStateValidator();

                _engine = new CombatEngine(
                    battleFieldFactory,
                    battleQueueFactory,
                    randomProvider,
                    endOfTurnProcessor,
                    battleTriggerProcessor,
                    accuracyChecker,
                    damagePipeline,
                    effectProcessorRegistry,
                    stateValidator,
                    _logger);

                // Initialize with configured battle mode
                var rules = new BattleRules
                {
                    PlayerSlots = (int)this.numericPlayerSlots.Value,
                    EnemySlots = (int)this.numericEnemySlots.Value
                };
                _engine.Initialize(rules, playerParty, enemyParty, playerAI, enemyAI, view);

                // Register detailed logger observer
                if (_logger != null)
                {
                    var detailedLogger = new DetailedBattleLoggerObserver(_logger);
                    _engine.Queue.AddObserver(detailedLogger);
                }

                // Run battle (configure await to continue on background thread)
                var result = await _engine.RunBattle().ConfigureAwait(false);

                // Log result
                if (_logger != null)
                {
                    _logger.LogInfo($"Battle ended: {result.Outcome}");
                }

                // Update UI on UI thread
                if (this.InvokeRequired)
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            if (!_isBattleRunning) return; // Battle was stopped
                            this.btnStartBattle.Enabled = true;
                            this.btnStopBattle.Enabled = false;
                            this.lblStatus.Text = $"Battle ended: {result.Outcome}";
                            _isBattleRunning = false;
                        }));
                    }
                    catch (ObjectDisposedException)
                    {
                        // Form is closing, ignore
                    }
                }
                else
                {
                    if (!_isBattleRunning) return; // Battle was stopped
                    this.btnStartBattle.Enabled = true;
                    this.btnStopBattle.Enabled = false;
                    this.lblStatus.Text = result.Outcome.ToString();
                    _isBattleRunning = false;
                }
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError($"Battle error: {ex.Message}");
                }

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show(
                            $"Battle error: {ex.Message}",
                            "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.btnStartBattle.Enabled = true;
                        this.btnStopBattle.Enabled = false;
                        this.lblStatus.Text = "Error occurred";
                        _isBattleRunning = false;
                    }));
                }
            }
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_isBattleRunning)
            {
                var result = MessageBox.Show("Battle is still running. Stop battle and close?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                _isBattleRunning = false;
            }

            // Unsubscribe from logger events
            if (_logger != null)
            {
                _logger.LogAdded -= Logger_LogAdded;
            }

            base.OnFormClosing(e);
        }
    }
}

