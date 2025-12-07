using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PokemonUltimate.Content.Catalogs.Field;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Infrastructure.Localization;
using PokemonUltimate.Core.Services;

namespace PokemonUltimate.DataViewer.Tabs
{
    /// <summary>
    /// Tab for displaying Field Effect data.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.7: Data Viewer
    /// **Documentation**: See `docs/features/6-development-tools/6.7-data-viewer/README.md`
    /// </remarks>
    public partial class FieldEffectDataTab : UserControl
    {
        private DataGridView dgvFieldEffect = null!;
        private RichTextBox txtDetails = null!;
        private Label lblTitle = null!;
        private Label lblCount = null!;

        public FieldEffectDataTab()
        {
            InitializeComponent();
            LoadFieldEffectData();
        }

        private void InitializeComponent()
        {
            this.dgvFieldEffect = new DataGridView();
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
                Text = "Field Effect Catalog",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.lblCount = new Label
            {
                Text = $"Total: {FieldEffectCatalog.All.Count}",
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.dgvFieldEffect = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            this.dgvFieldEffect.SelectionChanged += DgvFieldEffect_SelectionChanged;

            gridTableLayout.Controls.Add(this.lblTitle, 0, 0);
            gridTableLayout.Controls.Add(this.lblCount, 0, 1);
            gridTableLayout.Controls.Add(this.dgvFieldEffect, 0, 2);

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

        private void LoadFieldEffectData()
        {
            this.dgvFieldEffect.Columns.Clear();
            this.dgvFieldEffect.Rows.Clear();

            // Add columns
            this.dgvFieldEffect.Columns.Add("Name", "Name");
            this.dgvFieldEffect.Columns.Add("Type", "Type");
            this.dgvFieldEffect.Columns.Add("Duration", "Duration");

            // Set column widths
            this.dgvFieldEffect.Columns["Name"].Width = 200;
            this.dgvFieldEffect.Columns["Type"].Width = 200;
            this.dgvFieldEffect.Columns["Duration"].Width = 100;

            // Load data
            var fieldEffectList = FieldEffectCatalog.All.OrderBy(f => f.Name).ToList();
            var provider = LocalizationService.Instance;

            foreach (var fieldEffect in fieldEffectList)
            {
                var durationStr = fieldEffect.DefaultDuration == 0 ? "Indefinite" : $"{fieldEffect.DefaultDuration} turns";
                var typeStr = fieldEffect.Type.ToString();

                var row = new DataGridViewRow();
                row.CreateCells(this.dgvFieldEffect,
                    fieldEffect.GetLocalizedName(provider),
                    typeStr,
                    durationStr);

                row.Tag = fieldEffect;

                this.dgvFieldEffect.Rows.Add(row);
            }

            // Auto-size remaining columns
            this.dgvFieldEffect.Columns["Duration"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void DgvFieldEffect_SelectionChanged(object? sender, EventArgs e)
        {
            if (this.dgvFieldEffect.SelectedRows.Count == 0)
            {
                this.txtDetails.Text = string.Empty;
                return;
            }

            var selectedRow = this.dgvFieldEffect.SelectedRows[0];
            if (selectedRow.Tag is not FieldEffectData fieldEffect)
                return;

            var provider = LocalizationService.Instance;
            var details = new System.Text.StringBuilder();
            details.AppendLine(fieldEffect.GetLocalizedName(provider));
            details.AppendLine();
            details.AppendLine($"Type: {fieldEffect.Type}");

            if (fieldEffect.DefaultDuration == 0)
                details.AppendLine("Duration: Indefinite");
            else
                details.AppendLine($"Duration: {fieldEffect.DefaultDuration} turns");

            if (fieldEffect.IsToggle)
                details.AppendLine("Toggle: Yes (can be ended early)");

            if (fieldEffect.ReversesSpeedOrder)
                details.AppendLine("Effect: Reverses speed order");

            if (fieldEffect.DisablesItems)
                details.AppendLine("Effect: Disables held items");

            if (fieldEffect.SwapsDefenses)
                details.AppendLine("Effect: Swaps Defense and Sp. Defense");

            var description = fieldEffect.GetLocalizedDescription(provider);
            if (!string.IsNullOrEmpty(description))
            {
                details.AppendLine();
                details.AppendLine($"Description: {description}");
            }

            this.txtDetails.Text = details.ToString();
        }
    }
}

