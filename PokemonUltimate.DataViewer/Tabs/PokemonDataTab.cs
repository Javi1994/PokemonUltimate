using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Data.Blueprints;
using PokemonUltimate.Core.Utilities.Extensions;
using PokemonUltimate.Core.Infrastructure.Localization;
using PokemonUltimate.Core.Services;

namespace PokemonUltimate.DataViewer.Tabs
{
    /// <summary>
    /// Tab for displaying Pokemon data.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.7: Data Viewer
    /// **Documentation**: See `docs/features/6-development-tools/6.7-data-viewer/README.md`
    /// </remarks>
    public partial class PokemonDataTab : UserControl
    {
        private DataGridView dgvPokemon = null!;
        private RichTextBox txtDetails = null!;
        private Label lblTitle = null!;
        private Label lblCount = null!;

        public PokemonDataTab()
        {
            InitializeComponent();
            LoadPokemonData();
        }

        private void InitializeComponent()
        {
            this.dgvPokemon = new DataGridView();
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
                Text = "Pokemon Catalog",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.lblCount = new Label
            {
                Text = $"Total: {PokemonCatalog.Count}",
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.dgvPokemon = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            this.dgvPokemon.SelectionChanged += DgvPokemon_SelectionChanged;

            gridTableLayout.Controls.Add(this.lblTitle, 0, 0);
            gridTableLayout.Controls.Add(this.lblCount, 0, 1);
            gridTableLayout.Controls.Add(this.dgvPokemon, 0, 2);

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

        private void LoadPokemonData()
        {
            this.dgvPokemon.Columns.Clear();
            this.dgvPokemon.Rows.Clear();

            // Add columns
            this.dgvPokemon.Columns.Add("PokedexNumber", "#");
            this.dgvPokemon.Columns.Add("Name", "Name");
            this.dgvPokemon.Columns.Add("Types", "Types");
            this.dgvPokemon.Columns.Add("HP", "HP");
            this.dgvPokemon.Columns.Add("Attack", "Atk");
            this.dgvPokemon.Columns.Add("Defense", "Def");
            this.dgvPokemon.Columns.Add("SpAttack", "SpA");
            this.dgvPokemon.Columns.Add("SpDefense", "SpD");
            this.dgvPokemon.Columns.Add("Speed", "Spe");

            // Set column widths
            this.dgvPokemon.Columns["PokedexNumber"].Width = 60;
            this.dgvPokemon.Columns["Name"].Width = 150;
            this.dgvPokemon.Columns["Types"].Width = 120;
            this.dgvPokemon.Columns["HP"].Width = 50;
            this.dgvPokemon.Columns["Attack"].Width = 50;
            this.dgvPokemon.Columns["Defense"].Width = 50;
            this.dgvPokemon.Columns["SpAttack"].Width = 50;
            this.dgvPokemon.Columns["SpDefense"].Width = 50;
            this.dgvPokemon.Columns["Speed"].Width = 50;

            // Load data
            var pokemonList = PokemonCatalog.All.OrderBy(p => p.PokedexNumber).ToList();

            foreach (var pokemon in pokemonList)
            {
                var provider = LocalizationService.Instance;
                var typeStr = pokemon.IsDualType
                    ? $"{pokemon.PrimaryType.GetDisplayName(provider)}/{pokemon.SecondaryType.Value.GetDisplayName(provider)}"
                    : pokemon.PrimaryType.GetDisplayName(provider);

                var row = new DataGridViewRow();
                row.CreateCells(this.dgvPokemon,
                    pokemon.PokedexNumber,
                    pokemon.GetDisplayName(provider),
                    typeStr,
                    pokemon.BaseStats.HP,
                    pokemon.BaseStats.Attack,
                    pokemon.BaseStats.Defense,
                    pokemon.BaseStats.SpAttack,
                    pokemon.BaseStats.SpDefense,
                    pokemon.BaseStats.Speed);

                // Store Pokemon object in row tag
                row.Tag = pokemon;

                this.dgvPokemon.Rows.Add(row);
            }

            // Auto-size remaining columns
            this.dgvPokemon.Columns["Types"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void DgvPokemon_SelectionChanged(object? sender, EventArgs e)
        {
            if (this.dgvPokemon.SelectedRows.Count == 0)
            {
                this.txtDetails.Text = string.Empty;
                return;
            }

            var selectedRow = this.dgvPokemon.SelectedRows[0];
            if (selectedRow.Tag is not PokemonSpeciesData pokemon)
                return;

            var provider = LocalizationService.Instance;
            var details = new System.Text.StringBuilder();
            details.AppendLine($"#{pokemon.PokedexNumber:D3} {pokemon.GetDisplayName(provider)}");
            details.AppendLine();
            details.AppendLine($"Types: {GetTypeString(pokemon)}");
            details.AppendLine();
            details.AppendLine("Base Stats:");
            details.AppendLine($"  HP:     {pokemon.BaseStats.HP,3}");
            details.AppendLine($"  Attack: {pokemon.BaseStats.Attack,3}");
            details.AppendLine($"  Defense:{pokemon.BaseStats.Defense,3}");
            details.AppendLine($"  Sp.Atk: {pokemon.BaseStats.SpAttack,3}");
            details.AppendLine($"  Sp.Def: {pokemon.BaseStats.SpDefense,3}");
            details.AppendLine($"  Speed:  {pokemon.BaseStats.Speed,3}");
            details.AppendLine();

            if (pokemon.Ability1 != null)
            {
                details.AppendLine("Abilities:");
                details.AppendLine($"  {pokemon.Ability1.GetDisplayName(provider)}");
                if (pokemon.Ability2 != null)
                    details.AppendLine($"  {pokemon.Ability2.GetDisplayName(provider)}");
                if (pokemon.HiddenAbility != null)
                    details.AppendLine($"  {pokemon.HiddenAbility.GetDisplayName(provider)} (Hidden)");
                details.AppendLine();
            }

            if (pokemon.Learnset.Count > 0)
            {
                details.AppendLine($"Learnset: {pokemon.Learnset.Count} moves");
            }

            this.txtDetails.Text = details.ToString();
        }

        private string GetTypeString(PokemonSpeciesData pokemon)
        {
            var provider = LocalizationService.Instance;
            return pokemon.IsDualType
                ? $"{pokemon.PrimaryType.GetDisplayName(provider)}/{pokemon.SecondaryType.Value.GetDisplayName(provider)}"
                : pokemon.PrimaryType.GetDisplayName(provider);
        }
    }
}

