namespace InterviewApp.Data.Base
{
    public interface IEntity
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        object Id { get; set; }

        /// <summary>
        /// Gets a value indicating whether is new.
        /// </summary>
        bool IsNew { get; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        DateTime CreateDate { get; set; }
    }
}
