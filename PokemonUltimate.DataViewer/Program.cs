using System;
using System.Windows.Forms;

namespace PokemonUltimate.DataViewer;

/// <summary>
/// Windows Forms application for viewing game data.
/// </summary>
/// <remarks>
/// **Feature**: 6: Development Tools
/// **Sub-Feature**: 6.7: Data Viewer
/// **Documentation**: See `docs/features/6-development-tools/6.7-data-viewer/README.md`
/// </remarks>
static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
