namespace Reloaded.WPF.Animations.Enum
{
    /// <summary>
    /// Defines the state of a <see cref="ManualAnimationController"/>.
    /// </summary>
    public enum ManualAnimationState
    {
        /// <summary>
        /// The animation has not yet been started.
        /// </summary>
        NotStarted,

        /// <summary>
        /// The animation is currently running.
        /// </summary>
        Running,

        /// <summary>
        /// The animation is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The animation has been or is about to be cancelled.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The animation has finished running.
        /// </summary>
        Complete
    }
}