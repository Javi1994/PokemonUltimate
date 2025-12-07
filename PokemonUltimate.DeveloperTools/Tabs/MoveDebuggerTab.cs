using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Effects;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Extensions;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Localization;
using PokemonUltimate.DeveloperTools.Localization;
using PokemonUltimate.DeveloperTools.Runners;

namespace PokemonUltimate.DeveloperTools.Tabs
{
    public partial class MoveDebuggerTab : UserControl
    {
        private ComboBox comboMove = null!;
        private ComboBox comboAttackerPokemon = null!;
        private ComboBox comboTargetPokemon = null!;
        private NumericUpDown numericLevel = null!;
        private NumericUpDown numericTests = null!;
        private CheckBox checkDetailedOutput = null!;
        private Button btnRun = null!;
        private TabControl tabResults = null!;
        private TabPage tabSummary = null!;
        private TabPage tabDamage = null!;
        private TabPage tabStatusEffects = null!;
        private TabPage tabActions = null!;
        private RichTextBox txtSummary = null!;
        private DataGridView dgvDamage = null!;
        private DataGridView dgvStatusEffects = null!;
        private DataGridView dgvActions = null!;
        private ProgressBar progressBar = null!;
        private Label lblStatus = null!;

        public MoveDebuggerTab()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.comboMove = new ComboBox();
            this.comboAttackerPokemon = new ComboBox();
            this.comboTargetPokemon = new ComboBox();
            this.numericLevel = new NumericUpDown();
            this.numericTests = new NumericUpDown();
            this.checkDetailedOutput = new CheckBox();
            this.btnRun = new Button();
            this.tabResults = new TabControl();
            this.tabSummary = new TabPage();
            this.tabDamage = new TabPage();
            this.tabStatusEffects = new TabPage();
            this.tabActions = new TabPage();
            this.txtSummary = new RichTextBox();
            this.dgvDamage = new DataGridView();
            this.dgvStatusEffects = new DataGridView();
            this.dgvActions = new DataGridView();
            this.progressBar = new ProgressBar();
            this.lblStatus = new Label();

            this.SuspendLayout();

            // UserControl
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(0);

