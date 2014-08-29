using System.ComponentModel.Composition;
using Caliburn.Micro;
using DesktopOrganizer.Shell;
using Framework.Module;
using Framework.Mvvm;

namespace DesktopOrganizer.Main
{
    [Export(typeof(IModule))]
    public class MainModule : ModuleBase
    {
        private readonly IViewModel main;
        private readonly IEventAggregator event_aggregator;

        [ImportingConstructor]
        public MainModule([Import("Main")] IViewModel main, IEventAggregator event_aggregator)
        {
            this.main = main;
            this.event_aggregator = event_aggregator;
        }

        public override void Initialize()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.ShowMessage(main));
        }
    }
}
