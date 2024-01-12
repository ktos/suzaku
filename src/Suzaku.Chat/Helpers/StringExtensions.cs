namespace Suzaku.Chat.Helpers
{
    public static class StringExtensions
    {
        /// <summary>
        /// Changes the first letter of a string into an uppercase
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                ""
                    => throw new ArgumentException(
                        $"{nameof(input)} cannot be empty",
                        nameof(input)
                    ),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };
    }
}
