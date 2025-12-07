using System.Drawing;
using System.Windows.Forms;
using PokemonUltimate.Content.Providers;
using PokemonUltimate.Core.Localization;
using PokemonUltimate.DataViewer.Tabs;

namespace PokemonUltimate.DataViewer
{
    /// <summary>
    /// Main form for the Data Viewer application.
    /// </summary>
    /// <remarks>
    /// **Feature**: 6: Development Tools
    /// **Sub-Feature**: 6.7: Data Viewer
    /// **Documentation**: See `docs/features/6-development-tools/6.7-data-viewer/README.md`
    /// </remarks>
    public partial class MainForm : Form
    {
        private TabControl mainTabControl = null!;
        private TabPage tabPokemon = null!;
        private TabPage tabMoves = null!;
        private TabPage tabItems = null!;
        private TabPage tabAbilities = null!;
        private TabPage tabStatus = null!;
        private TabPage tabWeather = null!;
        private TabPage tabTerrain = null!;
        private TabPage tabHazards = null!;
        private TabPage tabSideConditions = null!;
        private TabPage tabFieldEffects = null!;
        private PokemonDataTab pokemonDataTab = null!;
        private MoveDataTab moveDataTab = null!;
        private ItemDataTab itemDataTab = null!;
        private AbilityDataTab abilityDataTab = null!;
        private StatusDataTab statusDataTab = null!;
        private WeatherDataTab weatherDataTab = null!;
        private TerrainDataTab terrainDataTab = null!;
        private HazardDataTab hazardDataTab = null!;
        private SideConditionDataTab sideConditionDataTab = null!;
        private FieldEffectDataTab fieldEffectDataTab = null!;

        public MainForm()
        {
            // Initialize localization (defaults to Spanish)
            LocalizationManager.Initialize(new LocalizationProvider(), "fr");

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.mainTabControl = new TabControl();
            this.tabPokemon = new TabPage();
            this.tabMoves = new TabPage();
            this.tabItems = new TabPage();
            this.tabAbilities = new TabPage();
            this.tabStatus = new TabPage();
            this.tabWeather = new TabPage();
            this.tabTerrain = new TabPage();
            this.tabHazards = new TabPage();
            this.tabSideConditions = new TabPage();
            this.tabFieldEffects = new TabPage();
            this.pokemonDataTab = new PokemonDataTab();
            this.moveDataTab = new MoveDataTab();
            this.itemDataTab = new ItemDataTab();
            this.abilityDataTab = new AbilityDataTab();
            this.statusDataTab = new StatusDataTab();
            this.weatherDataTab = new WeatherDataTab();
            this.terrainDataTab = new TerrainDataTab();
            this.hazardDataTab = new HazardDataTab();
            this.sideConditionDataTab = new SideConditionDataTab();
            this.fieldEffectDataTab = new FieldEffectDataTab();

            this.SuspendLayout();

            // Form
            this.Text = "Pokemon Ultimate - Data Viewer";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 600);
            this.Padding = new Padding(0);

            // Main TabControl
            this.mainTabControl.Dock = DockStyle.Fill;
            this.mainTabControl.Padding = new Point(10, 5);

            // Pokemon Tab
            this.tabPokemon.Text = "Pokemon";
            this.tabPokemon.Padding = new Padding(5);
            this.pokemonDataTab.Dock = DockStyle.Fill;
            this.tabPokemon.Controls.Add(this.pokemonDataTab);

            // Moves Tab
            this.tabMoves.Text = "Moves";
            this.tabMoves.Padding = new Padding(5);
            this.moveDataTab.Dock = DockStyle.Fill;
            this.tabMoves.Controls.Add(this.moveDataTab);

            // Items Tab
            this.tabItems.Text = "Items";
            this.tabItems.Padding = new Padding(5);
            this.itemDataTab.Dock = DockStyle.Fill;
            this.tabItems.Controls.Add(this.itemDataTab);

            // Abilities Tab
            this.tabAbilities.Text = "Abilities";
            this.tabAbilities.Padding = new Padding(5);
            this.abilityDataTab.Dock = DockStyle.Fill;
            this.tabAbilities.Controls.Add(this.abilityDataTab);

            // Status Tab
            this.tabStatus.Text = "Status Effects";
            this.tabStatus.Padding = new Padding(5);
            this.statusDataTab.Dock = DockStyle.Fill;
            this.tabStatus.Controls.Add(this.statusDataTab);

            // Weather Tab
            this.tabWeather.Text = "Weather";
            this.tabWeather.Padding = new Padding(5);
            this.weatherDataTab.Dock = DockStyle.Fill;
            this.tabWeather.Controls.Add(this.weatherDataTab);

            // Terrain Tab
            this.tabTerrain.Text = "Terrain";
            this.tabTerrain.Padding = new Padding(5);
            this.terrainDataTab.Dock = DockStyle.Fill;
            this.tabTerrain.Controls.Add(this.terrainDataTab);

            // Hazards Tab
            this.tabHazards.Text = "Hazards";
            this.tabHazards.Padding = new Padding(5);
            this.hazardDataTab.Dock = DockStyle.Fill;
            this.tabHazards.Controls.Add(this.hazardDataTab);

            // Side Conditions Tab
            this.tabSideConditions.Text = "Side Conditions";
            this.tabSideConditions.Padding = new Padding(5);
            this.sideConditionDataTab.Dock = DockStyle.Fill;
            this.tabSideConditions.Controls.Add(this.sideConditionDataTab);

            // Field Effects Tab
            this.tabFieldEffects.Text = "Field Effects";
            this.tabFieldEffects.Padding = new Padding(5);
            this.fieldEffectDataTab.Dock = DockStyle.Fill;
            this.tabFieldEffects.Controls.Add(this.fieldEffectDataTab);

            // Add tabs to TabControl
            this.mainTabControl.TabPages.AddRange(new TabPage[]
            {
                this.tabPokemon,
                this.tabMoves,
                this.tabItems,
                this.tabAbilities,
                this.tabStatus,
                this.tabWeather,
                this.tabTerrain,
                this.tabHazards,
                this.tabSideConditions,
                this.tabFieldEffects
            });

            // Add TabControl to Form
            this.Controls.Add(this.mainTabControl);

            this.ResumeLayout(false);
        }
    }
}

