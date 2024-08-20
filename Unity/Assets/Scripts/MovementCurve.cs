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
}
