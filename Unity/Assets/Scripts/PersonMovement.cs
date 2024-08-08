using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonMovement : MonoBehaviour
{

    /// <summary>
    /// Returns a random vector from a normal distribution with the given mean and covariance matrix.
    /// </summary>
    /// <param name="mean"> Mean vector of the 2D multivariate normal distribution. </param>
    /// <param name="sigma"> Standard deviation vector of the 2D multivariate normal distribution. </param></param>
    /// <returns> A random vector from the given normal distribution.</returns>
    private Vector2 GetRandomNormalVector2(Vector2 mean, Vector2 sigma)
    {
        // Generate two random numbers
        float r1 = UnityEngine.Random.Range(0.0f, 1.0f);
        float r2 = UnityEngine.Random.Range(0.0f, 1.0f);

        // Calculate the standard normal random variables (Box-Muller transform)
        float z1 = Mathf.Sqrt(-2.0f * Mathf.Log(r1)) * Mathf.Cos(2.0f * Mathf.PI * r2);
        float z2 = Mathf.Sqrt(-2.0f * Mathf.Log(r1)) * Mathf.Sin(2.0f * Mathf.PI * r2);

        // Create the random vector
        Vector2 randomVector = new Vector2(z1, z2);

        // Multiply the random vector by the sigma values
        randomVector.x *= sigma.x;
        randomVector.y *= sigma.y;

        // Add the mean values to the random vector
        randomVector.x += mean.x;
        randomVector.y += mean.y;

        return randomVector;

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
    private Vector2 GetCubicBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {

        // First level of interpolation
        Vector2 p01 = (1 - t) * p0 + t * p1;
        Vector2 p12 = (1 - t) * p1 + t * p2;
        Vector2 p23 = (1 - t) * p2 + t * p3;

        // Second level of interpolation
        Vector2 p012 = (1 - t) * p01 + t * p12;
        Vector2 p123 = (1 - t) * p12 + t * p23;

        // Third level of interpolation
        Vector2 p0123 = (1 - t) * p012 + t * p123;

        return p0123;
    }

    /// <summary>
    /// Creates a random cubic Bezier curve which is the next part of a G1 continuous Composite Bezier curve.
    /// </summary>
    /// <param name="old_p2"> The third control point of the previous curve. </param>
    /// <param name="old_p3"> The fourth control point of the previous curve. </param>
    /// <param name="beta"> Strictness of following velocity continuity. If beta is 1, the curves are C1 continuous. </param>
    /// <param name="mu"> Mean vector of the 2D multivariate normal distribution. (Used to generate random control points) </param>
    /// <param name="sigma"> Standard deviation vector of the 2D multivariate normal distribution. (Used to generate random control points) </param>
    /// <returns> The control points of the random cubic Bezier curve. </returns>
    private (Vector2, Vector2, Vector2, Vector2) GetRandomG1CubicBezier(Vector2 old_p2, Vector2 old_p3, float beta, Vector2 mu, Vector2 sigma)
    {
        Vector2 p0 = old_p3 + (old_p3 - old_p2) * beta;
        Vector2 p1 = p0 + GetRandomNormalVector2(mu, sigma);
        Vector2 p2 = p1 + GetRandomNormalVector2(mu, sigma);
        Vector2 p3 = p2 + GetRandomNormalVector2(mu, sigma);

        return (p0, p1, p2, p3);
    }

}