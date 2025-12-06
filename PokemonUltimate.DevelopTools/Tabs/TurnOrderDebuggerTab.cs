using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.DevelopTools.Runners;

namespace PokemonUltimate.DevelopTools.Tabs
{
    /// <summary>
    /// Debugger tab for visualizing turn order determination with speed and priority.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.4: Turn Order Debugger
    /// **Documentation**: See `docs/features/6-development-tools/6.4-turn-order-debugger/README.md`
    /// </remarks>
    public partial class TurnOrderDebuggerTab : UserControl
    {
        private const int MaxPokemonSlots = 4;
        private const int MinPokemonSlots = 2;

        private List<PokemonSlotPanel> _pokemonSlots = null!;
        private NumericUpDown numericLevel = null!;
        private Button btnCalculate = null!;
        private TabControl tabResults = null!;
        private TabPage tabSummary = null!;
        private TabPage tabSpeedCalculation = null!;
        private TabPage tabPriority = null!;
        private TabPage tabOrder = null!;
        private RichTextBox txtSummary = null!;
        private DataGridView dgvSpeedCalculation = null!;
        private DataGridView dgvPriority = null!;
        private DataGridView dgvOrder = null!;
        private TurnOrderRunner _runner = null!;

        private class PokemonSlotPanel
        {
            public Label Label { get; set; } = null!;
            public ComboBox ComboPokemon { get; set; } = null!;
            public ComboBox ComboMove { get; set; } = null!;
            public CheckBox ChkParalysis { get; set; } = null!;
            public CheckBox ChkTailwind { get; set; } = null!;
            public NumericUpDown NumericSpeedStage { get; set; } = null!;
            public Panel? Panel { get; set; }
        }

