using System;
using Caliburn.Micro;
using Core.Data;
using DesktopOrganizer.Data;
using DesktopOrganizer.Main;
using DesktopOrganizer.Utils;
using Framework.Mvvm;
using WindowManager = DesktopOrganizer.Utils.WindowManager;

namespace DesktopOrganizer.Capture
{
    public static class ViewModelFactory
    {
        public static IViewModel Create(CaptureCommand.CaptureKind kind)
        {
            var application_settings = IoC.Get<ApplicationSettings>();
            switch (kind)
            {
                case CaptureCommand.CaptureKind.Windows:
                    {
                        var layout = new Layout<Program> { Name = "Default" };
                        var vm = new CaptureViewModel<Program, ProgramViewModel>(layout)
                        {
                            Title = "Capture Windows Layout",
                            ItemsTitle = "Windows",
                            CaptureOnActivation = true,
                            AcceptAction = (l, _) => application_settings.Add(l),
                            CaptureAction = () => WindowManager.GetPrograms()
                        };
                        return vm;
                    }
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
                        return vm;
                    }
                default:
                    throw new Exception();
            }
        }

        public static IViewModel Edit(ILayoutViewModel layout)
        {
            var application_settings = IoC.Get<ApplicationSettings>();

            if (layout is ProgramLayoutViewModel)
            {
                var temp = layout as ProgramLayoutViewModel;
                var vm = new CaptureViewModel<Program, ProgramViewModel>(temp.AssociatedObject)
                {
                    Title = "Edit Windows Layout",
                    ItemsTitle = "Windows",
                    AcceptAction = (l1, l2) => application_settings.Update(l1, l2),
                    CaptureAction = () => WindowManager.GetPrograms()
                };
                return vm;
            }
            
            if (layout is IconLayoutViewModel)
            {
                var temp = layout as IconLayoutViewModel;
                var vm = new CaptureViewModel<Icon, IconViewModel>(temp.AssociatedObject)
                {
                    Title = "Edit Icons Layout",
                    ItemsTitle = "Icons",
                    AcceptAction = (l1, l2) => application_settings.Update(l1, l2),
                    CaptureAction = () => IconManagerWrapper.GetIcons()
                };
                return vm;
            }
            
            throw new Exception();
        }
    }
}
