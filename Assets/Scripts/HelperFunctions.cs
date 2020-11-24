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

    public static float p5Map(float n, float start1, float stop1, float start2, float stop2) {
        return ((n-start1)/(stop1-start1))*(stop2-start2)+start2;
    }
}
