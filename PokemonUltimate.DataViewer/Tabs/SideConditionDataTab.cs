using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Core.Blueprints;

namespace PokemonUltimate.DataViewer.Tabs
{
    /// <summary>
    /// Tab for displaying Side Condition data.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.7: Data Viewer
    /// **Documentation**: See `docs/features/6-development-tools/6.7-data-viewer/README.md`
    /// </remarks>
    public partial class SideConditionDataTab : UserControl
    {
        private DataGridView dgvSideCondition = null!;
        private RichTextBox txtDetails = null!;
        private Label lblTitle = null!;
        private Label lblCount = null!;

        public SideConditionDataTab()
        {
            InitializeComponent();
            LoadSideConditionData();
        }

        private void InitializeComponent()
        {
            this.dgvSideCondition = new DataGridView();
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
                Text = "Side Condition Catalog",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.lblCount = new Label
            {
                Text = $"Total: {SideConditionCatalog.All.Count}",
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.dgvSideCondition = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            this.dgvSideCondition.SelectionChanged += DgvSideCondition_SelectionChanged;

            gridTableLayout.Controls.Add(this.lblTitle, 0, 0);
            gridTableLayout.Controls.Add(this.lblCount, 0, 1);
            gridTableLayout.Controls.Add(this.dgvSideCondition, 0, 2);

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

        private void LoadSideConditionData()
        {
            this.dgvSideCondition.Columns.Clear();
            this.dgvSideCondition.Rows.Clear();

            // Add columns
            this.dgvSideCondition.Columns.Add("Name", "Name");
            this.dgvSideCondition.Columns.Add("Type", "Type");
            this.dgvSideCondition.Columns.Add("Duration", "Duration");

            // Set column widths
            this.dgvSideCondition.Columns["Name"].Width = 200;
            this.dgvSideCondition.Columns["Type"].Width = 200;
            this.dgvSideCondition.Columns["Duration"].Width = 100;

            // Load data
            var sideConditionList = SideConditionCatalog.All.OrderBy(s => s.Name).ToList();

            foreach (var sideCondition in sideConditionList)
            {
                var durationStr = sideCondition.IsSingleTurn ? "1 turn" : $"{sideCondition.DefaultDuration} turns";
                var typeStr = sideCondition.Type.ToString();

                var row = new DataGridViewRow();
                row.CreateCells(this.dgvSideCondition,
                    sideCondition.Name,
                    typeStr,
                    durationStr);

                row.Tag = sideCondition;

                this.dgvSideCondition.Rows.Add(row);
            }

            // Auto-size remaining columns
            this.dgvSideCondition.Columns["Duration"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void DgvSideCondition_SelectionChanged(object? sender, EventArgs e)
        {
            if (this.dgvSideCondition.SelectedRows.Count == 0)
            {
                this.txtDetails.Text = string.Empty;
                return;
            }

            var selectedRow = this.dgvSideCondition.SelectedRows[0];
            if (selectedRow.Tag is not SideConditionData sideCondition)
                return;

            var details = new System.Text.StringBuilder();
            details.AppendLine(sideCondition.Name);
            details.AppendLine();
            details.AppendLine($"Type: {sideCondition.Type}");

            if (sideCondition.IsSingleTurn)
                details.AppendLine("Duration: 1 turn");
            else
                details.AppendLine($"Duration: {sideCondition.DefaultDuration} turns");

            if (sideCondition.ReducesDamageFrom.HasValue)
            {
                var reduction = (1 - sideCondition.DamageMultiplierSingles) * 100;
                details.AppendLine($"Reduces {sideCondition.ReducesDamageFrom} damage by {reduction:F0}%");
            }

            if (!string.IsNullOrEmpty(sideCondition.Description))
            {
                details.AppendLine();
                details.AppendLine($"Description: {sideCondition.Description}");
            }

            this.txtDetails.Text = details.ToString();
        }
    }
}

