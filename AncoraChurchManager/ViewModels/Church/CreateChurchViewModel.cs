using Core.Services;

namespace AncoraChurchManager.ViewModels.Church;

public class CreateChurchViewModel : BaseViewModel
    {
        private readonly ChurchService _churchService;
        private string _name;
        private string _description;
        private string _address;
        private string _phone;
        private string _email;
        private string _pastorName;
        private DateTime _foundingDate = DateTime.UtcNow;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string PastorName
        {
            get => _pastorName;
            set => SetProperty(ref _pastorName, value);
        }

        public DateTime FoundingDate
        {
            get => _foundingDate;
            set => SetProperty(ref _foundingDate, value);
        }

        public CreateChurchViewModel(ChurchService churchService)
        {
            _churchService = churchService;
            Title = "New Church";
        }

        /// <summary>
        /// Saves the new church
        /// </summary>
        public async Task<bool> SaveAsync()
        {
            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(Name))
                {
                    await Shell.Current.DisplayAlert("Error", "Name is required", "OK");
                    return false;
                }

                IsLoading = true;

                var church = new Core.Models.Church
                {
                    Name = Name.Trim(),
                    Description = Description?.Trim(),
                    Address = Address?.Trim(),
                    Phone = Phone?.Trim(),
                    Email = Email?.Trim(),
                    PastorName = PastorName?.Trim(),
                    FoundingDate = FoundingDate
                };

                var result = await _churchService.CreateAsync(church);

                if (result.IsSuccessful)
                {
                    await Shell.Current.DisplayAlert("Success", "Church created successfully", "OK");
                    ClearForm();
                    return true;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", result.Message, "OK");
                    return false;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Clears the form
        /// </summary>
        private void ClearForm()
        {
            Name = string.Empty;
            Description = string.Empty;
            Address = string.Empty;
            Phone = string.Empty;
            Email = string.Empty;
            PastorName = string.Empty;
            FoundingDate = DateTime.UtcNow;
        }
    }