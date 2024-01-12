using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suzaku.Shared
{
    public static class StringExtensions
    {
        /// <summary>
        /// Normalizes the displayed name to use in a channel name
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToNormalizedChannelName(this string value)
        {
            var s = new Slugify.SlugHelper();
            return s.GenerateSlug(value);
        }

        /// <summary>
        /// Default version of a displayname is to make first uppercase and the rest lowercase
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDisplayName(this string value)
        {
            return value.ToLower().FirstCharToUpper();
        }

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
