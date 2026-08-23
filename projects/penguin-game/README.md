# Penguin Board Game

A C-based game where players control penguins on an ice floe board. Each tile contains a random number of fish, and players compete to collect as many fish as possible by placing and moving their penguins.

This project demonstrates core C programming techniques including dynamic memory allocation, file I/O and game state management.

## Features

- Randomly generated board dimensions with different fish values
- Turn-based gameplay for up to eight players
- Ability to save and load the current game state
- Simple text-based interface

## Build and Run

Compile the sources using GCC:

```bash
gcc main.c Source.c player.c -o penguin-game
./penguin-game
```

Follow the on-screen prompts to set up the board and start playing.

## Future Improvements

- Develop a GUI to replace or enhance the command-line interface
- Integrate AI-controlled players for single-player mode or to complement multiplayer sessions. For each penguin, generate legal moves in all directions, evaluate each move, and choose the best scoring option.

## Authors

- [Daizhongtian](https://github.com/daizhongtian)
- [Vladyslav Zelenkov](https://github.com/VladyslavZelenkov)

This project is released under the [MIT License](LICENSE).


