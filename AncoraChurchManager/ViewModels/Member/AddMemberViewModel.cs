using Core.Services;

namespace AncoraChurchManager.ViewModels.Member;

public class AddMemberViewModel : BaseViewModel
    {
        private readonly MemberService _memberService;
        private string _churchId;
        private string _fullName;
        private string _email;
        private string _phone;
        private string _address;
        private DateTime _dateOfBirth = DateTime.UtcNow.AddYears(-30);
        private string _role = "Member";
        private string _status = "Active";
        private string _notes;

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set => SetProperty(ref _dateOfBirth, value);
        }

        public string Role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }

        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public List<string> AvailableRoles => new() { "Member", "Deacon", "Pastor", "Evangelist" };
        public List<string> AvailableStatuses => new() { "Active", "Inactive", "Visitor" };

        public AddMemberViewModel(MemberService memberService)
        {
            _memberService = memberService;
            Title = "New Member";
        }

        /// <summary>
        /// Initializes with the church
        /// </summary>
        public void Initialize(string churchId)
        {
            _churchId = churchId;
        }

        /// <summary>
        /// Saves the new member
        /// </summary>
        public async Task<bool> SaveAsync()
        {
            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(FullName))
                {
                    await Shell.Current.DisplayAlert("Error", "Full name is required", "OK");
                    return false;
                }

                IsLoading = true;

                var member = new Core.Models.Member
                {
                    ChurchId = _churchId,
                    FullName = FullName.Trim(),
                    Email = Email?.Trim(),
                    Phone = Phone?.Trim(),
                    Address = Address?.Trim(),
                    DateOfBirth = DateOfBirth,
                    Role = Role,
                    Status = Status,
                    Notes = Notes?.Trim()
                };

                var result = await _memberService.CreateAsync(member);

                if (result.IsSuccessful)
                {
                    await Shell.Current.DisplayAlert("Success", "Member added successfully", "OK");
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
            FullName = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
            DateOfBirth = DateTime.UtcNow.AddYears(-30);
            Role = "Member";
            Status = "Active";
            Notes = string.Empty;
        }
    }