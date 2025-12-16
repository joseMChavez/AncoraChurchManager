using SQLite;
using System;
namespace Core.Models;

public abstract class BaseEntity
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indicates if the record was synchronized with the server
    /// </summary>
    public bool IsSynchronized { get; set; } = false;

    /// <summary>
    /// Marks whether this record should be synchronized to cloud
    /// </summary>
    public bool ShouldSyncToCloud { get; set; } = true;

    /// <summary>
    /// Hash of the record to detect changes for future sync
    /// </summary>
    public string? LastChangeHash { get; set; }

    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}