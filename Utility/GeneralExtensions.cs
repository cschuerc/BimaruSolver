namespace Utility
{
    public static class GeneralExtensions
    {
        /// <summary>
        /// Initializes all values of the given array to the given default value
        /// </summary>
        /// <returns> Itself but initialized </returns>
        public static T[] InitValues<T>(this T[] array, T value) where T : struct
        {
            for (int index = 0; index < array.Length; index++)
            {
                array[index] = value;
            }

            return array;
        }
    }
}
