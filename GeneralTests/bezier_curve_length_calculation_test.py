import numpy as np

import matplotlib.pyplot as plt

def create_cubic_bezier_curve(points):
    
    def bezier_curve(t):
        p01 = (1 - t) * points[0] + t * points[1]
        p12 = (1 - t) * points[1] + t * points[2]
        p23 = (1 - t) * points[2] + t * points[3]

        p012 = (1 - t) * p01 + t * p12
        p123 = (1 - t) * p12 + t * p23

        p0123 = (1 - t) * p012 + t * p123

        return p0123
    
    def bezier_curve_derivative(t):
        return 3 * (1 - t) ** 2 * (points[1] - points[0]) + 6 * (1 - t) * t * (points[2] - points[1]) + 3 * t ** 2 * (points[3] - points[2])
    
    return bezier_curve, bezier_curve_derivative

def calculate_bezier_curve_length_without_derivative(bezier_curve, t0, t1, n):
    dt = (t1 - t0) / n
    length = 0
    for i in range(n):
        t = t0 + i * dt
        length += np.linalg.norm(bezier_curve(t + dt) - bezier_curve(t))
    return length

def calculate_bezier_curve_length_with_derivative(bezier_curve_derivative, t0, t1, n):
    dt = (t1 - t0) / n
    length = 0
    for i in range(n):
        t = t0 + i * dt
        length += np.linalg.norm(bezier_curve_derivative(t)) * dt
    return length

# Test derivative vs non-derivative length calculation

bezier_points = [ np.random.rand(2) for _ in range(4) ]

bezier_curve, bezier_curve_derivative = create_cubic_bezier_curve(bezier_points)

n_points = []
lengths_without_derivative = []
lengths_with_derivative = []

for n in np.linspace(0, 2, 20):

    n = int(10 ** n)

    n_points.append(n)
    lengths_without_derivative.append(calculate_bezier_curve_length_without_derivative(bezier_curve, 0, 1, n))
    lengths_with_derivative.append(calculate_bezier_curve_length_with_derivative(bezier_curve_derivative, 0, 1, n))


plt.plot(n_points, lengths_without_derivative, label='Without derivative')
plt.plot(n_points, lengths_with_derivative, label='With derivative')
plt.xlabel('Number of points')
plt.ylabel('Length')
plt.legend()
plt.show()

# Test non-derivative saturation

n_trials = 100

n_points = np.linspace(0, 2, 20)
average_diffs = np.zeros(len(n_points) - 1)

for trial in range(n_trials):

    bezier_points = [ np.random.rand(2) for _ in range(4) ]

    bezier_curve, _ = create_cubic_bezier_curve(bezier_points)

    lengths = []

    for n in n_points:

        n = int(10 ** n)

        lengths.append(calculate_bezier_curve_length_without_derivative(bezier_curve, 0, 1, n))

    diffs = np.abs(np.diff(lengths))

    average_diffs += diffs / n_trials

plt.plot(10 ** n_points[:-1], average_diffs)
plt.xlabel('Number of points')
plt.ylabel('Average difference')
plt.show()


# 15 points seems good enough for the non-derivative calculation