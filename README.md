# Bimaru Solver
A project to build a Bimaru (see https://en.wikipedia.org/wiki/Battleship_(puzzle)) solver in C#. It emphasizes to have clean code by using the following design principles:

- Don't repeat yourself (see https://en.wikipedia.org/wiki/Don%27t_repeat_yourself)
- Dependency injection (see https://en.wikipedia.org/wiki/Dependency_injection)
- Many principles from the book: "Clean Code: A Handbook of Agile Software Craftsmanship" by Robert C. Martin

Solving a Bimaru is NP-complete (see https://en.wikipedia.org/wiki/Battleship_(puzzle)#Computers_and_Battleship) and hence it is not known how to solve general Bimarus efficiently. However, many Bimaru puzzles in newspapers/magazines or online are specially designed to be solvable by humans in a 'short' amount of time.

This project aims at solving those human-solvable Bimaru puzzles very efficiently. It uses a few simple heuristics but omits too sophisticated and complicated strategies. Although the current Bimaru solver is able to solve any Bimaru, it could use a lot of time to do so if the puzzle is too far away from being human-solvable.
