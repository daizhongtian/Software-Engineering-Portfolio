
#include <windows.h>
#include"player.h"


void initializePlayers(Player players[], int numPlayers) {
    for (int i = 0; i < numPlayers; i++) {
       
        players[i].player_number = i + 1;          
        players[i].fish_collected = 0;              
    }
}



void printPlayerScores(Player* players, int numPlayers) {
    
    HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);

    
    SetConsoleTextAttribute(hConsole, FOREGROUND_RED);

    printf("\nPlayer Scores:\n");
    for (int i = 0; i < numPlayers; i++) {
        printf("Player %s: %d fish\n", players[i].nickname, players[i].fish_collected);
    }

    
    SetConsoleTextAttribute(hConsole, FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE);
}
