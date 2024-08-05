import random
from typing import List, Optional
import numpy as np
import opensimplex
import pygame

from src.functions import (
    MAXIMUM_DISTANCE,
    friendship_breaking_probability,
    friendship_establishment_probability,
)

SEED = None
MAX_RANDOM_SPEED = 3.0e-3
MIN_RANDOM_SPEED = 3.0e-4

if SEED is not None:
    opensimplex.seed(SEED)
    random.seed(SEED)

class People:
    """A collection of people in the simulation.

    Attributes:
        num_people (int): The number of people in the simulation.
        x (np.ndarray): The x-coordinates of the people (Range: [0, 1]).
        y (np.ndarray): The y-coordinates of the people (Range: [0, 1]).
        speed_coefficient (np.ndarray): The speed coefficients for each person.
        sfc (np.ndarray): Selfishness coefficient for each person - A number between 0 and 1 that determines how selfish the person is.
        wpc (np.ndarray): Wellbeing production coefficient for each person - A number between 0 and 1 that determines how much wellbeing the person produces.
        irc (np.ndarray): Influence reach coefficient for each person - A number between 0 and 1 that determines how far the person's influence reaches.
        colors (np.ndarray): Colors representing each person.
        wellbeing (np.ndarray): Wellbeing values for each person.
    """
    
    def __init__(self, num_people: int):
        """
        Initialize a collection of people.

        Args:
            num_people (int): The number of people in the simulation.
        """
        self.num_people = num_people
        self.x = np.random.rand(num_people)
        self.y = np.random.rand(num_people)
        self.speed_coefficient = np.random.rand(num_people) * (MAX_RANDOM_SPEED - MIN_RANDOM_SPEED) + MIN_RANDOM_SPEED
        self.sfc = np.random.rand(num_people)
        self.wpc = np.random.rand(num_people)
        self.irc = np.random.rand(num_people)
        self.colors = np.random.randint(20, 255, (num_people, 3))
        self.wellbeing = np.zeros(num_people)

    def move(self, dx: np.ndarray, dy: np.ndarray):
        """
        Move the people by a given delta in x and y directions.

        Args:
            dx (np.ndarray): Change in x-coordinate.
            dy (np.ndarray): Change in y-coordinate.
        """
        self.x = np.clip(self.x + dx, 0, 1)
        self.y = np.clip(self.y + dy, 0, 1)

    def random_move(self, t: float):
        """
        Move a specific person randomly using opensimplex noise.

        Args:
            t (float): Time parameter for noise generation.
        """
        ids = np.arange(self.num_people)
        ts = np.array([t])

        dx = opensimplex.noise2array(2 * ids, ts).flatten()
        dy = opensimplex.noise2array(2 * ids + 1, ts).flatten()

        self.x = np.clip(self.x + dx * self.speed_coefficient, 0, 1)
        self.y = np.clip(self.y + dy * self.speed_coefficient, 0, 1)

    def draw_people(self, screen: pygame.Surface, width: int, height: int, radius: int):
        """
        Draw the people on a Pygame screen.

        Args:
            screen (pygame.Surface): Pygame screen to draw on.
            width (int): Width of the screen.
            height (int): Height of the screen.
            radius (int): Radius of the circles representing people.
        """
        for i in range(self.num_people):
            point = int(self.x[i] * width), int(self.y[i] * height)
            pygame.draw.circle(screen, self.colors[i], point, radius)


