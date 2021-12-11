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
        public string UsernameInput { get; set; }
        public string PasswordInput { get; set; }

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

        public ICommand LoginUserCommand { get; set; }
        public ICommand GotoRegisterUserCommand { get; set; }



        private async Task LoginUser()
        {
            // Remove user id from secure storage
            SecureStorage.Remove("user_id");

            // Reset error messages
            ErrorDisplay = false;
            ErrorMessage = "";

            // Run the LoginUser task in userService and await result.
            // The task returns a user from the database that matches the input data. Or null if there was no match.
            User user = await UserService.LoginUser(UsernameInput, PasswordInput);

            // When we get our result back from the LoginUser task:
            // check if we got a user from the database
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

                        // Redirect user to IdeasPage and replace the navigation stack with the IdeasPage, so it's not possible to go back to the login page.
                        await Shell.Current.GoToAsync($"//{nameof(IdeasPage)}");
                    }

                    // Clear inputs and error messages
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
            // If no user matched the input data, display error messages
            else if (user == null)
            {
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
