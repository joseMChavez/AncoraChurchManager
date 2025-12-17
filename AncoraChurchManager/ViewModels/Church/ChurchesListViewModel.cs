using System.Collections.ObjectModel;
using Core.Services;

namespace AncoraChurchManager.ViewModels.Church;

 
    public class ChurchesListViewModel : BaseViewModel
    {
        private readonly ChurchService _churchService;
        private ObservableCollection<Core.Models.Church> _churches;
        private Core.Models.Church _selectedChurch;

        public ObservableCollection<Core.Models.Church> Churches
        {
            get => _churches;
            set => SetProperty(ref _churches, value);
        }

        public Core.Models.Church SelectedChurch
        {
            get => _selectedChurch;
            set => SetProperty(ref _selectedChurch, value);
        }

        public ChurchesListViewModel(ChurchService churchService)
        {
            _churchService = churchService;
            Title = "My Churches";
            Churches = new ObservableCollection<Core.Models.Church>();
        }

        /// <summary>
        /// Loads all churches from database
        /// </summary>
        public async Task LoadChurchesAsync()
        {
            try
            {
                IsLoading = true;
                var churches = await _churchService.GetAllAsync();

                Churches.Clear();
                foreach (var church in churches)
                {
                    Churches.Add(church);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error loading churches: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Deletes a church
        /// </summary>
        public async Task DeleteChurchAsync(Core.Models.Church church)
        {
            try
            {
                bool confirm = await Shell.Current.DisplayAlert(
                    "Confirm",
                    $"Delete '{church.Name}' and all its members?",
                    "Yes", "No");

                if (!confirm)
                    return;

                IsLoading = true;
                var result = await _churchService.DeleteAsync(church.Id);

                if (result.IsSuccessful)
                {
                    Churches.Remove(church);
                    await Shell.Current.DisplayAlert("Success", "Church deleted", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", result.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
 