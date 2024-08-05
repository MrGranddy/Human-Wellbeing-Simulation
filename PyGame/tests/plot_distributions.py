import os
import sys

sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), "..")))

import matplotlib.pyplot as plt
import numpy as np

from src.functions import (
    MAXIMUM_DISTANCE,
    alternative_friendship_breaking_probability,
    friendship_breaking_probability,
    friendship_establishment_probability,
)


def plot_distributions():
    equ_prob_dist = 0.01
    max_distance = MAXIMUM_DISTANCE

    # Generate distances for plotting
    distances = np.linspace(0, max_distance, 100)

    # Establishment probability
    est_prob_func = friendship_establishment_probability(equ_prob_dist)
    est_probs = est_prob_func(distances)

    # Breaking probability
    break_prob_func = friendship_breaking_probability(MAXIMUM_DISTANCE - equ_prob_dist)
    break_probs = break_prob_func(distances)

    # Alternative breaking probability
    alt_break_prob_func = alternative_friendship_breaking_probability(
        MAXIMUM_DISTANCE - equ_prob_dist
    )
    alt_break_probs = alt_break_prob_func(distances)

    # Plotting the distributions
    plt.figure(figsize=(12, 6))

    plt.plot(distances, est_probs, label="Establishment Probability")
    plt.plot(distances, break_probs, label="Breaking Probability")
    plt.plot(distances, alt_break_probs, label="Alternative Breaking Probability")

    plt.xlabel("Distance")
    plt.ylabel("Probability")
    plt.title("Friendship Probabilities")
    plt.legend()

    plt.tight_layout()
    plt.show()


if __name__ == "__main__":
    plot_distributions()
