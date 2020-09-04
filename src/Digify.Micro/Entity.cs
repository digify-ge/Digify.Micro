using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Digify.Micro
{
    /// <summary>
    /// Represents a type that is distinguishable by a unique ID.
    /// </summary>
    /// <remarks>
    /// Two entities that has the same ID are to be considered equal regardless of the state of their properties.
    /// </remarks>
    public abstract class Entity<TEntityId> : IEntity<TEntityId>, IEquatable<Entity<TEntityId>> where TEntityId : IComparable
    {
        #region Properties

        /// <summary>
        /// Unique ID.
        /// </summary>
        public abstract TEntityId Id { get; protected set; }

        /// <summary>
        /// Date when entitity was created. 
        /// This will default to <see cref="DateTimeOffset.UtcNow"/> if no value has been provided in constructor.
        /// </summary>
        [JsonProperty]
        public DateTimeOffset Created { get; protected set; }

        /// <summary>
        /// Date when entity was last updated. 
        /// This will default to <see cref="DateTimeOffset.UtcNow"/> if no value has been provided in constructor.
        /// </summary>
        [JsonProperty]
        public DateTimeOffset Updated { get; protected set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// For Deserialization
        /// </summary>
        public Entity()
        {
            Created = DateTimeOffset.UtcNow;
            Updated = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// This will set <see cref="Created"/> and <see cref="Updated"/> properties to <see cref="DateTimeOffset.UtcNow"/>.
        /// </remarks>
        /// <param name="entityId">ID of entity.</param>
        public Entity(TEntityId entityId)
        {
            Id = entityId;
            Created = DateTimeOffset.UtcNow;
            Updated = DateTimeOffset.UtcNow;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Check if entity has the same identity as this entity instance.
        /// </summary>
        /// <param name="other">Entity.</param>
        /// <returns>True if entities have the same identity. Otherwise, false.</returns>
        public bool Equals(Entity<TEntityId> other)
        {
            if (other == null || GetType() != other.GetType()) return false;
            return Equals((object)other);
        }

        public override bool Equals(object obj)
        {
            var otherEntity = obj as Entity<TEntityId>;

            if (ReferenceEquals(this, otherEntity)) return true;
            if (otherEntity is null) return false;
            return Id.Equals(otherEntity.Id);

        }

        public override int GetHashCode()
        {
            unchecked { return GetType().GetHashCode() * 230 + Id.GetHashCode(); };
        }

        public static bool operator ==(Entity<TEntityId> first, Entity<TEntityId> second)
        {
            if (first is null && second is null) return true;
            if (first is null || second is null) return false;

            return first.Equals(second);
        }

        public static bool operator !=(Entity<TEntityId> first, Entity<TEntityId> second)
        {
            return !(first == second);
        }

        public override string ToString() => $"{GetType().Name} Id=>{Id}";

        #endregion Methods
    }


    /// <summary>
    /// Represents a type that is distinguishable by a unique ID.
    /// </summary>
    /// <remarks>
    /// Two entities that has the same ID are to be considered equal regardless of the state of their properties.
    /// </remarks>
    public interface IEntity<TEntityId> where TEntityId : IComparable
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        TEntityId Id { get; }

        /// <summary>
        /// Date when entity was created.
        /// </summary>
        DateTimeOffset Created { get; }

        /// <summary>
        /// Date when entity was last updated.
        /// </summary>
        DateTimeOffset Updated { get; }
    }
}
