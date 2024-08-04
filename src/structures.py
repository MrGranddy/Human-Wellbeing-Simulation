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


class Person:
    """A single person in the simulation."""

    # Global variable to assign unique IDs to each person automatically
    id_counter = 0

    def __init__(
        self,
        x: Optional[float] = None,
        y: Optional[float] = None,
        speed: Optional[float] = None,
        sfc: Optional[float] = None,
        wpc: Optional[float] = None,
        irc: Optional[float] = None,
        color: Optional[tuple] = None,
    ):
        """Initialize a person.

        Args:
            x (float): The x-coordinate of the person. (Range: [0, 1])
            y (float): The y-coordinate of the person. (Range: [0, 1])
            speed (float): The speed of the person.
            sfc (float): Selfishness coefficient - A number between 0 and 1 that determines how selfish the person is.
            wpc (float): Wellbeing production coefficient - A number between 0 and 1 that determines how much wellbeing the person produces.
            irc (float): Influence reach coefficient - A number between 0 and 1 that determines how far the person's influence reaches.
        """
        if x:
            self.x = x
        else:
            self.x = random.random()
        if y:
            self.y = y
        else:
            self.y = random.random()
        if speed:
            self.speed = speed
        else:
            self.speed = (
                random.random() * (MAX_RANDOM_SPEED - MIN_RANDOM_SPEED)
                + MIN_RANDOM_SPEED
            )

        self.sfc = sfc if sfc is not None else random.random()
        self.wpc = wpc if wpc is not None else random.random()
        self.irc = irc if irc is not None else random.random()

        if color:
            self.color = color
        else:
            self.color = (
                random.randint(20, 255),
                random.randint(20, 255),
                random.randint(20, 255),
            )

        Person.id_counter += 1
        self.id = Person.id_counter

        self.wellbeing = 0

    def move(self, dx, dy):
        self.x += dx
        self.y += dy

    def random_move(self, t):
        dx = opensimplex.noise2(self.id, t)
        dy = opensimplex.noise2(self.id + 1, t)

        self.x = max(0, min(1, self.x + dx * self.speed))
        self.y = max(0, min(1, self.y + dy * self.speed))

    def draw(self, screen, color, width, height, radius=3):
        x = int(self.x * width)
        y = int(self.y * height)
        pygame.draw.circle(screen, color, (x, y), radius)


class FriendshipGraph:
    def __init__(self, num_people: int, establishment_equ_prob_dist: float, break_equ_prob_dist: float):
        self.graph = np.zeros((num_people, num_people))
        self.establishment_equ_prob_dist = establishment_equ_prob_dist
        self.break_equ_prob_dist = break_equ_prob_dist

        self.establishment_prob = friendship_establishment_probability(establishment_equ_prob_dist)
        self.breaking_prob = friendship_breaking_probability(break_equ_prob_dist)

    def add_friendship(self, person1: int, person2: int):

        self.graph[person1, person2] = 1
        self.graph[person2, person1] = 1

    def remove_friendship(self, person1: int, person2: int):

        self.graph[person1, person2] = 0
        self.graph[person2, person1] = 0

    def get_friends(self, person: int) -> List[int]:
        return np.where(self.graph[person] == 1)[0].tolist()

    def add_random_friendships(self, people: List[Person]):
        for i in range(len(people)):
            for j in range(i):

                distance = np.sqrt(
                    (people[i].x - people[j].x) ** 2 + (people[i].y - people[j].y) ** 2
                )

                p = self.establishment_prob(distance)

                if random.random() < p:
                    self.add_friendship(i, j)

    def remove_random_friendships(self, people: List[Person]):
        for i in range(len(people)):
            for j in range(i):

                distance = np.sqrt(
                    (people[i].x - people[j].x) ** 2 + (people[i].y - people[j].y) ** 2
                )

                p = self.breaking_prob(distance)

                if random.random() < p:
                    self.remove_friendship(i, j)

    def draw_friendships(
        self, people: List[Person], screen, color, width, height, linewidth=1
    ):

        for i in range(len(people)):
            for j in range(i):
                if self.graph[i, j] == 1:
                    point1 = int(people[i].x * width), int(people[i].y * height)
                    point2 = int(people[j].x * width), int(people[j].y * height)
                    pygame.draw.line(screen, color, point1, point2, linewidth)
