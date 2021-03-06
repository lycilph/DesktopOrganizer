﻿using System.ComponentModel.Composition;
using Framework.Core;
using Framework.Dialogs;
using Framework.Module;
using Framework.Window;

namespace DesktopOrganizer.About
{
    [Export(typeof(IModule))]
    [ExportMetadata("Order", 2)]
    public class AboutModule : ModuleBase
    {
        public override void Create(IShell shell)
        {
            shell.RightShellCommands.Add(new WindowCommand("About", ShowAbout));
        }

        public async void ShowAbout()
        {
            await DialogController.ShowAsync(new AboutViewModel(), DialogButtonOptions.Ok);
        }
    }
}
