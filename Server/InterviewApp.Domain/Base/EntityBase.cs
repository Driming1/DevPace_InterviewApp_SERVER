using InterviewApp.Data.Base;

namespace InterviewApp.Domain.Base
{
    public abstract class Entity<TId> : IEntity
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public virtual TId Id { get; set; }

        /// <summary>
        /// Gets a value indicating whether is new.
        /// </summary>
        public abstract bool IsNew { get; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        public virtual DateTime CreateDate { get; set; }

        object IEntity.Id
        {
            get => Id;
            set
            {
                if (value is TId val)
                {
                    Id = val;
                }
            }
        }
    }

    public class EntityBase : Entity<long?>
    {
        public override bool IsNew => !Id.HasValue;
    }
}