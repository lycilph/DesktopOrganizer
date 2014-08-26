﻿using System;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using DesktopOrganizer.Data;
using DesktopOrganizer.Shell;
using DesktopOrganizer.Utils;
using ReactiveUI;

namespace DesktopOrganizer.Settings
{
    [Export(typeof(SettingsViewModel))]
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IEventAggregator event_aggregator;
        private readonly ApplicationSettings application_settings;

        private string _ExcludedProcesses;
        public string ExcludedProcesses
        {
            get { return _ExcludedProcesses; }
            set { this.RaiseAndSetIfChanged(ref _ExcludedProcesses, value); }
        }

        private bool _LaunchOnWindowsStart;
        public bool LaunchOnWindowsStart
        {
            get { return _LaunchOnWindowsStart; }
            set { this.RaiseAndSetIfChanged(ref _LaunchOnWindowsStart, value); }
        }

        [ImportingConstructor]
        public SettingsViewModel(ApplicationSettings application_settings, IEventAggregator event_aggregator)
        {
            this.application_settings = application_settings;
            this.event_aggregator = event_aggregator;
        }

        private void Initialize()
        {
            ExcludedProcesses = string.Join(", ", application_settings.ExcludedProcesses);
            LaunchOnWindowsStart = application_settings.LaunchOnWindowsStart;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Initialize();
        }

        public void Back()
        {
            event_aggregator.PublishOnCurrentThread(ShellMessage.BackMessage());
        }

        public void Ok()
        {
            application_settings.ExcludedProcesses = ExcludedProcesses.Split(new[] { "," }, StringSplitOptions.None).Select(s => s.Trim()).ToList();
            application_settings.LaunchOnWindowsStart = LaunchOnWindowsStart;
            Back();
        }

        public void Cancel()
        {
            Back();
        }

        public void Reset()
        {
            application_settings.Reset();
            Initialize();
        }
    }
}