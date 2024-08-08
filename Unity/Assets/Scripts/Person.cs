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
    private PersonMovement personMovement;
    private Vector2 position;
    private float internalTimer;
    private Vector2 p0, p1, p2, p3;
    private float randomMoveBeta;
    private Vector2 randomMoveMean, randomMoveStd;

    /// <summary>
    /// Initializes the person with random attributes.
    /// </summary>
    public void Initialize(float minSpeed, float maxSpeed, float randomMoveBeta, Vector2 randomMoveMean, Vector2 randomMoveStd)
    {
        speedCoefficient = Random.Range(minSpeed, maxSpeed);
        sfc = Random.value;
        wpc = Random.value;
        irc = Random.value;
        wellbeing = 0;

        position = new Vector2(Random.value, Random.value);
        internalTimer = 0;

        this.randomMoveBeta = randomMoveBeta;
        this.randomMoveMean = randomMoveMean;
        this.randomMoveStd = randomMoveStd;

        personMovement = GetComponent<PersonMovement>();
        if (personMovement == null)
        {
            Debug.LogError("PersonMovement component is missing on the Person prefab.");
            return;
        }

        (p0, p1, p2, p3) = personMovement.GetRandomG1CubicBezier(position, position, randomMoveBeta, randomMoveMean, randomMoveStd);
    }

    /// <summary> Randomly moves the person. </summary>
    public void Move()
    {
        if (personMovement == null)
        {
            Debug.LogError("Cannot move person because PersonMovement component is missing.");
            return;
        }

        position = personMovement.GetCubicBezierPoint(internalTimer, p0, p1, p2, p3);
        internalTimer += speedCoefficient;

        if (internalTimer > 1.0f)
        {
            internalTimer -= 1.0f;
            (p0, p1, p2, p3) = personMovement.GetRandomG1CubicBezier(p2, p3, randomMoveBeta, randomMoveMean, randomMoveStd);
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
}
