using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PokemonUltimate.Core.Enums;
using PokemonUltimate.Core.Extensions;
using PokemonUltimate.Core.Factories;
using PokemonUltimate.Core.Localization;
using PokemonUltimate.DeveloperTools.Localization;

namespace PokemonUltimate.DeveloperTools.Tabs
{
    public partial class TypeMatchupDebuggerTab : UserControl
    {
        private ComboBox comboAttackingType = null!;
        private ComboBox comboDefenderPrimaryType = null!;
        private ComboBox comboDefenderSecondaryType = null!;
        private Button btnCalculate = null!;
        private Label lblResult = null!;
        private RichTextBox txtBreakdown = null!;
        private DataGridView dgvTypeChart = null!;
        private Label lblTitle = null!;

        public TypeMatchupDebuggerTab()
        {
            InitializeComponent();
            LoadTypeLists();
        }

        private void InitializeComponent()
        {
            this.comboAttackingType = new ComboBox();
            this.comboDefenderPrimaryType = new ComboBox();
            this.comboDefenderSecondaryType = new ComboBox();
            this.btnCalculate = new Button();
            this.lblResult = new Label();
            this.txtBreakdown = new RichTextBox();
            this.dgvTypeChart = new DataGridView();
            this.lblTitle = new Label();

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
            this.lblTitle = new Label
            {
                Text = "Configuration",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(leftMargin, yPos)
            };
            yPos += 40;

            var lblAttackingType = new Label
            {
                Text = "Attacking Type",
                AutoSize = true,
                Location = new Point(leftMargin, yPos)
            };
            yPos += 25;

            this.comboAttackingType = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = controlWidth,
                Location = new Point(leftMargin, yPos)
            };
            yPos += spacing;

            var lblDefenderPrimaryType = new Label
            {
                Text = "Defender Primary Type",
                AutoSize = true,
                Location = new Point(leftMargin, yPos)
            };
            yPos += 25;

            this.comboDefenderPrimaryType = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = controlWidth,
                Location = new Point(leftMargin, yPos)
            };
            yPos += spacing;

            var lblDefenderSecondaryType = new Label
            {
                Text = "Defender Secondary Type",
                AutoSize = true,
                Location = new Point(leftMargin, yPos)
            };
            yPos += 25;

