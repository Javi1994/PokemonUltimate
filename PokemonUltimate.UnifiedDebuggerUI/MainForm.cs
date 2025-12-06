using System;
using System.Drawing;
using System.Windows.Forms;
using PokemonUltimate.UnifiedDebuggerUI.Tabs;

namespace PokemonUltimate.UnifiedDebuggerUI
{
    public partial class MainForm : Form
    {
        private TabControl mainTabControl;
        private TabPage tabBattle;
        private TabPage tabMove;
        private TabPage tabTypeMatchup;
        private BattleDebuggerTab battleDebuggerTab;
        private MoveDebuggerTab moveDebuggerTab;
        private TypeMatchupDebuggerTab typeMatchupDebuggerTab;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.mainTabControl = new TabControl();
            this.tabBattle = new TabPage();
            this.tabMove = new TabPage();
            this.tabTypeMatchup = new TabPage();
            this.battleDebuggerTab = new BattleDebuggerTab();
            this.moveDebuggerTab = new MoveDebuggerTab();
            this.typeMatchupDebuggerTab = new TypeMatchupDebuggerTab();

            this.SuspendLayout();

            // Form
            this.Text = "Pokemon Ultimate - Unified Debugger";
            this.Size = new Size(1400, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 600);
            this.Padding = new Padding(0);

            // Main TabControl
            this.mainTabControl.Dock = DockStyle.Fill;
            this.mainTabControl.Padding = new Point(10, 5);

            // Battle Tab
            this.tabBattle.Text = "Battle Debugger";
            this.tabBattle.Padding = new Padding(5);
            this.battleDebuggerTab.Dock = DockStyle.Fill;
            this.tabBattle.Controls.Add(this.battleDebuggerTab);

            // Move Tab
            this.tabMove.Text = "Move Debugger";
            this.tabMove.Padding = new Padding(5);
            this.moveDebuggerTab.Dock = DockStyle.Fill;
            this.tabMove.Controls.Add(this.moveDebuggerTab);

            // Type Matchup Tab
            this.tabTypeMatchup.Text = "Type Matchup";
            this.tabTypeMatchup.Padding = new Padding(5);
            this.typeMatchupDebuggerTab.Dock = DockStyle.Fill;
            this.tabTypeMatchup.Controls.Add(this.typeMatchupDebuggerTab);

            // Add tabs to TabControl
            this.mainTabControl.TabPages.AddRange(new TabPage[] {
                this.tabBattle,
                this.tabMove,
                this.tabTypeMatchup
            });

            // Add TabControl to Form
            this.Controls.Add(this.mainTabControl);

            this.ResumeLayout(false);
        }
    }
}

