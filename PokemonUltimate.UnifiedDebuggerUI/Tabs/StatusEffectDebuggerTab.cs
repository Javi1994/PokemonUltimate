using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.UnifiedDebuggerUI.Runners;

namespace PokemonUltimate.UnifiedDebuggerUI.Tabs
{
    /// <summary>
    /// Debugger tab for testing status effects and their interactions.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.3: Status Effect Debugger
    /// **Documentation**: See `docs/features/6-development-tools/6.3-status-effect-debugger/README.md`
    /// </remarks>
    public partial class StatusEffectDebuggerTab : UserControl
    {
        private ComboBox comboPokemon;
        private NumericUpDown numericLevel;
        private ComboBox comboPersistentStatus;
        private CheckedListBox chkListVolatileStatus;
        private Button btnApply;
        private TabControl tabResults;
        private TabPage tabSummary;
        private TabPage tabStatModifications;
        private TabPage tabDamagePerTurn;
        private TabPage tabInteractions;
        private RichTextBox txtSummary;
        private DataGridView dgvStatModifications;
        private DataGridView dgvDamagePerTurn;
        private DataGridView dgvInteractions;
        private StatusEffectRunner _runner;

        public StatusEffectDebuggerTab()
        {
            InitializeComponent();
            LoadData();
            _runner = new StatusEffectRunner();
        }

