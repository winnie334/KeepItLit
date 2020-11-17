using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class to contain helper methods which can be useful throughout the entire project.
public static class HelperFunctions {
    
    // Gets a value from a dictionary, or if it doesn't exist, a given default value
    public static TValue GetOrDef<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default) {
        TValue value;
        return dictionary.TryGetValue(key, out value) ? value : defaultValue;
    }
}
