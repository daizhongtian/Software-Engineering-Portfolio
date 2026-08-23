
#define MAX_PLAYERS 8  
#define MAX_PENGUINS_PER_PLAYER 10
#define _CRT_SECURE_NO_WARNINGS
#include"function.h"
#include <stdio.h>
#include <windows.h>
#include "player.h"

void read_board_size(struct board_size* size) {
    while (1) {
        printf("Enter board properties (width, height): ");
        if (scanf("%d %d", &size->board_width, &size->board_height) == 2 &&
            size->board_width > 0 && size->board_height > 0) {
            break;
        }
        else {
            printf("Invalid board dimensions, try again.\n");
            while (getchar() != '\n');
        }
    }

}

int read_penguin_quantity(int* quantity, struct board_size size) {
    while (1)
    {
        printf("Enter penguins quantity: ");




        if (scanf("%d", quantity) != 1 || *quantity <= 0 ||
            *quantity > (size.board_width * size.board_height)) {
            printf("Invalid penguin quantity, try again\n");

            while (getchar() != '\n'); //additional 
        }
        else {
            break;
        }
    }
    return 0;
}



int(*allocate_coordinates(int quantity))[2] {
    return (int(*)[2])malloc(quantity * sizeof(int[2]));
    }



    void free_memory(int(*coords)[2]) {
        free(coords);
    }

    int read_coordinates_overlap(int(*coords)[2], int quantity, struct board_size size) {
        while (1) { // Repeat until valid input is provided
            int allValid = 1; // Flag to track overall validity of all coordinates

            for (int i = 0; i < quantity; ++i)
            {

                printf("Enter coordinates for penguin %d (x, y): ", i + 1);


                int x = coords[i][0];
                int y = coords[i][1];
                //printf("%d, %d\t", penguin_coords[i][0], penguin_coords[i][1]); //swapped these i and 1



                if (scanf("%d %d", &coords[i][0], &coords[i][1]) != 2) {
                    printf("Invalid input, try again.\n");
                    allValid = 0;
                    while (getchar() != '\n'); // Clear buffer
                    break; // Exit the current loop, start over
                }

                if (coords[i][0] < 0 || coords[i][0] > size.board_width ||
                    coords[i][1] < 0 || coords[i][1] > size.board_height) {
                    printf("Coordinates out of range, try again.\n");
                    allValid = 0;
                    while (getchar() != '\n'); // Clear buffer
                    break; // Exit the current loop, start over
                }
                
                // Check for overlap
                for (int j = 0; j < i; ++j) {
                    if (coords[i][0] == coords[j][0] && coords[i][1] == coords[j][1]) {
                        printf("Overlap detected, try again.\n");
                        allValid = 0;
                        while (getchar() != '\n'); // Clear buffer
                        break; // Exit the inner loop
                    }
                }


                //if (!allValid) {
                //    break; // Exit the outer loop, start over
                //}

            }

            if (allValid) {
                return 0; // All coordinates are valid, exit the function
            }
        }
        // If not allValid, the loop will continue and prompt for all coordinates again
    }




    //initializeboard

    int** initializeBoard(int rows, int cols) {
        int** board = (int**)malloc(rows * sizeof(int*));

        //int fish[rows][cols];
        if (board == NULL)
        {
            return NULL;
        }
        //
        for (int i = 0; i < rows; i++)
        {
            board[i] = (int*)malloc(cols * sizeof(int));
             if (board[i] == NULL) {

                 for (int j = 0; j < i; j++) {

                     free(board[j]);
                 }
                 free(board);
                 return NULL;
             }

            for (int j = 0; j < cols; j++) {
                board[i][j] = rand() % 4;
                //fish[i][j] = board[i][j];
            }
        }

        if (board == NULL) {
            printf("Board is NULL.\n");
            return;
        }
        return board;
    }



    void printBoard(int** board, int rows, int cols) {
        if (board == NULL) {
            printf("Board is NULL.\n");
            return;
        }

        HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);


        printf("    ");
        for (int col = 0; col < cols; col++) {
            printf("%3d ", col);
        }
        printf("\n");
        int penguinColor = 0;
        for (int row = 0; row < rows; row++) {

            printf("%3d ", row);


            for (int col = 0; col < cols; col++) {

                if (board[row][col] >= 5 && board[row][col] <= 9) {


                    switch (penguinColor) {
                    case 0:
                        SetConsoleTextAttribute(hConsole, FOREGROUND_RED);
                        penguinColor++;

                        break;
                    case 1:
                        SetConsoleTextAttribute(hConsole, FOREGROUND_GREEN);
                        penguinColor++;
                        break;
                    case 2:
                        SetConsoleTextAttribute(hConsole, FOREGROUND_RED | FOREGROUND_GREEN);
                        penguinColor++;
                        break;
                    case 3:
                        SetConsoleTextAttribute(hConsole, FOREGROUND_RED | FOREGROUND_BLUE);
                        penguinColor++;
                        break;
                    case 4:
                        SetConsoleTextAttribute(hConsole, FOREGROUND_BLUE | FOREGROUND_GREEN);
                        penguinColor++;
                        break;
                    }
                    printf("%3d ", board[row][col]);
                    SetConsoleTextAttribute(hConsole, FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE);
                }
                else {
                    printf("%3d ", board[row][col]);
                }
            }
            printf("\n");
        }
    }




    // free memory
    void freeBoard(int rows, int cols, int** board) {

        for (int i = 0; i < rows; i++) {
            free(board[i]);
        }
        free(board);
    }


    void menu() {

        HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
        SetConsoleTextAttribute(hConsole, FOREGROUND_RED | FOREGROUND_GREEN);
        printf("******************************\n");
        printf("*****1.Play(PVP) new game ****\n");
        printf("*****2.continue last game ****\n");
        printf("**** 0.Exit ******************\n");
        printf("******************************\n");
        SetConsoleTextAttribute(hConsole, FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE);


    }


    int penguins_movement(int** board, int board_width, int board_height, int(*penguin_coords)[2], int penguin_index, int target_x, int target_y, int quantity, Player* players, int currentPlayerIndex)
    {
        
        int x = penguin_coords[penguin_index][0];
        int y = penguin_coords[penguin_index][1];

        // penguin for each player
        int penguin_code = 5 + currentPlayerIndex; // 5 6 7...

        // check if selected penguin is belone right player
        if (board[x][y] == penguin_code) {
            // check if target is in board
            if (target_x >= 0 && target_x < board_width && target_y >= 0 && target_y < board_height) {
                // check movement
                if ((abs(x - target_x) == 1 && y == target_y) || (abs(y - target_y) == 1 && x == target_x)) {
                    // check if target have other penguin 
                    if (board[target_x][target_y] > 0 && board[target_x][target_y] <= 4) {
                        // make movement
                        int fish = board[target_x][target_y];
                        board[x][y] = 0; // clean initial place
                        penguin_coords[penguin_index][0] = target_x; //update new place for penguin
                        penguin_coords[penguin_index][1] = target_y;
                        board[target_x][target_y] = penguin_code;
                        players[currentPlayerIndex].fish_collected += fish; // calculate score
                        return 1;//move successful
                    }
                    else {
                        printf("Invalid move: Target position is not valid.\n");
                    }
                }
                else {
                    printf("Invalid move: Too far.\n");
                }
            }
            else {
                printf("Invalid move: Coordinates out of range.\n");
            }
        }
        else {
            printf("This is not your penguin.\n");
        }
        return 0; //move failed
    }




    void save(int** board, int height, int width, const char* filepath, int penguin_index, Player* players, int numPlayers, int quantity,int currentPlayerIndex) {

        FILE* file = fopen(filepath, "w");

        if (file == NULL) {
            printf("Error\n");
            return;
        }
        fprintf(file, "%d %d\n", height, width);//size of board      
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                fprintf(file, "%d ", board[i][j]);
            }
            fprintf(file, "\n");
        }

        fprintf(file, "%d\n", numPlayers);


        for (int i = 0; i < numPlayers; i++) {
            fprintf(file, "%s %d %d\n", players[i].nickname, players[i].player_number, players[i].fish_collected);
        }
        fprintf(file, "%d\n", currentPlayerIndex);

        fclose(file);
    }
    


    
    void findPenguinPositions(int** board, int board_width, int board_height, Player* players)
    {
        // 将所有玩家的 penguin_count 先清 0
        for (int p = 0; p < MAX_PLAYERS; ++p) {
            players[p].penguin_count = 0;
        }

        // 扫描整个棋盘
        for (int i = 0; i < board_height; ++i) {
            for (int j = 0; j < board_width; ++j) {
                int cellValue = board[i][j];
                // 如果这个格子里是某位玩家的企鹅 (5,6,7...表示玩家)
                if (cellValue >= 5 && cellValue < 5 + MAX_PLAYERS) {
                    int playerIndex = cellValue - 5;
                    Player* player = &players[playerIndex];
                    // 将该格子坐标记录到 player->penguins[]
                    if (player->penguin_count < MAX_PENGUINS_PER_PLAYER) {
                        player->penguins[player->penguin_count].x = i;
                        player->penguins[player->penguin_count].y = j;
                        player->penguin_count++;
                    }
                }
            }
        }
    }


    void printPenguinsAtStart(int** board, int board_width, int board_height, Player* players, int numPlayers) {
        int penguin_coords[MAX_PLAYERS][MAX_PENGUINS][2];
        int penguin_count[MAX_PLAYERS] = { 0 };

        

        findPenguinPositions(board, board_width, board_height, players);


        //print
        for (int i = 0; i < numPlayers; ++i) {
            printf("Player %s's penguins:\n", players[i].nickname);
            for (int j = 0; j < penguin_count[i]; ++j) {
                printf("Penguin %d at (%d, %d)\n", j, penguin_coords[i][j][0], penguin_coords[i][j][1]);
            }
        }
    }

            
    void loadGameState(char* filepath, int*** board, Player* players, int* numPlayers, struct board_size* size, int *currentPlayerIndex) {
        FILE* file = fopen(filepath, "r");
        if (!file) {
            printf("Unable to open file for reading.\n");
            return;
        }

        // Read the size of the board
        if (fscanf(file, "%d %d\n", &size->board_width, &size->board_height) != 2) {
            printf("Error reading board dimensions.\n");
            fclose(file);
            return;
        }

        // Allocate memory for the board
        *board = (int**)malloc(size->board_height * sizeof(int*));
        for (int i = 0; i < size->board_height; ++i) {
            (*board)[i] = (int*)malloc(size->board_width * sizeof(int));
            if ((*board)[i] == NULL) {
                printf("Memory allocation failed for board row %d.\n", i);
                // Clean up already allocated memory
                for (int j = 0; j < i; ++j) {
                    free((*board)[j]);
                }
                free(*board);
                fclose(file);
                return;
            }
        }

        // Read board status
        for (int i = 0; i < size->board_height; ++i) {
            for (int j = 0; j < size->board_width; ++j) {
                if (fscanf(file, "%d", &(*board)[i][j]) != 1) {
                    printf("Error reading board data at position [%d][%d].\n", i, j);
                    // Clean up memory
                    for (int k = 0; k < size->board_height; ++k) {
                        free((*board)[k]);
                    }
                    free(*board);
                    fclose(file);
                    return;
                }
            }
        }

        // Read the number of players
        if (fscanf(file, "%d\n", numPlayers) != 1) {
            printf("Error reading the number of players.\n");
            // Clean up memory (注释)
            for (int i = 0; i < size->board_height; ++i) {
                free((*board)[i]);
            }
            free(*board);
            fclose(file);
            return;
        }
       

        // Ensure we do not exceed the max number of players
        *numPlayers = (*numPlayers > MAX_PLAYERS) ? MAX_PLAYERS : *numPlayers;

        // Read player data
        for (int i = 0; i < *numPlayers; ++i) {
            // Assuming Player has a nickname char array of fixed size
            if (fscanf(file, "%s %d %d\n", &players[i].nickname, &players[i].player_number, &players[i].fish_collected) != 3) {//read nickname,number of player,and fish_collected
                printf("Error reading data for player %d.\n", i + 1);
                // Clean up memory
             
                for (int j = 0; j < size->board_height; ++j) {
                    free((*board)[j]);
                }
                free(*board);
                fclose(file);
                return;
            }                    
           
        }

        if (fscanf(file, "%d\n", currentPlayerIndex) != 1) {
            printf("Error reading board dimensions.\n");
            fclose(file);
            return;
        }
       
        //printf("%d\n", *currentPlayerIndex);
        
        
        fclose(file);
    }







    int checkGameOver(int** board, int board_width, int board_height, int(*penguin_coords)[2], int penguin_quantity, int numPlayers) {
        for (int i = 0; i < penguin_quantity; i++)
        {
            int x = penguin_coords[i][0];
            int y = penguin_coords[i][1];
            int penguin_code = board[x][y];

            
            if (penguin_code >= 5 && penguin_code < 5 + numPlayers) {
                // check if there is avaliable place to move
                if ((x > 0 && board[x - 1][y] > 0 && board[x - 1][y] <= 4) ||//check left
                    (x < board_width - 1 && board[x + 1][y] > 0 && board[x + 1][y] <= 4) ||//check right
                    (y > 0 && board[x][y - 1] > 0 && board[x][y - 1] <= 4) ||//check up
                    (y < board_height - 1 && board[x][y + 1] > 0 && board[x][y + 1] <= 4)) {//check down
                    return 0; // it means there at least 1 penguin can move
                }
            }
        }
        return 1; //all penguin can not move ,game over
    }



    int checkgameover_for_case2(int** board, int board_width, int board_height, Player* players, int numPlayers)
    {
        
        for (int i = 0; i < numPlayers; i++) {
            
            for (int j = 0; j < players[i].penguin_count; j++) {
                int x = players[i].penguins[j].x;
                int y = players[i].penguins[j].y;

                // check if there  is possible place to move
                // check left
                if (x > 0 && board[x - 1][y] > 0 && board[x - 1][y] <= 4) {
                    return 0; 
                }
                // check right 
                if (x < board_width - 1 && board[x + 1][y] > 0 && board[x + 1][y] <= 4) {
                    return 0;
                }
                // check up
                if (y > 0 && board[x][y - 1] > 0 && board[x][y - 1] <= 4) {
                    return 0;
                }
                // check down
                if (y < board_height - 1 && board[x][y + 1] > 0 && board[x][y + 1] <= 4) {
                    return 0;
                }
                
            }
        }
        return 1; 
    }




    int canPenguinMove(int** board, int board_width, int board_height, int x, int y) {
        // check left right up down
        if ((x > 0 && board[x - 1][y] > 0 && board[x - 1][y] <= 4) ||
            (x < board_width - 1 && board[x + 1][y] > 0 && board[x + 1][y] <= 4) ||
            (y > 0 && board[x][y - 1] > 0 && board[x][y - 1] <= 4) ||
            (y < board_height - 1 && board[x][y + 1] > 0 && board[x][y + 1] <= 4)) {
            return 1; //it is movable
        }
        return 0; //cannot move
    }


