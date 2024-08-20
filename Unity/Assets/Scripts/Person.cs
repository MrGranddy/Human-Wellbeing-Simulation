using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an individual person in the simulation.
/// </summary>
public class Person : MonoBehaviour
{
    private float baseSpeed;
    // Selfishness coefficient
    private float sfc;
    // Wellbeing production coefficient
    private float wpc;
    // influence reach coefficient
    private float irc;

    private float wellbeing;
    private Vector2 position;
    private float curveTime;

    private MovementCurve movementCurve;

    private float simulationHeight;
    private float simulationWidth;

    private int bezierLengthResolution = 30;

    /// <summary>
    /// Initializes the person with random attributes.
    /// </summary>
    public void Initialize(Vector2 randomWalkMean, Vector2 randomWalkStd, float simulationWidth, float simulationHeight)
    {
        baseSpeed = Random.Range(0.0f, 1.0f);
        sfc = Random.value;
        wpc = Random.value;
        irc = Random.value;
        wellbeing = 0;

        this.simulationHeight = simulationHeight;
        this.simulationWidth = simulationWidth;

        position = new Vector2(Random.Range(0, simulationWidth), Random.Range(0, simulationHeight));

        movementCurve = new MovementCurve(position, randomWalkMean, randomWalkStd, bezierLengthResolution);
        
    }

    public void UpdateSimulationSize(float simulationWidth, float simulationHeight)
    {
        this.simulationWidth = simulationWidth;
        this.simulationHeight = simulationHeight;
    }

    /// <summary> Randomly moves the person. </summary>
    public void Move(float randomWalkBeta, Vector2 randomWalkMean, Vector2 randomWalkStd, float speedCoefficient)
    {
        position = cubicBezierCurve.GetPoint(curveTime);

        if (position.y > simulationHeight)
        {
            cubicBezierCurve.ReflectCurve(Flags.HitStatus.HitTop, simulationHeight, simulationWidth);
        }
        if (position.x > simulationWidth)
        {
            cubicBezierCurve.ReflectCurve(Flags.HitStatus.HitRight, simulationHeight, simulationWidth);
        }
        if (position.y < 0)
        {
            cubicBezierCurve.ReflectCurve(Flags.HitStatus.HitBottom, simulationHeight, simulationWidth);
        }
        if (position.x < 0)
        {
            cubicBezierCurve.ReflectCurve(Flags.HitStatus.HitLeft, simulationHeight, simulationWidth);
        }

        position = cubicBezierCurve.GetPoint(curveTime);

        // Calculate dt such that the person has a constant speed in terms of simulation units / real seconds
        float speed = baseSpeed * speedCoefficient;
        float totalTimeToTravelCurve = curveLength / speed;
        float realTimeStep = Time.deltaTime;
        float dt = realTimeStep / totalTimeToTravelCurve;
        curveTime += dt;

        if (curveTime > 1.0f)
        {
            curveTime -= 1.0f;
            cubicBezierCurve = MathUtils.CubicBezierCurve.GetRandomG1CubicBezier(cubicBezierCurve, randomWalkBeta, randomWalkMean, randomWalkStd);
            curveLength = cubicBezierCurve.GetLength(bezierLengthResolution);
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
    /// Maps simulationPosition position to world coordinates.
    /// </summary>
    /// <param name="simulationPosition">The simulation position.</param>
    /// <returns>The world position.</returns>
    private Vector3 MapToWorld(Vector2 simulationPosition)
    {
        Camera cam = Camera.main;

        // Adjust normalized position to account for the sprite's size
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        Vector2 normalizedPosition = new Vector2(simulationPosition.x / simulationWidth, simulationPosition.y / simulationHeight);

        Vector3 viewportPosition = new Vector3(normalizedPosition.x, normalizedPosition.y, cam.nearClipPlane);
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
        float aspectRatio = spriteRenderer.sprite.bounds.size.x / spriteRenderer.sprite.bounds.size.y;
        float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;
        float targetHeightInWorldUnits = personSize / pixelsPerUnit;

        transform.localScale = new Vector3(targetHeightInWorldUnits * aspectRatio, targetHeightInWorldUnits, 1);
    }

    void OnDrawGizmos()
    {

        float drawCurveResolution = 0.01f;
        float drawControlPointsSize = 0.01f;

        Gizmos.color = Color.red;

        Vector2 world_p0 = MapToWorld(cubicBezierCurve.p0);
        Vector2 world_p1 = MapToWorld(cubicBezierCurve.p1);
        Vector2 world_p2 = MapToWorld(cubicBezierCurve.p2);
        Vector2 world_p3 = MapToWorld(cubicBezierCurve.p3);

        Gizmos.DrawSphere(world_p0, drawControlPointsSize);
        Gizmos.DrawSphere(world_p1, drawControlPointsSize);
        Gizmos.DrawSphere(world_p2, drawControlPointsSize);
        Gizmos.DrawSphere(world_p3, drawControlPointsSize);

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


