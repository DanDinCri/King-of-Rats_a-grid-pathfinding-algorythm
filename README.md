# King-of-Rats_a-grid-pathfinding-algorythm
A personal pathfinding made from ground.
"King of Rats" refers to the pathfinding style:
-Recursively, a bunch of "rats" starts searching the shortest path through a maze.
-When a rat finds it, it's saved as the shortest, tho if another "rat" finds a shorter path, it will be overwriten, indefinitely.
-Once there's a shortest path saved, if a "rat's" path is larger than the shortest, it gives up and stops searching. This results on all "rats" searching around a stablished "circle" (limited by the shortest path steps).

I provide a bunch of scripts + functions for testing.

Note: this algorythm is not as efficient as A* in big grids, but works well on medium sized grids (for example, 20x20 cells grid).
