using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Core.Data;
using DesktopOrganizer.Capture;
using DesktopOrganizer.Data;
using DesktopOrganizer.Shell;
using DesktopOrganizer.Utils;
using Framework.Mvvm;
using ReactiveUI;
using WindowManager = DesktopOrganizer.Utils.WindowManager;

namespace DesktopOrganizer.Main
{
    [Export("Main", typeof(IViewModel))]
    public class MainViewModel : ViewModelBase
    {
        private readonly IEventAggregator event_aggregator;
        private readonly ApplicationSettings application_settings;

        public List<CaptureCommand> CaptureCommands { get; set; }

        private ILayoutViewModel _CurrentLayout;
        public ILayoutViewModel CurrentLayout
        {
            get { return _CurrentLayout; }
            set { this.RaiseAndSetIfChanged(ref _CurrentLayout, value); }
        }

        public IReactiveDerivedList<ILayoutViewModel> Layouts { get; private set; }

        private readonly ObservableAsPropertyHelper<bool> _CanApply;
        public bool CanApply { get { return _CanApply.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanEdit;
        public bool CanEdit { get { return _CanEdit.Value; } }

        private readonly ObservableAsPropertyHelper<bool> _CanDelete;
        public bool CanDelete { get { return _CanDelete.Value; } }

        [ImportingConstructor]
        public MainViewModel(ApplicationSettings application_settings, IEventAggregator event_aggregator)
        {
            this.application_settings = application_settings;
            this.event_aggregator = event_aggregator;

            CaptureCommands = new List<CaptureCommand>
            {
                new CaptureCommand {Kind = CaptureCommand.CaptureKind.Windows},
                new CaptureCommand {Kind = CaptureCommand.CaptureKind.Icons}
            };

            Layouts = application_settings.Layouts.CreateDerivedCollection(l =>
            {
                ILayoutViewModel vm = null;
                if (l is Layout<Program>)
                    vm = new ProgramLayoutViewModel(l as Layout<Program>);
                if (l is Layout<Icon>)
                    vm = new IconLayoutViewModel(l as Layout<Icon>);
                return vm;
            });

            var obs = this.WhenAny(x => x.CurrentLayout, x => x.Value != null);
            _CanApply = obs.ToProperty(this, x => x.CanApply);
            _CanEdit = obs.ToProperty(this, x => x.CanEdit);
            _CanDelete = obs.ToProperty(this, x => x.CanDelete);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            if (CurrentLayout == null && Layouts.Any())
                CurrentLayout = Layouts.First();
        }

        public void Capture(CaptureCommand capture_command)
        {
            var vm = ViewModelFactory.Create(capture_command.Kind);
            event_aggregator.PublishOnCurrentThread(ShellMessage.ShowMessage(vm));
        }

        public void Apply()
        {
            if (CurrentLayout is ProgramLayoutViewModel)
            {
                var layout = CurrentLayout as ProgramLayoutViewModel;
                WindowManager.ApplyLayout(layout.AssociatedObject);
            }
            else if (CurrentLayout is IconLayoutViewModel)
            {
                var layout = CurrentLayout as IconLayoutViewModel;
                IconManagerWrapper.ApplyLayout(layout.AssociatedObject);
            }
            else
                throw new Exception();
        }

        public void Edit()
        {
            var vm = ViewModelFactory.Edit(CurrentLayout);
            event_aggregator.PublishOnCurrentThread(ShellMessage.ShowMessage(vm));
        }

        public void Delete()
        {
            if (CurrentLayout is ProgramLayoutViewModel)
            {
                var layout = CurrentLayout as ProgramLayoutViewModel;
                application_settings.Remove(layout.AssociatedObject);
            }
            else if (CurrentLayout is IconLayoutViewModel)
            {
                var layout = CurrentLayout as IconLayoutViewModel;
                application_settings.Remove(layout.AssociatedObject);
            }
            else
                throw new Exception();
        }
    }
}
