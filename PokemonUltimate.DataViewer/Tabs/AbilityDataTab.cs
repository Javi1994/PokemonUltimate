using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PokemonUltimate.Content.Catalogs.Abilities;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Extensions;

namespace PokemonUltimate.DataViewer.Tabs
{
    /// <summary>
    /// Tab for displaying Ability data.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.7: Data Viewer
    /// **Documentation**: See `docs/features/6-development-tools/6.7-data-viewer/README.md`
    /// </remarks>
    public partial class AbilityDataTab : UserControl
    {
        private DataGridView dgvAbilities = null!;
        private RichTextBox txtDetails = null!;
        private Label lblTitle = null!;
        private Label lblCount = null!;

        public AbilityDataTab()
        {
            InitializeComponent();
            LoadAbilityData();
        }

        private void InitializeComponent()
        {
            this.dgvAbilities = new DataGridView();
            this.txtDetails = new RichTextBox();
            this.lblTitle = new Label();
            this.lblCount = new Label();

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
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

            // Left Panel - Data Grid
            var panelGrid = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            // Use TableLayoutPanel to prevent overlap
            var gridTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(0)
            };
            gridTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            gridTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            gridTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            this.lblTitle = new Label
            {
                Text = "Ability Catalog",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.lblCount = new Label
            {
                Text = $"Total: {AbilityCatalog.All.Count}",
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.dgvAbilities = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            this.dgvAbilities.SelectionChanged += DgvAbilities_SelectionChanged;

            gridTableLayout.Controls.Add(this.lblTitle, 0, 0);
            gridTableLayout.Controls.Add(this.lblCount, 0, 1);
            gridTableLayout.Controls.Add(this.dgvAbilities, 0, 2);

            panelGrid.Controls.Add(gridTableLayout);

            // Right Panel - Details
            var panelDetails = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            var lblDetailsTitle = new Label
            {
                Text = "Details",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(5, 5)
            };

            this.txtDetails = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", 9),
                Location = new Point(5, 35),
                Margin = new Padding(5)
            };

            panelDetails.Controls.Add(lblDetailsTitle);
            panelDetails.Controls.Add(this.txtDetails);

            mainTableLayout.Controls.Add(panelGrid, 0, 0);
            mainTableLayout.Controls.Add(panelDetails, 1, 0);

            this.Controls.Add(mainTableLayout);

            this.ResumeLayout(false);
        }

        private void LoadAbilityData()
        {
            this.dgvAbilities.Columns.Clear();
            this.dgvAbilities.Rows.Clear();

            // Add columns
            this.dgvAbilities.Columns.Add("Name", "Name");
            this.dgvAbilities.Columns.Add("ID", "ID");

            // Set column widths
            this.dgvAbilities.Columns["Name"].Width = 250;
            this.dgvAbilities.Columns["ID"].Width = 300;

            // Load data
            var abilityList = AbilityCatalog.All.OrderBy(a => a.Name).ToList();
            var provider = PokemonUltimate.Core.Localization.LocalizationManager.Instance;

            foreach (var ability in abilityList)
            {
                var row = new DataGridViewRow();
                row.CreateCells(this.dgvAbilities,
                    ability.GetDisplayName(provider),
                    ability.Id);

                // Store Ability object in row tag
                row.Tag = ability;

                this.dgvAbilities.Rows.Add(row);
            }

            // Auto-size remaining columns
            this.dgvAbilities.Columns["ID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void DgvAbilities_SelectionChanged(object? sender, EventArgs e)
        {
            if (this.dgvAbilities.SelectedRows.Count == 0)
            {
                this.txtDetails.Text = string.Empty;
                return;
            }

            var selectedRow = this.dgvAbilities.SelectedRows[0];
            if (selectedRow.Tag is not AbilityData ability)
                return;

            var provider = PokemonUltimate.Core.Localization.LocalizationManager.Instance;
            var details = new System.Text.StringBuilder();
            details.AppendLine(ability.GetDisplayName(provider));
            details.AppendLine();
            details.AppendLine($"ID: {ability.Id}");

            var description = ability.GetDescription(provider);
            if (!string.IsNullOrEmpty(description))
            {
                details.AppendLine();
                details.AppendLine($"Description: {description}");
            }

            this.txtDetails.Text = details.ToString();
        }
    }
}

