using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a collection of people in the simulation.
/// </summary>
public class People : MonoBehaviour
{
    [Header("Simulation Settings")]
    [Tooltip("The number of people in the simulation.")]
    [Range(1, 1000)]
    public int numPeople = 50;

    [Tooltip("The time modifier for Perlin noise.")]
    [Range(0.0f, 10.0f)]
    public float tModifier = 1.0f;

    [Tooltip("The id modifier for Perlin noise.")]
    [Range(0.0f, 10.0f)]
    public float idModifier = 1.0f;

    [Tooltip("The time offset for Perlin noise.")]
    [Range(0.0f, 100.0f)]
    public float tOffset = 0.0f;

    [Tooltip("The id offset for Perlin noise.")]
    [Range(0.0f, 100.0f)]
    public float idOffset = 0.0f;

    [Tooltip("The time limit for normalization of the time parameter.")]
    [Range(0.0f, 100.0f)]
    public float tLimit = 10.0f;

    [Tooltip("The attractiveness factor for friends. Determines how much friends attract each other (position-wise).")]
    public float friendAttractiveness = 0.0f;

    [Tooltip("The prefab used to represent a person.")]
    public GameObject personPrefab;

    [Tooltip("The size of each sprite (height).")]
    [Range(0.0f, 200.0f)]
    public float personSize = 15.0f;

    private List<Person> people = new List<Person>();
    private int previousNumPeople;

    /// <summary>
    /// Initializes the people in the simulation.
    /// </summary>
    private void Start()
    {
        InitializePeople();
        previousNumPeople = numPeople;
    }

    /// <summary>
    /// Updates the positions and sizes of the people every frame.
    /// </summary>
    private void Update()
    {
        if (numPeople != previousNumPeople)
        {
            AdjustNumberOfPeople();
            previousNumPeople = numPeople;
        }
        RandomMove();
        UpdateTransformPositions();
        UpdateSizes();

    }

    /// <summary>
    /// Initializes a collection of people with random attributes and positions.
    /// </summary>
    private void InitializePeople()
    {
        for (int i = 0; i < numPeople; i++)
        {
            people.Add(CreatePerson());
        }
    }

    /// <summary>
    /// Moves the people randomly using Perlin noise.
    /// </summary>
    /// <param name="t">The time parameter for noise generation.</param>
    private void RandomMove()
    {
        float t = Time.time;
        for(int i = 0; i < people.Count; i++)
        {
            people[i].RandomMove(t, i, tLimit, numPeople, tModifier, idModifier, tOffset, idOffset);
        }
    }

    /// <summary>
    /// Updates the sizes of the people according to the specified person size.
    /// </summary>
    private void UpdateSizes()
    {
        foreach (Person person in people)
        {
            person.SetPersonSize(personSize);
        }
    }

    /// <summary>
    /// Updates the transform positions of the people.
    /// </summary>
    private void UpdateTransformPositions()
    {
        foreach (Person person in people)
        {
            person.UpdateTransformPosition();
        }
    }

    /// <summary>
    /// Adjusts the number of people in the simulation.
    /// </summary>
    private void AdjustNumberOfPeople()
    {
        if (numPeople > people.Count)
        {
            // Add new people
            for (int i = people.Count; i < numPeople; i++)
            {
                people.Add(CreatePerson());
            }
        }
        else if (numPeople < people.Count)
        {
            // Remove excess people
            for (int i = numPeople; i < people.Count; i++)
            {
                Destroy(people[i].gameObject);
            }
            people.RemoveRange(numPeople, people.Count - numPeople);
        }
    }

    /// <summary>
    /// Creates a new person.
    /// </summary>
    /// <returns>The created Person instance.</returns>
    private Person CreatePerson()
    {
        GameObject personObject = Instantiate(personPrefab, Vector2.zero, Quaternion.identity);
        Person person = personObject.GetComponent<Person>();
        person.Initialize();
        person.SetPersonSize(personSize);
        person.UpdateTransformPosition();
        return person;
    }
}