using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokemonUltimate.Content.Catalogs.Pokemon;
using PokemonUltimate.Core.Blueprints;
using PokemonUltimate.DevelopTools.Runners;

namespace PokemonUltimate.DevelopTools.Tabs
{
    public partial class BattleDebuggerTab : UserControl
    {
        private ComboBox comboPlayerPokemon = null!;
        private ComboBox comboEnemyPokemon = null!;
        private NumericUpDown numericLevel = null!;
        private NumericUpDown numericBattles = null!;
        private CheckBox checkDetailedOutput = null!;
        private Button btnRun = null!;
        private TabControl tabResults = null!;
        private TabPage tabSummary = null!;
        private TabPage tabMoveUsage = null!;
        private TabPage tabStatusEffects = null!;
        private RichTextBox txtSummary = null!;
        private DataGridView dgvMoveUsage = null!;
        private DataGridView dgvStatusEffects = null!;
        private ProgressBar progressBar = null!;
        private Label lblStatus = null!;

        public BattleDebuggerTab()
        {
            InitializeComponent();

            LoadPokemonList();
        }

        private void InitializeComponent()
        {
            this.comboPlayerPokemon = new ComboBox();
            this.comboEnemyPokemon = new ComboBox();
            this.numericLevel = new NumericUpDown();
            this.numericBattles = new NumericUpDown();
            this.checkDetailedOutput = new CheckBox();
            this.btnRun = new Button();
            this.tabResults = new TabControl();
            this.tabSummary = new TabPage();
            this.tabMoveUsage = new TabPage();
            this.tabStatusEffects = new TabPage();
            this.txtSummary = new RichTextBox();
            this.dgvMoveUsage = new DataGridView();
            this.dgvStatusEffects = new DataGridView();
            this.progressBar = new ProgressBar();
            this.lblStatus = new Label();

            this.SuspendLayout();

            // UserControl
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(0);

            // Usar TableLayoutPanel para evitar solapamientos
            var mainTableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(10)
            };
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350));
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Panel izquierdo - Configuración
            var panelConfig = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(20),
                AutoScroll = false,
                Margin = new Padding(5)
            };

            int yPos = 10;
            int spacing = 50;
            int controlWidth = 290;
            int leftMargin = 5;

            var lblTitle = new Label
            {
                Text = "Configuration",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(leftMargin, yPos)
            };
            yPos += 40;

            var lblPlayerPokemon = new Label
            {
                Text = "Player Pokemon:",
                Location = new Point(leftMargin, yPos),
                AutoSize = true
            };
            yPos += 25;
            this.comboPlayerPokemon.Location = new Point(leftMargin, yPos);
            this.comboPlayerPokemon.Width = controlWidth;
            this.comboPlayerPokemon.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboPlayerPokemon.Height = 25;
            yPos += spacing;

            var lblEnemyPokemon = new Label
            {
                Text = "Enemy Pokemon:",
                Location = new Point(leftMargin, yPos),
                AutoSize = true
            };
            yPos += 25;
            this.comboEnemyPokemon.Location = new Point(leftMargin, yPos);
            this.comboEnemyPokemon.Width = controlWidth;
            this.comboEnemyPokemon.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboEnemyPokemon.Height = 25;
            yPos += spacing;

            var lblLevel = new Label
            {
                Text = "Level:",
                Location = new Point(leftMargin, yPos),
                AutoSize = true
            };
            yPos += 25;
            this.numericLevel.Location = new Point(leftMargin, yPos);
            this.numericLevel.Width = controlWidth;
            this.numericLevel.Minimum = 1;
            this.numericLevel.Maximum = 100;
            this.numericLevel.Value = 50;
            yPos += spacing;

            var lblBattles = new Label
            {
                Text = "Number of Battles:",
                Location = new Point(leftMargin, yPos),
                AutoSize = true
            };
            yPos += 25;
            this.numericBattles.Location = new Point(leftMargin, yPos);
            this.numericBattles.Width = controlWidth;
            this.numericBattles.Minimum = 1;
            this.numericBattles.Maximum = 10000;
            this.numericBattles.Value = 100;
            yPos += spacing;

            this.checkDetailedOutput.Text = "Detailed Output";
            this.checkDetailedOutput.Location = new Point(leftMargin, yPos);
            this.checkDetailedOutput.AutoSize = true;
            yPos += 45;

            this.btnRun.Text = "Run Battles";
            this.btnRun.Location = new Point(leftMargin, yPos);
            this.btnRun.Width = controlWidth;
            this.btnRun.Height = 50;
            this.btnRun.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            this.btnRun.Click += BtnRun_Click;
            yPos += 65;

            this.progressBar.Location = new Point(leftMargin, yPos);
            this.progressBar.Width = controlWidth;
            this.progressBar.Height = 25;
            this.progressBar.Style = ProgressBarStyle.Continuous;
            yPos += 40;

            this.lblStatus.Text = "Ready";
            this.lblStatus.Location = new Point(leftMargin, yPos);
            this.lblStatus.AutoSize = true;
            this.lblStatus.Width = controlWidth;

            panelConfig.Controls.AddRange(new Control[] {
                lblTitle, lblPlayerPokemon, comboPlayerPokemon,
                lblEnemyPokemon, comboEnemyPokemon,
                lblLevel, numericLevel,
                lblBattles, numericBattles,
                checkDetailedOutput,
                btnRun,
                progressBar,
                lblStatus
            });

            // Panel derecho - Resultados
            this.tabResults.Dock = DockStyle.Fill;
            this.tabResults.Padding = new Point(10, 5);
            this.tabResults.Margin = new Padding(5, 5, 5, 5);

            // Tab Summary
            this.tabSummary.Text = "Summary";
            this.tabSummary.Padding = new Padding(10);
            this.txtSummary.Dock = DockStyle.Fill;
            this.txtSummary.Font = new Font("Consolas", 9);
            this.txtSummary.ReadOnly = true;
            this.txtSummary.Margin = new Padding(10);
            this.txtSummary.Text = "Configure settings and click 'Run Battles' to see results here.";
            this.tabSummary.Controls.Add(this.txtSummary);

            // Tab Move Usage
            this.tabMoveUsage.Text = "Move Usage";
            this.tabMoveUsage.Padding = new Padding(10);
            this.dgvMoveUsage.Dock = DockStyle.Fill;
            this.dgvMoveUsage.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMoveUsage.ReadOnly = true;
            this.dgvMoveUsage.AllowUserToAddRows = false;
            this.dgvMoveUsage.Margin = new Padding(10);
            this.tabMoveUsage.Controls.Add(this.dgvMoveUsage);

            // Tab Status Effects
            this.tabStatusEffects.Text = "Status Effects";
            this.tabStatusEffects.Padding = new Padding(10);
            this.dgvStatusEffects.Dock = DockStyle.Fill;
            this.dgvStatusEffects.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStatusEffects.ReadOnly = true;
            this.dgvStatusEffects.AllowUserToAddRows = false;
            this.dgvStatusEffects.Margin = new Padding(10);
            this.tabStatusEffects.Controls.Add(this.dgvStatusEffects);

            this.tabResults.TabPages.AddRange(new TabPage[] {
                this.tabSummary,
                this.tabMoveUsage,
                this.tabStatusEffects
            });

            // Agregar paneles al TableLayoutPanel
            mainTableLayout.Controls.Add(panelConfig, 0, 0);
            mainTableLayout.Controls.Add(this.tabResults, 1, 0);

            // Agregar TableLayoutPanel al UserControl
            this.Controls.Add(mainTableLayout);

            this.ResumeLayout(true);
            this.PerformLayout();
        }

        private void LoadPokemonList()
        {
            var pokemonList = PokemonCatalog.All.OrderBy(p => p.Name).ToList();

            this.comboPlayerPokemon.Items.Add("(Random)");
            this.comboEnemyPokemon.Items.Add("(Random)");

            foreach (var pokemon in pokemonList)
            {
                this.comboPlayerPokemon.Items.Add(pokemon.Name);
                this.comboEnemyPokemon.Items.Add(pokemon.Name);
            }

            this.comboPlayerPokemon.SelectedIndex = 0;
            this.comboEnemyPokemon.SelectedIndex = 0;
        }

        private async void BtnRun_Click(object? sender, EventArgs e)
        {
            this.btnRun.Enabled = false;
            this.progressBar.Value = 0;
            this.lblStatus.Text = "Running battles...";
            this.txtSummary.Text = "Running battles...\n\nPlease wait...";

            try
            {
                // Obtener configuración
                PokemonSpeciesData? playerPokemon = null;
                PokemonSpeciesData? enemyPokemon = null;

                if (this.comboPlayerPokemon.SelectedIndex > 0)
                {
                    var playerName = this.comboPlayerPokemon.SelectedItem?.ToString();
                    if (playerName != null)
                    {
                        playerPokemon = PokemonCatalog.All.FirstOrDefault(p => p.Name == playerName);
                    }
                }

                if (this.comboEnemyPokemon.SelectedIndex > 0)
                {
                    var enemyName = this.comboEnemyPokemon.SelectedItem?.ToString();
                    if (enemyName != null)
                    {
                        enemyPokemon = PokemonCatalog.All.FirstOrDefault(p => p.Name == enemyName);
                    }
                }

                var config = new BattleRunner.BattleConfig
                {
                    PlayerPokemon = playerPokemon,
                    EnemyPokemon = enemyPokemon,
                    Level = (int)this.numericLevel.Value,
                    NumberOfBattles = (int)this.numericBattles.Value,
                    DetailedOutput = this.checkDetailedOutput.Checked
                };

                // Crear progress reporter
                var progress = new Progress<int>(percent =>
                {
                    this.progressBar.Value = percent;
                    this.lblStatus.Text = $"Running battles... {percent}%";
                    Application.DoEvents();
                });

                // Ejecutar batallas
                var runner = new BattleRunner();
                var stats = await runner.RunBattlesAsync(config, progress);

                // Mostrar resultados
                DisplayResults(stats, config);

                this.lblStatus.Text = "Complete";
                this.progressBar.Value = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\n\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.lblStatus.Text = "Error occurred";
                this.txtSummary.Text = $"Error: {ex.Message}";
            }
            finally
            {
                this.btnRun.Enabled = true;
            }
        }

        private void DisplayResults(BattleRunner.BattleStatistics stats, BattleRunner.BattleConfig config)
        {
            var playerName = config.PlayerPokemon?.Name ?? "Player";
            var enemyName = config.EnemyPokemon?.Name ?? "Enemy";
            var totalBattles = stats.PlayerWins + stats.EnemyWins + stats.Draws;

            // Mostrar resumen
            this.txtSummary.Clear();
            this.txtSummary.AppendText("═══════════════════════════════════════════════════════════════\n");
            this.txtSummary.AppendText("RESUMEN DE BATALLAS\n");
            this.txtSummary.AppendText("═══════════════════════════════════════════════════════════════\n\n");
            this.txtSummary.AppendText($"Total de batallas: {totalBattles}\n\n");
            this.txtSummary.AppendText($"{playerName} ganó: {stats.PlayerWins} ({stats.PlayerWins * 100.0 / totalBattles:F1}%)\n");
            this.txtSummary.AppendText($"{enemyName} ganó: {stats.EnemyWins} ({stats.EnemyWins * 100.0 / totalBattles:F1}%)\n");
            if (stats.Draws > 0)
            {
                this.txtSummary.AppendText($"Empates: {stats.Draws} ({stats.Draws * 100.0 / totalBattles:F1}%)\n");
            }
            this.txtSummary.AppendText("\n");

            // Mostrar estadísticas de movimientos
            if (stats.MoveUsageStats.Count > 0)
            {
                this.txtSummary.AppendText("═══════════════════════════════════════════════════════════════\n");
                this.txtSummary.AppendText("MOVIMIENTOS MÁS USADOS\n");
                this.txtSummary.AppendText("═══════════════════════════════════════════════════════════════\n\n");

                foreach (var pokemonStats in stats.MoveUsageStats.OrderByDescending(p => p.Value.Values.Sum()))
                {
                    var pokemonName = pokemonStats.Key;
                    var moves = pokemonStats.Value.OrderByDescending(m => m.Value).Take(5).ToList();
                    var totalMoves = pokemonStats.Value.Values.Sum();

                    this.txtSummary.AppendText($"{pokemonName} (Total: {totalMoves} movimientos):\n");
                    foreach (var move in moves)
                    {
                        var percentage = (move.Value * 100.0) / totalMoves;
                        this.txtSummary.AppendText($"  {move.Key}: {move.Value} veces ({percentage:F1}%)\n");
                    }
                    this.txtSummary.AppendText("\n");
                }
            }

            // Mostrar estadísticas de efectos de estado
            if (stats.StatusEffectStats.Count > 0)
            {
                this.txtSummary.AppendText("═══════════════════════════════════════════════════════════════\n");
                this.txtSummary.AppendText("EFECTOS DE ESTADO CAUSADOS\n");
                this.txtSummary.AppendText("═══════════════════════════════════════════════════════════════\n\n");

                foreach (var pokemonStats in stats.StatusEffectStats.OrderByDescending(p => p.Value.Values.Sum()))
                {
                    var pokemonName = pokemonStats.Key;
                    var statuses = pokemonStats.Value.OrderByDescending(s => s.Value).ToList();
                    var totalStatuses = pokemonStats.Value.Values.Sum();

                    this.txtSummary.AppendText($"{pokemonName} (Total: {totalStatuses} efectos):\n");
                    foreach (var status in statuses)
                    {
                        var percentage = (status.Value * 100.0) / totalBattles;
                        this.txtSummary.AppendText($"  {status.Key}: {status.Value} veces ({percentage:F1}%)\n");
                    }
                    this.txtSummary.AppendText("\n");
                }
            }

            // Actualizar tabla de movimientos
            UpdateMoveUsageTable(stats);

            // Actualizar tabla de efectos de estado
            UpdateStatusEffectsTable(stats);
        }

        private void UpdateMoveUsageTable(BattleRunner.BattleStatistics stats)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Pokemon", typeof(string));
            dataTable.Columns.Add("Movimiento", typeof(string));
            dataTable.Columns.Add("Usos", typeof(int));
            dataTable.Columns.Add("Porcentaje", typeof(string));

            foreach (var pokemonStats in stats.MoveUsageStats.OrderByDescending(p => p.Value.Values.Sum()))
            {
                var pokemonName = pokemonStats.Key;
                var totalMoves = pokemonStats.Value.Values.Sum();
                var moves = pokemonStats.Value.OrderByDescending(m => m.Value).ToList();

                foreach (var move in moves)
                {
                    var percentage = (move.Value * 100.0) / totalMoves;
                    dataTable.Rows.Add(pokemonName, move.Key, move.Value, $"{percentage:F1}%");
                }
            }

            this.dgvMoveUsage.DataSource = dataTable;
        }

        private void UpdateStatusEffectsTable(BattleRunner.BattleStatistics stats)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Pokemon", typeof(string));
            dataTable.Columns.Add("Efecto", typeof(string));
            dataTable.Columns.Add("Veces", typeof(int));
            dataTable.Columns.Add("Porcentaje", typeof(string));

            var totalBattles = stats.PlayerWins + stats.EnemyWins + stats.Draws;

            foreach (var pokemonStats in stats.StatusEffectStats.OrderByDescending(p => p.Value.Values.Sum()))
            {
                var pokemonName = pokemonStats.Key;
                var statuses = pokemonStats.Value.OrderByDescending(s => s.Value).ToList();

                foreach (var status in statuses)
                {
                    var percentage = (status.Value * 100.0) / totalBattles;
                    dataTable.Rows.Add(pokemonName, status.Key, status.Value, $"{percentage:F1}%");
                }
            }

            this.dgvStatusEffects.DataSource = dataTable;
        }
    }
}

