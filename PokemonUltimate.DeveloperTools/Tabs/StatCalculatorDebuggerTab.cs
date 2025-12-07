using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Data.Constants;
using PokemonUltimate.Core.Data.Enums;
using PokemonUltimate.Core.Infrastructure.Localization;
using PokemonUltimate.Core.Services;
using PokemonUltimate.Core.Utilities.Extensions;
using PokemonUltimate.DeveloperTools.Localization;
using PokemonUltimate.DeveloperTools.Runners;

namespace PokemonUltimate.DeveloperTools.Tabs
{
    /// <summary>
    /// Debugger tab for calculating and visualizing Pokemon stats with different configurations.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.1: Stat Calculator Debugger
    /// **Documentation**: See `docs/features/6-development-tools/6.1-stat-calculator-debugger/README.md`
    /// </remarks>
    public partial class StatCalculatorDebuggerTab : UserControl
    {
        private ComboBox comboPokemon = null!;
        private NumericUpDown numericLevel = null!;
        private ComboBox comboNature = null!;
        private NumericUpDown numericIV_HP = null!;
        private NumericUpDown numericIV_Attack = null!;
        private NumericUpDown numericIV_Defense = null!;
        private NumericUpDown numericIV_SpAttack = null!;
        private NumericUpDown numericIV_SpDefense = null!;
        private NumericUpDown numericIV_Speed = null!;
        private NumericUpDown numericEV_HP = null!;
        private NumericUpDown numericEV_Attack = null!;
        private NumericUpDown numericEV_Defense = null!;
        private NumericUpDown numericEV_SpAttack = null!;
        private NumericUpDown numericEV_SpDefense = null!;
        private NumericUpDown numericEV_Speed = null!;
        private Label lblTotalEVs = null!;
        private Button btnCalculate = null!;
        private TabControl tabResults = null!;
        private TabPage tabSummary = null!;
        private TabPage tabStatsTable = null!;
        private RichTextBox txtSummary = null!;
        private DataGridView dgvStatsTable = null!;
        private StatCalculatorRunner _runner = null!;

        public StatCalculatorDebuggerTab()
        {
            InitializeComponent();
            LoadData();
            _runner = new StatCalculatorRunner();
            UpdateTotalEVs(null, EventArgs.Empty); // Initialize total EVs display
        }

