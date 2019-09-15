using System;

namespace GrappyServerless
{
    static class Extensions
    {
        static internal bool ToProcessorBool(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return string.Compare(value, bool.TrueString, StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(value, "yes", StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(value, "t", StringComparison.OrdinalIgnoreCase) == 0
                || string.Compare(value, "y", StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
