using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokemonUltimate.BattleSimulator.Helpers;
using PokemonUltimate.BattleSimulator.Logging;
using PokemonUltimate.Combat;
using PokemonUltimate.Combat.AI;
using PokemonUltimate.Combat.Damage;
using PokemonUltimate.Combat.Effects;
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
        private TabPage tabResults = null!;

        // Logs Tab controls
        private RichTextBox txtLogs = null!;
        private Button btnClearLogs = null!;
        private Button btnCopyLogs = null!;
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
        private CheckBox checkRandomPlayerTeam = null!;
        private CheckBox checkRandomEnemyTeam = null!;
        private NumericUpDown numericRandomTeamLevel = null!;
        private NumericUpDown numericBatchSimulations = null!;
        private CheckBox checkExportLogs = null!;

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
        private bool _isGeneratingRandomTeam = false; // Flag to prevent manual selection logic during random generation
        private BattleStatisticsCollector? _statisticsCollector;
        private Dictionary<string, string>? _pokemonNameMapping;

        // Results Tab controls - Two separate tabs for each team
        private TabPage tabPlayerResults = null!;
        private TabPage tabEnemyResults = null!;
        private RichTextBox txtPlayerResults = null!;
        private RichTextBox txtEnemyResults = null!;
        private Button btnRefreshPlayerResults = null!;
        private Button btnRefreshEnemyResults = null!;
        private Button btnCopyPlayerResults = null!;
        private Button btnCopyEnemyResults = null!;

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
            this.Size = new Size(1000, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(900, 700);

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
                       "• Singles: 1v1 (1 active Pokemon per team)\n" +
                       "• Doubles: 2v2 (2 active Pokemon per team)\n" +
                       "• Triples: 3v3 (3 active Pokemon per team)\n" +
                       "• Horde: 1v3 or 1v5\n\n" +
                       "Note: Each team can have up to 6 Pokemon. When a Pokemon faints,\n" +
                       "the next Pokemon automatically switches in. Battle ends when all\n" +
                       "Pokemon of one team are defeated.",
                Location = new Point(280, 20),
                Size = new Size(500, 120),
                AutoSize = false
            };

            var groupSlots = new GroupBox
            {
                Text = "Custom Slots Configuration",
                Location = new Point(10, 100),
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

            // Random Team Selection GroupBox
            var groupRandomTeams = new GroupBox
            {
                Text = "Random Team Selection",
                Location = new Point(10, 230),
                Size = new Size(500, 270),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            this.checkRandomPlayerTeam = new CheckBox
            {
                Text = "Random Player Team",
                Location = new Point(20, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                Checked = true
            };
            this.checkRandomPlayerTeam.CheckedChanged += CheckRandomPlayerTeam_CheckedChanged;

            this.checkRandomEnemyTeam = new CheckBox
            {
                Text = "Random Enemy Team",
                Location = new Point(20, 60),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                Checked = true
            };
            this.checkRandomEnemyTeam.CheckedChanged += CheckRandomEnemyTeam_CheckedChanged;

            var lblRandomLevel = new Label
            {
                Text = "Level:",
                Location = new Point(20, 95),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            this.numericRandomTeamLevel = new NumericUpDown
            {
                Location = new Point(80, 93),
                Size = new Size(100, 25),
                Minimum = 1,
                Maximum = 100,
                Value = 50
            };

            // Batch Simulation GroupBox
            var groupBatchSimulation = new GroupBox
            {
                Text = "Batch Simulation",
                Location = new Point(15, 130),
                Size = new Size(470, 140),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            var lblBatchSimulations = new Label
            {
                Text = "Number of Battles:",
                Location = new Point(20, 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };

            this.numericBatchSimulations = new NumericUpDown
            {
                Location = new Point(20, 55),
                Size = new Size(150, 25),
                Minimum = 1,
                Maximum = 10000,
                Value = 1
            };

            this.checkExportLogs = new CheckBox
            {
                Text = "Export Logs to File",
                Location = new Point(20, 90),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                Checked = false
            };

            groupBatchSimulation.Controls.AddRange(new Control[]
            {
                lblBatchSimulations, numericBatchSimulations, checkExportLogs
            });

            groupRandomTeams.Controls.AddRange(new Control[]
            {
                checkRandomPlayerTeam, checkRandomEnemyTeam, lblRandomLevel, numericRandomTeamLevel, groupBatchSimulation
            });

            this.tabBattleMode.Controls.AddRange(new Control[]
            {
                lblBattleMode, comboBattleMode, lblModeInfo, groupSlots, groupRandomTeams
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

            this.btnCopyLogs = new Button
            {
                Text = "Copy to Clipboard",
                Location = new Point(230, 10),
                Size = new Size(120, 30)
            };
            this.btnCopyLogs.Click += BtnCopyLogs_Click;

            this.checkAutoScroll = new CheckBox
            {
                Text = "Auto-scroll",
                Location = new Point(360, 15),
                Checked = true,
                AutoSize = true
            };

            var lblFilter = new Label
            {
                Text = "Filter:",
                Location = new Point(460, 15),
                AutoSize = true
            };
            this.comboLogFilter = new ComboBox
            {
                Location = new Point(510, 13),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.comboLogFilter.Items.AddRange(new[] { "All", "Debug", "Info", "Warning", "Error", "Battle Events" });
            this.comboLogFilter.SelectedIndex = 0;
            this.comboLogFilter.SelectedIndexChanged += ComboLogFilter_SelectedIndexChanged;

            logsHeaderPanel.Controls.AddRange(new Control[]
            {
                lblLogs, btnClearLogs, btnCopyLogs, checkAutoScroll, lblFilter, comboLogFilter
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

            // Tab 4: Player Results
            this.tabPlayerResults = new TabPage("Equipo Jugador")
            {
                Padding = new Padding(10),
                UseVisualStyleBackColor = true
            };

            // Header Panel for Player Results
            var playerResultsHeaderPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(5)
            };

            var lblPlayerResults = new Label
            {
                Text = "Estadísticas del Equipo Jugador:",
                Location = new Point(10, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            this.btnRefreshPlayerResults = new Button
            {
                Text = "Actualizar",
                Location = new Point(300, 10),
                Size = new Size(100, 30)
            };
            this.btnRefreshPlayerResults.Click += BtnRefreshPlayerResults_Click;

            this.btnCopyPlayerResults = new Button
            {
                Text = "Copiar al Portapapeles",
                Location = new Point(410, 10),
                Size = new Size(150, 30)
            };
            this.btnCopyPlayerResults.Click += BtnCopyPlayerResults_Click;

            playerResultsHeaderPanel.Controls.AddRange(new Control[]
            {
                lblPlayerResults, btnRefreshPlayerResults, btnCopyPlayerResults
            });

            // Player Results TextBox
            this.txtPlayerResults = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9),
                ReadOnly = true,
                BackColor = Color.White,
                ForeColor = Color.Black,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            var panelPlayerResults = new Panel
            {
                Dock = DockStyle.Fill
            };
            panelPlayerResults.Controls.AddRange(new Control[]
            {
                txtPlayerResults, playerResultsHeaderPanel
            });

            this.tabPlayerResults.Controls.Add(panelPlayerResults);

            // Tab 5: Enemy Results
            this.tabEnemyResults = new TabPage("Equipo Enemigo")
            {
                Padding = new Padding(10),
                UseVisualStyleBackColor = true
            };

            // Header Panel for Enemy Results
            var enemyResultsHeaderPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(5)
            };

            var lblEnemyResults = new Label
            {
                Text = "Estadísticas del Equipo Enemigo:",
                Location = new Point(10, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            this.btnRefreshEnemyResults = new Button
            {
                Text = "Actualizar",
                Location = new Point(300, 10),
                Size = new Size(100, 30)
            };
            this.btnRefreshEnemyResults.Click += BtnRefreshEnemyResults_Click;

            this.btnCopyEnemyResults = new Button
            {
                Text = "Copiar al Portapapeles",
                Location = new Point(410, 10),
                Size = new Size(150, 30)
            };
            this.btnCopyEnemyResults.Click += BtnCopyEnemyResults_Click;

            enemyResultsHeaderPanel.Controls.AddRange(new Control[]
            {
                lblEnemyResults, btnRefreshEnemyResults, btnCopyEnemyResults
            });

            // Enemy Results TextBox
            this.txtEnemyResults = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9),
                ReadOnly = true,
                BackColor = Color.White,
                ForeColor = Color.Black,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            var panelEnemyResults = new Panel
            {
                Dock = DockStyle.Fill
            };
            panelEnemyResults.Controls.AddRange(new Control[]
            {
                txtEnemyResults, enemyResultsHeaderPanel
            });

            this.tabEnemyResults.Controls.Add(panelEnemyResults);

            // Add tabs to TabControl
            this.tabControl.TabPages.AddRange(new TabPage[] { tabBattleMode, tabPokemon, tabLogs, tabPlayerResults, tabEnemyResults });

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

            // Generate random teams by default since checkboxes are checked
            // Use a small delay to ensure controls are fully initialized
            this.Load += (s, e) =>
            {
                if (checkRandomPlayerTeam.Checked)
                    GenerateRandomTeam(playerSlotControls, isPlayerTeam: true);
                if (checkRandomEnemyTeam.Checked)
                    GenerateRandomTeam(enemySlotControls, isPlayerTeam: false);
            };
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
            // For team battles, allow up to 6 Pokemon per team (full party)
            // The battle format (slots) determines how many are active at once
            int maxPokemonPerTeam = PokemonParty.MaxPartySize;

            // Clear existing controls
            ClearPokemonSlots();

            // Create Player Pokemon controls (up to 6 Pokemon for full team)
            CreatePokemonSlotControls(panelPlayerPokemon, playerSlotControls, maxPokemonPerTeam, "Player");

            // Create Enemy Pokemon controls (up to 6 Pokemon for full team)
            CreatePokemonSlotControls(panelEnemyPokemon, enemySlotControls, maxPokemonPerTeam, "Enemy");
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
                    Text = $"{teamName} Pokemon {i + 1}",
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
                // Subscribe to selection change to disable random when manually selecting
                controls.ComboPokemon.SelectedIndexChanged += (s, e) => OnPokemonManuallySelected(teamName);

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

        private void CheckRandomPlayerTeam_CheckedChanged(object? sender, EventArgs e)
        {
            if (checkRandomPlayerTeam.Checked)
            {
                GenerateRandomTeam(playerSlotControls, isPlayerTeam: true);
            }
        }

        private void CheckRandomEnemyTeam_CheckedChanged(object? sender, EventArgs e)
        {
            if (checkRandomEnemyTeam.Checked)
            {
                GenerateRandomTeam(enemySlotControls, isPlayerTeam: false);
            }
        }

        private void OnPokemonManuallySelected(string teamName)
        {
            // Skip if we're currently generating random teams
            if (_isGeneratingRandomTeam)
                return;

            // When user manually selects a Pokemon, disable random team for that team
            if (teamName == "Player" && checkRandomPlayerTeam.Checked)
            {
                // Temporarily disable event to avoid recursion
                checkRandomPlayerTeam.CheckedChanged -= CheckRandomPlayerTeam_CheckedChanged;
                checkRandomPlayerTeam.Checked = false;
                checkRandomPlayerTeam.CheckedChanged += CheckRandomPlayerTeam_CheckedChanged;
            }
            else if (teamName == "Enemy" && checkRandomEnemyTeam.Checked)
            {
                // Temporarily disable event to avoid recursion
                checkRandomEnemyTeam.CheckedChanged -= CheckRandomEnemyTeam_CheckedChanged;
                checkRandomEnemyTeam.Checked = false;
                checkRandomEnemyTeam.CheckedChanged += CheckRandomEnemyTeam_CheckedChanged;
            }
        }

        private void GenerateRandomTeam(List<PokemonSlotControls> slotControls, bool isPlayerTeam)
        {
            var random = new Random();
            var allPokemon = PokemonCatalog.All.ToList();

            if (allPokemon.Count == 0)
                return;

            // Set flag to prevent manual selection logic from interfering
            _isGeneratingRandomTeam = true;

            try
            {
                // Get the level from the random team level field
                int level = (int)numericRandomTeamLevel.Value;

                // Generate random Pokemon for each slot
                // If random is enabled, always randomize (overwrite manual selections)
                foreach (var slot in slotControls)
                {
                    var randomPokemon = allPokemon[random.Next(allPokemon.Count)];
                    slot.ComboPokemon.SelectedItem = randomPokemon.Name;

                    // Set level from the random team level field
                    slot.NumericLevel.Value = level;

                    // Randomize nature (keep "Random" selected)
                    slot.ComboNature.SelectedIndex = 0;
                }
            }
            finally
            {
                _isGeneratingRandomTeam = false;
            }
        }

        private void BtnStartBattle_Click(object? sender, EventArgs e)
        {
            if (_isBattleRunning)
                return;

            // Generate random teams if enabled
            if (checkRandomPlayerTeam.Checked)
            {
                GenerateRandomTeam(playerSlotControls, isPlayerTeam: true);
            }
            if (checkRandomEnemyTeam.Checked)
            {
                GenerateRandomTeam(enemySlotControls, isPlayerTeam: false);
            }

            // Validate at least one Pokemon is selected per team
            bool hasPlayerPokemon = playerSlotControls.Any(s => s.ComboPokemon.SelectedItem != null);
            bool hasEnemyPokemon = enemySlotControls.Any(s => s.ComboPokemon.SelectedItem != null);

            if (!hasPlayerPokemon || !hasEnemyPokemon)
            {
                MessageBox.Show(
                    "Each team must have at least one Pokemon selected.",
                    "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int numberOfBattles = (int)numericBatchSimulations.Value;

            // Note: Automatic log saving happens in SaveLogsToFile() for each battle
            // checkExportLogs is for optional consolidated export (handled automatically in RunBatchBattlesAsync)

            // Create logger
            _logger = new UIBattleLogger();

            // Subscribe to log events for the logs tab
            _logger.LogAdded += Logger_LogAdded;

            // Clear and refresh logs display
            this.txtLogs.Clear();
            RefreshLogDisplay();

            // Clear statistics display
            this.txtPlayerResults.Clear();
            this.txtPlayerResults.Text = "Las estadísticas se mostrarán al finalizar el combate.";
            this.txtEnemyResults.Clear();
            this.txtEnemyResults.Text = "Las estadísticas se mostrarán al finalizar el combate.";

            // Update UI
            this.btnStartBattle.Enabled = false;
            this.btnStopBattle.Enabled = true;
            this.lblStatus.Text = numberOfBattles > 1 ? $"Running {numberOfBattles} battles..." : "Battle running...";
            _isBattleRunning = true;

            // Start battle(s) in background thread to avoid blocking UI
            if (numberOfBattles > 1)
            {
                _battleTask = Task.Run(async () => await RunBatchBattlesAsync(numberOfBattles));
            }
            else
            {
                _battleTask = Task.Run(async () => await RunBattleAsync());
            }
        }

        private void BtnClearLogs_Click(object? sender, EventArgs e)
        {
            this.txtLogs.Clear();
            _logger?.Clear();
        }

        private void BtnCopyLogs_Click(object? sender, EventArgs e)
        {
            try
            {
                if (this.txtLogs.IsDisposed || string.IsNullOrEmpty(this.txtLogs.Text))
                {
                    MessageBox.Show(
                        "No logs to copy.",
                        "Copy Logs",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                // Copy all text to clipboard
                Clipboard.SetText(this.txtLogs.Text);

                MessageBox.Show(
                    "Logs copied to clipboard successfully!",
                    "Copy Logs",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error copying logs to clipboard: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
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


        private async Task<BattleResult> RunSingleBattleAsync(bool updateUI = true)
        {
            try
            {
                // Create player party from slot controls using PokemonParty
                var playerParty = new PokemonParty();
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

                    var pokemon = builder.Build();
                    if (!playerParty.TryAdd(pokemon))
                    {
                        // Party is full (shouldn't happen with current UI, but handle gracefully)
                        if (_logger != null)
                            _logger.LogWarning($"Player party is full, skipping {pokemonName}");
                    }
                }

                // Create enemy party from slot controls using PokemonParty
                var enemyParty = new PokemonParty();
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

                    var pokemon = builder.Build();
                    if (!enemyParty.TryAdd(pokemon))
                    {
                        // Party is full (shouldn't happen with current UI, but handle gracefully)
                        if (_logger != null)
                            _logger.LogWarning($"Enemy party is full, skipping {pokemonName}");
                    }
                }

                if (playerParty.Count == 0 || enemyParty.Count == 0)
                {
                    throw new InvalidOperationException("Both teams must have at least one Pokemon.");
                }

                // Validate parties are valid for battle
                if (!playerParty.IsValidForBattle())
                {
                    throw new InvalidOperationException("Player party must have at least one active Pokemon.");
                }
                if (!enemyParty.IsValidForBattle())
                {
                    throw new InvalidOperationException("Enemy party must have at least one active Pokemon.");
                }

                // Create AI for team battles (handles auto-switch when Pokemon faint)
                // Note: Event publisher will be set after engine initialization
                var playerAI = new TeamBattleAI(switchThreshold: 0.3, switchChance: 0.6, logger: _logger);
                var enemyAI = new TeamBattleAI(switchThreshold: 0.25, switchChance: 0.7, logger: _logger);

                // Create view (null view for simulation)
                var view = NullBattleView.Instance;

                // Create logger and inject into engine
                // Create engine manually to use custom logger
                var randomProvider = new RandomProvider();
                var battleFieldFactory = new BattleFieldFactory();
                var battleQueueFactory = new BattleQueueFactory();
                var accuracyChecker = new AccuracyChecker(randomProvider);
                var damagePipeline = new DamagePipeline(randomProvider);
                var damageContextFactory = new DamageContextFactory();
                var effectProcessorRegistry = new MoveEffectProcessorRegistry(randomProvider, damageContextFactory);
                var stateValidator = new BattleStateValidator();

                _engine = new CombatEngine(
                    battleFieldFactory,
                    battleQueueFactory,
                    randomProvider,
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

                // Set event publisher for AIs after engine initialization
                var eventBus = _engine.EventBus;
                if (eventBus != null)
                {
                    playerAI.SetEventPublisher(eventBus);
                    enemyAI.SetEventPublisher(eventBus);
                }

                // Register detailed logger observer that publishes events
                // This observer acts as a bridge: it observes actions and publishes events
                if (_logger != null && eventBus != null)
                {
                    var detailedLogger = new DetailedBattleLoggerObserver(_logger, eventBus);
                    _engine.Queue.AddObserver(detailedLogger);
                }

                // Register statistics collector
                // For batch mode, collector is created outside and Reset() is called once
                // For single battle, create new collector
                if (_statisticsCollector == null)
                {
                    _statisticsCollector = new BattleStatisticsCollector();
                    _statisticsCollector.OnBattleStart(_engine.Field);
                }
                else
                {
                    // For single battle mode, always reset kill history to show only current battle
                    // (other stats may accumulate if needed, but kill history should be per-battle)
                    var stats = _statisticsCollector.GetStatistics();
                    stats.KillHistory.Clear();
                }
                // Note: For batch mode, OnBattleStart (which calls Reset) is NOT called here
                // to allow statistics to accumulate across battles
                _engine.Queue.AddObserver(_statisticsCollector);

                // Create Pokemon name mapping for logs and statistics
                _pokemonNameMapping = PokemonNameMapper.CreateNameMapping(_engine.Field);

                // Update logger with name mapping if it supports it
                if (_logger is UIBattleLogger uiLogger)
                {
                    uiLogger.SetPokemonNameMapping(_pokemonNameMapping);
                }

                // Subscribe event-based logger to events (converts events to logs)
                // This is the single source of truth for logging - no hardcoded logs
                if (_logger != null && eventBus != null)
                {
                    var eventLogger = new Logging.EventBasedBattleLogger(_logger);
                    eventBus.Subscribe(eventLogger);
                }

                // Run battle (configure await to continue on background thread)
                var result = await _engine.RunBattle().ConfigureAwait(false);

                // Battle end logging and team statistics handled by EventBasedBattleLogger via events

                // Notify statistics collector that battle ended
                if (_statisticsCollector != null && _engine.Field != null)
                {
                    _statisticsCollector.OnBattleEnd(result.Outcome, _engine.Field);
                }

                // Save logs automatically to Logs folder (always for single battles)
                // Note: For batch battles, SaveLogsToFile is called in RunBatchBattlesAsync for each battle
                if (_logger != null && updateUI) // Single battle mode
                {
                    SaveLogsToFile(_logger, result.Outcome);
                }

                // Update UI only if requested (not during batch)
                if (updateUI)
                {
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
                                UpdateStatisticsDisplay();
                                // Switch to player results tab after battle
                                this.tabControl.SelectedTab = tabPlayerResults;
                            }));
                        }
                        catch (ObjectDisposedException)
                        {
                            // Form is closing, ignore
                        }
                    }
                    else
                    {
                        if (!_isBattleRunning) return result; // Battle was stopped
                        this.btnStartBattle.Enabled = true;
                        this.btnStopBattle.Enabled = false;
                        this.lblStatus.Text = result.Outcome.ToString();
                        _isBattleRunning = false;
                        UpdateStatisticsDisplay();
                        // Switch to player results tab after battle
                        this.tabControl.SelectedTab = tabPlayerResults;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError($"Battle error: {ex.Message}");
                }

                if (updateUI && this.InvokeRequired)
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

                throw; // Re-throw for batch handling
            }
        }

        private async Task RunBattleAsync()
        {
            await RunSingleBattleAsync(updateUI: true);
        }

        private async Task RunBatchBattlesAsync(int numberOfBattles)
        {
            try
            {
                var allBattleLogs = new List<string>();
                var summaryStats = new Dictionary<BattleOutcome, int>();
                int completedBattles = 0;

                // Create shared statistics collector for batch and reset once
                _statisticsCollector = new BattleStatisticsCollector();
                _statisticsCollector.Reset(); // Reset once at the start of batch

                for (int i = 0; i < numberOfBattles; i++)
                {
                    if (!_isBattleRunning) break; // User stopped

                    // Update status
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new Action(() =>
                        {
                            this.lblStatus.Text = $"Running battle {i + 1} of {numberOfBattles}...";
                        }));
                    }

                    // Create new logger for each battle
                    var battleLogger = new UIBattleLogger();
                    battleLogger.LogAdded += Logger_LogAdded;
                    _logger = battleLogger;

                    // Clean up previous engine observers if exists
                    if (_engine != null)
                    {
                        // Remove observers from previous battle
                        _engine.Queue.RemoveObserver(_statisticsCollector);
                        // Note: DetailedLoggerObserver and EventLogger are recreated each battle
                    }

                    // Clear previous engine reference
                    _engine = null;

                    // Reset kill history before each battle in batch mode (but keep other accumulated stats)
                    // This ensures each battle shows only its own kill history
                    if (_statisticsCollector != null)
                    {
                        var stats = _statisticsCollector.GetStatistics();
                        stats.KillHistory.Clear();
                    }

                    // Run single battle without UI updates
                    var result = await RunSingleBattleAsync(updateUI: false);

                    // Clean up observers after battle
                    if (_engine != null && _statisticsCollector != null)
                    {
                        _engine.Queue.RemoveObserver(_statisticsCollector);
                    }

                    // Unsubscribe logger for cleanup
                    battleLogger.LogAdded -= Logger_LogAdded;

                    // Track outcome
                    if (!summaryStats.ContainsKey(result.Outcome))
                    {
                        summaryStats[result.Outcome] = 0;
                    }
                    summaryStats[result.Outcome]++;

                    // Save logs automatically to Logs folder for each battle
                    if (_logger != null)
                    {
                        SaveLogsToFile(_logger, result.Outcome, battleNumber: i + 1, totalBattles: numberOfBattles);
                    }

                    // Collect logs for this battle (for batch export if enabled)
                    if (_logger != null && checkExportLogs.Checked)
                    {
                        var battleLog = new StringBuilder();
                        battleLog.AppendLine($"═══════════════════════════════════════════════════════════════");
                        battleLog.AppendLine($"BATTLE #{i + 1} of {numberOfBattles}");
                        battleLog.AppendLine($"═══════════════════════════════════════════════════════════════");
                        battleLog.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        battleLog.AppendLine($"Outcome: {result.Outcome}");
                        battleLog.AppendLine();

                        foreach (var logEntry in _logger.Logs)
                        {
                            battleLog.AppendLine(logEntry.ToString());
                        }

                        battleLog.AppendLine();
                        battleLog.AppendLine($"═══════════════════════════════════════════════════════════════");
                        battleLog.AppendLine($"END OF BATTLE #{i + 1} - Outcome: {result.Outcome}");
                        battleLog.AppendLine($"═══════════════════════════════════════════════════════════════");
                        battleLog.AppendLine();
                        battleLog.AppendLine();

                        allBattleLogs.Add(battleLog.ToString());
                    }

                    // Clear logger for next battle
                    _logger?.Clear();
                    _logger = null;

                    completedBattles++;

                    // Small delay to allow UI updates
                    await Task.Delay(10);
                }

                // Export consolidated logs if requested (automatic, no dialog)
                if (checkExportLogs.Checked && allBattleLogs.Count > 0)
                {
                    try
                    {
                        // Generate automatic path in Logs folder (project directory)
                        var projectDir = GetProjectDirectory();
                        var logsFolder = Path.Combine(projectDir, "Logs");
                        if (!Directory.Exists(logsFolder))
                        {
                            Directory.CreateDirectory(logsFolder);
                        }

                        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        var consolidatedFileName = $"battle_logs_batch_{timestamp}_{numberOfBattles}battles.txt";
                        var consolidatedPath = Path.Combine(logsFolder, consolidatedFileName);

                        var fullLog = new StringBuilder();
                        fullLog.AppendLine($"BATCH BATTLE SIMULATION REPORT");
                        fullLog.AppendLine($"═══════════════════════════════════════════════════════════════");
                        fullLog.AppendLine($"Total Battles: {completedBattles}");
                        fullLog.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        fullLog.AppendLine();

                        // Add summary statistics
                        if (summaryStats.Count > 0)
                        {
                            fullLog.AppendLine("SUMMARY:");
                            foreach (var stat in summaryStats.OrderByDescending(s => s.Value))
                            {
                                var percentage = (stat.Value * 100.0) / completedBattles;
                                fullLog.AppendLine($"  {stat.Key}: {stat.Value} ({percentage.ToString("F1", CultureInfo.InvariantCulture)}%)");
                            }
                            fullLog.AppendLine();
                            fullLog.AppendLine();
                        }

                        foreach (var battleLog in allBattleLogs)
                        {
                            fullLog.Append(battleLog);
                        }

                        File.WriteAllText(consolidatedPath, fullLog.ToString(), Encoding.UTF8);

                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() =>
                            {
                                MessageBox.Show(
                                    $"Successfully exported consolidated log with {completedBattles} battles to:\n{consolidatedPath}",
                                    "Export Complete",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() =>
                            {
                                MessageBox.Show(
                                    $"Error exporting consolidated logs: {ex.Message}",
                                    "Export Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }));
                        }
                    }
                }

                // Update UI on UI thread
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        if (!_isBattleRunning) return; // Battle was stopped
                        this.btnStartBattle.Enabled = true;
                        this.btnStopBattle.Enabled = false;
                        this.lblStatus.Text = $"Completed {completedBattles} battles";
                        _isBattleRunning = false;
                        UpdateStatisticsDisplay();
                        // Switch to player results tab after batch
                        this.tabControl.SelectedTab = tabPlayerResults;
                    }));
                }
            }
            catch (Exception ex)
            {
                if (_logger != null)
                {
                    _logger.LogError($"Batch battle error: {ex.Message}");
                }

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show(
                            $"Batch battle error: {ex.Message}",
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

        private void BtnRefreshPlayerResults_Click(object? sender, EventArgs e)
        {
            UpdateStatisticsDisplay();
        }

        private void BtnRefreshEnemyResults_Click(object? sender, EventArgs e)
        {
            UpdateStatisticsDisplay();
        }

        private void BtnCopyPlayerResults_Click(object? sender, EventArgs e)
        {
            try
            {
                if (this.txtPlayerResults.IsDisposed || string.IsNullOrEmpty(this.txtPlayerResults.Text))
                {
                    MessageBox.Show(
                        "No hay estadísticas para copiar.",
                        "Copiar Estadísticas",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                Clipboard.SetText(this.txtPlayerResults.Text);

                MessageBox.Show(
                    "Estadísticas copiadas al portapapeles exitosamente!",
                    "Copiar Estadísticas",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al copiar estadísticas: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnCopyEnemyResults_Click(object? sender, EventArgs e)
        {
            try
            {
                if (this.txtEnemyResults.IsDisposed || string.IsNullOrEmpty(this.txtEnemyResults.Text))
                {
                    MessageBox.Show(
                        "No hay estadísticas para copiar.",
                        "Copiar Estadísticas",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                Clipboard.SetText(this.txtEnemyResults.Text);

                MessageBox.Show(
                    "Estadísticas copiadas al portapapeles exitosamente!",
                    "Copiar Estadísticas",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al copiar estadísticas: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void UpdateStatisticsDisplay()
        {
            try
            {
                if (_statisticsCollector == null || _engine?.Field == null)
                    return;

                var stats = _statisticsCollector.GetStatistics();

                // Get battle outcome
                var outcome = _engine.Outcome;
                string outcomeText = outcome switch
                {
                    BattleOutcome.Victory => "VICTORIA DEL JUGADOR",
                    BattleOutcome.Defeat => "VICTORIA DEL ENEMIGO",
                    BattleOutcome.Draw => "EMPATE",
                    _ => "EN CURSO"
                };

                // Get Pokemon status for both teams
                var playerAlive = new List<string>();
                var playerFainted = new List<string>();
                var enemyAlive = new List<string>();
                var enemyFainted = new List<string>();

                if (_engine.Field.PlayerSide?.Party != null)
                {
                    foreach (var pokemon in _engine.Field.PlayerSide.Party)
                    {
                        if (pokemon.IsFainted)
                            playerFainted.Add(pokemon.DisplayName);
                        else
                            playerAlive.Add(pokemon.DisplayName);
                    }
                }

                if (_engine.Field.EnemySide?.Party != null)
                {
                    foreach (var pokemon in _engine.Field.EnemySide.Party)
                    {
                        if (pokemon.IsFainted)
                            enemyFainted.Add(pokemon.DisplayName);
                        else
                            enemyAlive.Add(pokemon.DisplayName);
                    }
                }

                // Get kill history separated by team
                var playerKillHistory = stats.KillHistory.Where(k => k.KillerIsPlayer).ToList();
                var enemyKillHistory = stats.KillHistory.Where(k => !k.KillerIsPlayer).ToList();

                // Calculate kills based on kill history (excludes self-inflicted deaths from status effects)
                int playerKills = playerKillHistory.Count;
                int enemyKills = enemyKillHistory.Count;

                const int boxWidth = 60;

                // ===== EQUIPO JUGADOR - Player Results Tab =====
                var playerSb = new StringBuilder();
                playerSb.AppendLine("═══════════════════════════════════════════════════════════════");
                playerSb.AppendLine("                    RESULTADO DEL COMBATE");
                playerSb.AppendLine("═══════════════════════════════════════════════════════════════");
                playerSb.AppendLine();
                playerSb.AppendLine($"RESULTADO: {outcomeText}");
                playerSb.AppendLine($"Turnos totales: {stats.TotalTurns}");
                playerSb.AppendLine();
                playerSb.AppendLine("EQUIPO JUGADOR");
                playerSb.AppendLine(new string('─', 60));
                playerSb.AppendLine();
                playerSb.AppendLine($"Kills: {playerKills}");
                playerSb.AppendLine();

                // Pokémon Vivos
                if (playerAlive.Count > 0)
                {
                    playerSb.AppendLine("Pokémon Vivos:");
                    foreach (var pokemon in playerAlive)
                        playerSb.AppendLine($"  ✓ {pokemon}");
                    playerSb.AppendLine();
                }

                // Pokémon Muertos
                if (playerFainted.Count > 0)
                {
                    playerSb.AppendLine("Pokémon Muertos:");
                    foreach (var pokemon in playerFainted)
                        playerSb.AppendLine($"  ✗ {pokemon}");
                    playerSb.AppendLine();
                }

                // Historial de Kills
                if (playerKillHistory.Count > 0)
                {
                    playerSb.AppendLine("Historial de Kills:");
                    foreach (var kill in playerKillHistory)
                        playerSb.AppendLine($"  {kill.Killer} -> {kill.Victim}");
                    playerSb.AppendLine();
                }

                playerSb.AppendLine("═══════════════════════════════════════════════════════════════");

                if (!this.txtPlayerResults.IsDisposed)
                    this.txtPlayerResults.Text = playerSb.ToString();

                // ===== EQUIPO ENEMIGO - Enemy Results Tab =====
                var enemySb = new StringBuilder();
                enemySb.AppendLine("═══════════════════════════════════════════════════════════════");
                enemySb.AppendLine("                    RESULTADO DEL COMBATE");
                enemySb.AppendLine("═══════════════════════════════════════════════════════════════");
                enemySb.AppendLine();
                enemySb.AppendLine($"RESULTADO: {outcomeText}");
                enemySb.AppendLine($"Turnos totales: {stats.TotalTurns}");
                enemySb.AppendLine();
                enemySb.AppendLine("EQUIPO ENEMIGO");
                enemySb.AppendLine(new string('─', 60));
                enemySb.AppendLine();
                enemySb.AppendLine($"Kills: {enemyKills}");
                enemySb.AppendLine();

                // Pokémon Vivos
                if (enemyAlive.Count > 0)
                {
                    enemySb.AppendLine("Pokémon Vivos:");
                    foreach (var pokemon in enemyAlive)
                        enemySb.AppendLine($"  ✓ {pokemon}");
                    enemySb.AppendLine();
                }

                // Pokémon Muertos
                if (enemyFainted.Count > 0)
                {
                    enemySb.AppendLine("Pokémon Muertos:");
                    foreach (var pokemon in enemyFainted)
                        enemySb.AppendLine($"  ✗ {pokemon}");
                    enemySb.AppendLine();
                }

                // Historial de Kills
                if (enemyKillHistory.Count > 0)
                {
                    enemySb.AppendLine("Historial de Kills:");
                    foreach (var kill in enemyKillHistory)
                        enemySb.AppendLine($"  {kill.Killer} -> {kill.Victim}");
                    enemySb.AppendLine();
                }

                enemySb.AppendLine("═══════════════════════════════════════════════════════════════");

                if (!this.txtEnemyResults.IsDisposed)
                    this.txtEnemyResults.Text = enemySb.ToString();
            }
            catch (Exception ex)
            {
                if (!this.txtPlayerResults.IsDisposed)
                    this.txtPlayerResults.Text = $"Error al cargar estadísticas: {ex.Message}";
                if (!this.txtEnemyResults.IsDisposed)
                    this.txtEnemyResults.Text = $"Error al cargar estadísticas: {ex.Message}";
            }
        }

        /// <summary>
        /// Gets the project directory path (PokemonUltimate.BattleSimulator folder).
        /// Navigates up from bin/Debug/netX.X/ to find the project root.
        /// </summary>
        private string GetProjectDirectory()
        {
            // Start from the executable directory
            var currentDir = Application.StartupPath;

            // Navigate up from bin/Debug/netX.X/ or bin/Release/netX.X/ to project root
            var directory = new DirectoryInfo(currentDir);

            // Look for the project directory by checking for:
            // 1. Directory name contains "BattleSimulator"
            // 2. Has Logs folder OR Forms folder OR BattleSimulator.csproj file
            while (directory != null)
            {
                var directoryName = directory.Name;
                var logsPath = Path.Combine(directory.FullName, "Logs");
                var formsPath = Path.Combine(directory.FullName, "Forms");
                var csprojFiles = directory.GetFiles("*.csproj");

                // Check if this directory name contains "BattleSimulator"
                bool isBattleSimulatorDir = directoryName.Contains("BattleSimulator", StringComparison.OrdinalIgnoreCase);

                // Check if this directory has BattleSimulator.csproj
                bool hasBattleSimulatorProject = csprojFiles.Any(f =>
                    f.Name.Contains("BattleSimulator", StringComparison.OrdinalIgnoreCase));

                // Check if this is the project directory
                if (isBattleSimulatorDir &&
                    (Directory.Exists(logsPath) || Directory.Exists(formsPath) || hasBattleSimulatorProject))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            // Fallback: if we can't find project directory, use a relative path from executable
            // Try going up 3 levels (bin/Debug/netX.X/ -> bin/ -> project/)
            var fallbackPath = Path.GetFullPath(Path.Combine(Application.StartupPath, "..", "..", ".."));

            // Verify fallback path has Logs folder or create it
            var fallbackLogsPath = Path.Combine(fallbackPath, "Logs");
            if (!Directory.Exists(fallbackLogsPath))
            {
                Directory.CreateDirectory(fallbackLogsPath);
            }

            return fallbackPath;
        }

        /// <summary>
        /// Saves battle logs automatically to the Logs folder.
        /// </summary>
        /// <param name="logger">The logger containing the battle logs.</param>
        /// <param name="outcome">The battle outcome.</param>
        /// <param name="battleNumber">Optional battle number for batch simulations.</param>
        /// <param name="totalBattles">Optional total battles count for batch simulations.</param>
        private void SaveLogsToFile(UIBattleLogger logger, BattleOutcome outcome, int? battleNumber = null, int? totalBattles = null)
        {
            try
            {
                // Get the Logs folder path relative to the project directory
                var projectDir = GetProjectDirectory();
                var logsFolder = Path.Combine(projectDir, "Logs");

                // Ensure the Logs directory exists
                if (!Directory.Exists(logsFolder))
                {
                    Directory.CreateDirectory(logsFolder);
                }

                // Generate filename with timestamp
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName;
                if (battleNumber.HasValue && totalBattles.HasValue)
                {
                    fileName = $"battle_logs_{timestamp}_battle{battleNumber.Value}of{totalBattles.Value}.txt";
                }
                else
                {
                    fileName = $"battle_logs_{timestamp}.txt";
                }

                var filePath = Path.Combine(logsFolder, fileName);

                // Build log content
                var logContent = new StringBuilder();
                logContent.AppendLine("═══════════════════════════════════════════════════════════════");
                if (battleNumber.HasValue && totalBattles.HasValue)
                {
                    logContent.AppendLine($"BATTLE #{battleNumber.Value} of {totalBattles.Value}");
                }
                else
                {
                    logContent.AppendLine("BATTLE LOG");
                }
                logContent.AppendLine("═══════════════════════════════════════════════════════════════");
                logContent.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                logContent.AppendLine($"Outcome: {outcome}");
                logContent.AppendLine();

                // Add all log entries
                foreach (var logEntry in logger.Logs)
                {
                    logContent.AppendLine(logEntry.ToString());
                }

                logContent.AppendLine();
                logContent.AppendLine("═══════════════════════════════════════════════════════════════");
                logContent.AppendLine($"END OF BATTLE - Outcome: {outcome}");
                logContent.AppendLine("═══════════════════════════════════════════════════════════════");

                // Write to file
                File.WriteAllText(filePath, logContent.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                // Log error but don't interrupt battle flow
                // Try to log to UI logger if available, otherwise silently fail
                try
                {
                    if (logger != null)
                    {
                        logger.LogWarning($"Failed to save logs to file: {ex.Message}");
                    }
                }
                catch
                {
                    // Ignore errors during error logging
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

