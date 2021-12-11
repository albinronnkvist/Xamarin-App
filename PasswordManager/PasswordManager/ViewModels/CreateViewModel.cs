using MvvmHelpers;
using MvvmHelpers.Commands;
using PasswordManager.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;

namespace PasswordManager.ViewModels
{
    public class CreateViewModel : BaseViewModel
    {
        private string titleInput;
        public string TitleInput
        {
            get => titleInput;
            set
            {
                titleInput = value;
                OnPropertyChanged();
            }
        }

        private string descriptionInput;
        public string DescriptionInput
        {
            get => descriptionInput;
            set
            {
                descriptionInput = value;
                OnPropertyChanged();
            }
        }

        private bool successDisplay = false;
        public bool SuccessDisplay
        {
            get => successDisplay;
            set
            {
                successDisplay = value;
                OnPropertyChanged();
            }
        }

        private string successMessage = "";
        public string SuccessMessage
        {
            get => successMessage;
            set
            {
                successMessage = value;
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
        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                errorMessage = value;
                OnPropertyChanged();
            }
        }



        public CreateViewModel()
        {
            CreateIdeaCommand = new AsyncCommand(CreateIdea);
        }
        public ICommand CreateIdeaCommand { get; set; }



        private async Task CreateIdea()
        {
            // Clear messages
            SuccessDisplay = false;
            SuccessMessage = "";
            ErrorDisplay = false;
            ErrorMessage = "";

            // If user inputs are null, empty or whitespace
            if (!String.IsNullOrWhiteSpace(TitleInput) && !String.IsNullOrWhiteSpace(DescriptionInput))
            {
                try
                {
                    // Get the user id value from secure storage
                    var userId = await SecureStorage.GetAsync("user_id");

                    // After we go the user id:
                    // Run the CreateIdea task and pass the user id and user inputs
                    // This task returns the number of rows added to the database table
                    int created = await IdeasService.CreateIdea(Convert.ToInt32(userId), TitleInput, DescriptionInput);

                    // If some rows were added to the database
                    if (created > 0)
                    {
                        SuccessDisplay = true;
                        SuccessMessage = "Creation succeeded: successfully created a new idea!";

                        TitleInput = "";
                        DescriptionInput = "";
                    }
                    else
                    {
                        ErrorDisplay = true;
                        ErrorMessage = "Creation failed: could not create a new idea.";
                    }                    

                    // Wait 3 seconds before clearing messages
                    await Task.Delay(3000);

                    SuccessDisplay = false;
                    SuccessMessage = "";
                    ErrorDisplay = false;
                    ErrorMessage = "";
                }
                catch (Exception ex)
                {
                    ErrorDisplay = true;
                    ErrorMessage = $"Creation failed: your device doesn't support secure storage. More info: {ex}";
                }
            }
            else
            {
                ErrorDisplay = true;
                ErrorMessage = "Creation failed: you need to enter a title and a description.";
            }
        }
    }
}
