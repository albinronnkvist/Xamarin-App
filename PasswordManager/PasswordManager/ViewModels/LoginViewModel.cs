using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PasswordManager.Models;
using PasswordManager.Views;
using PasswordManager.Services;
using MvvmHelpers.Commands;
using MvvmHelpers;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace PasswordManager.ViewModels
{
    public class LoginViewModel : ObservableObject
    {

        // Properties
        private string userNameInput;
        public string UsernameInput {
            get => userNameInput;
            set
            {
                userNameInput = value;
                OnPropertyChanged(); // Update value in view when the properties value changes
            }
        }

        private string passwordInput;
        public string PasswordInput {
            get => passwordInput;
            set
            {
                passwordInput = value;
                OnPropertyChanged();
            }
        }

        private bool errorDisplay = false;
        public bool ErrorDisplay
        {
            get => errorDisplay;
            set
            {
                errorDisplay = value;
                OnPropertyChanged();
            }
        }

        private string errorMessage = "";
        public string ErrorMessage {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged();
            }
        }

        public LoginViewModel()
        {
            LoginUserCommand = new AsyncCommand(LoginUser);
            GotoRegisterUserCommand = new MvvmHelpers.Commands.Command(GotoRegisterUser);
        }



        // Commands
        public ICommand LoginUserCommand { get; set; }
        public ICommand GotoRegisterUserCommand { get; set; }



        // Methods
        private async Task LoginUser()
        {
            SecureStorage.Remove("user_id");

            // Reset error messages
            ErrorDisplay = false;
            ErrorMessage = "";

            // The service returns a user from the database that matches the input data.
            User user = await UserService.LoginUser(UsernameInput, PasswordInput);

            // Check if we got a user from the database
            if (user != null)
            {
                try
                {
                    // Store the user id in a secure key/value pair
                    await SecureStorage.SetAsync("user_id", Convert.ToString(user.Id));

                    // If the key exists
                    if (await SecureStorage.GetAsync("user_id") != null)
                    {
                        // Set root page of the application to the Appshell
                        Application.Current.MainPage = new AppShell();

                        // Redirect user to ideas page and replace the navigation stack with the IdeasPage
                        await Shell.Current.GoToAsync($"//{nameof(IdeasPage)}");
                    }

                    UsernameInput = "";
                    PasswordInput = "";
                    ErrorDisplay = false;
                    ErrorMessage = "";
                }
                catch (Exception ex)
                {
                    ErrorDisplay = true;
                    ErrorMessage = $"Login failed: your device doesn't support secure storage. More info: {ex}";
                }       
            }
            else if (user == null)
            {
                // If no user matched the input data, display error messages
                ErrorDisplay = true;
                ErrorMessage = "Login failed: wrong username or password.";
            }
            else
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }

        private async void GotoRegisterUser()
        {
            // Redirect to the registration page and push the page onto the navigation stack, so it's possible to go back.
            await Shell.Current.GoToAsync($"{nameof(RegistrationPage)}");
        }
    }
}
