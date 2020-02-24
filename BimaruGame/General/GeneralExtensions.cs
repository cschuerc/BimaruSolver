namespace Bimaru
{
    /// <summary>
    /// General extension class
    /// </summary>
    public static class GeneralExtensions
    {
        /// <summary>
        /// Initializes all values of an array to the given default value
        /// </summary>
        /// <typeparam name="T"> Type of array elements </typeparam>
        /// <param name="array"> Array whose elements shall be initialized </param>
        /// <param name="value"> Value to which all elements are initialized </param>
        /// <returns> Itself but initialized </returns>
        public static T[] InitValues<T>(this T[] array, T value) where T: struct
        {
            for (int index = 0; index < array.Length; index++)
            {
                array[index] = value;
            }

            return array;
        }
    }
}
