using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an individual person in the simulation.
/// </summary>
public class Person : MonoBehaviour
{
    private const float MAX_RANDOM_SPEED = 3.0e-3f;
    private const float MIN_RANDOM_SPEED = 3.0e-4f;

    [Header("Person Attributes")]
    [Tooltip("The speed coefficient of the person.")]
    public float speedCoefficient;

    [Tooltip("Selfishness coefficient - A number between 0 and 1 that determines how selfish the person is.")]
    public float sfc;

    [Tooltip("Wellbeing production coefficient - A number between 0 and 1 that determines how much wellbeing the person produces.")]
    public float wpc;

    [Tooltip("Influence reach coefficient - A number between 0 and 1 that determines how far the person's influence reaches.")]
    public float irc;

    [Tooltip("The wellbeing value of the person.")]
    public float wellbeing;

    private Vector2 position;

    /// <summary>
    /// Initializes the person with random attributes.
    /// </summary>
    public void Initialize()
    {
        speedCoefficient = Random.Range(MIN_RANDOM_SPEED, MAX_RANDOM_SPEED);
        sfc = Random.value;
        wpc = Random.value;
        irc = Random.value;
        wellbeing = 0;

        position = new Vector2(Random.value, Random.value);
    }

    /// <summary>
    /// Moves the person randomly using Perlin noise.
    /// </summary>
    /// <param name="t">The time parameter for noise generation.</param>
    public void RandomMove(float t, float noiseTimeModifier)
    {

        float id = transform.GetSiblingIndex();

        float modifiedT = t * noiseTimeModifier;

        Vector2 randomDirection = new Vector2(
            Mathf.PerlinNoise(modifiedT, id * 2.0f) * 2 - 1,
            Mathf.PerlinNoise(modifiedT, id * 2.0f + 1) * 2 - 1
        );
        position += randomDirection * speedCoefficient;
        position = new Vector2(Mathf.Clamp01(position.x), Mathf.Clamp01(position.y));
    }

    /// <summary>
    /// Moves the person by a given delta in x and y directions.
    /// </summary>
    /// <param name="delta">Change in position vector.</param>
    public void Move(Vector2 delta)
    {
        position = new Vector2(Mathf.Clamp01(position.x + delta.x), Mathf.Clamp01(position.y + delta.y));
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
        float spriteHalfWidth = spriteRenderer.bounds.size.x / 2f;
        float spriteHalfHeight = spriteRenderer.bounds.size.y / 2f;

        // Clamp normalized position within 0 to 1 range considering the sprite size
        float clampedX = Mathf.Clamp(normalizedPosition.x, 0 + spriteHalfWidth / Screen.width, 1 - spriteHalfWidth / Screen.width);
        float clampedY = Mathf.Clamp(normalizedPosition.y, 0 + spriteHalfHeight / Screen.height, 1 - spriteHalfHeight / Screen.height);

        // Convert normalized position to viewport position
        Vector3 viewportPosition = new Vector3(clampedX, clampedY, cam.nearClipPlane);

        // Convert viewport position to world position
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

        // Calculate the aspect ratio of the sprite
        float aspectRatio = spriteRenderer.sprite.bounds.size.x / spriteRenderer.sprite.bounds.size.y;

        // Calculate the scale factor needed to achieve the target height in world units
        float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;
        float targetHeightInWorldUnits = personSize / pixelsPerUnit;

        // Apply the scale factor to both x and y to maintain aspect ratio
        transform.localScale = new Vector3(targetHeightInWorldUnits * aspectRatio, targetHeightInWorldUnits, 1);
    }
}
