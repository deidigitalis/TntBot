﻿using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System.Globalization;
using System.Windows.Input;
using TntBot.Properties;
using TntBot.View;

namespace TntBot.ViewModel
{
    internal class InputBaseNameViewModel : BindableBase
    {
        private readonly InputBaseNameDialog view;
        private string input;
        private string message;

        internal InputBaseNameViewModel(InputBaseNameDialog view)
        {
            this.view = view;

            Message = Resources.InputBaseNameMessage;

            CloseCommand = new DelegateCommand<bool?>(Close);
        }

        public ICommand CloseCommand { get; private set; }

        public string Input
        {
            get { return input; }
            set { SetProperty(ref input, value, "Input"); }
        }

        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value, "Message"); }
        }

        private void Close(bool? accepted)
        {
            if (accepted.HasValue)
            {
                if (!accepted.Value)
                {
                    view.DialogResult = false;
                    view.Close();
                }
                else
                {
                    var rule = new BaseNameValidationRule();
                    if (rule.Validate(Input, CultureInfo.CurrentCulture).IsValid)
                    {
                        view.DialogResult = true;
                        view.Close();
                    }
                }
            }
        }
    }
}