using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PokemonUltimate.Content.Catalogs.Status;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Extensions;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.DataViewer.Tabs
{
    /// <summary>
    /// Tab for displaying Status Effect data.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.7: Data Viewer
    /// **Documentation**: See `docs/features/6-development-tools/6.7-data-viewer/README.md`
    /// </remarks>
    public partial class StatusDataTab : UserControl
    {
        private DataGridView dgvStatus = null!;
        private RichTextBox txtDetails = null!;
        private Label lblTitle = null!;
        private Label lblCount = null!;

        public StatusDataTab()
        {
            InitializeComponent();
            LoadStatusData();
        }

        private void InitializeComponent()
        {
            this.dgvStatus = new DataGridView();
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
                Text = "Status Effect Catalog",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.lblCount = new Label
            {
                Text = $"Total: {StatusCatalog.All.Count}",
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.dgvStatus = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            this.dgvStatus.SelectionChanged += DgvStatus_SelectionChanged;

            gridTableLayout.Controls.Add(this.lblTitle, 0, 0);
            gridTableLayout.Controls.Add(this.lblCount, 0, 1);
            gridTableLayout.Controls.Add(this.dgvStatus, 0, 2);

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

        private void LoadStatusData()
        {
            this.dgvStatus.Columns.Clear();
            this.dgvStatus.Rows.Clear();

            // Add columns
            this.dgvStatus.Columns.Add("Name", "Name");
            this.dgvStatus.Columns.Add("Type", "Type");
            this.dgvStatus.Columns.Add("Category", "Category");

            // Set column widths
            this.dgvStatus.Columns["Name"].Width = 200;
            this.dgvStatus.Columns["Type"].Width = 150;
            this.dgvStatus.Columns["Category"].Width = 150;

            // Load data
            var statusList = StatusCatalog.All.OrderBy(s => s.Name).ToList();
            var provider = LocalizationManager.Instance;

            foreach (var status in statusList)
            {
                var categoryStr = status.IsPersistent ? "Persistent" : "Volatile";
                var typeStr = status.IsPersistent
                    ? status.PersistentStatus.GetDisplayName(provider)
                    : status.VolatileStatus.GetDisplayName(provider);

                var row = new DataGridViewRow();
                row.CreateCells(this.dgvStatus,
                    status.GetLocalizedName(provider),
                    typeStr,
                    categoryStr);

                row.Tag = status;

                this.dgvStatus.Rows.Add(row);
            }

            // Auto-size remaining columns
            this.dgvStatus.Columns["Category"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void DgvStatus_SelectionChanged(object? sender, EventArgs e)
        {
            if (this.dgvStatus.SelectedRows.Count == 0)
            {
                this.txtDetails.Text = string.Empty;
                return;
            }

            var selectedRow = this.dgvStatus.SelectedRows[0];
            if (selectedRow.Tag is not StatusEffectData status)
                return;

            var provider = LocalizationManager.Instance;
            var details = new System.Text.StringBuilder();
            details.AppendLine(status.GetLocalizedName(provider));
            details.AppendLine();
            details.AppendLine($"Type: {(status.IsPersistent ? status.PersistentStatus.GetDisplayName(provider) : status.VolatileStatus.GetDisplayName(provider))}");
            details.AppendLine($"Category: {(status.IsPersistent ? "Persistent" : "Volatile")}");

            var description = status.GetLocalizedDescription(provider);
            if (!string.IsNullOrEmpty(description))
            {
                details.AppendLine();
                details.AppendLine($"Description: {description}");
            }

            if (status.IsIndefinite)
                details.AppendLine("Duration: Indefinite");
            else if (status.HasRandomDuration)
                details.AppendLine($"Duration: {status.MinTurns}-{status.MaxTurns} turns");
            else if (status.MinTurns > 0)
                details.AppendLine($"Duration: {status.MinTurns} turns");

            this.txtDetails.Text = details.ToString();
        }
    }
}

