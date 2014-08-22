﻿using System.Windows.Media.Imaging;
using Core;
using Core.Data;
using DesktopOrganizer.Utils;
using ReactiveUI;

namespace DesktopOrganizer.CaptureIcons
{
    public class IconViewModel : ItemViewModelBase<Icon>
    {
        private BitmapSource _Icon;
        public BitmapSource Icon
        {
            get { return _Icon; }
            set { this.RaiseAndSetIfChanged(ref _Icon, value); }
        }

        public string Title { get { return AssociatedObject.Name; } }

        public string Info { get; set; }

        public IconViewModel(Icon icon) : base(icon)
        {
            Info = string.Format("Position [{0},{1}]", icon.X, icon.Y);
            Icon = IconManager.GetIcon(icon.Name);
        }
    }
}
