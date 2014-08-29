using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using Core;
using Framework.Core;
using Framework.Module;
using Framework.Starter;

namespace DesktopOrganizer.Shell
{
    [Export(typeof (IShell))]
    public class ShellViewModel : ShellViewModelBase, IHandle<ShellMessage>
    {
        private readonly List<IModule> modules;
            
        [ImportingConstructor]
        public ShellViewModel([ImportMany] IEnumerable<Lazy<IModule, IOrderMetadata>> modules, IEventAggregator event_aggregator)
        {
            this.modules = modules.OrderBy(obj => obj.Metadata.Order).Select(obj => obj.Value).ToList();
            this.modules.Apply(m => m.Create(this));

            event_aggregator.Subscribe(this);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            DisplayName = "Desktop Organizer" + (Advapi32.IsRunningAsAdministrator(Process.GetCurrentProcess()) ? " (Administrator)" : string.Empty);
            modules.Apply(m => m.Initialize());
        }

        private void Exit()
        {
            var shell_view = GetView() as IShellView;
            if (shell_view != null)
                shell_view.IsExiting = true;

            TryClose();
        }

        public void Handle(ShellMessage message)
        {
            switch (message.Kind)
            {
                case ShellMessage.MessageKind.Back:
                    Back();
                    break;
                case ShellMessage.MessageKind.Show:
                    Show(message.ViewModel);
                    break;
                case ShellMessage.MessageKind.Exit:
                    Exit();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
