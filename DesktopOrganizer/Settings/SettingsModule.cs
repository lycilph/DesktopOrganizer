using System.ComponentModel.Composition;
using Framework.Core;
using Framework.Module;
using Framework.Window;

namespace DesktopOrganizer.Settings
{
    [Export(typeof(IModule))]
    [ExportMetadata("Order", 1)]
    public class SettingsModule : ModuleBase
    {
        private readonly IFlyout settings;

        [ImportingConstructor]
        public SettingsModule([Import("Settings")] IFlyout settings)
        {
            this.settings = settings;
        }

        public override void Create(IShell shell)
        {
            shell.ShellFlyouts.Add(settings);
            shell.RightShellCommands.Add(new WindowCommand("Settings", settings.Show));
        }
    }
}
