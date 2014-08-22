using Caliburn.Micro.ReactiveUI;
using DesktopOrganizer.Data;

namespace DesktopOrganizer.Utils
{
    public class ViewModelBase : ReactiveScreen
    {
        protected readonly ApplicationSettings application_settings;

        public ViewModelBase(ApplicationSettings application_settings)
        {
            this.application_settings = application_settings;
        }
    }
}
