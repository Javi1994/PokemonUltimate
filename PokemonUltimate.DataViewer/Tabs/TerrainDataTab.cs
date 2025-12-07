using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PokemonUltimate.Content.Catalogs.Terrain;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.DataViewer.Tabs
{
    /// <summary>
    /// Tab for displaying Terrain data.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.7: Data Viewer
    /// **Documentation**: See `docs/features/6-development-tools/6.7-data-viewer/README.md`
    /// </remarks>
    public partial class TerrainDataTab : UserControl
    {
        private DataGridView dgvTerrain = null!;
        private RichTextBox txtDetails = null!;
        private Label lblTitle = null!;
        private Label lblCount = null!;

        public TerrainDataTab()
        {
            InitializeComponent();
            LoadTerrainData();
        }

        private void InitializeComponent()
        {
            this.dgvTerrain = new DataGridView();
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
                Text = "Terrain Catalog",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.lblCount = new Label
            {
                Text = $"Total: {TerrainCatalog.All.Count}",
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.dgvTerrain = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            this.dgvTerrain.SelectionChanged += DgvTerrain_SelectionChanged;

            gridTableLayout.Controls.Add(this.lblTitle, 0, 0);
            gridTableLayout.Controls.Add(this.lblCount, 0, 1);
            gridTableLayout.Controls.Add(this.dgvTerrain, 0, 2);

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

        private void LoadTerrainData()
        {
            this.dgvTerrain.Columns.Clear();
            this.dgvTerrain.Rows.Clear();

            // Add columns
            this.dgvTerrain.Columns.Add("Name", "Name");
            this.dgvTerrain.Columns.Add("Type", "Type");
            this.dgvTerrain.Columns.Add("Duration", "Duration");

            // Set column widths
            this.dgvTerrain.Columns["Name"].Width = 200;
            this.dgvTerrain.Columns["Type"].Width = 200;
            this.dgvTerrain.Columns["Duration"].Width = 100;

            // Load data
            var terrainList = TerrainCatalog.All.OrderBy(t => t.Name).ToList();
            var provider = LocalizationManager.Instance;

            foreach (var terrain in terrainList)
            {
                var durationStr = $"{terrain.DefaultDuration} turns";
                var typeStr = terrain.Terrain.ToString();

                var row = new DataGridViewRow();
                row.CreateCells(this.dgvTerrain,
                    terrain.GetLocalizedName(provider),
                    typeStr,
                    durationStr);

                row.Tag = terrain;

                this.dgvTerrain.Rows.Add(row);
            }

            // Auto-size remaining columns
            this.dgvTerrain.Columns["Duration"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void DgvTerrain_SelectionChanged(object? sender, EventArgs e)
        {
            if (this.dgvTerrain.SelectedRows.Count == 0)
            {
                this.txtDetails.Text = string.Empty;
                return;
            }

            var selectedRow = this.dgvTerrain.SelectedRows[0];
            if (selectedRow.Tag is not TerrainData terrain)
                return;

            var provider = LocalizationManager.Instance;
            var details = new System.Text.StringBuilder();
            details.AppendLine(terrain.GetLocalizedName(provider));
            details.AppendLine();
            details.AppendLine($"Type: {terrain.Terrain}");
            details.AppendLine($"Duration: {terrain.DefaultDuration} turns");

            if (terrain.BoostedType.HasValue)
                details.AppendLine($"Boosts: {terrain.BoostedType} moves ({terrain.BoostMultiplier}x)");

            var description = terrain.GetLocalizedDescription(provider);
            if (!string.IsNullOrEmpty(description))
            {
                details.AppendLine();
                details.AppendLine($"Description: {description}");
            }

            this.txtDetails.Text = details.ToString();
        }
    }
}

