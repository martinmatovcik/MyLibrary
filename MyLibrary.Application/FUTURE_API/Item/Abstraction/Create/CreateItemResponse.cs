namespace MyLibrary.Application.FUTURE_API.Item.Abstraction.Create;

/// <summary>
/// Abstract base response for item creation operations.
/// Contains the essential properties returned after successfully creating an item.
/// </summary>
/// <param name="Id">The unique identifier of the created item.</param>
/// <param name="Name">The name of the created item.</param>
/// <param name="Description">Optional description of the created item.</param>
/// <param name="OwnerId">The identifier of the user who owns this item.</param>
public abstract record CreateItemResponse(Guid Id, string Name, string? Description, Guid OwnerId);