            // Usar TableLayoutPanel para evitar solapamientos
            var mainTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10)
            };
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350));
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Panel izquierdo - Configuración
            var panelConfig = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20),
                AutoScroll = false,
                Margin = new Padding(5)
            };

            int yPos = 10;
            int spacing = 50;
            int controlWidth = 290;
            int leftMargin = 5;

            var provider = LocalizationManager.Instance;
            var lblTitle = new Label
            {
                Text = "Configuration",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(leftMargin, yPos)
            };
            yPos += 40;

            var lblMove = new Label
            {
                Text = "Move to Test",
                Location = new Point(leftMargin, yPos),
                AutoSize = true
            };
            yPos += 25;
            this.comboMove.Location = new Point(leftMargin, yPos);
            this.comboMove.Width = controlWidth;
            this.comboMove.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboMove.Height = 25;
            yPos += spacing;

            var lblAttackerPokemon = new Label
            {
                Text = "Attacker Pokemon",
                Location = new Point(leftMargin, yPos),
                AutoSize = true
            };
            yPos += 25;
            this.comboAttackerPokemon.Location = new Point(leftMargin, yPos);
            this.comboAttackerPokemon.Width = controlWidth;
            this.comboAttackerPokemon.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboAttackerPokemon.Height = 25;
            yPos += spacing;

            var lblTargetPokemon = new Label
            {
                Text = "Target Pokemon",
                Location = new Point(leftMargin, yPos),
                AutoSize = true
            };
            yPos += 25;
            this.comboTargetPokemon.Location = new Point(leftMargin, yPos);
            this.comboTargetPokemon.Width = controlWidth;
            this.comboTargetPokemon.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboTargetPokemon.Height = 25;
            yPos += spacing;

            var lblLevel = new Label
            {
                Text = "Level",
                Location = new Point(leftMargin, yPos),
                AutoSize = true
            };
            yPos += 25;
            this.numericLevel.Location = new Point(leftMargin, yPos);
            this.numericLevel.Width = controlWidth;
            this.numericLevel.Minimum = 1;
            this.numericLevel.Maximum = 100;
            this.numericLevel.Value = 50;
            yPos += spacing;

            var lblTests = new Label
            {
                Text = "Number of Tests",
                Location = new Point(leftMargin, yPos),
                AutoSize = true
            };
            yPos += 25;
            this.numericTests.Location = new Point(leftMargin, yPos);
            this.numericTests.Width = controlWidth;
            this.numericTests.Minimum = 1;
            this.numericTests.Maximum = 10000;
            this.numericTests.Value = 100;
            yPos += spacing;

            this.checkDetailedOutput.Text = "Detailed Output";
            this.checkDetailedOutput.Location = new Point(leftMargin, yPos);
            this.checkDetailedOutput.AutoSize = true;
            yPos += 45;

            this.btnRun.Text = "Run Tests";
            this.btnRun.Location = new Point(leftMargin, yPos);
            this.btnRun.Width = controlWidth;
            this.btnRun.Height = 50;
            this.btnRun.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            this.btnRun.Click += BtnRun_Click;
            yPos += 65;

            this.progressBar.Location = new Point(leftMargin, yPos);
            this.progressBar.Width = controlWidth;
            this.progressBar.Height = 25;
            this.progressBar.Style = ProgressBarStyle.Continuous;
            yPos += 40;

            this.lblStatus.Text = "Ready";
            this.lblStatus.Location = new Point(leftMargin, yPos);
            this.lblStatus.AutoSize = true;
            this.lblStatus.Width = controlWidth;

            panelConfig.Controls.AddRange(new Control[] {
                lblTitle, lblMove, comboMove,
                lblAttackerPokemon, comboAttackerPokemon,
                lblTargetPokemon, comboTargetPokemon,
                lblLevel, numericLevel,
                lblTests, numericTests,
                checkDetailedOutput,
                btnRun,
                progressBar,
                lblStatus
            });

            // Panel derecho - Resultados
            this.tabResults.Dock = DockStyle.Fill;
            this.tabResults.Padding = new Point(10, 5);
            this.tabResults.Margin = new Padding(5, 5, 5, 5);

            // Tab Summary
            this.tabSummary.Text = "Summary";
            this.tabSummary.Padding = new Padding(10);
            this.txtSummary.Dock = DockStyle.Fill;
            this.txtSummary.Font = new Font("Consolas", 9);
            this.txtSummary.ReadOnly = true;
            this.txtSummary.Margin = new Padding(10);
            this.txtSummary.Text = "Configure and run tests";
            this.tabSummary.Controls.Add(this.txtSummary);

            // Tab Damage
            this.tabDamage.Text = "Damage";
            this.tabDamage.Padding = new Padding(10);
            this.dgvDamage.Dock = DockStyle.Fill;
            this.dgvDamage.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDamage.ReadOnly = true;
            this.dgvDamage.AllowUserToAddRows = false;
            this.dgvDamage.Margin = new Padding(10);
            this.tabDamage.Controls.Add(this.dgvDamage);

            // Tab Status Effects
            this.tabStatusEffects.Text = "Status Effects";
            this.tabStatusEffects.Padding = new Padding(10);
            this.dgvStatusEffects.Dock = DockStyle.Fill;
            this.dgvStatusEffects.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStatusEffects.ReadOnly = true;
            this.dgvStatusEffects.AllowUserToAddRows = false;
            this.dgvStatusEffects.Margin = new Padding(10);
            this.tabStatusEffects.Controls.Add(this.dgvStatusEffects);

            // Tab Actions
            this.tabActions.Text = "Actions";
            this.tabActions.Padding = new Padding(10);
            this.dgvActions.Dock = DockStyle.Fill;
            this.dgvActions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvActions.ReadOnly = true;
            this.dgvActions.AllowUserToAddRows = false;
            this.dgvActions.Margin = new Padding(10);
            this.tabActions.Controls.Add(this.dgvActions);

            this.tabResults.TabPages.AddRange(new TabPage[] {
                this.tabSummary,
                this.tabDamage,
                this.tabStatusEffects,
                this.tabActions
            });

            // Agregar paneles al TableLayoutPanel
            mainTableLayout.Controls.Add(panelConfig, 0, 0);
            mainTableLayout.Controls.Add(this.tabResults, 1, 0);

            // Agregar TableLayoutPanel al UserControl
            this.Controls.Add(mainTableLayout);

            this.ResumeLayout(true);
            this.PerformLayout();
        }

        private void LoadData()
        {
            // Cargar movimientos con nombres traducidos
            var provider = LocalizationManager.Instance;
            var moveList = MoveCatalog.All.OrderBy(m => m.Name).ToList();
            foreach (var move in moveList)
            {
                this.comboMove.Items.Add(move.GetDisplayName(provider));
            }
            if (this.comboMove.Items.Count > 0)
            {
                // Seleccionar Thunderbolt por defecto si existe (buscar por nombre traducido)
                var thunderbolt = MoveCatalog.All.FirstOrDefault(m => m.Name == "Thunderbolt");
                if (thunderbolt != null)
                {
                    var thunderboltDisplayName = thunderbolt.GetDisplayName(provider);
                    var thunderboltIndex = this.comboMove.Items.IndexOf(thunderboltDisplayName);
                    this.comboMove.SelectedIndex = thunderboltIndex >= 0 ? thunderboltIndex : 0;
                }
                else
                {
                    this.comboMove.SelectedIndex = 0;
                }
            }

            // Cargar Pokemon
            var pokemonList = PokemonCatalog.All.OrderBy(p => p.Name).ToList();
            foreach (var pokemon in pokemonList)
            {
                this.comboAttackerPokemon.Items.Add(pokemon.Name);
                this.comboTargetPokemon.Items.Add(pokemon.Name);
            }

            if (this.comboAttackerPokemon.Items.Count > 0)
            {
                var pikachuIndex = this.comboAttackerPokemon.Items.IndexOf("Pikachu");
                this.comboAttackerPokemon.SelectedIndex = pikachuIndex >= 0 ? pikachuIndex : 0;
            }

            if (this.comboTargetPokemon.Items.Count > 0)
            {
                var charmanderIndex = this.comboTargetPokemon.Items.IndexOf("Charmander");
                this.comboTargetPokemon.SelectedIndex = charmanderIndex >= 0 ? charmanderIndex : 0;
            }
        }

        private async void BtnRun_Click(object? sender, EventArgs e)
        {
            var provider = LocalizationManager.Instance;
            this.btnRun.Enabled = false;
            this.progressBar.Value = 0;
            this.lblStatus.Text = "Running tests...";
            this.txtSummary.Text = "Running tests...\n\nPlease wait...";

            try
            {
                // Obtener configuración
                var moveDisplayName = this.comboMove.SelectedItem?.ToString();
                var attackerName = this.comboAttackerPokemon.SelectedItem?.ToString();
                var targetName = this.comboTargetPokemon.SelectedItem?.ToString();

                if (moveDisplayName == null || attackerName == null || targetName == null)
                {
                    MessageBox.Show("Please select move, attacker, and target.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Buscar movimiento por nombre traducido
                var move = MoveCatalog.All.FirstOrDefault(m => m.GetDisplayName(provider) == moveDisplayName);
                var attackerPokemon = PokemonCatalog.All.FirstOrDefault(p => p.Name == attackerName);
                var targetPokemon = PokemonCatalog.All.FirstOrDefault(p => p.Name == targetName);

                if (move == null || attackerPokemon == null || targetPokemon == null)
                {
                    MessageBox.Show("Invalid selection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var config = new MoveRunner.MoveTestConfig
                {
                    MoveToTest = move,
                    AttackerPokemon = attackerPokemon,
                    TargetPokemon = targetPokemon,
                    Level = (int)this.numericLevel.Value,
                    NumberOfTests = (int)this.numericTests.Value,
                    DetailedOutput = this.checkDetailedOutput.Checked
                };

                // Crear progress reporter
                var progress = new Progress<int>(percent =>
                {
                    this.progressBar.Value = percent;
                    this.lblStatus.Text = $"Running tests... {percent}%";
                    Application.DoEvents();
                });

                // Ejecutar pruebas
                var runner = new MoveRunner();
                var stats = await runner.RunTestsAsync(config, progress);

                // Mostrar resultados
                DisplayResults(stats, config);

                this.lblStatus.Text = "Complete";
                this.progressBar.Value = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.lblStatus.Text = "Error occurred";
                this.txtSummary.Text = $"Error: {ex.Message}";
            }
            finally
            {
                this.btnRun.Enabled = true;
            }
        }

        private void DisplayResults(MoveRunner.MoveTestStatistics stats, MoveRunner.MoveTestConfig config)
        {
            var provider = LocalizationManager.Instance;
            var move = config.MoveToTest;
            var attackerName = config.AttackerPokemon.Name;
            var targetName = config.TargetPokemon.Name;
            var typeEffectiveness = TypeEffectiveness.GetEffectiveness(
                move.Type,
                config.TargetPokemon.PrimaryType,
                config.TargetPokemon.SecondaryType);

            // Mostrar resumen
            this.txtSummary.Clear();
            this.txtSummary.AppendText("═══════════════════════════════════════════════════════════════\n");
            this.txtSummary.AppendText("Move Information\n");
            this.txtSummary.AppendText("═══════════════════════════════════════════════════════════════\n\n");
            this.txtSummary.AppendText($"Move: {move.GetDisplayName(provider)}\n");
            this.txtSummary.AppendText($"  Type: {move.Type.GetDisplayName(provider)}\n");
            this.txtSummary.AppendText($"  Power: {move.Power}\n");
            this.txtSummary.AppendText($"  Category: {move.Category.GetDisplayName(provider)}\n");
            this.txtSummary.AppendText($"  Accuracy: {move.Accuracy}%\n");
            this.txtSummary.AppendText($"  PP: {move.MaxPP}\n");
            this.txtSummary.AppendText($"  Priority: {move.Priority}\n");

            if (move.Effects != null && move.Effects.Count > 0)
            {
                this.txtSummary.AppendText($"\n  Move Effects:\n");
                foreach (var effect in move.Effects)
                {
                    var effectType = effect.GetType().Name.Replace("Effect", "");
                    string effectDescription = effect.Description;

                    // Traducir estados en la descripción si es un StatusEffect
                    if (effect is StatusEffect statusEffect)
                    {
                        var statusName = statusEffect.Status.GetDisplayName(provider);
                        effectDescription = effectDescription.Replace(statusEffect.Status.ToString(), statusName);
                    }
                    else if (effect is VolatileStatusEffect volatileStatusEffect)
                    {
                        var statusName = volatileStatusEffect.Status.GetDisplayName(provider);
                        effectDescription = effectDescription.Replace(volatileStatusEffect.Status.ToString(), statusName);
                    }

                    this.txtSummary.AppendText($"    - {effectType}: {effectDescription}\n");
                }
            }

            this.txtSummary.AppendText($"\nType Effectiveness: {typeEffectiveness:F2}x\n");
            this.txtSummary.AppendText("\n");

            // Estadísticas de daño
            if (stats.DamageValues.Count > 0)
            {
                var avgDamage = stats.DamageValues.Average();
                var minDamage = stats.DamageValues.Min();
                var maxDamage = stats.DamageValues.Max();
                var medianDamage = stats.DamageValues.OrderBy(d => d).Skip(stats.DamageValues.Count / 2).First();

                this.txtSummary.AppendText("═══════════════════════════════════════════════════════════════\n");
                this.txtSummary.AppendText("Damage Statistics\n");
                this.txtSummary.AppendText("═══════════════════════════════════════════════════════════════\n\n");
                this.txtSummary.AppendText($"Total Successful Hits: {stats.DamageValues.Count} / {config.NumberOfTests}\n");
                this.txtSummary.AppendText($"Average Damage: {avgDamage:F1} HP\n");
                this.txtSummary.AppendText($"Min Damage: {minDamage} HP\n");
                this.txtSummary.AppendText($"Max Damage: {maxDamage} HP\n");
                this.txtSummary.AppendText($"Median Damage: {medianDamage} HP\n");

                if (stats.CriticalHits > 0)
                {
                    var critRate = (stats.CriticalHits * 100.0) / stats.DamageValues.Count;
                    this.txtSummary.AppendText($"Critical Hits: {stats.CriticalHits} ({critRate:F1}% of successful hits)\n");
                }
            }

            if (stats.Misses > 0)
            {
                var missRate = (stats.Misses * 100.0) / config.NumberOfTests;
                this.txtSummary.AppendText($"Misses: {stats.Misses} ({missRate:F1}% of total)\n");
            }

            // Estadísticas de efectos de estado
            if (stats.StatusEffectsCaused.Count > 0)
            {
                var totalStatusEffects = stats.StatusEffectsCaused.Values.Sum();
                this.txtSummary.AppendText("\n═══════════════════════════════════════════════════════════════\n");
                this.txtSummary.AppendText("Status Effects Caused\n");
                this.txtSummary.AppendText("═══════════════════════════════════════════════════════════════\n\n");
                this.txtSummary.AppendText($"Total: {totalStatusEffects} effects in {config.NumberOfTests} tests\n");
                foreach (var effect in stats.StatusEffectsCaused.OrderByDescending(e => e.Value))
                {
                    var percentage = (effect.Value * 100.0) / config.NumberOfTests;
                    var statusName = TranslateStatusName(effect.Key, provider);
                    this.txtSummary.AppendText($"  {statusName}: {effect.Value} times ({percentage:F1}%)\n");
                }
            }

            if (stats.VolatileStatusEffectsCaused.Count > 0)
            {
                var totalVolatileEffects = stats.VolatileStatusEffectsCaused.Values.Sum();
                this.txtSummary.AppendText("\n═══════════════════════════════════════════════════════════════\n");
                this.txtSummary.AppendText("Volatile Status Effects Caused\n");
                this.txtSummary.AppendText("═══════════════════════════════════════════════════════════════\n\n");
                this.txtSummary.AppendText($"Total: {totalVolatileEffects} effects in {config.NumberOfTests} tests\n");
                foreach (var effect in stats.VolatileStatusEffectsCaused.OrderByDescending(e => e.Value))
                {
                    var percentage = (effect.Value * 100.0) / config.NumberOfTests;
                    var statusName = TranslateVolatileStatusName(effect.Key, provider);
                    this.txtSummary.AppendText($"  {statusName}: {effect.Value} times ({percentage:F1}%)\n");
                }
            }

            // Estadísticas de acciones generadas
            if (stats.ActionsGenerated.Count > 0)
            {
                var totalActions = stats.ActionsGenerated.Values.Sum();
                this.txtSummary.AppendText("\n═══════════════════════════════════════════════════════════════\n");
                this.txtSummary.AppendText("Actions Generated\n");
                this.txtSummary.AppendText("═══════════════════════════════════════════════════════════════\n\n");
                this.txtSummary.AppendText($"Total Actions Generated: {totalActions}\n");
                foreach (var action in stats.ActionsGenerated.OrderByDescending(a => a.Value))
                {
                    var percentage = (action.Value * 100.0) / config.NumberOfTests;
                    this.txtSummary.AppendText($"  {action.Key}: {action.Value} times ({percentage:F1}% of tests)\n");
                }
            }

            // Actualizar tablas
            UpdateDamageTable(stats, config);
            UpdateStatusEffectsTable(stats, config);
            UpdateActionsTable(stats, config);
        }

        private void UpdateDamageTable(MoveRunner.MoveTestStatistics stats, MoveRunner.MoveTestConfig config)
        {
            var provider = LocalizationManager.Instance;
            var dataTable = new DataTable();
            dataTable.Columns.Add("Metric", typeof(string));
            dataTable.Columns.Add("Value", typeof(string));

            if (stats.DamageValues.Count > 0)
            {
                var avgDamage = stats.DamageValues.Average();
                var minDamage = stats.DamageValues.Min();
                var maxDamage = stats.DamageValues.Max();
                var medianDamage = stats.DamageValues.OrderBy(d => d).Skip(stats.DamageValues.Count / 2).First();

                dataTable.Rows.Add("Total Successful Hits", $"{stats.DamageValues.Count} / {config.NumberOfTests}");
                dataTable.Rows.Add("Average Damage", $"{avgDamage:F1} HP");
                dataTable.Rows.Add("Min Damage", $"{minDamage} HP");
                dataTable.Rows.Add("Max Damage", $"{maxDamage} HP");
                dataTable.Rows.Add("Median Damage", $"{medianDamage} HP");

                if (stats.CriticalHits > 0)
                {
                    var critRate = (stats.CriticalHits * 100.0) / stats.DamageValues.Count;
                    dataTable.Rows.Add("Critical Hits", $"{stats.CriticalHits} ({critRate:F1}%)");
                }
            }

            if (stats.Misses > 0)
            {
                var missRate = (stats.Misses * 100.0) / config.NumberOfTests;
                dataTable.Rows.Add("Misses", $"{stats.Misses} ({missRate:F1}%)");
            }

            this.dgvDamage.DataSource = dataTable;
        }

        private void UpdateStatusEffectsTable(MoveRunner.MoveTestStatistics stats, MoveRunner.MoveTestConfig config)
        {
            var provider = LocalizationManager.Instance;
            var dataTable = new DataTable();
            dataTable.Columns.Add("Type", typeof(string));
            dataTable.Columns.Add("Effect", typeof(string));
            dataTable.Columns.Add("Times", typeof(int));
            dataTable.Columns.Add("Percentage", typeof(string));

            foreach (var effect in stats.StatusEffectsCaused.OrderByDescending(e => e.Value))
            {
                var percentage = (effect.Value * 100.0) / config.NumberOfTests;
                var statusName = TranslateStatusName(effect.Key, provider);
                dataTable.Rows.Add("Persistent", statusName, effect.Value, $"{percentage:F1}%");
            }

            foreach (var effect in stats.VolatileStatusEffectsCaused.OrderByDescending(e => e.Value))
            {
                var percentage = (effect.Value * 100.0) / config.NumberOfTests;
                var statusName = TranslateVolatileStatusName(effect.Key, provider);
                dataTable.Rows.Add("Volatile", statusName, effect.Value, $"{percentage:F1}%");
            }

            this.dgvStatusEffects.DataSource = dataTable;
        }

        private void UpdateActionsTable(MoveRunner.MoveTestStatistics stats, MoveRunner.MoveTestConfig config)
        {
            var provider = LocalizationManager.Instance;
            var dataTable = new DataTable();
            dataTable.Columns.Add("Action", typeof(string));
            dataTable.Columns.Add("Times", typeof(int));
            dataTable.Columns.Add("Percentage", typeof(string));

            foreach (var action in stats.ActionsGenerated.OrderByDescending(a => a.Value))
            {
                var percentage = (action.Value * 100.0) / config.NumberOfTests;
                dataTable.Rows.Add(action.Key, action.Value, $"{percentage:F1}%");
            }

            this.dgvActions.DataSource = dataTable;
        }

        /// <summary>
        /// Translates a persistent status name from enum string to localized name.
        /// </summary>
        private string TranslateStatusName(string statusName, ILocalizationProvider provider)
        {
            if (Enum.TryParse<PersistentStatus>(statusName, out var status))
            {
                return status.GetDisplayName(provider);
            }
            return statusName; // Fallback to original if parsing fails
        }

        /// <summary>
        /// Translates a volatile status name from enum string to localized name.
        /// Handles the "Volatile_" prefix that may be present.
        /// </summary>
        private string TranslateVolatileStatusName(string statusName, ILocalizationProvider provider)
        {
            // Remove "Volatile_" prefix if present
            var cleanName = statusName.StartsWith("Volatile_")
                ? statusName.Substring("Volatile_".Length)
                : statusName;

            if (Enum.TryParse<VolatileStatus>(cleanName, out var status))
            {
                return status.GetDisplayName(provider);
            }
            return statusName; // Fallback to original if parsing fails
        }
    }
}

