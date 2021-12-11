using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PasswordManager.Services;
using PasswordManager.Views;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PasswordManager.ViewModels
{
    public class AccountViewModel : ObservableObject
    {
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
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged();
            }
        }



        public AccountViewModel()
        {
            LogoutUserCommand = new MvvmHelpers.Commands.Command(LogoutUser);
            DeleteUserCommand = new AsyncCommand(DeleteUser);
        }

        // Define commands
        public ICommand LogoutUserCommand { get; set; }
        public ICommand DeleteUserCommand { get; set; }



        private async void LogoutUser()
        {
            // Remove the user id key from secure storate
            var remove = SecureStorage.Remove("user_id");

            // If the key was removed
            if (remove)
            {
                // Redirect to login page and replace the navigation stack with the LoginPage
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            else
            {
                ErrorDisplay = true;
                ErrorMessage = "Logout failed: the user id key was not removed.";
            }
        }

        private async Task DeleteUser()
        {
            try
            {
                // Get the user id key from secure storage
                var userId = await SecureStorage.GetAsync("user_id");

                // Pass the user id as an integer to the DeleteUser task
                int deleted = await UserService.DeleteUser(Convert.ToInt32(userId));

                // If some rows were deleted
                if (deleted > 0)
                {
                    // Remove the user id key
                    // Returns true if the key value pair was removed
                    var remove = SecureStorage.Remove("user_id");

                    // If the key was removed
                    if (remove)
                    {
                        // Redirect to login page and replace the navigation stack with the LoginPage
                        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                    }
                    else
                    {
                        ErrorDisplay = true;
                        ErrorMessage = "Deletion succeeded but logout failed: the user id key was not removed.";
                    }
                }
                else
                {
                    ErrorDisplay = true;
                    ErrorMessage = "Deletion failed: could not delete the user.";
                }
            }
            catch (Exception ex)
            {
                ErrorDisplay = true;
                ErrorMessage = $"Deletion failed: your device doesn't support secure storage. More info: {ex}";
            }
        }
    }
}
