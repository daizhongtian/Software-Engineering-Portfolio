#define _CRT_SECURE_NO_WARNINGS

#ifndef FUNCTION_H
#define FUNCTION_H

#include <stdio.h>
#include <stdlib.h>
#include "player.h"


struct board_size {
    int board_width;
    int board_height;
};

void read_board_size(struct board_size* size);  //ask user board size

int read_penguin_quantity(int* quantity, struct board_size size);//ask user how many penguin

int read_coordinates_overlap(int(*coords)[2], int quantity, struct board_size size);
//check if user enterd invalid value

int(*allocate_coordinates(int quantity))[2];

//int read_coordinates(int(*coords)[2], int quantity, struct board_size size);

void free_memory(int(*coords)[2]);

//initialize and print board
int** initializeBoard(int rows, int cols);
void printBoard(int** board, int rows, int cols);

void menu();

void freeBoard(int rows, int cols, int** board);

//void penguin_placement();
int penguins_movement(int** board, int board_width, int board_height, int (*penguin_coords)[2],
    int penguin_index, int target_x, int target_y,
    int quantity, Player* players, int currentPlayerIndex);


void check_gameover();

//game over function
int checkGameOver(int** board, int board_width, int board_height, int(*penguin_coords)[2], int penguin_quantity, int numPlayers);

int checkgameover_for_case2(int** board, int board_width, int board_height,
    Player* players, int numPlayers);

//save and read game
int** initializeBoard(int width, int height);
void loadGameState(char* filepath, int*** board, Player* players,
    int* numPlayers, struct board_size* size,
    int* currentPlayerIndex);

void findPenguinPositions(int** board, int board_width, int board_height, Player* players);



void printPenguinsAtStart();

int canPenguinMove(int** board, int board_width, int board_height, int x, int y); 

#endif  // FUNCTION_H
