namespace VMManager.Models
{
    /// <summary>
    /// Specifies how frequently a scheduled virtual machine maintenance job should recur.
    /// </summary>
    public enum RecurrenceType
    {
        /// <summary>
        /// An undefined recurrence type. Used as a fallback or uninitialized state.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Executes the job only once and does not repeat.
        /// </summary>
        OneTime = 1,

        /// <summary>
        /// Repeats the job every 24 hours.
        /// </summary>
        Daily = 2,

        /// <summary>
        /// Repeats the job once a week.
        /// </summary>
        Weekly = 3,

        /// <summary>
        /// Repeats the job monthly on the same calendar day.
        /// </summary>
        Monthly = 4
    }
}
