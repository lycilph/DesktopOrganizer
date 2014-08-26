﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Core.Data;
using DesktopOrganizer.Data;
using DesktopOrganizer.Dialogs;
using DesktopOrganizer.Shell;
using DesktopOrganizer.Utils;
using ReactiveUI;

namespace DesktopOrganizer.Capture
{
    public class CaptureViewModel<TM, TVM> : ViewModelBase where TVM : ItemViewModelBase<TM>, new()
    {
        private readonly ApplicationSettings application_settings;
        private readonly Layout<TM> layout;

        public Action<Layout<TM>, Layout<TM>> AcceptAction { get; set; }
        public Func<IEnumerable<TM>> CaptureAction { get; set; }
        public bool CaptureOnActivation { get; set; }
        public string Title { get; set; }
        public string ItemsTitle { get; set; }

        private string _LayoutName;
        public string LayoutName
        {
            get { return _LayoutName; }
            set { this.RaiseAndSetIfChanged(ref _LayoutName, value); }
        }

        private Shortcut _Shortcut;
        public Shortcut Shortcut
        {
            get { return _Shortcut; }
            set { this.RaiseAndSetIfChanged(ref _Shortcut, value); }
        }

        private TVM _CurrentItem;
        public TVM CurrentItem
        {
            get { return _CurrentItem; }
            set { this.RaiseAndSetIfChanged(ref _CurrentItem, value); }
        }

        private ReactiveList<TVM> _Items;
        public ReactiveList<TVM> Items
        {
            get { return _Items; }
            set { this.RaiseAndSetIfChanged(ref _Items, value); }
        }

        public CaptureViewModel(Layout<TM> layout)
        {
            this.layout = layout;

            application_settings = IoC.Get<ApplicationSettings>();

            _LayoutName = layout.Name;
            _Shortcut = layout.Shortcut;
            _Items = layout.Items.Select(i => (TVM)Activator.CreateInstance(typeof(TVM), i)).ToReactiveList();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            application_settings.SuppressShortcuts = true;

            if (CaptureOnActivation)
                Capture();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                application_settings.SuppressShortcuts = false;
        }

        public async void Capture()
        {
            var controller = await DialogController.ShowBusyDialog("Busy", Title);
            var captured_items = await Task.Factory.StartNew(() => CaptureAction());
            await Task.Delay(1000);
            Items = captured_items.Select(i => (TVM)Activator.CreateInstance(typeof(TVM), i)).ToReactiveList();
            await controller.CloseAsync();
        }

        public void Back()
        {
            var event_aggregator = IoC.Get<IEventAggregator>();
            event_aggregator.PublishOnCurrentThread(ShellMessage.BackMessage());
        }

        public void Ok()
        {
            var new_layout = new Layout<TM>
            {
                Name = LayoutName,
                Shortcut = Shortcut.Clone(),
                Items = Items.Select(i => i.AssociatedObject).ToList()
            };

            AcceptAction(new_layout, layout);
            Back();
        }

        public void Cancel()
        {
            Back();
        }

        public void Delete()
        {
            if (CurrentItem != null)
                Items.Remove(CurrentItem);
        }
    }
}