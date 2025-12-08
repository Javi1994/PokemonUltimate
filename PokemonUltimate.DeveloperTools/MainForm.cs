using System;
using System.Drawing;
using System.Windows.Forms;
using PokemonUltimate.Content.Providers;
using PokemonUltimate.Localization.Services;
using PokemonUltimate.Core.Services;
using PokemonUltimate.DeveloperTools.Tabs;
using PokemonUltimate.Localization.Providers;

namespace PokemonUltimate.DeveloperTools
{
    public partial class MainForm : Form
    {
        private TabControl mainTabControl = null!;
        private TabPage tabBattle = null!;
        private TabPage tabMove = null!;
        private TabPage tabTypeMatchup = null!;
        private TabPage tabStatCalculator = null!;
        private TabPage tabDamageCalculator = null!;
        private TabPage tabStatusEffect = null!;
        private TabPage tabTurnOrder = null!;
        private BattleDebuggerTab battleDebuggerTab = null!;
        private MoveDebuggerTab moveDebuggerTab = null!;
        private TypeMatchupDebuggerTab typeMatchupDebuggerTab = null!;
        private StatCalculatorDebuggerTab statCalculatorDebuggerTab = null!;
        private DamageCalculatorDebuggerTab damageCalculatorDebuggerTab = null!;
        private StatusEffectDebuggerTab statusEffectDebuggerTab = null!;
        private TurnOrderDebuggerTab turnOrderDebuggerTab = null!;

        public MainForm()
        {
            // Initialize localization (defaults to Spanish)
            LocalizationService.Initialize(new LocalizationProvider(), "es");

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.mainTabControl = new TabControl();
            this.tabBattle = new TabPage();
            this.tabMove = new TabPage();
            this.tabTypeMatchup = new TabPage();
            this.tabStatCalculator = new TabPage();
            this.tabDamageCalculator = new TabPage();
            this.tabStatusEffect = new TabPage();
            this.tabTurnOrder = new TabPage();
            this.battleDebuggerTab = new BattleDebuggerTab();
            this.moveDebuggerTab = new MoveDebuggerTab();
            this.typeMatchupDebuggerTab = new TypeMatchupDebuggerTab();
            this.statCalculatorDebuggerTab = new StatCalculatorDebuggerTab();
            this.damageCalculatorDebuggerTab = new DamageCalculatorDebuggerTab();
            this.statusEffectDebuggerTab = new StatusEffectDebuggerTab();
            this.turnOrderDebuggerTab = new TurnOrderDebuggerTab();

            this.SuspendLayout();

            // Form
            this.Text = "Developer Tools";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 600);
            this.Padding = new Padding(0);

            // Main TabControl
            this.mainTabControl.Dock = DockStyle.Fill;
            this.mainTabControl.Padding = new Point(10, 5);

            // Battle Tab
            this.tabBattle.Text = "Battle";
            this.tabBattle.Padding = new Padding(5);
            this.battleDebuggerTab.Dock = DockStyle.Fill;
            this.tabBattle.Controls.Add(this.battleDebuggerTab);

            // Move Tab
            this.tabMove.Text = "Move";
            this.tabMove.Padding = new Padding(5);
            this.moveDebuggerTab.Dock = DockStyle.Fill;
            this.tabMove.Controls.Add(this.moveDebuggerTab);

            // Type Matchup Tab
            this.tabTypeMatchup.Text = "Type Matchup";
            this.tabTypeMatchup.Padding = new Padding(5);
            this.typeMatchupDebuggerTab.Dock = DockStyle.Fill;
            this.tabTypeMatchup.Controls.Add(this.typeMatchupDebuggerTab);

            // Stat Calculator Tab
            this.tabStatCalculator.Text = "Stat Calculator";
            this.tabStatCalculator.Padding = new Padding(5);
            this.statCalculatorDebuggerTab.Dock = DockStyle.Fill;
            this.tabStatCalculator.Controls.Add(this.statCalculatorDebuggerTab);

            // Damage Calculator Tab
            this.tabDamageCalculator.Text = "Damage Calculator";
            this.tabDamageCalculator.Padding = new Padding(5);
            this.damageCalculatorDebuggerTab.Dock = DockStyle.Fill;
            this.tabDamageCalculator.Controls.Add(this.damageCalculatorDebuggerTab);

            // Status Effect Tab
            this.tabStatusEffect.Text = "Status Effect";
            this.tabStatusEffect.Padding = new Padding(5);
            this.statusEffectDebuggerTab.Dock = DockStyle.Fill;
            this.tabStatusEffect.Controls.Add(this.statusEffectDebuggerTab);

            // Turn Order Tab
            this.tabTurnOrder.Text = "Turn Order";
            this.tabTurnOrder.Padding = new Padding(5);
            this.turnOrderDebuggerTab.Dock = DockStyle.Fill;
            this.tabTurnOrder.Controls.Add(this.turnOrderDebuggerTab);

            // Add tabs to TabControl
            this.mainTabControl.TabPages.AddRange(new TabPage[] {
                this.tabBattle,
                this.tabMove,
                this.tabTypeMatchup,
                this.tabStatCalculator,
                this.tabDamageCalculator,
                this.tabStatusEffect,
                this.tabTurnOrder
            });

            // Add TabControl to Form
            this.Controls.Add(this.mainTabControl);

            this.ResumeLayout(false);
        }
    }
}