        private void InitializeComponent()
        {
            this.comboPokemon = new ComboBox();
            this.numericLevel = new NumericUpDown();
            this.comboNature = new ComboBox();
            this.numericIV_HP = new NumericUpDown();
            this.numericIV_Attack = new NumericUpDown();
            this.numericIV_Defense = new NumericUpDown();
            this.numericIV_SpAttack = new NumericUpDown();
            this.numericIV_SpDefense = new NumericUpDown();
            this.numericIV_Speed = new NumericUpDown();
            this.numericEV_HP = new NumericUpDown();
            this.numericEV_Attack = new NumericUpDown();
            this.numericEV_Defense = new NumericUpDown();
            this.numericEV_SpAttack = new NumericUpDown();
            this.numericEV_SpDefense = new NumericUpDown();
            this.numericEV_Speed = new NumericUpDown();
            this.lblTotalEVs = new Label();
            this.btnCalculate = new Button();
            this.tabResults = new TabControl();
            this.tabSummary = new TabPage();
            this.tabStatsTable = new TabPage();
            this.txtSummary = new RichTextBox();
            this.dgvStatsTable = new DataGridView();

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

            // Panel izquierdo - Configuración (con scroll)
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

            var provider = LocalizationService.Instance;
            var lblTitle = new Label
            {
                Text = "Configuration",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(leftMargin, yPos)
            };
            yPos += 40;

            // Pokemon
            var lblPokemon = new Label { Text = "Pokemon", Location = new Point(leftMargin, yPos), AutoSize = true };
            yPos += 25;
            this.comboPokemon.Location = new Point(leftMargin, yPos);
            this.comboPokemon.Width = controlWidth;
            this.comboPokemon.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboPokemon.Height = 25;
            yPos += spacing;

            // Level
            var lblLevel = new Label { Text = "Level", Location = new Point(leftMargin, yPos), AutoSize = true };
            yPos += 25;
            this.numericLevel.Location = new Point(leftMargin, yPos);
            this.numericLevel.Width = controlWidth;
            this.numericLevel.Minimum = 1;
            this.numericLevel.Maximum = 100;
            this.numericLevel.Value = 50;
            yPos += spacing;

            // Nature
            var lblNature = new Label { Text = "Nature", Location = new Point(leftMargin, yPos), AutoSize = true };
            yPos += 25;
            this.comboNature.Location = new Point(leftMargin, yPos);
            this.comboNature.Width = controlWidth;
            this.comboNature.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboNature.Height = 25;
            yPos += spacing;

            // IVs Section - Compact layout with 2 columns
            var lblIVsTitle = new Label
            {
                Text = "IVs",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(leftMargin, yPos),
                AutoSize = true
            };
            yPos += 25;

            // Create IV controls in 2 columns
            var (numericIV_HP, lblIV_HP) = CreateCompactNumeric(leftMargin, yPos, 150, "HP", 0, CoreConstants.MaxIV, CoreConstants.DefaultIV);
            this.numericIV_HP = numericIV_HP;
            var (numericIV_Attack, lblIV_Attack) = CreateCompactNumeric(leftMargin + 165, yPos, 150, "Atk", 0, CoreConstants.MaxIV, CoreConstants.DefaultIV);
            this.numericIV_Attack = numericIV_Attack;
            yPos += spacing;

            var (numericIV_Defense, lblIV_Defense) = CreateCompactNumeric(leftMargin, yPos, 150, "Def", 0, CoreConstants.MaxIV, CoreConstants.DefaultIV);
            this.numericIV_Defense = numericIV_Defense;
            var (numericIV_SpAttack, lblIV_SpAttack) = CreateCompactNumeric(leftMargin + 165, yPos, 150, "SpA", 0, CoreConstants.MaxIV, CoreConstants.DefaultIV);
            this.numericIV_SpAttack = numericIV_SpAttack;
            yPos += spacing;

            var (numericIV_SpDefense, lblIV_SpDefense) = CreateCompactNumeric(leftMargin, yPos, 150, "SpD", 0, CoreConstants.MaxIV, CoreConstants.DefaultIV);
            this.numericIV_SpDefense = numericIV_SpDefense;
            var (numericIV_Speed, lblIV_Speed) = CreateCompactNumeric(leftMargin + 165, yPos, 150, "Spe", 0, CoreConstants.MaxIV, CoreConstants.DefaultIV);
            this.numericIV_Speed = numericIV_Speed;
            yPos += spacing + 5;

            // EVs Section - Compact layout with 2 columns
            var lblEVsTitle = new Label
            {
                Text = "EVs",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(leftMargin, yPos),
                AutoSize = true
            };
            yPos += 25;

            // Default EVs: 252 Atk, 252 Speed, 4 HP (total 508, under 510 limit)
            var (numericEV_HP, lblEV_HP) = CreateCompactNumeric(leftMargin, yPos, 150, "HP", 0, CoreConstants.MaxEV, 4);
            this.numericEV_HP = numericEV_HP;
            var (numericEV_Attack, lblEV_Attack) = CreateCompactNumeric(leftMargin + 165, yPos, 150, "Atk", 0, CoreConstants.MaxEV, 252);
            this.numericEV_Attack = numericEV_Attack;
            yPos += spacing;

            var (numericEV_Defense, lblEV_Defense) = CreateCompactNumeric(leftMargin, yPos, 150, "Def", 0, CoreConstants.MaxEV, 0);
            this.numericEV_Defense = numericEV_Defense;
            var (numericEV_SpAttack, lblEV_SpAttack) = CreateCompactNumeric(leftMargin + 165, yPos, 150, "SpA", 0, CoreConstants.MaxEV, 0);
            this.numericEV_SpAttack = numericEV_SpAttack;
            yPos += spacing;

            var (numericEV_SpDefense, lblEV_SpDefense) = CreateCompactNumeric(leftMargin, yPos, 150, "SpD", 0, CoreConstants.MaxEV, 0);
            this.numericEV_SpDefense = numericEV_SpDefense;
            var (numericEV_Speed, lblEV_Speed) = CreateCompactNumeric(leftMargin + 165, yPos, 150, "Spe", 0, CoreConstants.MaxEV, 252);
            this.numericEV_Speed = numericEV_Speed;
            yPos += spacing;

            // Total EVs label
            this.lblTotalEVs = new Label
            {
                Text = "Total EVs: 0 / 510",
                Location = new Point(leftMargin, yPos),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            yPos += spacing + 10;

            // Calculate button
            this.btnCalculate = new Button
            {
                Text = "Calculate Stats",
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

            // Attach EV change handlers to update total
            this.numericEV_HP.ValueChanged += UpdateTotalEVs;
            this.numericEV_Attack.ValueChanged += UpdateTotalEVs;
            this.numericEV_Defense.ValueChanged += UpdateTotalEVs;
            this.numericEV_SpAttack.ValueChanged += UpdateTotalEVs;
            this.numericEV_SpDefense.ValueChanged += UpdateTotalEVs;
            this.numericEV_Speed.ValueChanged += UpdateTotalEVs;

            panelConfig.Controls.AddRange(new Control[]
            {
                lblTitle, lblPokemon, comboPokemon,
                lblLevel, numericLevel,
                lblNature, comboNature,
                lblIVsTitle,
                lblIV_HP, numericIV_HP,
                lblIV_Attack, numericIV_Attack,
                lblIV_Defense, numericIV_Defense,
                lblIV_SpAttack, numericIV_SpAttack,
                lblIV_SpDefense, numericIV_SpDefense,
                lblIV_Speed, numericIV_Speed,
                lblEVsTitle,
                lblEV_HP, numericEV_HP,
                lblEV_Attack, numericEV_Attack,
                lblEV_Defense, numericEV_Defense,
                lblEV_SpAttack, numericEV_SpAttack,
                lblEV_SpDefense, numericEV_SpDefense,
                lblEV_Speed, numericEV_Speed,
                lblTotalEVs,
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
            this.txtSummary.Text = "Configure and calculate stats";
            this.tabSummary.Controls.Add(this.txtSummary);

            // Tab Stats Table
            this.tabStatsTable.Text = "Stats Breakdown";
            this.tabStatsTable.Padding = new Padding(10);
            this.dgvStatsTable.Dock = DockStyle.Fill;
            this.dgvStatsTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStatsTable.ReadOnly = true;
            this.dgvStatsTable.AllowUserToAddRows = false;
            this.tabStatsTable.Controls.Add(this.dgvStatsTable);

            this.tabResults.TabPages.AddRange(new TabPage[]
            {
                this.tabSummary,
                this.tabStatsTable
            });

            mainTableLayout.Controls.Add(panelConfig, 0, 0);
            mainTableLayout.Controls.Add(this.tabResults, 1, 0);

            this.Controls.Add(mainTableLayout);

            this.ResumeLayout(false);
        }

        private (NumericUpDown numeric, Label label) CreateCompactNumeric(int x, int y, int width, string labelText, int min, int max, int defaultValue)
        {
            var lbl = new Label
            {
                Text = $"{labelText}:",
                Location = new Point(x, y + 3),
                AutoSize = true,
                Width = 45,
                TextAlign = ContentAlignment.MiddleRight
            };

            var numeric = new NumericUpDown
            {
                Location = new Point(x + 50, y),
                Width = width - 55,
                Height = 22,
                Minimum = min,
                Maximum = max,
                Value = defaultValue
            };

            return (numeric, lbl);
        }

        private void LoadData()
        {
            // Load Pokemon
            var pokemonList = PokemonCatalog.All.OrderBy(p => p.Name).ToList();
            foreach (var pokemon in pokemonList)
            {
                this.comboPokemon.Items.Add(pokemon.Name);
            }
            if (this.comboPokemon.Items.Count > 0)
                this.comboPokemon.SelectedIndex = 0;

            // Load Natures - ensure all 25 natures are loaded
            var provider = LocalizationService.Instance;
            var natures = Enum.GetValues<Nature>().OrderBy(n => n.ToString()).ToList();
            foreach (var nature in natures)
            {
                this.comboNature.Items.Add(new NatureDisplayItem(nature, provider));
            }
            this.comboNature.SelectedIndex = 0;

            // Verify all natures are loaded
            if (this.comboNature.Items.Count != 25)
            {
                MessageBox.Show($"Warning: Only {this.comboNature.Items.Count} natures loaded (expected 25).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UpdateTotalEVs(object? sender, EventArgs e)
        {
            int total = (int)(numericEV_HP.Value + numericEV_Attack.Value + numericEV_Defense.Value +
                             numericEV_SpAttack.Value + numericEV_SpDefense.Value + numericEV_Speed.Value);

            var provider = LocalizationService.Instance;
            this.lblTotalEVs.Text = $"Total EVs: {total} / {CoreConstants.MaxTotalEV}";

            if (total > CoreConstants.MaxTotalEV)
            {
                this.lblTotalEVs.ForeColor = Color.Red;
            }
            else
            {
                this.lblTotalEVs.ForeColor = Color.Black;
            }
        }

        private void BtnCalculate_Click(object? sender, EventArgs e)
        {
            var provider = LocalizationService.Instance;
            if (comboPokemon.SelectedItem == null)
            {
                MessageBox.Show("Please select a Pokemon.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboNature.SelectedItem == null)
            {
                MessageBox.Show("Please select a Nature.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int totalEVs = (int)(numericEV_HP.Value + numericEV_Attack.Value + numericEV_Defense.Value +
                                numericEV_SpAttack.Value + numericEV_SpDefense.Value + numericEV_Speed.Value);

            if (totalEVs > CoreConstants.MaxTotalEV)
            {
                MessageBox.Show($"Total EVs ({totalEVs}) exceeds maximum ({CoreConstants.MaxTotalEV}).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var pokemonName = comboPokemon.SelectedItem.ToString();
            var pokemon = PokemonCatalog.All.FirstOrDefault(p => p.Name == pokemonName);

            if (pokemon == null)
            {
                MessageBox.Show("Pokemon not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get selected nature with validation
            Nature selectedNature = Nature.Hardy; // Default value
            if (comboNature.SelectedItem is NatureDisplayItem natureItem)
            {
                selectedNature = natureItem.Nature;
            }
            else if (comboNature.SelectedItem is Nature nature)
            {
                selectedNature = nature;
            }
            else if (comboNature.SelectedIndex >= 0 && comboNature.SelectedIndex < comboNature.Items.Count)
            {
                // Fallback: get by index
                var item = comboNature.Items[comboNature.SelectedIndex];
                if (item is NatureDisplayItem natureDisplayItem)
                {
                    selectedNature = natureDisplayItem.Nature;
                }
                else if (item is Nature natureByIndex)
                {
                    selectedNature = natureByIndex;
                }
                else if (item != null && Enum.TryParse<Nature>(item.ToString(), out var parsedNature))
                {
                    selectedNature = parsedNature;
                }
                else
                {
                    MessageBox.Show($"Error validating nature: {item?.ToString() ?? "null"}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please select a Nature.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verify nature is valid in NatureData (all 25 natures should be valid)
            // This check ensures the nature exists in the dictionary
            try
            {
                var testMultiplier = NatureData.GetStatMultiplier(selectedNature, Stat.Attack);
                var increasedStat = NatureData.GetIncreasedStat(selectedNature);
                var decreasedStat = NatureData.GetDecreasedStat(selectedNature);
                // If we get here, nature is valid
            }
            catch (KeyNotFoundException ex)
            {
                MessageBox.Show($"Nature {selectedNature} not implemented: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error validating nature {selectedNature}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var config = new StatCalculatorRunner.StatCalculationConfig
            {
                PokemonSpecies = pokemon,
                Level = (int)numericLevel.Value,
                Nature = selectedNature,
                IV_HP = (int)numericIV_HP.Value,
                IV_Attack = (int)numericIV_Attack.Value,
                IV_Defense = (int)numericIV_Defense.Value,
                IV_SpAttack = (int)numericIV_SpAttack.Value,
                IV_SpDefense = (int)numericIV_SpDefense.Value,
                IV_Speed = (int)numericIV_Speed.Value,
                EV_HP = (int)numericEV_HP.Value,
                EV_Attack = (int)numericEV_Attack.Value,
                EV_Defense = (int)numericEV_Defense.Value,
                EV_SpAttack = (int)numericEV_SpAttack.Value,
                EV_SpDefense = (int)numericEV_SpDefense.Value,
                EV_Speed = (int)numericEV_Speed.Value
            };

            var result = _runner.CalculateStats(config);
            DisplayResults(result, config);
        }

        private void DisplayResults(StatCalculatorRunner.StatCalculationResult result, StatCalculatorRunner.StatCalculationConfig config)
        {
            var provider = LocalizationService.Instance;
            // Summary tab
            var summary = new System.Text.StringBuilder();
            summary.AppendLine("=== Stat Calculation Results ===");
            summary.AppendLine();
            summary.AppendLine($"Pokemon: {config.PokemonSpecies.Name}");
            summary.AppendLine($"Level: {config.Level}");
            summary.AppendLine($"Nature: {config.Nature.GetDisplayName(provider)}");
            summary.AppendLine();
            summary.AppendLine($"Total EVs: {result.TotalEVs} / {CoreConstants.MaxTotalEV}");
            if (!result.IsValidEVTotal)
                summary.AppendLine("⚠ Warning: Total EVs exceeds maximum");
            summary.AppendLine();
            summary.AppendLine("=== Final Stats ===");

            // Show stats with nature indicators
            var increasedStat = NatureData.GetIncreasedStat(config.Nature);
            var decreasedStat = NatureData.GetDecreasedStat(config.Nature);

            summary.AppendLine($"{Stat.HP.GetDisplayName(provider)}:       {result.HP.FinalStat,4} (HP not affected by nature)");
            summary.AppendLine($"{Stat.Attack.GetDisplayName(provider)}:   {result.Attack.FinalStat,4} {GetNatureIndicator(Stat.Attack, increasedStat, decreasedStat)}");
            summary.AppendLine($"{Stat.Defense.GetDisplayName(provider)}:  {result.Defense.FinalStat,4} {GetNatureIndicator(Stat.Defense, increasedStat, decreasedStat)}");
            summary.AppendLine($"{Stat.SpAttack.GetDisplayName(provider)}:   {result.SpAttack.FinalStat,4} {GetNatureIndicator(Stat.SpAttack, increasedStat, decreasedStat)}");
            summary.AppendLine($"{Stat.SpDefense.GetDisplayName(provider)}:   {result.SpDefense.FinalStat,4} {GetNatureIndicator(Stat.SpDefense, increasedStat, decreasedStat)}");
            summary.AppendLine($"{Stat.Speed.GetDisplayName(provider)}:    {result.Speed.FinalStat,4} {GetNatureIndicator(Stat.Speed, increasedStat, decreasedStat)}");
            summary.AppendLine();
            summary.AppendLine("=== Nature Effects ===");
            if (increasedStat.HasValue)
            {
                var increasedMultiplier = GetMultiplierForStat(increasedStat.Value, result);
                summary.AppendLine($"Increase {increasedStat.Value.GetDisplayName(provider)}: ×{increasedMultiplier:F2}");
            }
            if (decreasedStat.HasValue)
            {
                var decreasedMultiplier = GetMultiplierForStat(decreasedStat.Value, result);
                summary.AppendLine($"Decrease {decreasedStat.Value.GetDisplayName(provider)}: ×{decreasedMultiplier:F2}");
            }
            if (!increasedStat.HasValue)
                summary.AppendLine("Neutral nature (no stat changes)");

            // Show comparison with raw stats
            summary.AppendLine();
            summary.AppendLine("=== Raw Stats ===");
            summary.AppendLine($"{Stat.Attack.GetDisplayName(provider)}:   {result.Attack.RawStat,4} → {result.Attack.FinalStat,4} (×{result.Attack.NatureMultiplier:F2})");
            summary.AppendLine($"{Stat.Defense.GetDisplayName(provider)}:  {result.Defense.RawStat,4} → {result.Defense.FinalStat,4} (×{result.Defense.NatureMultiplier:F2})");
            summary.AppendLine($"{Stat.SpAttack.GetDisplayName(provider)}:   {result.SpAttack.RawStat,4} → {result.SpAttack.FinalStat,4} (×{result.SpAttack.NatureMultiplier:F2})");
            summary.AppendLine($"{Stat.SpDefense.GetDisplayName(provider)}:   {result.SpDefense.RawStat,4} → {result.SpDefense.FinalStat,4} (×{result.SpDefense.NatureMultiplier:F2})");
            summary.AppendLine($"{Stat.Speed.GetDisplayName(provider)}:    {result.Speed.RawStat,4} → {result.Speed.FinalStat,4} (×{result.Speed.NatureMultiplier:F2})");

            this.txtSummary.Text = summary.ToString();

            // Stats Table tab
            dgvStatsTable.Columns.Clear();
            dgvStatsTable.Rows.Clear();

            dgvStatsTable.Columns.Add("Stat", "Stat");
            dgvStatsTable.Columns.Add("Base", "Base");
            dgvStatsTable.Columns.Add("IV", "IV");
            dgvStatsTable.Columns.Add("EV", "EV");
            dgvStatsTable.Columns.Add("EV Bonus", "EV Bonus");
            dgvStatsTable.Columns.Add("Level", "Level");
            dgvStatsTable.Columns.Add("Nature Mult", "Nature Mult");
            dgvStatsTable.Columns.Add("Raw Stat", "Raw Stat");
            dgvStatsTable.Columns.Add("Final Stat", "Final Stat");

            AddStatRow(result.HP);
            AddStatRow(result.Attack);
            AddStatRow(result.Defense);
            AddStatRow(result.SpAttack);
            AddStatRow(result.SpDefense);
            AddStatRow(result.Speed);

            // Adjust column widths
            dgvStatsTable.Columns[0].Width = 80;
            dgvStatsTable.Columns[1].Width = 60;
            dgvStatsTable.Columns[2].Width = 50;
            dgvStatsTable.Columns[3].Width = 50;
            dgvStatsTable.Columns[4].Width = 70;
            dgvStatsTable.Columns[5].Width = 60;
            dgvStatsTable.Columns[6].Width = 90;
            dgvStatsTable.Columns[7].Width = 80;
            dgvStatsTable.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void AddStatRow(StatCalculatorRunner.StatBreakdown breakdown)
        {
            var provider = LocalizationService.Instance;
            var row = new DataGridViewRow();
            row.CreateCells(dgvStatsTable,
                breakdown.Stat.GetDisplayName(provider),
                breakdown.BaseStat,
                breakdown.IV,
                breakdown.EV,
                breakdown.EVBonus,
                breakdown.Level,
                breakdown.NatureMultiplier.ToString("F2"),
                breakdown.RawStat,
                breakdown.FinalStat);

            // Highlight if nature affects this stat
            if (breakdown.NatureMultiplier > 1.0f)
                row.DefaultCellStyle.BackColor = Color.LightGreen;
            else if (breakdown.NatureMultiplier < 1.0f)
                row.DefaultCellStyle.BackColor = Color.LightCoral;

            dgvStatsTable.Rows.Add(row);
        }

        private string GetNatureIndicator(Stat stat, Stat? increasedStat, Stat? decreasedStat)
        {
            if (increasedStat == stat)
                return "(+10%)";
            if (decreasedStat == stat)
                return "(-10%)";
            return "";
        }

        private float GetMultiplierForStat(Stat stat, StatCalculatorRunner.StatCalculationResult result)
        {
            return stat switch
            {
                Stat.Attack => result.Attack.NatureMultiplier,
                Stat.Defense => result.Defense.NatureMultiplier,
                Stat.SpAttack => result.SpAttack.NatureMultiplier,
                Stat.SpDefense => result.SpDefense.NatureMultiplier,
                Stat.Speed => result.Speed.NatureMultiplier,
                _ => 1.0f
            };
        }

        /// <summary>
        /// Helper class to display Nature with translated names in ComboBox.
        /// </summary>
        private class NatureDisplayItem
        {
            public Nature Nature { get; }
            private readonly ILocalizationProvider _provider;

            public NatureDisplayItem(Nature nature, ILocalizationProvider provider)
            {
                Nature = nature;
                _provider = provider;
            }

            public override string ToString()
            {
                return Nature.GetDisplayName(_provider);
            }
        }
    }
}

