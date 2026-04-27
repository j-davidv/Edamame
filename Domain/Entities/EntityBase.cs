using LiteDB;

namespace Edamam.Domain.Entities;

/// Base class for all domain entities 
public abstract class EntityBase
{
    [BsonId]
    public ObjectId Id { get; set; } = ObjectId.NewObjectId();

    private DateTime _createdDate = DateTime.UtcNow;
    public DateTime CreatedDate 
    { 
        get => _createdDate;
        private set => _createdDate = value;
    }

    private DateTime _modifiedDate = DateTime.UtcNow;
    public DateTime ModifiedDate 
    { 
        get => _modifiedDate;
        set
        {
            if (value < _createdDate)
                throw new ArgumentException("ModifiedDate cannot be earlier than CreatedDate", nameof(value));
            _modifiedDate = value;
        }
    }

    /// abstract method for entity state validation
    public abstract void ValidateState();

    /// Abstract method to get a display name for the entity
    public abstract string GetDisplayName();

    /// update the modification timestamp
    public virtual void MarkAsModified()
    {
        ModifiedDate = DateTime.UtcNow;
    }
}
