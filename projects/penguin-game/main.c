#define _CRT_SECURE_NO_WARNINGS

#define MAX_PENGUINS_PER_PLAYER 10
#define MAX_PLAYERS 8  // max player
#include <stdio.h>
#include <stdlib.h> 
#include "function.h"
#include <time.h>
#include <windows.h>
#include"player.h"





int main()
{
    srand(time(NULL)); // ramdom
    struct board_size size;
    int input = 0;

    int gameInitialized = 0;

    int penguin_quantity;
    int(*penguin_coords)[2] = NULL;
    int** board = NULL;

    //for player

    int player_penguin_positions[MAX_PLAYERS][MAX_PENGUINS_PER_PLAYER][2];
    int penguin_counts[MAX_PLAYERS];



    do
    {
        menu();
        printf("Please choose an option: ");
        scanf("%d", &input);

        switch (input)
        {
        case 1:
            int numPlayers;//??
            printf("Enter the number of players: ");
            scanf("%d", &numPlayers);
            if (numPlayers > 8)
            {
                printf("too much player");
                return -1;
            }

            // make sure that max player is lower than 8
            numPlayers = (numPlayers > MAX_PLAYERS) ? MAX_PLAYERS : numPlayers;

            Player players[MAX_PLAYERS];

            for (int i = 0; i < numPlayers; i++) {
                printf("nickname for player %d: ", i + 1);
                scanf("%s", players[i].nickname);  // get nickname from user
                players[i].player_number = i + 1;
                players[i].fish_collected = 0;
            }



            read_board_size(&size); // read size of board
            int** board = initializeBoard(size.board_width, size.board_height); // initialize board
            if (board == NULL) {
                printf("Failed to initialize the board.\n");
                break;
            }
            //board[1][2] = 5;
            printBoard(board, size.board_width, size.board_height); //printboard





            if (read_penguin_quantity(&penguin_quantity, size) != 0) { //read amount of penguins
                printf("Failed to read penguin quantity.\n");
                break;
            }


            penguin_coords = allocate_coordinates(penguin_quantity);
            if (penguin_coords == NULL) {
                printf("Failed to allocate memory for penguin coordinates.\n");
                break;
            }

            if (read_coordinates_overlap(penguin_coords, penguin_quantity, size) != 0) { // read coordinate and check validation of coordinate
                printf("Error in reading coordinates.\n");
                break;
            }



            //placement penguins

            for (int i = 0; i < penguin_quantity; i++)
            {
                int currentPlayerIndex = i % numPlayers;


                int currentPenguinNumber = (i / numPlayers) + 1;  // Calculate which penguin of each player is currently

                int x = penguin_coords[i][0];
                int y = penguin_coords[i][1];




                printf("Player %s is placing a penguin at (%d, %d).\n", players[currentPlayerIndex].nickname, x, y);
                int penguinCode = 5 + currentPlayerIndex; // 5, 6, 7, 8... for each player
                if (board[x][y] == 1)
                {
                    board[x][y] = 5 + currentPlayerIndex;

                    players[currentPlayerIndex].fish_collected += 1;
                    save(board, size.board_height, size.board_width, "saved_game.txt", penguin_quantity, players, numPlayers, penguin_quantity, currentPlayerIndex);//save game
                }



                else if (board[x][y] == 0)
                {
                    // If the current position is water (no ice floe), a penguin cannot be placed
                    printf("Cannot place a penguin at (%d, %d) - it's water .\n", x, y);
                    if (read_coordinates_overlap(penguin_coords, penguin_quantity, size) != 0)
                    {

                        printf("Error in reading coordinates.\n");
                    }
                    i = -1;
                    continue;
                }

                else if (board[x][y] == 2 || board[x][y] == 3)
                {
                    printf("Too many fish at (%d, %d), try again\n", x, y);

                    if (read_coordinates_overlap(penguin_coords, penguin_quantity, size) != 0)
                    {

                        printf("Error in reading coordinates.\n");

                    }

                    i = -1;
                    continue;
                }
            }
            //penguin movement and check if game over
            int game_over = 0;
            int currentPlayerIndex = 0;
            while (game_over != 1)//check if game over
            {
                int penguinCanMove = 0;
                int moveCompleted = 0;



                while (!moveCompleted) {    //check  valid move

                    // Check if at least one of the current player's penguins can move
                    for (int i = 0; i < penguin_quantity; i++) {
                        int x = penguin_coords[i][0];
                        int y = penguin_coords[i][1];
                        if (board[x][y] == 5 + currentPlayerIndex && canPenguinMove(board, size.board_width, size.board_height, x, y)) {
                            penguinCanMove = 1;
                            break;
                        }
                    }

                    if (!penguinCanMove) {
                        printf("No penguins can move for player %s, skipping turn.\n", players[currentPlayerIndex].nickname);
                        currentPlayerIndex = (currentPlayerIndex + 1) % numPlayers;
                        continue; // skip player
                    }

                    printf("\nNow it's %s's turn.\n", players[currentPlayerIndex].nickname);
                    printBoard(board, size.board_width, size.board_height);

                    printf("Your penguins:\n");
                    for (int i = 0; i < penguin_quantity; i++) {
                        int x = penguin_coords[i][0];
                        int y = penguin_coords[i][1];
                        if (board[x][y] == 5 + currentPlayerIndex) {
                            printf("Penguin %d at (%d, %d)\n", i, x, y);
                        }
                    }

                    int selected_penguin = -1;
                    printf("Select a penguin to move: ");
                    scanf("%d", &selected_penguin);

                    if (selected_penguin >= 0 && selected_penguin < penguin_quantity) {
                        int x = penguin_coords[selected_penguin][0];
                        int y = penguin_coords[selected_penguin][1];
                        if (board[x][y] == 5 + currentPlayerIndex) {
                            while (1)  //check if user choosed correct destination 
                            {
                                int target_x, target_y;
                                printf("Enter new coordinates (x y) for penguin: ");
                                scanf("%d %d", &target_x, &target_y);

                                if (target_x >= 0 && target_x < size.board_width && target_y >= 0 && target_y < size.board_height) {
                                    int moveValid = penguins_movement(board, size.board_width, size.board_height, penguin_coords, selected_penguin, target_x, target_y, numPlayers, players, currentPlayerIndex);
                                    if (moveValid) {
                                        moveCompleted = 1;
                                        save(board, size.board_height, size.board_width, "saved_game.txt", penguin_quantity, players, numPlayers, penguin_quantity, currentPlayerIndex); // ±£´æÓÎÏ·×´Ì¬
                                        break;
                                    }
                                    else {
                                        printf("Invalid move. Please try again.\n");
                                    }
                                }
                                else {
                                    printf("Invalid coordinates. Please try again.\n");
                                }
                            }
                        }
                        else {
                            printf("Invalid selection. Please try again.\n");
                        }
                    }
                    else {
                        printf("Invalid penguin selection.\n");
                    }
                }

                printPlayerScores(players, numPlayers);
                currentPlayerIndex = (currentPlayerIndex + 1) % numPlayers;

                // ....check if the game is over
                game_over = checkGameOver(board, size.board_width, size.board_height, penguin_coords, penguin_quantity, numPlayers);
            }
            //interract with user after game over
            printf("Game over! Final scores:\n");
            printPlayerScores(players, numPlayers);

            int maxFish = -1;
            int winnerIndex = -1;

            //find best scored player
            for (int i = 0; i < numPlayers; i++) {
                if (players[i].fish_collected > maxFish) {
                    maxFish = players[i].fish_collected;
                    winnerIndex = i;
                }
            }

            // //print winner
            if (winnerIndex != -1) {
                printf("Game over! The winner is %s with %d fish!\n", players[winnerIndex].nickname, maxFish);
            }
            else {
                printf("Game over! No winner.\n");
            }

            break;




        case 0:
            printf("Exiting the game.\n");
            break;

        case 2:
        {

            int penguin_quantity = 0;
            FILE* file = fopen("saved_game.txt", "r");
            if (file) {
                fclose(file);

                //nitialize all players in the game, setting their penguin count to 0(inportant)

                Player players[MAX_PLAYERS];
                for (int i = 0; i < MAX_PLAYERS; i++) {
                    players[i].penguin_count = 0;
                    memset(players[i].penguins, 0, sizeof(players[i].penguins));//initialize(important)
                }

                // load game
                loadGameState("saved_game.txt", &board, players, &numPlayers, &size, &currentPlayerIndex);



                printf("Game state loaded successfully. Continuing the game...\n");

                printPlayerScores(players, numPlayers);



                //int currentPlayerIndex = 0; 
                currentPlayerIndex = (currentPlayerIndex + 1) % numPlayers;
                int game_over = 0; // check gameover

                //same idea with case 1n
                while (!game_over)
                {

                    findPenguinPositions(board, size.board_width, size.board_height, players);

                    //
                    int penguinCanMove = 0;
                    for (int i = 0; i < players[currentPlayerIndex].penguin_count; i++) {
                        int x = players[currentPlayerIndex].penguins[i].x;
                        int y = players[currentPlayerIndex].penguins[i].y;
                        if (canPenguinMove(board, size.board_width, size.board_height, x, y)) {
                            penguinCanMove = 1;
                            break;
                        }
                    }

                    if (!penguinCanMove) {
                        printf("No penguins can move for player %s, skipping turn.\n", players[currentPlayerIndex].nickname);
                        currentPlayerIndex = (currentPlayerIndex + 1) % numPlayers;
                        continue;
                    }

                    int moveCompleted = 0;
                    while (!moveCompleted) {
                        printf("\nNow it's %s's turn.\n", players[currentPlayerIndex].nickname);
                        printBoard(board, size.board_width, size.board_height);



                        printf("Your penguins:\n");
                        for (int i = 0; i < players[currentPlayerIndex].penguin_count; i++) {
                            printf("Penguin %d at (%d, %d)\n", i,
                                players[currentPlayerIndex].penguins[i].x,
                                players[currentPlayerIndex].penguins[i].y);
                        }

                        int selected_penguin = -1;
                        while (selected_penguin < 0 || selected_penguin >= players[currentPlayerIndex].penguin_count)
                        {
                            printf("Select a penguin to move: ");
                            scanf("%d", &selected_penguin);
                            if (selected_penguin < 0 || selected_penguin >= players[currentPlayerIndex].penguin_count) {
                                printf("Invalid penguin selection. Please try again.\n");
                            }
                        }

                        int x = players[currentPlayerIndex].penguins[selected_penguin].x;
                        int y = players[currentPlayerIndex].penguins[selected_penguin].y;

                        if (board[x][y] == 5 + currentPlayerIndex) {
                            int target_x, target_y;
                            printf("Enter new coordinates (x y) for penguin: ");
                            scanf("%d %d", &target_x, &target_y);

                            if (target_x >= 0 && target_x < size.board_width && target_y >= 0 && target_y < size.board_height) {
                                int moveValid = penguins_movement(board, size.board_width, size.board_height, players[currentPlayerIndex].penguins, selected_penguin, target_x, target_y, numPlayers, players, currentPlayerIndex);
                                if (moveValid) {
                                    moveCompleted = 1; //finish move
                                    save(board, size.board_height, size.board_width, "saved_game.txt", penguin_quantity, players, numPlayers, penguin_quantity, currentPlayerIndex);
                                    printf("Current scores:\n");
                                    printPlayerScores(players, numPlayers);
                                }
                                else {
                                    printf("Invalid move. Please try again.\n");
                                }
                            }
                            else {
                                printf("Invalid coordinates. Please try again.\n");
                            }
                        }
                        else {
                            printf("This is not your penguin.\n");
                        }
                    }


                    // save
                    //save(board, size.board_height, size.board_width, "saved_game.txt", penguin_quantity, players, numPlayers, penguin_quantity, currentPlayerIndex);

                    // check if gameover
                    game_over = checkgameover_for_case2(board, size.board_width, size.board_height, players, numPlayers);
                    if (!game_over) {
                        currentPlayerIndex = (currentPlayerIndex + 1) % numPlayers;
                    }

                }


                printf("Game over! Final scores:\n");
                printPlayerScores(players, numPlayers);
                int maxFish = -1;
                int winnerIndex = -1;

                //find best scored player
                for (int i = 0; i < numPlayers; i++) {
                    if (players[i].fish_collected > maxFish) {
                        maxFish = players[i].fish_collected;
                        winnerIndex = i;
                    }
                }

                // //print winner
                if (winnerIndex != -1) {
                    printf("Game over! The winner is %s with %d fish!\n", players[winnerIndex].nickname, maxFish);
                }
                else {
                    printf("Game over! No winner.\n");
                }

            }
            else {
                printf("No saved game found.\n");
            }
            break;
        }
        case 3:
        {

            break;
        }
        default:
            printf("Invalid option, please try again.\n");
            break;
        }


    } while (input != 0);
    return 0;
}