        public TurnOrderDebuggerTab()
        {
            _runner = new TurnOrderRunner();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.numericLevel = new NumericUpDown();
            this.btnCalculate = new Button();
            this.tabResults = new TabControl();
            this.tabSummary = new TabPage();
            this.tabSpeedCalculation = new TabPage();
            this.tabPriority = new TabPage();
            this.tabOrder = new TabPage();
            this.txtSummary = new RichTextBox();
            this.dgvSpeedCalculation = new DataGridView();
            this.dgvPriority = new DataGridView();
            this.dgvOrder = new DataGridView();
            this._pokemonSlots = new List<PokemonSlotPanel>();

            this.SuspendLayout();

            // UserControl
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(0);

            // TableLayoutPanel
            var mainTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10)
            };
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400));
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Panel izquierdo - Configuración
            var panelConfig = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15),
                AutoScroll = true,
                Margin = new Padding(5)
            };

            int yPos = 15;
            int spacing = 32;
            int controlWidth = 320;
            int leftMargin = 10;

            var lblTitle = new Label
            {
                Text = "Configuration",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(leftMargin, yPos)
            };
            yPos += 30;

            // Level
            var lblLevel = new Label { Text = "Level:", Location = new Point(leftMargin, yPos), AutoSize = true };
            yPos += 22;
            this.numericLevel.Location = new Point(leftMargin, yPos);
            this.numericLevel.Width = 100;
            this.numericLevel.Minimum = 1;
            this.numericLevel.Maximum = 100;
            this.numericLevel.Value = 50;
            yPos += 38;

            // Pokemon slots - simplified design
            for (int i = 0; i < MaxPokemonSlots; i++)
            {
                var slotPanel = CreatePokemonSlotPanel(i + 1, leftMargin, ref yPos, controlWidth, spacing);
                _pokemonSlots.Add(slotPanel);
                
                // Add speed label (positioned above the numeric control with proper spacing)
                var speedLabel = new Label
                {
                    Text = "Speed Stage:",
                    Location = new Point(leftMargin, slotPanel.NumericSpeedStage.Location.Y - 20),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 8)
                };
                
                panelConfig.Controls.AddRange(new Control[]
                {
                    slotPanel.Label,
                    slotPanel.ComboPokemon,
                    slotPanel.ComboMove,
                    slotPanel.ChkParalysis,
                    slotPanel.ChkTailwind,
                    speedLabel,
                    slotPanel.NumericSpeedStage
                });
            }

            yPos += spacing;

            // Calculate button
            this.btnCalculate = new Button
            {
                Text = "Calculate Turn Order",
                Location = new Point(leftMargin, yPos),
                Width = controlWidth,
                Height = 50,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            this.btnCalculate.FlatAppearance.BorderSize = 0;
            this.btnCalculate.Click += BtnCalculate_Click;

            panelConfig.Controls.AddRange(new Control[]
            {
                lblTitle,
                lblLevel, numericLevel,
                btnCalculate
            });

            // Set panel height
            panelConfig.AutoScrollMinSize = new Size(0, yPos + 100);

            // Panel derecho - Resultados
            this.tabResults.Dock = DockStyle.Fill;
            this.tabResults.Padding = new Point(10, 5);

            // Tab Summary
            this.tabSummary.Text = "Summary";
            this.tabSummary.Padding = new Padding(10);
            this.txtSummary.Dock = DockStyle.Fill;
            this.txtSummary.Font = new Font("Consolas", 9);
            this.txtSummary.ReadOnly = true;
            this.txtSummary.Text = "Configure Pokemon and click 'Calculate Turn Order' to see results here.";
            this.tabSummary.Controls.Add(this.txtSummary);

            // Tab Speed Calculation
            this.tabSpeedCalculation.Text = "Speed Calculation";
            this.tabSpeedCalculation.Padding = new Padding(10);
            this.dgvSpeedCalculation.Dock = DockStyle.Fill;
            this.dgvSpeedCalculation.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSpeedCalculation.ReadOnly = true;
            this.dgvSpeedCalculation.AllowUserToAddRows = false;
            this.tabSpeedCalculation.Controls.Add(this.dgvSpeedCalculation);

            // Tab Priority
            this.tabPriority.Text = "Priority";
            this.tabPriority.Padding = new Padding(10);
            this.dgvPriority.Dock = DockStyle.Fill;
            this.dgvPriority.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPriority.ReadOnly = true;
            this.dgvPriority.AllowUserToAddRows = false;
            this.tabPriority.Controls.Add(this.dgvPriority);

            // Tab Order
            this.tabOrder.Text = "Order";
            this.tabOrder.Padding = new Padding(10);
            this.dgvOrder.Dock = DockStyle.Fill;
            this.dgvOrder.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvOrder.ReadOnly = true;
            this.dgvOrder.AllowUserToAddRows = false;
            this.tabOrder.Controls.Add(this.dgvOrder);

            this.tabResults.TabPages.AddRange(new TabPage[]
            {
                this.tabSummary,
                this.tabSpeedCalculation,
                this.tabPriority,
                this.tabOrder
            });

            mainTableLayout.Controls.Add(panelConfig, 0, 0);
            mainTableLayout.Controls.Add(this.tabResults, 1, 0);

            this.Controls.Add(mainTableLayout);

            this.ResumeLayout(false);
        }

        private PokemonSlotPanel CreatePokemonSlotPanel(int slotNumber, int leftMargin, ref int yPos, int controlWidth, int spacing)
        {
            var panel = new PokemonSlotPanel();
            int smallMargin = 5; // Small margin between fields

            // Label
            panel.Label = new Label
            {
                Text = $"Pokemon {slotNumber}:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(leftMargin, yPos),
                AutoSize = true
            };
            yPos += 24;

            // Pokemon dropdown
            panel.ComboPokemon = new ComboBox
            {
                Location = new Point(leftMargin, yPos),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 23
            };
            yPos += 23 + smallMargin + 5;

            // Move dropdown
            panel.ComboMove = new ComboBox
            {
                Location = new Point(leftMargin, yPos),
                Width = controlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 23
            };
            yPos += 23 + smallMargin + 5;

            // Checkboxes side by side
            panel.ChkParalysis = new CheckBox
            {
                Text = "Paralysis",
                Location = new Point(leftMargin, yPos),
                AutoSize = true
            };
            
            panel.ChkTailwind = new CheckBox
            {
                Text = "Tailwind",
                Location = new Point(leftMargin + 100, yPos),
                AutoSize = true
            };
            yPos += 32; // More space after checkboxes to prevent overlap with Speed Stage

            // Speed stat stage numeric control (label is added separately in InitializeComponent)
            panel.NumericSpeedStage = new NumericUpDown
            {
                Location = new Point(leftMargin, yPos),
                Width = 80,
                Minimum = -6,
                Maximum = 6,
                Value = 0
            };
            yPos += 38 + smallMargin; // More space after numeric control

            return panel;
        }

        private void LoadData()
        {
            // Load Pokemon
            var pokemonList = PokemonCatalog.All.OrderBy(p => p.Name).ToList();
            foreach (var slot in _pokemonSlots)
            {
                foreach (var pokemon in pokemonList)
                {
                    slot.ComboPokemon.Items.Add(pokemon.Name);
                }
                if (slot.ComboPokemon.Items.Count > 0)
                {
                    slot.ComboPokemon.SelectedIndex = Math.Min(_pokemonSlots.IndexOf(slot), slot.ComboPokemon.Items.Count - 1);
                }
            }

            // Load Moves
            var moveList = MoveCatalog.All.OrderBy(m => m.Name).ToList();
            foreach (var slot in _pokemonSlots)
            {
                foreach (var move in moveList)
                {
                    slot.ComboMove.Items.Add(move.Name);
                }
                if (slot.ComboMove.Items.Count > 0)
                {
                    slot.ComboMove.SelectedIndex = 0;
                }
            }
        }

        private void BtnCalculate_Click(object? sender, EventArgs e)
        {
            // Collect valid configurations
            var configs = new List<TurnOrderRunner.PokemonConfig>();

            foreach (var slot in _pokemonSlots)
            {
                if (slot.ComboPokemon.SelectedItem == null || slot.ComboMove.SelectedItem == null)
                    continue;

                var pokemonName = slot.ComboPokemon.SelectedItem.ToString();
                var moveName = slot.ComboMove.SelectedItem.ToString();

                var species = PokemonCatalog.All.FirstOrDefault(p => p.Name == pokemonName);
                var move = MoveCatalog.All.FirstOrDefault(m => m.Name == moveName);

                if (species == null || move == null)
                    continue;

                configs.Add(new TurnOrderRunner.PokemonConfig
                {
                    Species = species,
                    Move = move,
                    Level = (int)numericLevel.Value,
                    HasParalysis = slot.ChkParalysis.Checked,
                    HasTailwind = slot.ChkTailwind.Checked,
                    SpeedStatStage = (int)slot.NumericSpeedStage.Value,
                    Name = $"{pokemonName} ({slot.Label.Text.Replace(":", "")})"
                });
            }

            if (configs.Count < MinPokemonSlots)
            {
                MessageBox.Show($"Please configure at least {MinPokemonSlots} Pokemon.", "Missing Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var result = _runner.CalculateTurnOrder(configs);
                DisplayResults(result, configs);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating turn order: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayResults(TurnOrderRunner.TurnOrderResult result, List<TurnOrderRunner.PokemonConfig> configs)
        {
            // Summary tab
            var summary = new System.Text.StringBuilder();
            summary.AppendLine($"=== TURN ORDER CALCULATION RESULTS ===");
            summary.AppendLine();
            summary.AppendLine($"Level: {numericLevel.Value}");
            summary.AppendLine($"Pokemon Count: {configs.Count}");
            summary.AppendLine();

            summary.AppendLine($"=== FINAL TURN ORDER ===");
            if (result.TurnOrder.Count == 0)
            {
                summary.AppendLine("No actions were created. Check that all Pokemon have valid slots and moves.");
            }
            else
            {
                foreach (var entry in result.TurnOrder)
                {
                    summary.AppendLine($"{entry.Position}. {entry.PokemonName} - {entry.MoveName}");
                    summary.AppendLine($"   Priority: {entry.Priority}, Speed: {entry.EffectiveSpeed:F2}");
                    summary.AppendLine($"   Reasoning: {entry.Reasoning}");
                    summary.AppendLine();
                }
            }
            
            // Show Pokemon that have speed calculations but no turn order entry
            if (result.SpeedCalculations.Count > result.TurnOrder.Count)
            {
                summary.AppendLine();
                summary.AppendLine($"=== NOTE: {result.SpeedCalculations.Count - result.TurnOrder.Count} Pokemon have speed calculations but no turn order entry ===");
                var turnOrderPokemonNames = new HashSet<string>(result.TurnOrder.Select(e => e.PokemonName));
                foreach (var speedCalc in result.SpeedCalculations)
                {
                    if (!turnOrderPokemonNames.Contains(speedCalc.PokemonName))
                    {
                        summary.AppendLine($"- {speedCalc.PokemonName} (Speed: {speedCalc.EffectiveSpeed:F2}) - No action created");
                    }
                }
            }

            this.txtSummary.Text = summary.ToString();

            // Speed Calculation tab
            dgvSpeedCalculation.Columns.Clear();
            dgvSpeedCalculation.Rows.Clear();

            dgvSpeedCalculation.Columns.Add("Pokemon", "Pokemon");
            dgvSpeedCalculation.Columns.Add("Base Speed", "Base Speed");
            dgvSpeedCalculation.Columns.Add("Stat Stage Mult", "Stat Stage");
            dgvSpeedCalculation.Columns.Add("Status Mult", "Status");
            dgvSpeedCalculation.Columns.Add("Side Mult", "Side");
            dgvSpeedCalculation.Columns.Add("Effective Speed", "Effective Speed");
            dgvSpeedCalculation.Columns.Add("Details", "Details");

            foreach (var calc in result.SpeedCalculations)
            {
                var row = new DataGridViewRow();
                row.CreateCells(dgvSpeedCalculation,
                    calc.PokemonName,
                    calc.BaseSpeed.ToString("F2"),
                    calc.StatStageMultiplier.ToString("F2") + "x",
                    calc.StatusMultiplier.ToString("F2") + "x",
                    calc.SideConditionMultiplier.ToString("F2") + "x",
                    calc.EffectiveSpeed.ToString("F2"),
                    calc.Details);
                dgvSpeedCalculation.Rows.Add(row);
            }

            // Adjust column widths
            dgvSpeedCalculation.Columns[0].Width = 150;
            dgvSpeedCalculation.Columns[1].Width = 100;
            dgvSpeedCalculation.Columns[2].Width = 100;
            dgvSpeedCalculation.Columns[3].Width = 100;
            dgvSpeedCalculation.Columns[4].Width = 100;
            dgvSpeedCalculation.Columns[5].Width = 120;
            dgvSpeedCalculation.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Priority tab
            dgvPriority.Columns.Clear();
            dgvPriority.Rows.Clear();

            dgvPriority.Columns.Add("Pokemon", "Pokemon");
            dgvPriority.Columns.Add("Move", "Move");
            dgvPriority.Columns.Add("Priority", "Priority");
            dgvPriority.Columns.Add("Description", "Description");

            foreach (var priority in result.PriorityInfo)
            {
                var row = new DataGridViewRow();
                row.CreateCells(dgvPriority,
                    priority.PokemonName,
                    priority.MoveName,
                    priority.Priority.ToString(),
                    priority.Description);
                dgvPriority.Rows.Add(row);
            }

            // Adjust column widths
            dgvPriority.Columns[0].Width = 150;
            dgvPriority.Columns[1].Width = 150;
            dgvPriority.Columns[2].Width = 80;
            dgvPriority.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Order tab
            dgvOrder.Columns.Clear();
            dgvOrder.Rows.Clear();

            dgvOrder.Columns.Add("Position", "Position");
            dgvOrder.Columns.Add("Pokemon", "Pokemon");
            dgvOrder.Columns.Add("Move", "Move");
            dgvOrder.Columns.Add("Priority", "Priority");
            dgvOrder.Columns.Add("Speed", "Speed");
            dgvOrder.Columns.Add("Reasoning", "Reasoning");

            foreach (var entry in result.TurnOrder)
            {
                var row = new DataGridViewRow();
                row.CreateCells(dgvOrder,
                    entry.Position.ToString(),
                    entry.PokemonName,
                    entry.MoveName,
                    entry.Priority.ToString(),
                    entry.EffectiveSpeed.ToString("F2"),
                    entry.Reasoning);
                dgvOrder.Rows.Add(row);
            }

            // Adjust column widths
            dgvOrder.Columns[0].Width = 80;
            dgvOrder.Columns[1].Width = 150;
            dgvOrder.Columns[2].Width = 150;
            dgvOrder.Columns[3].Width = 80;
            dgvOrder.Columns[4].Width = 100;
            dgvOrder.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
    }
}

