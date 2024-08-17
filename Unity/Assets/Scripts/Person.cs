using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an individual person in the simulation.
/// </summary>
public class Person : MonoBehaviour
{
    private float speedCoefficient;
    // Selfishness coefficient
    private float sfc;
    // Wellbeing production coefficient
    private float wpc;
    // influence reach coefficient
    private float irc;

    private float wellbeing;
    private Vector2 position;
    private float internalTimer;

    private MathUtils.CubicBezierCurve cubicBezierCurve;

    /// <summary>
    /// Initializes the person with random attributes.
    /// </summary>
    public void Initialize(float minSpeed, float maxSpeed, Vector2 randomMoveMean, Vector2 randomMoveStd)
    {
        speedCoefficient = Random.Range(minSpeed, maxSpeed);
        sfc = Random.value;
        wpc = Random.value;
        irc = Random.value;
        wellbeing = 0;

        position = new Vector2(Random.value, Random.value);
        internalTimer = 0;

        Vector2 p0 = position;
        Vector2 p1 = p0 + MathUtils.Statistics.GetRandomNormalVector2(randomMoveMean, randomMoveStd);
        Vector2 p2 = p1 + MathUtils.Statistics.GetRandomNormalVector2(randomMoveMean, randomMoveStd);
        Vector2 p3 = p2 + MathUtils.Statistics.GetRandomNormalVector2(randomMoveMean, randomMoveStd);

        cubicBezierCurve = new MathUtils.CubicBezierCurve(p0, p1, p2, p3);
    }

    /// <summary> Randomly moves the person. </summary>
    public void Move(float randomMoveBeta, Vector2 randomMoveMean, Vector2 randomMoveStd)
    {
        position = cubicBezierCurve.GetPoint(internalTimer);
        internalTimer += speedCoefficient;

        if (internalTimer > 1.0f)
        {
            internalTimer -= 1.0f;
            cubicBezierCurve = MathUtils.CubicBezierCurve.GetRandomG1CubicBezier(cubicBezierCurve, randomMoveBeta, randomMoveMean, randomMoveStd);
        }
    }

    /// <summary>
    /// Updates the transform position of the person.
    /// </summary>
    public void UpdateTransformPosition()
    {
        transform.position = MapToWorld(position);
    }

    /// <summary>
    /// Maps normalized position to world coordinates.
    /// </summary>
    /// <param name="normalizedPosition">The normalized position.</param>
    /// <returns>The world position.</returns>
    private Vector3 MapToWorld(Vector2 normalizedPosition)
    {
        Camera cam = Camera.main;

        // Adjust normalized position to account for the sprite's size
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is missing.");
            return Vector3.zero;
        }

        float spriteHalfWidth = spriteRenderer.bounds.size.x / 2f;
        float spriteHalfHeight = spriteRenderer.bounds.size.y / 2f;

        float clampedX = Mathf.Clamp(normalizedPosition.x, 0 + spriteHalfWidth / Screen.width, 1 - spriteHalfWidth / Screen.width);
        float clampedY = Mathf.Clamp(normalizedPosition.y, 0 + spriteHalfHeight / Screen.height, 1 - spriteHalfHeight / Screen.height);

        Vector3 viewportPosition = new Vector3(clampedX, clampedY, cam.nearClipPlane);
        Vector3 worldPosition = cam.ViewportToWorldPoint(viewportPosition);

        return worldPosition;
    }

    /// <summary>
    /// Sets the size of the sprite (height) and adjusts the width accordingly.
    /// </summary>
    /// <param name="personSize">The size of the person. (height in pixels)</param>
    public void SetPersonSize(float personSize)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is missing.");
            return;
        }

        float aspectRatio = spriteRenderer.sprite.bounds.size.x / spriteRenderer.sprite.bounds.size.y;
        float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;
        float targetHeightInWorldUnits = personSize / pixelsPerUnit;

        transform.localScale = new Vector3(targetHeightInWorldUnits * aspectRatio, targetHeightInWorldUnits, 1);
    }

    void OnDrawGizmos()
    {

        float drawCurveResolution = 0.01f;

        Gizmos.color = Color.red;

        Vector2 world_p0 = MapToWorld(cubicBezierCurve.p0);
        Vector2 world_p1 = MapToWorld(cubicBezierCurve.p1);
        Vector2 world_p2 = MapToWorld(cubicBezierCurve.p2);
        Vector2 world_p3 = MapToWorld(cubicBezierCurve.p3);

        Gizmos.DrawSphere(world_p0, 0.01f);
        Gizmos.DrawSphere(world_p1, 0.01f);
        Gizmos.DrawSphere(world_p2, 0.01f);
        Gizmos.DrawSphere(world_p3, 0.01f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(world_p0, world_p1);
        Gizmos.DrawLine(world_p1, world_p2);
        Gizmos.DrawLine(world_p2, world_p3);

        Gizmos.color = Color.blue;
        Vector2 lastPoint = world_p0;
        for (float t = drawCurveResolution; t <= 1.0f; t += drawCurveResolution)
        {
            Vector2 nextPoint = cubicBezierCurve.GetPoint(t);
            nextPoint = MapToWorld(nextPoint);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }

    }
}


