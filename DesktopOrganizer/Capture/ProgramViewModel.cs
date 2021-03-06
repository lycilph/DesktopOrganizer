﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Core;
using Core.Data;
using Framework.Mvvm;
using ReactiveUI;

namespace DesktopOrganizer.Capture
{
    public class ProgramViewModel : ItemViewModelBase<Program>
    {
        private BitmapSource _Icon;
        public BitmapSource Icon
        {
            get { return _Icon; }
            private set { this.RaiseAndSetIfChanged(ref _Icon, value); }
        }

        public string Info { get; set; }

        public List<WindowViewModel> Windows { get; set; }

        public ProgramViewModel() : this(null) { }

        public ProgramViewModel(Program program) : base(program)
        {
            Windows = AssociatedObject.Windows.Select(w => new WindowViewModel(w)).ToList();

            var icon = System.Drawing.Icon.ExtractAssociatedIcon(AssociatedObject.ProcessPath);
            Icon = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            var placement = (program.Placement.show_command == User32.ShowWindowCommands.Normal
                ? program.Placement.normal_position.ToString()
                : program.Placement.show_command.ToString());

            var admin = (program.Admin ? " (Administrator)" : string.Empty);

            Info = string.Format("{0} [{1}]{2}", program.ProcessName, placement, admin);
        }
    }
}
