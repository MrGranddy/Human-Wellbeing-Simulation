import pygame

from src.structures import FriendshipGraph, Person

# Game parameters
WIDTH = 800
HEIGHT = 600
FPS = 60

# Colors
BLACK = (0, 0, 0)
WHITE = (255, 255, 255)
LIGHT_GRAY = (200, 200, 200)
BLUE = (0, 0, 255)

if __name__ == "__main__":

    # Initialize Pygame
    pygame.init()

    # Set up the drawing window
    screen = pygame.display.set_mode([WIDTH, HEIGHT])
    pygame.display.set_caption("Wellbeing Simulation")

    # Create a clock object to manage the frame rate
    clock = pygame.time.Clock()

    ########################### SIMULATION SETUP ##############################

    # Simulation time
    t = 0
    time_step = 1.0e-2  # Mainly used for simplex noise
    N = 100

    # Initialize people
    people = [Person() for _ in range(N)]
    friendship_graph = FriendshipGraph(N, establishment_equ_prob_dist=0.01, break_equ_prob_dist=1.414 * 0.75)

    ###########################################################################

    # Run until the user asks to quit
    running = True
    while running:

        # Did the user click the window close button?
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                running = False

        # Fill the background with white
        screen.fill(LIGHT_GRAY)

        ########################### SIMULATION STEP ##############################

        # Update the simulation
        for person in people:
            person.random_move(t)

        friendship_graph.add_random_friendships(people)
        friendship_graph.remove_random_friendships(people)

        t += time_step

        ###########################################################################

        ########################### DRAWING STEP ##############################

        # Draw graph
        friendship_graph.draw_friendships(people, screen, BLACK, WIDTH, HEIGHT)

        # Draw the people
        for person in people:
            person.draw(screen, BLUE, WIDTH, HEIGHT)

        #######################################################################

        # Flip the display
        pygame.display.flip()

        # Ensure the program maintains a rate of FPS frames per second
        clock.tick(FPS)

    # Done! Time to quit.
    pygame.quit()
