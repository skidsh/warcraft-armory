namespace WarcraftArmory.Domain.Interfaces;

/// <summary>
/// Base interface for all domain entities.
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier</typeparam>
public interface IEntity<TId> where TId : IEquatable<TId>
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    TId Id { get; }
}
