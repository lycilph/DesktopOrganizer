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
using ReactiveUI;
using WindowManager = DesktopOrganizer.Utils.WindowManager;

namespace DesktopOrganizer.Main
{
    [Export(typeof(MainViewModel))]
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
            switch (capture_command.Kind)
            {
                case CaptureCommand.CaptureKind.Windows:
                {
                    var layout = new Layout<Program> {Name = "Default"};
                    var vm = new CaptureViewModel<Program, ProgramViewModel>(layout)
                    {
                        Title = "Capture Windows Layout",
                        ItemsTitle = "Windows",
                        CaptureOnActivation = true,
                        AcceptAction = (l, _) => application_settings.Add(l),
                        CaptureAction = () => WindowManager.GetPrograms()
                    };
                    event_aggregator.PublishOnCurrentThread(ShellMessage.ShowMessage(vm));
                }
                break;
                case CaptureCommand.CaptureKind.Icons:
                {
                    var layout = new Layout<Icon> { Name = "Default" };
                    var vm = new CaptureViewModel<Icon, IconViewModel>(layout)
                    {
                        Title = "Capture Icons Layout",
                        ItemsTitle = "Icons",
                        CaptureOnActivation = true,
                        AcceptAction = (l, _) => application_settings.Add(l),
                        CaptureAction = () => IconManagerWrapper.GetIcons()
                    };
                    event_aggregator.PublishOnCurrentThread(ShellMessage.ShowMessage(vm));
                }
                break;
                default:
                    throw new Exception();
            }
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
            if (CurrentLayout is ProgramLayoutViewModel)
            {
                var layout = CurrentLayout as ProgramLayoutViewModel;
                var vm = new CaptureViewModel<Program, ProgramViewModel>(layout.AssociatedObject)
                {
                    Title = "Edit Windows Layout",
                    ItemsTitle = "Windows",
                    AcceptAction = (l1, l2) => application_settings.Update(l1, l2),
                    CaptureAction = () => WindowManager.GetPrograms()
                };
                event_aggregator.PublishOnCurrentThread(ShellMessage.ShowMessage(vm));
            }
            else if (CurrentLayout is IconLayoutViewModel)
            {
                var layout = CurrentLayout as IconLayoutViewModel;
                var vm = new CaptureViewModel<Icon, IconViewModel>(layout.AssociatedObject)
                {
                    Title = "Edit Icons Layout",
                    ItemsTitle = "Icons",
                    AcceptAction = (l1, l2) => application_settings.Update(l1, l2),
                    CaptureAction = () => IconManagerWrapper.GetIcons()
                };
                event_aggregator.PublishOnCurrentThread(ShellMessage.ShowMessage(vm));
            }
            else
                throw new Exception();
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
