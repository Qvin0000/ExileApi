using System.Collections.Generic;
using System.Linq;

namespace Shared.Helpers
{
    public static class DictionaryExtensions
    {
        // Taken from: https://stackoverflow.com/a/2679857
        // Works in C#3/VS2008:
        // Returns a new dictionary of this ... others merged leftward.
        // Keeps the type of 'this', which must be default-instantiable.
        // Example: 
        //   result = map.MergeLeft(other1, other2, ...)
        public static T MergeLeft<T, TK, TV>(this T me, params IDictionary<TK, TV>[] others) where T : IDictionary<TK, TV>, new() {
            var newMap = new T();
            foreach (var src in new List<IDictionary<TK, TV>> {me}.Concat(others))
                // ^-- echk. Not quite there type-system.
            foreach (var p in src)
                newMap[p.Key] = p.Value;

            return newMap;
        }
    }
}