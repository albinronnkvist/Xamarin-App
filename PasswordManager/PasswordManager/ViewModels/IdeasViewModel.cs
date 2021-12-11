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

            // !!!!
            // Problem: populate the collection view onload. For some reason this populates the list twice with duplicate items.
            // Task.Run(() => ReadIdeas());
            // !!!!

            ReadIdeasCommand = new AsyncCommand(ReadIdeas);
            DeleteIdeaCommand = new AsyncCommand<Idea>(DeleteIdea);
        }

        public ICommand ReadIdeasCommand { get; set; }
        public ICommand DeleteIdeaCommand { get; set; }



        private async Task ReadIdeas()
        {
            // Mark the page as busy while we get our data, this property is bound to the RefreshView in our view.
            IsBusy = true;

            // Clear the ideas collection
            Ideas.Clear();

            try
            {
                // Get the user id value from secure storage
                var userId = await SecureStorage.GetAsync("user_id");

                // Run the ReadIdeas task in the IdeasService and pass the current user id as a parameter.
                // The task returns a list of idea items from the database.
                var ideas = await IdeasService.ReadIdeas(Convert.ToInt32(userId));

                // Add the list we got from the database to our Ideas ObservableRangeCollection
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

        private async Task DeleteIdea(Idea idea)
        {
            // Run the DeleteIdea task and pass the chosen object's id.
            // This task returns the number of rows that were deleted in the database, this can be useful if you want to do some error handling. But i have decided to not do that in this case.
            await IdeasService.DeleteIdea(idea.Id);

            // When the above task has finished:
            // Run the ReadIdeas task which gets the updated list of ideas.
            await ReadIdeas();
        }
    }
}