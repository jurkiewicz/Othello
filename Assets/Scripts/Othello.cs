using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
public class Othello
{
    public int[,] Board = new int[8, 8];
    public int player = 0;
    public int points;

    public Othello()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if ((i == 3 && j == 3) || (i == 4 && j == 4))
                {
                    Board[i, j] = 2;
                }
                else if ((i == 3 && j == 4) || (i == 4 && j == 3))
                {
                    Board[i, j] = 1;
                }
                else
                {
                    Board[i, j] = 0;
                }
            }
        }
        player = 1;
    }

    public Othello(Othello ttt)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Board[i, j] = ttt.Board[i, j];
            }
        }
        player = ttt.player;
    }

    public int whoseMove()
    {
        return player;
    }

    public void makeMove(int move)
    {
        int i = move / 8;
        int j = move % 8;
        if (Board[i, j] == 0 && CheckClosed(Board, i, j, false))
        {
            Board[i, j] = player;
        }
    }

    public void NextTurn()
    {
        if (player == 1)
        {
            player = 2;
        }
        else
        {
            player = 1;
        }
    }

    public ArrayList availableMoves()
    {
        ArrayList am = new ArrayList(64);
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (this.Board[i, j] == 0 && this.CheckClosed(this.Board, i, j, true))
                {
                    am.Add(8 * i + j);
                }
            }
        }
        return am;
    }

    public bool CheckClosed(int[,] board, int col, int row, bool trying)
    {
        points = 0;
        bool closed = false;
        int cells = player == 1 ? 2 : 1;
        int x;
        int y;
        int[,] directions = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 }, { 1, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 }, };

        for (int i = 0; i < 8; i++)
        {
            x = col;
            y = row;

            x += directions[i, 0];
            y += directions[i, 1];

            if (IsOnBoard(x, y) && board[x, y] == cells)
            {
                x += directions[i, 0];
                y += directions[i, 1];

                if (IsOnBoard(x, y))
                {
                    while (board[x, y] == cells)
                    {
                        x += directions[i, 0];
                        y += directions[i, 1];

                        if (!IsOnBoard(x, y))
                        {
                            break;
                        }
                    }

                    if (IsOnBoard(x, y) && board[x, y] == player)
                    {
                        closed = true;

                        if (i == 0)
                        {
                            Rate(x, y);
                        }

                        while (true)
                        {
                            x -= directions[i, 0];
                            y -= directions[i, 1];

                            if (x == col && y == row)
                            {
                                break;
                            }

                            Rate(x, y);

                            if (!trying)
                            {
                                board[x, y] = player;
                            }
                        }
                    }
                }
            }
        }
        return closed;
    }

    void Rate(int col, int row)
    {
        if ((col == 0 && row == 0) || (col == 0 && row == 7) || (col == 7 && row == 0) || (col == 7 && row == 7))
        {
            points += 9;
        }
        else if (col == 0 || row == 0 || col == 7 || row == 7)
        {
            points += 7;
        }
        else if (col == 1 || row == 1 || col == 6 || row == 6)
        {
            points += 1;
        }
        else
        {
            points += 2;
        }
    }

    bool IsOnBoard(int x, int y)
    {
        return x >= 0 && x <= 7 && y >= 0 && y <= 7;
    }

    public int checkWin()
    // return 1 if end of the game and -1 if no winner yet
    {
        int state = -1;
        int savePlayer = this.player;
        if (this.CanMove(this.Board) == false)
        {
            this.NextTurn();

            if (this.CanMove(this.Board) == false)
            {
                int blackPoints = this.CountPoints(this.Board, 1);
                int whitePoints = this.CountPoints(this.Board, 2);

                if (blackPoints > whitePoints)
                {
                    state = 1;
                }
                else if (blackPoints < whitePoints)
                {
                    state = 2;
                }
                else
                {
                    state = 0;
                }
            }
            else
            {
                state = -2;
            }
        }
        else
        {
            state = -1;
        }
        this.player = savePlayer;
        return state;
    }

    public bool CanMove(int[,] board)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] == 0 && CheckClosed(board, i, j, true))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public int CountPoints(int[,] board, int side)
    {
        int endPoints = 0;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] == side)
                {
                    endPoints++;
                }
            }
        }

        return endPoints;
    }
}