        private void InitializeComponent()
        {
            this.comboPokemon = new ComboBox();
            this.numericLevel = new NumericUpDown();
            this.comboPersistentStatus = new ComboBox();
            this.chkListVolatileStatus = new CheckedListBox();
            this.btnApply = new Button();
            this.tabResults = new TabControl();
            this.tabSummary = new TabPage();
            this.tabStatModifications = new TabPage();
            this.tabDamagePerTurn = new TabPage();
            this.tabInteractions = new TabPage();
            this.txtSummary = new RichTextBox();
            this.dgvStatModifications = new DataGridView();
            this.dgvDamagePerTurn = new DataGridView();
            this.dgvInteractions = new DataGridView();

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

            // Pokemon
            var lblPokemon = new Label { Text = "Pokemon:", Location = new Point(leftMargin, yPos), AutoSize = true };
            yPos += 25;
            this.comboPokemon.Location = new Point(leftMargin, yPos);
            this.comboPokemon.Width = controlWidth;
            this.comboPokemon.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboPokemon.Height = 25;
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

            // Persistent Status
            var lblPersistentStatus = new Label { Text = "Persistent Status:", Location = new Point(leftMargin, yPos), AutoSize = true };
            yPos += 25;
            this.comboPersistentStatus.Location = new Point(leftMargin, yPos);
            this.comboPersistentStatus.Width = controlWidth;
            this.comboPersistentStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboPersistentStatus.Height = 25;
            yPos += spacing;

            // Volatile Status
            var lblVolatileStatus = new Label { Text = "Volatile Status:", Location = new Point(leftMargin, yPos), AutoSize = true };
            yPos += 25;
            this.chkListVolatileStatus.Location = new Point(leftMargin, yPos);
            this.chkListVolatileStatus.Width = controlWidth;
            this.chkListVolatileStatus.Height = 150;
            this.chkListVolatileStatus.CheckOnClick = true;
            yPos += 155;

            // Apply button
            this.btnApply = new Button
            {
                Text = "Apply Status",
                Location = new Point(leftMargin, yPos),
                Width = controlWidth,
                Height = 50,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            this.btnApply.FlatAppearance.BorderSize = 0;
            this.btnApply.Click += BtnApply_Click;

            panelConfig.Controls.AddRange(new Control[]
            {
                lblTitle, lblPokemon, comboPokemon,
                lblLevel, numericLevel,
                lblPersistentStatus, comboPersistentStatus,
                lblVolatileStatus, chkListVolatileStatus,
                btnApply
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
            this.txtSummary.Text = "Configure settings and click 'Apply Status' to see results here.";
            this.tabSummary.Controls.Add(this.txtSummary);

            // Tab Stat Modifications
            this.tabStatModifications.Text = "Stat Modifications";
            this.tabStatModifications.Padding = new Padding(10);
            this.dgvStatModifications.Dock = DockStyle.Fill;
            this.dgvStatModifications.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStatModifications.ReadOnly = true;
            this.dgvStatModifications.AllowUserToAddRows = false;
            this.tabStatModifications.Controls.Add(this.dgvStatModifications);

            // Tab Damage Per Turn
            this.tabDamagePerTurn.Text = "Damage Per Turn";
            this.tabDamagePerTurn.Padding = new Padding(10);
            this.dgvDamagePerTurn.Dock = DockStyle.Fill;
            this.dgvDamagePerTurn.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDamagePerTurn.ReadOnly = true;
            this.dgvDamagePerTurn.AllowUserToAddRows = false;
            this.tabDamagePerTurn.Controls.Add(this.dgvDamagePerTurn);

            // Tab Interactions
            this.tabInteractions.Text = "Interactions";
            this.tabInteractions.Padding = new Padding(10);
            this.dgvInteractions.Dock = DockStyle.Fill;
            this.dgvInteractions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInteractions.ReadOnly = true;
            this.dgvInteractions.AllowUserToAddRows = false;
            this.tabInteractions.Controls.Add(this.dgvInteractions);

            this.tabResults.TabPages.AddRange(new TabPage[]
            {
                this.tabSummary,
                this.tabStatModifications,
                this.tabDamagePerTurn,
                this.tabInteractions
            });

            mainTableLayout.Controls.Add(panelConfig, 0, 0);
            mainTableLayout.Controls.Add(this.tabResults, 1, 0);

            this.Controls.Add(mainTableLayout);

            this.ResumeLayout(false);
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

            // Load Persistent Status
            this.comboPersistentStatus.Items.Add("None");
            foreach (PersistentStatus status in Enum.GetValues<PersistentStatus>())
            {
                if (status != PersistentStatus.None)
                {
                    this.comboPersistentStatus.Items.Add(status);
                }
            }
            this.comboPersistentStatus.SelectedIndex = 0;

            // Load Volatile Status
            foreach (VolatileStatus status in Enum.GetValues<VolatileStatus>())
            {
                if (status != VolatileStatus.None)
                {
                    this.chkListVolatileStatus.Items.Add(status, false);
                }
            }
        }

        private void BtnApply_Click(object? sender, EventArgs e)
        {
            if (comboPokemon.SelectedItem == null)
            {
                MessageBox.Show("Please select a Pokemon.", "Missing Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var pokemonName = comboPokemon.SelectedItem.ToString();
            var pokemon = PokemonCatalog.All.FirstOrDefault(p => p.Name == pokemonName);

            if (pokemon == null)
            {
                MessageBox.Show("Selected Pokemon not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get persistent status
            PersistentStatus persistentStatus = PersistentStatus.None;
            if (comboPersistentStatus.SelectedItem != null)
            {
                if (comboPersistentStatus.SelectedItem is PersistentStatus status)
                {
                    persistentStatus = status;
                }
                else if (comboPersistentStatus.SelectedItem.ToString() != "None")
                {
                    if (Enum.TryParse<PersistentStatus>(comboPersistentStatus.SelectedItem.ToString(), out var parsedStatus))
                    {
                        persistentStatus = parsedStatus;
                    }
                }
            }

            // Get volatile status
            VolatileStatus volatileStatus = VolatileStatus.None;
            foreach (int index in chkListVolatileStatus.CheckedIndices)
            {
                if (chkListVolatileStatus.Items[index] is VolatileStatus status)
                {
                    volatileStatus |= status;
                }
            }

            var config = new StatusEffectRunner.StatusEffectConfig
            {
                PokemonSpecies = pokemon,
                Level = (int)numericLevel.Value,
                PersistentStatus = persistentStatus,
                VolatileStatus = volatileStatus
            };

            var result = _runner.ApplyStatus(config);
            DisplayResults(result);
        }

        private void DisplayResults(StatusEffectRunner.StatusEffectResult result)
        {
            // Summary tab
            var summary = new StringBuilder();
            summary.AppendLine("=== STATUS EFFECT ANALYSIS ===");
            summary.AppendLine();
            summary.AppendLine($"Pokemon: {result.Pokemon.Species.Name}");
            summary.AppendLine($"Level: {result.Pokemon.Level}");
            summary.AppendLine($"Type: {result.Pokemon.Species.PrimaryType}" + (result.Pokemon.Species.SecondaryType.HasValue ? $" / {result.Pokemon.Species.SecondaryType.Value}" : ""));
            summary.AppendLine();
            summary.AppendLine("=== CURRENT STATUS ===");
            summary.AppendLine($"Persistent Status: {result.CurrentPersistentStatus}");
            if (result.PersistentStatusData != null)
            {
                summary.AppendLine($"  Description: {result.PersistentStatusData.Description}");
            }
            summary.AppendLine($"Volatile Status: {result.CurrentVolatileStatus}");
            if (result.VolatileStatusDataList.Count > 0)
            {
                foreach (var volatileData in result.VolatileStatusDataList)
                {
                    summary.AppendLine($"  - {volatileData.Name}: {volatileData.Description}");
                }
            }
            summary.AppendLine();

            // Stat modifications summary
            if (result.StatModifications.Count > 0)
            {
                summary.AppendLine("=== STAT MODIFICATIONS ===");
                foreach (var mod in result.StatModifications)
                {
                    summary.AppendLine($"{mod.Stat}: {mod.BaseValue} → {mod.ModifiedValue} ({mod.Multiplier:F2}x) - {mod.Description}");
                }
                summary.AppendLine();
            }

            // Damage per turn summary
            if (result.DamagePerTurnList.Count > 0)
            {
                summary.AppendLine("=== DAMAGE PER TURN ===");
                foreach (var damage in result.DamagePerTurnList)
                {
                    summary.AppendLine($"{damage.StatusName}: {damage.DamageAmount} HP ({damage.DamageFraction * 100:F1}% of max HP)");
                    if (damage.Escalates)
                    {
                        summary.AppendLine("  (Damage escalates each turn)");
                    }
                }
                summary.AppendLine();
            }

            this.txtSummary.Text = summary.ToString();

            // Stat Modifications tab
            dgvStatModifications.Columns.Clear();
            dgvStatModifications.Rows.Clear();

            dgvStatModifications.Columns.Add("Stat", "Stat");
            dgvStatModifications.Columns.Add("Base Value", "Base Value");
            dgvStatModifications.Columns.Add("Modified Value", "Modified Value");
            dgvStatModifications.Columns.Add("Multiplier", "Multiplier");
            dgvStatModifications.Columns.Add("Description", "Description");

            foreach (var mod in result.StatModifications)
            {
                var row = new DataGridViewRow();
                row.CreateCells(dgvStatModifications,
                    mod.Stat.ToString(),
                    mod.BaseValue,
                    mod.ModifiedValue,
                    mod.Multiplier.ToString("F2"),
                    mod.Description);

                // Highlight if stat is reduced
                if (mod.Multiplier < 1.0f)
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                else if (mod.Multiplier > 1.0f)
                    row.DefaultCellStyle.BackColor = Color.LightGreen;

                dgvStatModifications.Rows.Add(row);
            }

            // Adjust column widths
            if (dgvStatModifications.Columns.Count > 0)
            {
                dgvStatModifications.Columns[0].Width = 100;
                dgvStatModifications.Columns[1].Width = 100;
                dgvStatModifications.Columns[2].Width = 120;
                dgvStatModifications.Columns[3].Width = 100;
                dgvStatModifications.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            // Damage Per Turn tab
            dgvDamagePerTurn.Columns.Clear();
            dgvDamagePerTurn.Rows.Clear();

            dgvDamagePerTurn.Columns.Add("Status", "Status");
            dgvDamagePerTurn.Columns.Add("Damage Fraction", "Damage Fraction");
            dgvDamagePerTurn.Columns.Add("Damage Amount", "Damage Amount");
            dgvDamagePerTurn.Columns.Add("Max HP", "Max HP");
            dgvDamagePerTurn.Columns.Add("Escalates", "Escalates");
            dgvDamagePerTurn.Columns.Add("Description", "Description");

            foreach (var damage in result.DamagePerTurnList)
            {
                var row = new DataGridViewRow();
                row.CreateCells(dgvDamagePerTurn,
                    damage.StatusName,
                    $"{damage.DamageFraction * 100:F2}%",
                    damage.DamageAmount,
                    damage.MaxHP,
                    damage.Escalates ? "Yes" : "No",
                    damage.Description);

                dgvDamagePerTurn.Rows.Add(row);
            }

            // Adjust column widths
            if (dgvDamagePerTurn.Columns.Count > 0)
            {
                dgvDamagePerTurn.Columns[0].Width = 120;
                dgvDamagePerTurn.Columns[1].Width = 100;
                dgvDamagePerTurn.Columns[2].Width = 100;
                dgvDamagePerTurn.Columns[3].Width = 80;
                dgvDamagePerTurn.Columns[4].Width = 80;
                dgvDamagePerTurn.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            // Interactions tab
            dgvInteractions.Columns.Clear();
            dgvInteractions.Rows.Clear();

            dgvInteractions.Columns.Add("Status", "Status");
            dgvInteractions.Columns.Add("Can Apply", "Can Apply");
            dgvInteractions.Columns.Add("Reason", "Reason");

            foreach (var interaction in result.StatusInteractions)
            {
                var row = new DataGridViewRow();
                row.CreateCells(dgvInteractions,
                    interaction.StatusName,
                    interaction.CanApplyWithCurrentStatus ? "Yes" : "No",
                    interaction.IsImmune ? interaction.ImmuneReason : interaction.Reason);

                // Highlight based on status
                if (interaction.IsImmune)
                    row.DefaultCellStyle.BackColor = Color.LightYellow;
                else if (!interaction.CanApplyWithCurrentStatus)
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                else
                    row.DefaultCellStyle.BackColor = Color.LightGreen;

                dgvInteractions.Rows.Add(row);
            }

            // Adjust column widths
            if (dgvInteractions.Columns.Count > 0)
            {
                dgvInteractions.Columns[0].Width = 150;
                dgvInteractions.Columns[1].Width = 80;
                dgvInteractions.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }
    }
}