            this.comboDefenderSecondaryType = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = controlWidth,
                Location = new Point(leftMargin, yPos)
            };
            yPos += spacing;

            this.btnCalculate = new Button
            {
                Text = "Calculate Effectiveness",
                Width = controlWidth,
                Height = 40,
                Location = new Point(leftMargin, yPos),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            this.btnCalculate.FlatAppearance.BorderSize = 0;
            this.btnCalculate.Click += BtnCalculate_Click;
            yPos += 60;

            this.lblResult = new Label
            {
                Text = "Select types and calculate",
                AutoSize = false,
                Width = controlWidth,
                Height = 80,
                Location = new Point(leftMargin, yPos),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                TextAlign = ContentAlignment.TopLeft
            };
            yPos += 100;

            this.txtBreakdown = new RichTextBox
            {
                ReadOnly = true,
                Width = controlWidth,
                Height = 150,
                Location = new Point(leftMargin, yPos),
                Font = new Font("Consolas", 9),
                BorderStyle = BorderStyle.FixedSingle,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            panelConfig.Controls.AddRange(new Control[]
            {
                this.lblTitle,
                lblAttackingType,
                this.comboAttackingType,
                lblDefenderPrimaryType,
                this.comboDefenderPrimaryType,
                lblDefenderSecondaryType,
                this.comboDefenderSecondaryType,
                this.btnCalculate,
                this.lblResult,
                this.txtBreakdown
            });

            // Panel derecho - Resultados
            var panelResults = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                Margin = new Padding(5)
            };

            var lblResultsTitle = new Label
            {
                Text = "Complete Type Chart",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(5, 5)
            };

            this.dgvTypeChart = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                Location = new Point(5, 35),
                Margin = new Padding(5)
            };

            panelResults.Controls.Add(lblResultsTitle);
            panelResults.Controls.Add(this.dgvTypeChart);

            mainTableLayout.Controls.Add(panelConfig, 0, 0);
            mainTableLayout.Controls.Add(panelResults, 1, 0);

            this.Controls.Add(mainTableLayout);
            this.ResumeLayout(false);
        }

        private void LoadTypeLists()
        {
            var allTypes = Enum.GetValues<PokemonType>().ToList();
            var provider = LocalizationManager.Instance;

            // Agregar tipos traducidos a los ComboBox
            foreach (var type in allTypes)
            {
                comboAttackingType.Items.Add(new TypeDisplayItem(type, provider));
                comboDefenderPrimaryType.Items.Add(new TypeDisplayItem(type, provider));
                comboDefenderSecondaryType.Items.Add(new TypeDisplayItem(type, provider));
            }

            comboDefenderSecondaryType.Items.Insert(0, "None");

            // Defaults
            SetSelectedType(comboAttackingType, PokemonType.Fire);
            SetSelectedType(comboDefenderPrimaryType, PokemonType.Grass);
            SetSelectedType(comboDefenderSecondaryType, PokemonType.Poison);
        }

        private void BtnCalculate_Click(object? sender, EventArgs e)
        {
            if (comboAttackingType.SelectedItem == null || comboDefenderPrimaryType.SelectedItem == null)
            {
                MessageBox.Show("Please select attacking and defender types.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var attackingType = GetTypeFromItem(comboAttackingType.SelectedItem);
            var defenderPrimaryType = GetTypeFromItem(comboDefenderPrimaryType.SelectedItem);
            PokemonType? defenderSecondaryType = null;

            var provider = LocalizationManager.Instance;
            if (comboDefenderSecondaryType.SelectedItem != null &&
                comboDefenderSecondaryType.SelectedItem.ToString() != "None")
            {
                defenderSecondaryType = GetTypeFromItem(comboDefenderSecondaryType.SelectedItem);
            }

            CalculateEffectiveness(attackingType, defenderPrimaryType, defenderSecondaryType);
        }

        private void CalculateEffectiveness(PokemonType attackingType, PokemonType defenderPrimaryType, PokemonType? defenderSecondaryType)
        {
            // Calcular efectividad
            var effectiveness = TypeEffectiveness.GetEffectiveness(
                attackingType,
                defenderPrimaryType,
                defenderSecondaryType);

            var provider = LocalizationManager.Instance;
            var effectivenessText = effectiveness switch
            {
                0.0f => "Immune (0x)",
                0.25f => "Not Very Effective (0.25x)",
                0.5f => "Not Very Effective (0.5x)",
                1.0f => "Normal (1x)",
                2.0f => "Super Effective (2x)",
                4.0f => "Super Effective (4x)",
                _ => $"{effectiveness:F2}x"
            };

            // Mostrar resultado con tipos traducidos
            var attackingTypeName = attackingType.GetDisplayName(provider);
            var defenderPrimaryTypeName = defenderPrimaryType.GetDisplayName(provider);
            var defenderTypeText = defenderPrimaryTypeName;
            if (defenderSecondaryType.HasValue)
            {
                defenderTypeText += $"/{defenderSecondaryType.Value.GetDisplayName(provider)}";
            }

            lblResult.Text = $"Attacking: {attackingTypeName}\n" +
                           $"Defender: {defenderTypeText}\n\n" +
                           $"Effectiveness: {effectivenessText}";

            lblResult.ForeColor = GetEffectivenessColor(effectiveness);

            // Mostrar desglose si es tipo dual
            txtBreakdown.Clear();
            if (defenderSecondaryType.HasValue)
            {
                var primaryEff = TypeEffectiveness.GetEffectiveness(attackingType, defenderPrimaryType);
                var secondaryEff = TypeEffectiveness.GetEffectiveness(attackingType, defenderSecondaryType.Value);

                txtBreakdown.AppendText("Breakdown:\n");
                txtBreakdown.AppendText($"{attackingTypeName} vs {defenderPrimaryTypeName}: {primaryEff:F2}x\n");
                txtBreakdown.AppendText($"{attackingTypeName} vs {defenderSecondaryType.Value.GetDisplayName(provider)}: {secondaryEff:F2}x\n");
                txtBreakdown.AppendText($"Total: {primaryEff:F2}x × {secondaryEff:F2}x = {effectiveness:F2}x");
            }

            // Mostrar tabla completa
            LoadTypeChart(attackingType);
        }

        /// <summary>
        /// Helper class to display types with translated names in ComboBox.
        /// </summary>
        private class TypeDisplayItem
        {
            public PokemonType Type { get; }
            private readonly ILocalizationProvider _provider;

            public TypeDisplayItem(PokemonType type, ILocalizationProvider provider)
            {
                Type = type;
                _provider = provider;
            }

            public override string ToString()
            {
                return Type.GetDisplayName(_provider);
            }
        }

        /// <summary>
        /// Gets PokemonType from ComboBox item (handles both TypeDisplayItem and direct PokemonType).
        /// </summary>
        private PokemonType GetTypeFromItem(object item)
        {
            if (item is TypeDisplayItem displayItem)
                return displayItem.Type;
            if (item is PokemonType type)
                return type;
            throw new ArgumentException("Invalid item type", nameof(item));
        }

        /// <summary>
        /// Sets the selected type in a ComboBox by finding the matching TypeDisplayItem.
        /// </summary>
        private void SetSelectedType(ComboBox comboBox, PokemonType type)
        {
            foreach (var item in comboBox.Items)
            {
                // Skip string items like "(None)"
                if (item is string)
                    continue;

                try
                {
                    if (GetTypeFromItem(item) == type)
                    {
                        comboBox.SelectedItem = item;
                        return;
                    }
                }
                catch (ArgumentException)
                {
                    // Skip invalid items
                    continue;
                }
            }
        }

        private void LoadTypeChart(PokemonType attackingType)
        {
            var provider = LocalizationManager.Instance;
            dgvTypeChart.Columns.Clear();
            dgvTypeChart.Rows.Clear();

            dgvTypeChart.Columns.Add("DefendingType", "Defending Type");
            dgvTypeChart.Columns.Add("Effectiveness", "Effectiveness");
            dgvTypeChart.Columns.Add("Description", "Description");

            var allTypes = Enum.GetValues<PokemonType>();
            foreach (var defendingType in allTypes)
            {
                var eff = TypeEffectiveness.GetEffectiveness(attackingType, defendingType);
                var desc = GetEffectivenessDescription(eff);
                var color = GetEffectivenessColor(eff);

                var row = new DataGridViewRow();
                row.CreateCells(dgvTypeChart, defendingType.GetDisplayName(provider), $"{eff:F2}x", desc);
                row.DefaultCellStyle.ForeColor = color;
                dgvTypeChart.Rows.Add(row);
            }

            // Ajustar ancho de columnas
            dgvTypeChart.Columns[0].Width = 150;
            dgvTypeChart.Columns[1].Width = 120;
            dgvTypeChart.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private string GetEffectivenessDescription(float effectiveness)
        {
            return effectiveness switch
            {
                0.0f => "Immune (0x)",
                0.25f => "Not Very Effective (0.25x)",
                0.5f => "Not Very Effective (0.5x)",
                1.0f => "Normal (1x)",
                2.0f => "Super Effective (2x)",
                4.0f => "Super Effective (4x)",
                _ => effectiveness > 1.0f
                    ? "Super Effective (2x)"
                    : "Not Very Effective (0.5x)"
            };
        }

        private Color GetEffectivenessColor(float effectiveness)
        {
            return effectiveness switch
            {
                0.0f => Color.Red,
                > 0.0f and < 1.0f => Color.Orange,
                1.0f => Color.Black,
                > 1.0f => Color.Green,
                _ => Color.Black
            };
        }
    }
}

