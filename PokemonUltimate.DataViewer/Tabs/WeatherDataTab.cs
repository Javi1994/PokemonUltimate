using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PokemonUltimate.Content.Catalogs.Weather;
using PokemonUltimate.Content.Extensions;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.Core.Localization;

namespace PokemonUltimate.DataViewer.Tabs
{
    /// <summary>
    /// Tab for displaying Weather data.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.7: Data Viewer
    /// **Documentation**: See `docs/features/6-development-tools/6.7-data-viewer/README.md`
    /// </remarks>
    public partial class WeatherDataTab : UserControl
    {
        private DataGridView dgvWeather = null!;
        private RichTextBox txtDetails = null!;
        private Label lblTitle = null!;
        private Label lblCount = null!;

        public WeatherDataTab()
        {
            InitializeComponent();
            LoadWeatherData();
        }

        private void InitializeComponent()
        {
            this.dgvWeather = new DataGridView();
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
                Text = "Weather Catalog",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.lblCount = new Label
            {
                Text = $"Total: {WeatherCatalog.All.Count}",
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.dgvWeather = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            this.dgvWeather.SelectionChanged += DgvWeather_SelectionChanged;

            gridTableLayout.Controls.Add(this.lblTitle, 0, 0);
            gridTableLayout.Controls.Add(this.lblCount, 0, 1);
            gridTableLayout.Controls.Add(this.dgvWeather, 0, 2);

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

        private void LoadWeatherData()
        {
            this.dgvWeather.Columns.Clear();
            this.dgvWeather.Rows.Clear();

            // Add columns
            this.dgvWeather.Columns.Add("Name", "Name");
            this.dgvWeather.Columns.Add("Type", "Type");
            this.dgvWeather.Columns.Add("Duration", "Duration");

            // Set column widths
            this.dgvWeather.Columns["Name"].Width = 200;
            this.dgvWeather.Columns["Type"].Width = 200;
            this.dgvWeather.Columns["Duration"].Width = 100;

            // Load data
            var weatherList = WeatherCatalog.All.OrderBy(w => w.Name).ToList();
            var provider = LocalizationManager.Instance;

            foreach (var weather in weatherList)
            {
                var durationStr = weather.DefaultDuration == 0 ? "Indefinite" : $"{weather.DefaultDuration} turns";
                var typeStr = weather.Weather.ToString();

                var row = new DataGridViewRow();
                row.CreateCells(this.dgvWeather,
                    weather.GetLocalizedName(provider),
                    typeStr,
                    durationStr);

                row.Tag = weather;

                this.dgvWeather.Rows.Add(row);
            }

            // Auto-size remaining columns
            this.dgvWeather.Columns["Duration"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void DgvWeather_SelectionChanged(object? sender, EventArgs e)
        {
            if (this.dgvWeather.SelectedRows.Count == 0)
            {
                this.txtDetails.Text = string.Empty;
                return;
            }

            var selectedRow = this.dgvWeather.SelectedRows[0];
            if (selectedRow.Tag is not WeatherData weather)
                return;

            var provider = LocalizationManager.Instance;
            var details = new System.Text.StringBuilder();
            details.AppendLine(weather.GetLocalizedName(provider));
            details.AppendLine();
            details.AppendLine($"Type: {weather.Weather}");

            if (weather.DefaultDuration == 0)
                details.AppendLine("Duration: Indefinite");
            else
                details.AppendLine($"Duration: {weather.DefaultDuration} turns");

            if (weather.IsPrimal)
                details.AppendLine("Primal: Yes (cannot be overwritten)");

            var description = weather.GetLocalizedDescription(provider);
            if (!string.IsNullOrEmpty(description))
            {
                details.AppendLine();
                details.AppendLine($"Description: {description}");
            }

            this.txtDetails.Text = details.ToString();
        }
    }
}

