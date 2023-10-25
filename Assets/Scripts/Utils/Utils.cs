using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public class Area
    {
        /**
         * Get a random coordinate in a cube
         * Define by its center and its scale
         */
        public static Vector3 GetRandomCoord(Vector3 center, Vector3 scale)
        {
            Bounds b = new Bounds(center, scale);
            return new Vector3(
                UnityEngine.Random.Range(b.min.x, b.max.x),
                UnityEngine.Random.Range(b.min.y, b.max.y),
                UnityEngine.Random.Range(b.min.z, b.max.z)
            );
        }
    }

    public class WaitAction
    {
        public static IEnumerator ForSeconds(float seconds, Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback();
        }
    }
}
