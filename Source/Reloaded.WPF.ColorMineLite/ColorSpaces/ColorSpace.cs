
#pragma warning disable 1591


namespace Reloaded.WPF.ColorMineLite.ColorSpaces
{
    /// <summary>
    /// Defines the public methods for all color spaces
    /// </summary>
    public interface IColorSpace
    {
        /// <summary>
        /// Initialize settings from an Rgb object
        /// </summary>
        /// <param name="color"></param>
        void Initialize(IRgb color);

        /// <summary>
        /// Convert any IColorSpace to any other IColorSpace.
        /// </summary>
        /// <typeparam name="T">IColorSpace type to convert to</typeparam>
        /// <returns></returns>
        T To<T>() where T : IColorSpace, new();
    }

    /// <summary>
    /// Abstract ColorSpace class, defines the To method that converts between any IColorSpace.
    /// </summary>
    public abstract class ColorSpace : IColorSpace
    {
        public abstract void Initialize(IRgb color);
        public abstract IRgb ToRgb();

        /// <summary>
        /// Convert any IColorSpace to any other IColorSpace
        /// </summary>
        /// <typeparam name="T">Must implement IColorSpace, new()</typeparam>
        public T To<T>() where T : IColorSpace, new()
        {
            if (typeof(T) == GetType())
            {
                return (T)MemberwiseClone();
            }

            var newColorSpace = new T();
            newColorSpace.Initialize(ToRgb());

            return newColorSpace;
        }
    }
}