﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.PowerToys.Settings.UI.Helpers;
using Microsoft.PowerToys.Settings.UI.Lib;
using Microsoft.PowerToys.Settings.UI.ViewModels.Commands;
using Microsoft.PowerToys.Settings.UI.Views;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace Microsoft.PowerToys.Settings.UI.ViewModels
{
    public class GeneralViewModel : Observable
    {
        private GeneralSettings GeneralSettingsConfigs { get; set; }

        public ButtonClickCommand CheckFoUpdatesEventHandler { get; set; }

        public ButtonClickCommand RestartElevatedButtonEventHandler { get; set; }

        public GeneralViewModel()
        {
            this.CheckFoUpdatesEventHandler = new ButtonClickCommand(CheckForUpdates_Click);
            this.RestartElevatedButtonEventHandler = new ButtonClickCommand(Restart_Elevated);

            GeneralSettingsConfigs = SettingsUtils.GetSettings<GeneralSettings>(string.Empty);

            switch (GeneralSettingsConfigs.theme.ToLower())
            {
                case "light":
                    _isLightThemeRadioButtonChecked = true;
                    ShellPage.ShellHandler.RequestedTheme = ElementTheme.Light;
                    break;
                case "dark":
                    _isDarkThemeRadioButtonChecked = true;
                    ShellPage.ShellHandler.RequestedTheme = ElementTheme.Dark;
                    break;
                case "system":
                    _isSystemThemeRadioButtonChecked = true;
                    ShellPage.ShellHandler.RequestedTheme = ElementTheme.Default;
                    break;
            }

            _startup = GeneralSettingsConfigs.startup;
        }

        private bool _packaged = false;
        private bool _startup = false;
        private bool _isElevated = false;
        private bool _runElevated = false;
        private bool _isDarkThemeRadioButtonChecked = false;
        private bool _isLightThemeRadioButtonChecked = false;
        private bool _isSystemThemeRadioButtonChecked = false;

        // Gets or sets a value indicating whether packaged.
        public bool Packaged
        {
            get
            {
                return _packaged;
            }

            set
            {
                if (_packaged != value)
                {
                    _packaged = value;
                    RaisePropertyChanged();
                }
            }
        }

        // Gets or sets a value indicating whether run powertoys on start-up.
        public bool Startup
        {
            get
            {
                return _startup;
            }

            set
            {
                if (_startup != value)
                {
                    RaisePropertyChanged();
                }

                _startup = value;
            }
        }

        // Gets or sets a value indicating whether the powertoy elevated.
        public bool IsElevated
        {
            get
            {
                return _isElevated;
            }

            set
            {
                if (_isElevated != value)
                {
                    RaisePropertyChanged();
                }

                _isElevated = value;
            }
        }

        // Gets or sets a value indicating whether powertoys should run elevated.
        public bool RunElevated
        {
            get
            {
                return _runElevated;
            }

            set
            {
                if (_runElevated != value)
                {
                    RaisePropertyChanged();
                }

                _runElevated = value;
            }
        }

        public bool IsDarkThemeRadioButtonChecked
        {
            get
            {
                return _isDarkThemeRadioButtonChecked;
            }

            set
            {
                if (value == true)
                {
                    GeneralSettingsConfigs.theme = "dark";
                    ShellPage.ShellHandler.RequestedTheme = ElementTheme.Dark;
                    RaisePropertyChanged();
                }

                _isDarkThemeRadioButtonChecked = value;
            }
        }

        public bool IsLightThemeRadioButtonChecked
        {
            get
            {
                return _isLightThemeRadioButtonChecked;
            }

            set
            {
                if (value == true)
                {
                    GeneralSettingsConfigs.theme = "light";
                    ShellPage.ShellHandler.RequestedTheme = ElementTheme.Light;
                    RaisePropertyChanged();
                }

                _isLightThemeRadioButtonChecked = value;
            }
        }

        public bool IsSystemThemeRadioButtonChecked
        {
            get
            {
                return _isSystemThemeRadioButtonChecked;
            }

            set
            {
                if (value == true)
                {
                    GeneralSettingsConfigs.theme = "system";
                    ShellPage.ShellHandler.RequestedTheme = ElementTheme.Default;
                    RaisePropertyChanged();
                }

                _isSystemThemeRadioButtonChecked = value;
            }
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Notify UI of property change
            OnPropertyChanged(propertyName);

            SettingsUtils.SaveSettings(GeneralSettingsConfigs.ToJsonString(), string.Empty);

            OutGoingGeneralSettings outsettings = new OutGoingGeneralSettings(GeneralSettingsConfigs);

            ShellPage.DefaultSndMSGCallback(outsettings.ToString());
        }

        // callback function to launch the URL to check for updates.
        private async void CheckForUpdates_Click()
        {
            await Launcher.LaunchUriAsync(new Uri("https://github.com/microsoft/PowerToys/releases"));
        }

        private void Restart_Elevated()
        {
            GeneralSettings settings = SettingsUtils.GetSettings<GeneralSettings>(string.Empty);
            settings.run_elevated = true;
            OutGoingGeneralSettings outsettings = new OutGoingGeneralSettings(settings);

            if (ShellPage.DefaultSndMSGCallback != null)
            {
                ShellPage.DefaultSndMSGCallback(outsettings.ToString());
            }
        }
    }
}