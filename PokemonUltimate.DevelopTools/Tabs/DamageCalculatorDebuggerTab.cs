using System;
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
    /// Debugger tab for step-by-step damage calculation pipeline visualization.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.2: Damage Calculator Debugger
    /// **Documentation**: See `docs/features/6-development-tools/6.2-damage-calculator-debugger/README.md`
    /// </remarks>
    public partial class DamageCalculatorDebuggerTab : UserControl
    {
        private ComboBox comboAttacker = null!;
        private ComboBox comboDefender = null!;
        private ComboBox comboMove = null!;
        private NumericUpDown numericLevel = null!;
        private CheckBox chkForceCritical = null!;
        private CheckBox chkFixedRandom = null!;
        private NumericUpDown numericFixedRandom = null!;
        private Button btnCalculate = null!;
        private TabControl tabResults = null!;
        private TabPage tabSummary = null!;
        private TabPage tabPipelineSteps = null!;
        private TabPage tabDamageRange = null!;
        private RichTextBox txtSummary = null!;
        private DataGridView dgvPipelineSteps = null!;
        private RichTextBox txtDamageRange = null!;
        private DamageCalculatorRunner _runner = null!;

        public DamageCalculatorDebuggerTab()
        {
            InitializeComponent();
            LoadData();
            _runner = new DamageCalculatorRunner();
        }

        private void InitializeComponent()
        {
            this.comboAttacker = new ComboBox();
            this.comboDefender = new ComboBox();
            this.comboMove = new ComboBox();
            this.numericLevel = new NumericUpDown();
            this.chkForceCritical = new CheckBox();
            this.chkFixedRandom = new CheckBox();
            this.numericFixedRandom = new NumericUpDown();
            this.btnCalculate = new Button();
            this.tabResults = new TabControl();
            this.tabSummary = new TabPage();
            this.tabPipelineSteps = new TabPage();
            this.tabDamageRange = new TabPage();
            this.txtSummary = new RichTextBox();
            this.dgvPipelineSteps = new DataGridView();
            this.txtDamageRange = new RichTextBox();

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
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360));
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

            int yPos = 10;
            int spacing = 35;
            int controlWidth = 320;
            int leftMargin = 5;

            var lblTitle = new Label
            {
                Text = "Configuration",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(leftMargin, yPos)
            };
            yPos += 40;

            // Attacker Pokemon
            var lblAttacker = new Label { Text = "Attacker Pokemon:", Location = new Point(leftMargin, yPos), AutoSize = true };
            yPos += 25;
            this.comboAttacker.Location = new Point(leftMargin, yPos);
            this.comboAttacker.Width = controlWidth;
            this.comboAttacker.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboAttacker.Height = 25;
            yPos += spacing;

            // Defender Pokemon
            var lblDefender = new Label { Text = "Defender Pokemon:", Location = new Point(leftMargin, yPos), AutoSize = true };
            yPos += 25;
            this.comboDefender.Location = new Point(leftMargin, yPos);
            this.comboDefender.Width = controlWidth;
            this.comboDefender.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboDefender.Height = 25;
            yPos += spacing;

            // Move
            var lblMove = new Label { Text = "Move:", Location = new Point(leftMargin, yPos), AutoSize = true };
            yPos += 25;
            this.comboMove.Location = new Point(leftMargin, yPos);
            this.comboMove.Width = controlWidth;
            this.comboMove.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboMove.Height = 25;
            yPos += spacing;

            // Level
            var lblLevel = new Label { Text = "Level:", Location = new Point(leftMargin, yPos), AutoSize = true };
            yPos += 25;
            this.numericLevel.Location = new Point(leftMargin, yPos);
            this.numericLevel.Width = controlWidth;
            this.numericLevel.Minimum = 1;
            this.numericLevel.Maximum = 100;
            this.numericLevel.Value = 50;
            yPos += spacing;

            // Force Critical
            this.chkForceCritical = new CheckBox
            {
                Text = "Force Critical Hit",
                Location = new Point(leftMargin, yPos),
                AutoSize = true
            };
            yPos += spacing;

            // Fixed Random
            this.chkFixedRandom = new CheckBox
            {
                Text = "Use Fixed Random Value",
                Location = new Point(leftMargin, yPos),
                AutoSize = true
            };
            this.chkFixedRandom.CheckedChanged += ChkFixedRandom_CheckedChanged;
            yPos += 25;
            this.numericFixedRandom.Location = new Point(leftMargin + 20, yPos);
            this.numericFixedRandom.Width = controlWidth - 20;
            this.numericFixedRandom.Minimum = 0;
            this.numericFixedRandom.Maximum = 100;
            this.numericFixedRandom.DecimalPlaces = 2;
            this.numericFixedRandom.Increment = 0.01m;
            this.numericFixedRandom.Value = 0.925m; // Default to middle of range
            this.numericFixedRandom.Enabled = false;
            yPos += spacing + 10;

            // Calculate button
            this.btnCalculate = new Button
            {
                Text = "Calculate Damage",
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
                lblTitle, lblAttacker, comboAttacker,
                lblDefender, comboDefender,
                lblMove, comboMove,
                lblLevel, numericLevel,
                chkForceCritical,
                chkFixedRandom, numericFixedRandom,
                btnCalculate
            });

            // Set panel height to ensure scroll works
            panelConfig.AutoScrollMinSize = new Size(0, yPos + 80);

            // Panel derecho - Resultados
            this.tabResults.Dock = DockStyle.Fill;
            this.tabResults.Padding = new Point(10, 5);

            // Tab Summary
            this.tabSummary.Text = "Summary";
            this.tabSummary.Padding = new Padding(10);
            this.txtSummary.Dock = DockStyle.Fill;
            this.txtSummary.Font = new Font("Consolas", 9);
            this.txtSummary.ReadOnly = true;
            this.txtSummary.Text = "Configure settings and click 'Calculate Damage' to see results here.";
            this.tabSummary.Controls.Add(this.txtSummary);

            // Tab Pipeline Steps
            this.tabPipelineSteps.Text = "Pipeline Steps";
            this.tabPipelineSteps.Padding = new Padding(10);
            this.dgvPipelineSteps.Dock = DockStyle.Fill;
            this.dgvPipelineSteps.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPipelineSteps.ReadOnly = true;
            this.dgvPipelineSteps.AllowUserToAddRows = false;
            this.tabPipelineSteps.Controls.Add(this.dgvPipelineSteps);

            // Tab Damage Range
            this.tabDamageRange.Text = "Damage Range";
            this.tabDamageRange.Padding = new Padding(10);
            this.txtDamageRange.Dock = DockStyle.Fill;
            this.txtDamageRange.Font = new Font("Consolas", 9);
            this.txtDamageRange.ReadOnly = true;
            this.tabDamageRange.Controls.Add(this.txtDamageRange);

            this.tabResults.TabPages.AddRange(new TabPage[]
            {
                this.tabSummary,
                this.tabPipelineSteps,
                this.tabDamageRange
            });

            mainTableLayout.Controls.Add(panelConfig, 0, 0);
            mainTableLayout.Controls.Add(this.tabResults, 1, 0);

            this.Controls.Add(mainTableLayout);

            this.ResumeLayout(false);
        }

        private void ChkFixedRandom_CheckedChanged(object? sender, EventArgs e)
        {
            this.numericFixedRandom.Enabled = this.chkFixedRandom.Checked;
        }

        private void LoadData()
        {
            // Load Pokemon
            var pokemonList = PokemonCatalog.All.OrderBy(p => p.Name).ToList();
            foreach (var pokemon in pokemonList)
            {
                this.comboAttacker.Items.Add(pokemon.Name);
                this.comboDefender.Items.Add(pokemon.Name);
            }
            if (this.comboAttacker.Items.Count > 0)
                this.comboAttacker.SelectedIndex = 0;
            if (this.comboDefender.Items.Count > 0)
                this.comboDefender.SelectedIndex = Math.Min(1, this.comboDefender.Items.Count - 1);

            // Load Moves
            var moveList = MoveCatalog.All.OrderBy(m => m.Name).ToList();
            foreach (var move in moveList)
            {
                this.comboMove.Items.Add(move.Name);
            }
            if (this.comboMove.Items.Count > 0)
                this.comboMove.SelectedIndex = 0;
        }

        private void BtnCalculate_Click(object? sender, EventArgs e)
        {
            if (comboAttacker.SelectedItem == null)
            {
                MessageBox.Show("Please select an attacker Pokemon.", "Missing Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboDefender.SelectedItem == null)
            {
                MessageBox.Show("Please select a defender Pokemon.", "Missing Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboMove.SelectedItem == null)
            {
                MessageBox.Show("Please select a move.", "Missing Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var attackerName = comboAttacker.SelectedItem.ToString();
            var defenderName = comboDefender.SelectedItem.ToString();
            var moveName = comboMove.SelectedItem.ToString();

            var attacker = PokemonCatalog.All.FirstOrDefault(p => p.Name == attackerName);
            var defender = PokemonCatalog.All.FirstOrDefault(p => p.Name == defenderName);
            var move = MoveCatalog.All.FirstOrDefault(m => m.Name == moveName);

            if (attacker == null)
            {
                MessageBox.Show("Selected attacker Pokemon not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (defender == null)
            {
                MessageBox.Show("Selected defender Pokemon not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (move == null)
            {
                MessageBox.Show("Selected move not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var config = new DamageCalculatorRunner.DamageCalculationConfig
            {
                AttackerSpecies = attacker,
                DefenderSpecies = defender,
                Move = move,
                Level = (int)numericLevel.Value,
                ForceCritical = chkForceCritical.Checked,
                FixedRandomValue = chkFixedRandom.Checked ? (float?)((float)numericFixedRandom.Value) : null
            };

            var result = _runner.CalculateDamage(config);
            DisplayResults(result, config);
        }

        private void DisplayResults(DamageCalculatorRunner.DamageCalculationResult result, DamageCalculatorRunner.DamageCalculationConfig config)
        {
            // Summary tab
            var summary = new System.Text.StringBuilder();
            summary.AppendLine($"=== DAMAGE CALCULATION RESULTS ===");
            summary.AppendLine();
            summary.AppendLine($"Attacker: {config.AttackerSpecies.Name} (Level {config.Level})");
            summary.AppendLine($"Defender: {config.DefenderSpecies.Name} (Level {config.Level})");
            summary.AppendLine($"Move: {config.Move.Name} ({config.Move.Type} {config.Move.Category}, Power: {config.Move.Power})");
            summary.AppendLine();
            summary.AppendLine($"=== FINAL DAMAGE ===");
            summary.AppendLine($"Base Damage: {result.BaseDamage:F2}");
            summary.AppendLine($"Final Multiplier: {result.FinalMultiplier:F4}x");
            summary.AppendLine($"Final Damage: {result.FinalDamage} HP");
            summary.AppendLine($"Final Damage (float): {result.FinalDamageFloat:F2} HP");
            summary.AppendLine();
            summary.AppendLine($"Defender Max HP: {result.DefenderMaxHP}");
            summary.AppendLine($"Damage Percentage: {result.DamagePercentage:F2}%");
            summary.AppendLine();
            summary.AppendLine($"=== DETAILS ===");
            summary.AppendLine($"Critical Hit: {(result.IsCritical ? "Yes (×1.5)" : "No")}");
            summary.AppendLine($"STAB: {(result.IsStab ? "Yes (×1.5)" : "No")}");
            summary.AppendLine($"Random Factor: {result.RandomFactor:F3} (range: 0.85-1.0)");
            summary.AppendLine($"Type Effectiveness: {result.TypeEffectiveness:F2}x ({GetTypeEffectivenessName(result.TypeEffectiveness)})");
            summary.AppendLine();
            summary.AppendLine($"=== DAMAGE RANGE ===");
            summary.AppendLine($"Minimum Damage: {result.MinDamage} HP ({GetDamagePercentage(result.MinDamage, result.DefenderMaxHP):F2}%)");
            summary.AppendLine($"Maximum Damage: {result.MaxDamage} HP ({GetDamagePercentage(result.MaxDamage, result.DefenderMaxHP):F2}%)");

            this.txtSummary.Text = summary.ToString();

            // Pipeline Steps tab
            dgvPipelineSteps.Columns.Clear();
            dgvPipelineSteps.Rows.Clear();

            dgvPipelineSteps.Columns.Add("Step", "Step");
            dgvPipelineSteps.Columns.Add("Multiplier Before", "Mult Before");
            dgvPipelineSteps.Columns.Add("Multiplier After", "Mult After");
            dgvPipelineSteps.Columns.Add("Change", "Change");
            dgvPipelineSteps.Columns.Add("Damage Before", "Dmg Before");
            dgvPipelineSteps.Columns.Add("Damage After", "Dmg After");
            dgvPipelineSteps.Columns.Add("Details", "Details");

            foreach (var step in result.PipelineSteps)
            {
                var row = new DataGridViewRow();
                row.CreateCells(dgvPipelineSteps,
                    step.StepName,
                    step.MultiplierBefore.ToString("F4"),
                    step.MultiplierAfter.ToString("F4"),
                    step.MultiplierChange.ToString("F4"),
                    step.DamageBefore.ToString("F2"),
                    step.DamageAfter.ToString("F2"),
                    step.Details);
                dgvPipelineSteps.Rows.Add(row);
            }

            // Adjust column widths
            dgvPipelineSteps.Columns[0].Width = 150;
            dgvPipelineSteps.Columns[1].Width = 100;
            dgvPipelineSteps.Columns[2].Width = 100;
            dgvPipelineSteps.Columns[3].Width = 80;
            dgvPipelineSteps.Columns[4].Width = 100;
            dgvPipelineSteps.Columns[5].Width = 100;
            dgvPipelineSteps.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Damage Range tab
            var rangeText = new System.Text.StringBuilder();
            rangeText.AppendLine($"=== DAMAGE RANGE ANALYSIS ===");
            rangeText.AppendLine();
            rangeText.AppendLine($"Base Damage: {result.BaseDamage:F2}");
            rangeText.AppendLine($"Final Multiplier (without random): {result.FinalMultiplier / result.RandomFactor:F4}x");
            rangeText.AppendLine();
            rangeText.AppendLine($"=== RANDOM FACTOR EFFECTS ===");
            rangeText.AppendLine($"Random factor range: 0.85 - 1.0");
            rangeText.AppendLine();
            rangeText.AppendLine($"Minimum Damage (0.85x random):");
            rangeText.AppendLine($"  Damage: {result.MinDamage} HP");
            rangeText.AppendLine($"  Percentage: {GetDamagePercentage(result.MinDamage, result.DefenderMaxHP):F2}%");
            rangeText.AppendLine();
            rangeText.AppendLine($"Maximum Damage (1.0x random):");
            rangeText.AppendLine($"  Damage: {result.MaxDamage} HP");
            rangeText.AppendLine($"  Percentage: {GetDamagePercentage(result.MaxDamage, result.DefenderMaxHP):F2}%");
            rangeText.AppendLine();
            rangeText.AppendLine($"Damage Spread: {result.MaxDamage - result.MinDamage} HP");
            rangeText.AppendLine($"Average Damage: {(result.MinDamage + result.MaxDamage) / 2} HP");

            this.txtDamageRange.Text = rangeText.ToString();
        }

        private string GetTypeEffectivenessName(float effectiveness)
        {
            if (effectiveness == 0f)
                return "Immune";
            if (effectiveness == 0.25f)
                return "0.25x";
            if (effectiveness == 0.5f)
                return "0.5x";
            if (effectiveness == 1f)
                return "1x";
            if (effectiveness == 2f)
                return "2x";
            if (effectiveness == 4f)
                return "4x";
            return $"{effectiveness:F2}x";
        }

        private float GetDamagePercentage(int damage, int maxHP)
        {
            if (maxHP <= 0)
                return 0f;
            return (damage / (float)maxHP) * 100f;
        }
    }
}

