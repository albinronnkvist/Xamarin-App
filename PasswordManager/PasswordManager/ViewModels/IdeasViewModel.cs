using System;
using System.Threading.Tasks;
using System.Windows.Input;
using PasswordManager.Models;
using PasswordManager.Services;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Xamarin.Essentials;

namespace PasswordManager.ViewModels
{
    public class IdeasViewModel : BaseViewModel
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

        // ObservableRangeCollection provide notifications when items get added, removed, or when the whole list is refreshed. 
        // It only updates the view once when we add multiple items, unlike ObservableCollection that updates the view for every item added, which can slow down the application.
        public ObservableRangeCollection<Idea> Ideas { get; set; }



        public IdeasViewModel()
        {
            Ideas = new ObservableRangeCollection<Idea>();

            // Populate the collection-view onload
            Task.Run(async () => await ReadIdeas());

            ReadIdeasCommand = new AsyncCommand(ReadIdeas);
            CreateIdeaCommand = new AsyncCommand(CreateIdea);
            DeleteIdeaCommand = new AsyncCommand<Idea>(DeleteIdea);
        }

        public ICommand ReadIdeasCommand { get; set; }
        public ICommand CreateIdeaCommand { get; set; }
        public ICommand DeleteIdeaCommand { get; set; }



        private async Task ReadIdeas()
        {
            // Mark the page as busy while we get our data
            IsBusy = true;

            try
            {
                var userId = await SecureStorage.GetAsync("user_id");

                await Task.Delay(500);

                Ideas.Clear();

                var ideas = await IdeasService.ReadIdeas(Convert.ToInt32(userId));

                Ideas.AddRange(ideas);
            }
            catch (Exception ex)
            {
                ErrorDisplay = true;
                ErrorMessage = $"Read failed: your device doesn't support secure storage. More info: {ex}";
            }
            // When the data fetching completed or we caught an exception.
            finally
            {
                // Mark the page as not busy since we finished 
                IsBusy = false;
            }
        }

        private async Task CreateIdea()
        {
            SuccessDisplay = false;
            SuccessMessage = "";
            ErrorDisplay = false;
            ErrorMessage = "";

            try
            {
                var userId = await SecureStorage.GetAsync("user_id");

                int created = await IdeasService.CreateIdea(Convert.ToInt32(userId), TitleInput, DescriptionInput);

                if (created > 0)
                {
                    SuccessDisplay = true;
                    SuccessMessage = "Creation succeeded: successfully created a new idea!";
                }
                else
                {
                    ErrorDisplay = true;
                    ErrorMessage = "Creation failed: could not create a new idea.";
                }

                TitleInput = "";
                DescriptionInput = "";

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

        private async Task DeleteIdea(Idea idea)
        {
            await IdeasService.DeleteIdea(idea.Id);

            await ReadIdeas();
        }
    }
}