using SQLite;

namespace Core.Models;

public class Church:BaseEntity
{
    // <summary>
    /// Name of the church/congregation
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description or additional information
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Physical address
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// Contact phone number
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// Contact email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Church logo or photo URL (local path or base64)
    /// </summary>
    public string PhotoUrl { get; set; }

    /// <summary>
    /// Name of the lead pastor
    /// </summary>
    public string PastorName { get; set; }

    /// <summary>
    /// Founding date
    /// </summary>
    public DateTime FoundingDate { get; set; }

    /// <summary>
    /// Total member count (denormalized for fast offline queries)
    /// </summary>
    [Ignore]
    public int TotalMembers { get; set; }

   

    public override string ToString() => Name;
}