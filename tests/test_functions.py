import math
import unittest

from src.functions import (
    MAXIMUM_DISTANCE,
    alternative_friendship_breaking_probability,
    friendship_breaking_probability,
    friendship_establishment_probability,
)


class TestFriendshipProbabilities(unittest.TestCase):
    def setUp(self):
        self.equ_prob_dist = 0.5
        self.max_distance = MAXIMUM_DISTANCE

    def test_friendship_establishment_probability(self):
        prob_func = friendship_establishment_probability(self.equ_prob_dist)
        self.assertAlmostEqual(float(prob_func(0)), 1.0)
        self.assertAlmostEqual(float(prob_func(self.equ_prob_dist)), 0.5, places=6)

    def test_friendship_breaking_probability(self):
        prob_func = friendship_breaking_probability(
            MAXIMUM_DISTANCE - self.equ_prob_dist
        )
        self.assertAlmostEqual(float(prob_func(0)), 0.0)
        self.assertAlmostEqual(
            float(prob_func(MAXIMUM_DISTANCE - self.equ_prob_dist)), 0.5, places=6
        )
        self.assertAlmostEqual(float(prob_func(self.max_distance)), 1.0)

    def test_alternative_friendship_breaking_probability(self):
        prob_func = alternative_friendship_breaking_probability(
            MAXIMUM_DISTANCE - self.equ_prob_dist
        )
        self.assertAlmostEqual(float(prob_func(self.max_distance)), 1.0)
        self.assertAlmostEqual(
            float(prob_func(MAXIMUM_DISTANCE - self.equ_prob_dist)), 0.5, places=6
        )


if __name__ == "__main__":
    unittest.main()
