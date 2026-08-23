
#ifndef PLAYER_H
#define PLAYER_H

#define MAX_PENGUINS 4 //each player have no more than 4 penguin


typedef struct {
    int x;
    int y;
} PenguinPosition;

typedef struct {
    char nickname[50];
    int player_number;
    int fish_collected;
    PenguinPosition penguins[10];
    int penguin_count;
    
} Player;

void printPlayerScores(Player* players, int numPlayers);

#endif  // PLAYER_H
