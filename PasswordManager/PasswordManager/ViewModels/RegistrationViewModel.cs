using System;
using System.Windows.Input;
using System.Threading.Tasks;
using PasswordManager.Views;
using PasswordManager.Services;
using MvvmHelpers.Commands;
using MvvmHelpers;
using Xamarin.Forms;

namespace PasswordManager.ViewModels
{
    // I'm using MvvmHelpers with ObservableObject which implements the INotifyPropertyChanged interface that enables two-way data binding.
    public class RegistrationViewModel : ObservableObject
    {
        // User inputs are bound to these auto implemented properties
        public string UsernameInput { get; set; }
        public string PasswordInput { get; set; }


        // Error properties
        private bool errorDisplay = false;
        public bool ErrorDisplay
        {
            get => errorDisplay;
            set
            {
                errorDisplay = value;

                // Listen to changes on the property and automatically update the view to show the new value
                OnPropertyChanged();
            }
        }

        private string errorMessage = "";
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged();
            }
        }



        public RegistrationViewModel()
        {
            // Xamarin and Mvvm helpers has built in command implementation
            // These commands run when the user interacts with the UI, like clicking a button.

            // Create async command that runs the RegisterUser task
            RegisterUserCommand = new AsyncCommand(RegisterUser);

            // Create command that runs the GoToLogin action
            GotoLoginCommand = new MvvmHelpers.Commands.Command(GotoLogin);
        }

        // Define commands that the views can bind to
        public ICommand RegisterUserCommand { get; set; }
        public ICommand GotoLoginCommand { get; set; }



        // Register a user
        private async Task RegisterUser()
        {
            ErrorDisplay = false;
            ErrorMessage = "";

            // If user input are null, empty or whitespace
            if (!String.IsNullOrWhiteSpace(UsernameInput) && !String.IsNullOrWhiteSpace(PasswordInput))
            {
                // Run the RegisterUser task with the user inputs as parameters
                // The task returns the number of rows added to the table
                int created = await UserService.RegisterUser(UsernameInput, PasswordInput);

                // If 1 or more rows were added to the table
                if (created > 0)
                {
                    // Go to login page and replace the navigation stack with the LoginPage
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
                else
                {
                    ErrorDisplay = true;
                    ErrorMessage = "Registration failed: something went wrong when trying to store user in database.";
                }
            } 
            else
            {
                ErrorDisplay = true;
                ErrorMessage = "Registration failed: you need to enter a username and a password.";
            }
        }

        private async void GotoLogin()
        {
            // Go to login page and replace the navigation stack with the LoginPage
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}
