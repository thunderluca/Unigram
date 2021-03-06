﻿using LinqToVisualTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unigram.Common;
using Unigram.Services;
using Unigram.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Unigram.Controls.Views
{
    public sealed partial class SettingsPasscodeConfirmView : TLContentDialog
    {
        private readonly IPasscodeService _passcodeService;

        public SettingsPasscodeConfirmView(IPasscodeService passcodeService)
        {
            InitializeComponent();

            _passcodeService = passcodeService;

            Title = Strings.Resources.Passcode;
            PrimaryButtonText = Strings.Resources.OK;
            SecondaryButtonText = Strings.Resources.Cancel;

            var confirmScope = new InputScope();
            confirmScope.Names.Add(new InputScopeName(passcodeService.IsSimple ? InputScopeNameValue.NumericPin : InputScopeNameValue.Password));
            Confirm.InputScope = confirmScope;
            Confirm.MaxLength = passcodeService.IsSimple ? 4 : int.MaxValue;
        }

        public bool IsSimple { get; set; }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (/*Confirm.Password.Length != 4 || !Confirm.Password.All(x => x >= '0' && x <= '9') ||*/ !_passcodeService.Check(Confirm.Password))
            {
                VisualUtilities.ShakeView(Confirm);
                Confirm.Password = string.Empty;
                args.Cancel = true;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void Confirm_Changed(object sender, RoutedEventArgs e)
        {
            if (IsSimple && Confirm.Password.Length == 4 && Confirm.Password.All(x => x >= '0' && x <= '9'))
            {
                Done();
            }
        }

        private void Done()
        {
            var button = this.Descendants<Button>().FirstOrDefault(x => x is Button y && y.Parent is Border border && border.Name == "Button1Host") as Button;
            if (button != null)
            {
                new ButtonAutomationPeer(button).Invoke();
            }
        }

        private void Password_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key >= Windows.System.VirtualKey.Number0 && e.Key <= Windows.System.VirtualKey.Number9) { }
            else if (e.Key >= Windows.System.VirtualKey.NumberPad0 && e.Key <= Windows.System.VirtualKey.NumberPad9) { }
            else if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Done();
                e.Handled = true;
            }
            else
            {
                e.Handled = IsSimple;
            }
        }
    }
}
