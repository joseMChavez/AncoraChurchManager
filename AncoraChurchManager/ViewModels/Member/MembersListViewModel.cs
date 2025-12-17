using System.Collections.ObjectModel;
using Core.Models;
using Core.Services;

namespace AncoraChurchManager.ViewModels.Member;

public class MembersListViewModel : BaseViewModel
{
    private readonly MemberService _memberService;
    private readonly ChurchService _churchService;
    private string _churchId;
    private Core.Models.Church _church;
    private ObservableCollection<Core.Models.Member> _members;
    private string _statusFilter = "All";
    private string _searchTerm;

    public ObservableCollection<Core.Models.Member> Members
    {
        get => _members;
        set => SetProperty(ref _members, value);
    }

    public Core.Models.Member SelectedMember
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Core.Models.Church Church
    {
        get => _church;
        set => SetProperty(ref _church, value);
    }

    public string StatusFilter
    {
        get => _statusFilter;
        set
        {
            SetProperty(ref _statusFilter, value);
            // Reload with new filter
            _ = LoadMembersAsync();
        }
    }

    public string SearchTerm
    {
        get => _searchTerm;
        set => SetProperty(ref _searchTerm, value);
    }

    public MembersListViewModel(MemberService memberService, ChurchService churchService)
    {
        _memberService = memberService;
        _churchService = churchService;
        Members = new ObservableCollection<Core.Models.Member>();
    }

    /// <summary>
    /// Initializes with the selected church
    /// </summary>
    public async Task InitializeAsync(string churchId)
    {
        try
        {
            IsLoading = true;
            _churchId = churchId;

            // Get church data
            var (church, _) = await _churchService.GetDetailsAsync(churchId);
            Church = church;
            Title = $"Members of {church?.Name}";

            await LoadMembersAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error initializing: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Loads church members with filters
    /// </summary>
    public async Task LoadMembersAsync()
    {
        try
        {
            IsLoading = true;

            List<Core.Models.Member> members;

            if (StatusFilter == "All")
            {
                members = await _memberService.GetMembersByChurchAsync(_churchId);
            }
            else
            {
                members = await _memberService.GetMembersByStatusAsync(_churchId, StatusFilter);
            }

            // Apply search if exists
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                members = members
                    .Where(m => m.FullName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            Members.Clear();
            foreach (var member in members)
            {
                Members.Add(member);
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

    /// <summary>
    /// Searches members by name
    /// </summary>
    public async Task SearchAsync()
    {
        await LoadMembersAsync();
    }

    /// <summary>
    /// Deletes a member
    /// </summary>
    public async Task DeleteMemberAsync(Core.Models.Member member)
    {
        try
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Confirm",
                $"Delete '{member.FullName}'?",
                "Yes", "No");

            if (!confirm)
                return;

            IsLoading = true;
            var result = await _memberService.DeleteAsync(member.Id);

            if (result.IsSuccessful)
            {
                Members.Remove(member);
                await Shell.Current.DisplayAlert("Success", "Member deleted", "OK");
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