using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Services;
using PokemonUltimate.Localization.Extensions;
using PokemonUltimate.Localization.Services;

namespace PokemonUltimate.DataViewer.Tabs
{
    /// <summary>
    /// Tab for displaying Hazard data.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.7: Data Viewer
    /// **Documentation**: See `docs/features/6-development-tools/6.7-data-viewer/README.md`
    /// </remarks>
    public partial class HazardDataTab : UserControl
    {
        private DataGridView dgvHazard = null!;
        private RichTextBox txtDetails = null!;
        private Label lblTitle = null!;
        private Label lblCount = null!;

        public HazardDataTab()
        {
            InitializeComponent();
            LoadHazardData();
        }

        private void InitializeComponent()
        {
            this.dgvHazard = new DataGridView();
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
                Text = "Hazard Catalog",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.lblCount = new Label
            {
                Text = $"Total: {HazardCatalog.All.Count}",
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.dgvHazard = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            this.dgvHazard.SelectionChanged += DgvHazard_SelectionChanged;

            gridTableLayout.Controls.Add(this.lblTitle, 0, 0);
            gridTableLayout.Controls.Add(this.lblCount, 0, 1);
            gridTableLayout.Controls.Add(this.dgvHazard, 0, 2);

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

        private void LoadHazardData()
        {
            this.dgvHazard.Columns.Clear();
            this.dgvHazard.Rows.Clear();

            // Add columns
            this.dgvHazard.Columns.Add("Name", "Name");
            this.dgvHazard.Columns.Add("Type", "Type");
            this.dgvHazard.Columns.Add("MaxLayers", "Max Layers");

            // Set column widths
            this.dgvHazard.Columns["Name"].Width = 200;
            this.dgvHazard.Columns["Type"].Width = 200;
            this.dgvHazard.Columns["MaxLayers"].Width = 100;

            // Load data
            var hazardList = HazardCatalog.All.OrderBy(h => h.Name).ToList();
            var provider = LocalizationService.Instance;

            foreach (var hazard in hazardList)
            {
                var typeStr = hazard.Type.ToString();
                var layersStr = hazard.MaxLayers > 1 ? $"{hazard.MaxLayers}" : "1";

                var row = new DataGridViewRow();
                row.CreateCells(this.dgvHazard,
                    hazard.GetDisplayName(provider),
                    typeStr,
                    layersStr);

                row.Tag = hazard;

                this.dgvHazard.Rows.Add(row);
            }

            // Auto-size remaining columns
            this.dgvHazard.Columns["MaxLayers"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void DgvHazard_SelectionChanged(object? sender, EventArgs e)
        {
            if (this.dgvHazard.SelectedRows.Count == 0)
            {
                this.txtDetails.Text = string.Empty;
                return;
            }

            var selectedRow = this.dgvHazard.SelectedRows[0];
            if (selectedRow.Tag is not HazardData hazard)
                return;

            var provider = LocalizationService.Instance;
            var details = new System.Text.StringBuilder();
            details.AppendLine(hazard.GetDisplayName(provider));
            details.AppendLine();
            details.AppendLine($"Type: {hazard.Type}");
            details.AppendLine($"Max Layers: {hazard.MaxLayers}");

            if (hazard.AffectsFlying)
                details.AppendLine("Affects Flying: Yes");
            else
                details.AppendLine("Affects Flying: No (grounded only)");

            var description = hazard.GetDescription(provider);
            if (!string.IsNullOrEmpty(description))
            {
                details.AppendLine();
                details.AppendLine($"Description: {description}");
            }

            this.txtDetails.Text = details.ToString();
        }
    }
}

