using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PokemonUltimate.Content.Catalogs.Moves;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Enums;

namespace PokemonUltimate.DataViewer.Tabs
{
    /// <summary>
    /// Tab for displaying Move data.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.7: Data Viewer
    /// **Documentation**: See `docs/features/6-development-tools/6.7-data-viewer/README.md`
    /// </remarks>
    public partial class MoveDataTab : UserControl
    {
        private DataGridView dgvMoves = null!;
        private RichTextBox txtDetails = null!;
        private Label lblTitle = null!;
        private Label lblCount = null!;

        public MoveDataTab()
        {
            InitializeComponent();
            LoadMoveData();
        }

        private void InitializeComponent()
        {
            this.dgvMoves = new DataGridView();
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
                Text = "Move Catalog",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.lblCount = new Label
            {
                Text = $"Total: {MoveCatalog.Count}",
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.dgvMoves = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            this.dgvMoves.SelectionChanged += DgvMoves_SelectionChanged;

            gridTableLayout.Controls.Add(this.lblTitle, 0, 0);
            gridTableLayout.Controls.Add(this.lblCount, 0, 1);
            gridTableLayout.Controls.Add(this.dgvMoves, 0, 2);

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

        private void LoadMoveData()
        {
            this.dgvMoves.Columns.Clear();
            this.dgvMoves.Rows.Clear();

            // Add columns
            this.dgvMoves.Columns.Add("Name", "Name");
            this.dgvMoves.Columns.Add("Type", "Type");
            this.dgvMoves.Columns.Add("Category", "Category");
            this.dgvMoves.Columns.Add("Power", "Power");
            this.dgvMoves.Columns.Add("Accuracy", "Accuracy");
            this.dgvMoves.Columns.Add("PP", "PP");
            this.dgvMoves.Columns.Add("Priority", "Priority");

            // Set column widths
            this.dgvMoves.Columns["Name"].Width = 150;
            this.dgvMoves.Columns["Type"].Width = 100;
            this.dgvMoves.Columns["Category"].Width = 80;
            this.dgvMoves.Columns["Power"].Width = 60;
            this.dgvMoves.Columns["Accuracy"].Width = 80;
            this.dgvMoves.Columns["PP"].Width = 50;
            this.dgvMoves.Columns["Priority"].Width = 60;

            // Load data
            var moveList = MoveCatalog.All.OrderBy(m => m.Name).ToList();

            foreach (var move in moveList)
            {
                var categoryStr = move.Category == MoveCategory.Status ? "Status" :
                                 move.Category == MoveCategory.Physical ? "Physical" : "Special";
                var powerStr = move.Power > 0 ? move.Power.ToString() : "-";
                var accuracyStr = move.NeverMisses ? "---" : $"{move.Accuracy}%";

                var row = new DataGridViewRow();
                row.CreateCells(this.dgvMoves,
                    move.Name,
                    move.Type.ToString(),
                    categoryStr,
                    powerStr,
                    accuracyStr,
                    move.MaxPP,
                    move.Priority);

                // Store Move object in row tag
                row.Tag = move;

                this.dgvMoves.Rows.Add(row);
            }

            // Auto-size remaining columns
            this.dgvMoves.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void DgvMoves_SelectionChanged(object? sender, EventArgs e)
        {
            if (this.dgvMoves.SelectedRows.Count == 0)
            {
                this.txtDetails.Text = string.Empty;
                return;
            }

            var selectedRow = this.dgvMoves.SelectedRows[0];
            if (selectedRow.Tag is not MoveData move)
                return;

            var details = new System.Text.StringBuilder();
            details.AppendLine(move.Name);
            details.AppendLine();
            details.AppendLine($"Type: {move.Type}");
            details.AppendLine($"Category: {move.Category}");
            
            if (move.Power > 0)
                details.AppendLine($"Power: {move.Power}");
            else
                details.AppendLine("Power: - (Status move)");

            if (move.NeverMisses)
                details.AppendLine("Accuracy: --- (Never misses)");
            else
                details.AppendLine($"Accuracy: {move.Accuracy}%");

            details.AppendLine($"PP: {move.MaxPP}");
            details.AppendLine($"Priority: {move.Priority}");
            details.AppendLine($"Target: {move.TargetScope}");

            if (!string.IsNullOrEmpty(move.Description))
            {
                details.AppendLine();
                details.AppendLine($"Description: {move.Description}");
            }

            if (move.Effects.Count > 0)
            {
                details.AppendLine();
                details.AppendLine($"Effects: {move.Effects.Count}");
            }

            this.txtDetails.Text = details.ToString();
        }
    }
}

