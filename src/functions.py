import math
from typing import Callable, Union

import numpy as np
from scipy.optimize import root

# Constants
MAXIMUM_DISTANCE = math.sqrt(2)


def friendship_establishment_probability(
    equ_prob_dist: float,
) -> Callable[[Union[float, np.ndarray]], Union[float, np.ndarray]]:
    """
    Returns a function that calculates the probability of friendship establishment based on the distance between two nodes.

    Args:
        equ_prob_dist: The distance at which the probability of friendship establishment is 0.5.

    Returns:
        A function that calculates the probability of friendship establishment based on the distance between two nodes.
    """
    lam = -np.log(0.5) / equ_prob_dist

    def establishment_probability(
        distance: Union[float, np.ndarray]
    ) -> Union[float, np.ndarray]:
        return np.exp(-lam * distance)

    return establishment_probability


def friendship_breaking_probability(
    equ_prob_dist: float,
) -> Callable[[Union[float, np.ndarray]], Union[float, np.ndarray]]:
    """
    Returns a function that calculates the probability of friendship breaking based on the distance between two nodes.

    Args:
        equ_prob_dist: The distance at which the probability of friendship breaking is 0.5.

    Returns:
        A function that calculates the probability of friendship breaking based on the distance between two nodes.
    """
    K = equ_prob_dist / MAXIMUM_DISTANCE

    def equation(c1: float) -> float:
        return K * np.log(1 + 1 / c1) - np.log(1 + 1 / (2 * c1))

    # Using the hybrid method to find the root for better stability
    c1_solution = root(equation, 1.0e-6, method="hybr")
    c1 = float(c1_solution.x[0])
    lam = (1 / MAXIMUM_DISTANCE) * np.log(1 + 1 / c1)

    def breaking_probability(
        distance: Union[float, np.ndarray]
    ) -> Union[float, np.ndarray]:
        return np.minimum(c1 * (np.exp(lam * distance) - 1), 1)

    return breaking_probability


def alternative_friendship_breaking_probability(
    equ_prob_dist: float,
) -> Callable[[Union[float, np.ndarray]], Union[float, np.ndarray]]:
    """
    Returns an alternative function that calculates the probability of friendship breaking based on the distance between two nodes.

    Args:
        equ_prob_dist: The distance at which the probability of friendship breaking is 0.5.

    Returns:
        A function that calculates the probability of friendship breaking based on the distance between two nodes.
    """
    symmetric_equ_prob_dist = MAXIMUM_DISTANCE - equ_prob_dist
    establishment_prob = friendship_establishment_probability(symmetric_equ_prob_dist)

    def sym_breaking_probability(
        distance: Union[float, np.ndarray]
    ) -> Union[float, np.ndarray]:
        return establishment_prob(np.maximum(0, MAXIMUM_DISTANCE - distance))

    return sym_breaking_probability
