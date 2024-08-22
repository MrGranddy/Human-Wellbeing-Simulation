using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUtils;

public class MovementCurve
{
    private CubicBezierCurve curve;
    private float curveTime;
    private int nSegments;
    private List<Vector2> segments;

    public MovementCurve(Vector2 position, Vector2 randomWalkMean, Vector2 randomWalkStd, int nSegments)
    {
        Vector2 p0 = position;
        Vector2 p1 = p0 + Statistics.GetRandomNormalVector2(randomWalkMean, randomWalkStd);
        Vector2 p2 = p1 + Statistics.GetRandomNormalVector2(randomWalkMean, randomWalkStd);
        Vector2 p3 = p2 + Statistics.GetRandomNormalVector2(randomWalkMean, randomWalkStd);

        curve = new CubicBezierCurve(p0, p1, p2, p3);
        curveTime = 0.0f;
        this.nSegments = nSegments;
        segments = new List<Vector2>();
        for (int i = 0; i <= nSegments; i++)
        {
            segments.Add(curve.GetPoint((float)i / nSegments));
        }
    }

    public void ReflectCurve(Flags.HitStatus side, float height, float width)
    {
        if (side == Flags.HitStatus.HitTop) // Top side
        {
            curve.p0.y = 2 * height - curve.p0.y;
            curve.p1.y = 2 * height - curve.p1.y;
            curve.p2.y = 2 * height - curve.p2.y;
            curve.p3.y = 2 * height - curve.p3.y;
        }
        else if (side == Flags.HitStatus.HitRight) // Right side
        {
            curve.p0.x = 2 * width - curve.p0.x;
            curve.p1.x = 2 * width - curve.p1.x;
            curve.p2.x = 2 * width - curve.p2.x;
            curve.p3.x = 2 * width - curve.p3.x;
        }
        else if (side == Flags.HitStatus.HitBottom) // Bottom side
        {
            curve.p0.y = -curve.p0.y;
            curve.p1.y = -curve.p1.y;
            curve.p2.y = -curve.p2.y;
            curve.p3.y = -curve.p3.y;
        }
        else if (side == Flags.HitStatus.HitLeft) // Left side
        {
            curve.p0.x = -curve.p0.x;
            curve.p1.x = -curve.p1.x;
            curve.p2.x = -curve.p2.x;
            curve.p3.x = -curve.p3.x;
        }
    }
    public Vector2 GetPoint()
    {
        return curve.GetPoint(curveTime);
    }
    public void UpdateCurveTime(float speed)
    {
        curveTime += speed;
        if (curveTime > 1.0f)
        {
            curveTime = 0.0f;
        }
    }

}
