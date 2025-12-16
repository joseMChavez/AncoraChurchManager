namespace Core.Models;

public class Member : BaseEntity
{
    /// <summary>
    /// ID of the church this member belongs to
    /// </summary>
    public string ChurchId { get; set; }

    /// <summary>
    /// Member's full name
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Member's email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Contact phone number
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// Date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Member's address
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Member role: Member, Deacon, Pastor, Evangelist, etc.
    /// </summary>
    public string Role { get; set; } = "Member";

    /// <summary>
    /// Member status: Active, Inactive, Visitor
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Date when joined the church
    /// </summary>
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Member profile photo
    /// </summary>
    public string PhotoUrl { get; set; }

    /// <summary>
    /// Personal notes about the member
    /// </summary>
    public string Notes { get; set; }

    public Member()
    {
    }

    public Member(string churchId, string fullName)
    {
        ChurchId = churchId;
        FullName = fullName;
    }

    public override string ToString() => FullName;
}