using System.Collections.Generic;
using System.Linq;

namespace GregBot.Domain.Extensions;

public static class EnumerableExtensions
{
    public static bool ContainsSequentially<T>(this IEnumerable<T> list, IEnumerable<T> sublist)
    {
        var needle = sublist.ToArray();
        var haystack = list.ToArray();
        
        // Essentially a sliding window
        for (var i = 0; i <= haystack.Length - needle.Length; i++)
        {
            var match = true;
            for (var j = 0; j < needle.Length; j++)
            {
                if (haystack[i + j]?.Equals(needle[j]) ?? needle[j] is null) continue;
                
                match = false;
                break;
            }
            
            if (match) return true;
        }

        return false;
    }
}