class FriendshipGraph:
    """A graph representing friendships between people."""
    
    def __init__(self, num_people: int, establishment_equ_prob_dist: float, break_equ_prob_dist: float):
        """
        Initialize the friendship graph.

        Args:
            num_people (int): Number of people in the simulation.
            establishment_equ_prob_dist (float): Parameter for the friendship establishment probability distribution.
            break_equ_prob_dist (float): Parameter for the friendship breaking probability distribution.
        """
        self.graph = np.zeros((num_people, num_people))
        self.establishment_equ_prob_dist = establishment_equ_prob_dist
        self.break_equ_prob_dist = break_equ_prob_dist

        self.establishment_prob = friendship_establishment_probability(establishment_equ_prob_dist)
        self.breaking_prob = friendship_breaking_probability(break_equ_prob_dist)

    def add_friendship(self, person1: int, person2: int):
        """
        Add a friendship between two people.

        Args:
            person1 (int): Index of the first person.
            person2 (int): Index of the second person.
        """
        self.graph[person1, person2] = 1
        self.graph[person2, person1] = 1

    def add_friendships(self, people1: np.ndarray, people2: np.ndarray):
        """
        Add friendships between two sets of people.

        Args:
            people1 (np.ndarray): Indices of the first set of people.
            people2 (np.ndarray): Indices of the second set of people.
        """
        self.graph[people1, people2] = 1
        self.graph[people2, people1] = 1

    def remove_friendship(self, person1: int, person2: int):
        """
        Remove a friendship between two people.

        Args:
            person1 (int): Index of the first person.
            person2 (int): Index of the second person.
        """
        self.graph[person1, person2] = 0
        self.graph[person2, person1] = 0

    def remove_friendships(self, people1: np.ndarray, people2: np.ndarray):
        """
        Remove friendships between two sets of people.

        Args:
            people1 (np.ndarray): Indices of the first set of people.
            people2 (np.ndarray): Indices of the second set of people.
        """
        self.graph[people1, people2] = 0
        self.graph[people2, people1] = 0

    def get_friends(self, person: int) -> List[int]:
        """
        Get the list of friends of a person.

        Args:
            person (int): Index of the person.

        Returns:
            List[int]: List of indices of the friends.
        """
        return np.where(self.graph[person] == 1)[0].tolist()

    def add_random_friendships(self, people: People):
        """
        Add random friendships based on the distance between people.

        Args:
            people (People): Instance of the People class.
        """

        x_diff = people.x.reshape(-1, 1) - people.x.reshape(1, -1)
        y_diff = people.y.reshape(-1, 1) - people.y.reshape(1, -1)

        distance_matrix = np.sqrt(x_diff ** 2 + y_diff ** 2)

        p = self.establishment_prob(distance_matrix)
        rand = np.random.rand(people.num_people, people.num_people)

        xs, ys = np.where(np.triu(rand < p, k=1))

        self.add_friendships(xs, ys)
                                

    def remove_random_friendships(self, people: People):
        """
        Remove random friendships based on the distance between people.

        Args:
            people (People): Instance of the People class.
        """

        x_diff = people.x.reshape(-1, 1) - people.x.reshape(1, -1)
        y_diff = people.y.reshape(-1, 1) - people.y.reshape(1, -1)

        distance_matrix = np.sqrt(x_diff ** 2 + y_diff ** 2)

        p = self.breaking_prob(distance_matrix)
        rand = np.random.rand(people.num_people, people.num_people)

        xs, ys = np.where(np.triu(rand < p, k=1))

        self.remove_friendships(xs, ys)


    def draw_friendships(
        self, people: People, screen: pygame.Surface, color: tuple, width: int, height: int, linewidth: int = 1
    ):
        """
        Draw the friendships on a Pygame screen.

        Args:
            people (People): Instance of the People class.
            screen (pygame.Surface): Pygame screen to draw on.
            color (tuple): Color of the lines representing friendships.
            width (int): Width of the screen.
            height (int): Height of the screen.
            linewidth (int): Width of the lines. Default is 1.
        """
        for i in range(people.num_people):
            for j in range(i):
                if self.graph[i, j] == 1:
                    point1 = int(people.x[i] * width), int(people.y[i] * height)
                    point2 = int(people.x[j] * width), int(people.y[j] * height)
                    pygame.draw.line(screen, color, point1, point2, linewidth)
