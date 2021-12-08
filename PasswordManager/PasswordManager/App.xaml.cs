using PasswordManager.Views;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PasswordManager
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // After creating the XAML file that subclasses the Shell object (AppShell),
            // the MainPage property of the App class should be set to the subclassed Shell object.

            // Get user id key from secure storage
            var userId = SecureStorage.GetAsync("user_id");

            // If they key exists (has a value)
            if (userId != null)
            {
                // Set application root page to AppShell
                MainPage = new AppShell();
            }
            // If the key doesn't exist
            else
            {
                // Set application root page to LoginPage
                MainPage = new LoginPage();
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
