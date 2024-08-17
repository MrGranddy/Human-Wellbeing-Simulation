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

    [Tooltip("The height of the simulation.")]
    [Range(0.0f, 5.0f)]
    public float simulationHeight = 1.0f;

    [Tooltip("The width of the simulation.")]
    [Range(0.0f, 5.0f)]
    public float simulationWidth = 1.0f;

    [Tooltip("Scale of the speed of the people.")]
    [Range(0.0f, 0.05f)]
    public float speedCoefficient = 0.001f;

    [Tooltip("The attractiveness factor for friends. Determines how much friends attract each other (position-wise).")]
    public float friendAttractiveness = 0.0f;

    [Tooltip("The prefab used to represent a person.")]
    public GameObject personPrefab;

    [Tooltip("The size of each sprite (height).")]
    [Range(0.0f, 200.0f)]
    public float personSize = 15.0f;

    [Tooltip("Beta parameter for random movement.")]
    [Range(0.0f, 1.0f)]
    public float randomWalkBeta = 1.0f;

    [Tooltip("Mean for the random movement.")]
    public Vector2 randomWalkMean = new Vector2(0.0f, 0.0f);

    [Tooltip("Standard deviation for the random movement.")]
    public Vector2 randomWalkStd = new Vector2(0.01f, 0.01f);

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
        UpdateSimulationSize();
        MovePeople();
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

    private void UpdateSimulationSize()
    {
        foreach (Person person in people)
        {
            person.UpdateSimulationSize(simulationHeight, simulationWidth);
        }
    }

    /// <summary>
    /// Moves the people randomly using Perlin noise.
    /// </summary>
    private void MovePeople()
    {
        foreach (Person person in people)
        {
            person.Move(randomWalkBeta, randomWalkMean, randomWalkStd, speedCoefficient);
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
            AddPeople(numPeople - people.Count);
        }
        else if (numPeople < people.Count)
        {
            RemovePeople(people.Count - numPeople);
        }
    }

    /// <summary>
    /// Adds a specified number of people to the simulation.
    /// </summary>
    /// <param name="count">The number of people to add.</param>
    private void AddPeople(int count)
    {
        for (int i = 0; i < count; i++)
        {
            people.Add(CreatePerson());
        }
    }

    /// <summary>
    /// Removes a specified number of people from the simulation.
    /// </summary>
    /// <param name="count">The number of people to remove.</param>
    private void RemovePeople(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int lastIndex = people.Count - 1;
            Destroy(people[lastIndex].gameObject);
            people.RemoveAt(lastIndex);
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
        if (person != null)
        {
            person.Initialize(randomWalkMean, randomWalkStd, simulationHeight, simulationWidth);
            person.SetPersonSize(personSize);
            person.UpdateTransformPosition();
            return person;
        }
        else
        {
            Debug.LogError("Person prefab is missing the Person component.");
            Destroy(personObject);
            return null;
        }
    }

}
