using System.Collections.Generic;
using UnityEngine;

namespace MathUtils
{
    public class CubicBezierCurve
    {
        public Vector2 p0, p1, p2, p3;

        public CubicBezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        /// <summary>
        /// Evaluates a cubic Bezier curve at a given parameter t.
        /// </summary>
        /// <param name="t"> The parameter at which to evaluate the curve. t is in [0, 1]. </param>
        /// <param name="p0"> The first control point of the curve. </param>
        /// <param name="p1"> The second control point of the curve. </param>
        /// <param name="p2"> The third control point of the curve. </param>
        /// <param name="p3"> The fourth control point of the curve. </param>
        /// <returns> The point on the curve at parameter t. </returns>
        public Vector2 GetPoint(float t)
        {
            // First level of interpolation
            Vector2 p01 = Vector2.Lerp(p0, p1, t);
            Vector2 p12 = Vector2.Lerp(p1, p2, t);
            Vector2 p23 = Vector2.Lerp(p2, p3, t);

            // Second level of interpolation
            Vector2 p012 = Vector2.Lerp(p01, p12, t);
            Vector2 p123 = Vector2.Lerp(p12, p23, t);

            // Third level of interpolation
            Vector2 p0123 = Vector2.Lerp(p012, p123, t);

            return p0123;
        }
        public static CubicBezierCurve GetRandomG1CubicBezier(MathUtils.CubicBezierCurve oldCurve, float beta, Vector2 mu, Vector2 std)
        {
            Vector2 p0 = oldCurve.p3;
            Vector2 p1 = oldCurve.p3 + (oldCurve.p3 - oldCurve.p2) * beta;
            Vector2 p2 = p1 + Statistics.GetRandomNormalVector2(mu, std);
            Vector2 p3 = p2 + Statistics.GetRandomNormalVector2(mu, std);

            CubicBezierCurve curve = new CubicBezierCurve(p0, p1, p2, p3);

            return curve;

        }
    }

    public static class Statistics
    {
        /// <summary>
        /// Returns a random vector from a normal distribution with the given mean and covariance matrix.
        /// </summary>
        /// <param name="mean"> Mean vector of the 2D multivariate normal distribution. </param>
        /// <param name="std"> Standard deviation vector of the 2D multivariate normal distribution. </param></param>
        /// <returns> A random vector from the given normal distribution.</returns>
        public static Vector2 GetRandomNormalVector2(Vector2 mean, Vector2 std)
        {
            // Generate two random numbers
            float r1 = UnityEngine.Random.Range(0.0f, 1.0f);
            float r2 = UnityEngine.Random.Range(0.0f, 1.0f);

            // Calculate the standard normal random variables (Box-Muller transform)
            float z1 = Mathf.Sqrt(-2.0f * Mathf.Log(r1)) * Mathf.Cos(2.0f * Mathf.PI * r2);
            float z2 = Mathf.Sqrt(-2.0f * Mathf.Log(r1)) * Mathf.Sin(2.0f * Mathf.PI * r2);

            // Create the random vector
            Vector2 randomVector = new Vector2(z1, z2);

            // Multiply the random vector by the std values
            randomVector.x *= std.x;
            randomVector.y *= std.y;

            // Add the mean values to the random vector
            randomVector.x += mean.x;
            randomVector.y += mean.y;

            return randomVector;

        }
    }

}