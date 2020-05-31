using System.Windows;
using TeamsAppLib.Log;
using TeamsAppLib.Settings;

namespace TeamsAppWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            TraceManager.Init(System.Diagnostics.SourceLevels.All, Constants.LOG_LISTENERNAME, Constants.LOG_FILEPATH, Constants.LOG_FILENAME);
            base.OnStartup(e);
        }
    }
}
