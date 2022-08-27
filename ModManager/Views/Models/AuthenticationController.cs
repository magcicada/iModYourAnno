﻿using Imya.Models;
using Imya.Models.NotifyPropertyChanged;
using Imya.UI.Popup;
using Imya.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imya.UI.Views.Models
{
    public class AuthenticationController : PropertyChangedNotifier
    {
        private AuthCodePopup? AuthCodePopup;

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set => SetProperty(ref _isAuthenticated, value);
        }
        private bool _isAuthenticated = false;

        public String? AuthenticatedUser
        {
            get => _authenticatedUser;
            set => SetProperty(ref _authenticatedUser, value);
        }
        private String? _authenticatedUser;

        public Uri? AvatarUri
        {
            get => _uri;
            set => SetProperty(ref _uri, value);
        }
        private Uri? _uri;

        public AuthenticationController()
        {
            GithubClientProvider.Authenticator.UserCodeReceived += OnAuthCodeReceived;
            GithubClientProvider.Authenticator.AuthenticationSuccess += OnAuthSuccess;
        }

        public void Authenticate()
        {
            Task.Run(async () => await GithubClientProvider.RunAuthenticate());
        }

        public void Logout()
        {
            var dialogresult = new GenericOkayPopup() { MESSAGE = new SimpleText("Are you sure you want to log out?") }.ShowDialog();
            if (dialogresult is false) return;

            GithubClientProvider.Client.Credentials = Octokit.Credentials.Anonymous;
            IsAuthenticated = false;
            AvatarUri = null;
            AuthenticatedUser = null;
        }

        private void OnAuthCodeReceived(String AuthCode)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (AuthCodePopup is AuthCodePopup)
                    AuthCodePopup.Close();
                AuthCodePopup = new AuthCodePopup(AuthCode);
                AuthCodePopup.Show();
            });
        }

        private void OnAuthSuccess()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                IsAuthenticated = true;
                AuthCodePopup?.Close();
                Task.Run(async () => await UpdateUserLogin());
            });
        }

        private async Task UpdateUserLogin()
        {
            var user = await GithubClientProvider.Client.User.Current();
            AuthenticatedUser = user.Login;
            Console.WriteLine($"Authenticated as {AuthenticatedUser}");
            AvatarUri = new Uri(user.AvatarUrl);
        }
    }
}