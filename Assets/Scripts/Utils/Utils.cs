using UnityEngine;

namespace Utils
{
    public static class Utils
    {
        public static Vector2 GetRandomVector2(int maxXValue, int maxYValue)
        {
            int x = Random.Range(0, maxXValue);
            int y = Random.Range(0, maxYValue);

            Vector2 vector = new Vector2(x, y);
            return vector;
        }
    }
}

