using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class AI : MonoBehaviour
{

    public Button[] m_Buttons;
    public float waitTime, enemyWait;

    private int[,] mainBoard = new int[8, 8];
    private int player, points;
    private Text endText;
    private GameObject endPanel;
    private Text pointsBlackT;
    private Text pointsWhiteT;
    private Button whoPlay;

    // Use this for initialization
    void Start()
    {
        endText = GameObject.Find("EndText").GetComponent<Text>();
        endText.enabled = false;

        endPanel = GameObject.Find("EndPanel");
        endPanel.SetActive(false);

        pointsBlackT = GameObject.Find("TBlack").GetComponent<Text>();
        pointsWhiteT = GameObject.Find("TWhite").GetComponent<Text>();

        whoPlay = GameObject.Find("WhoPlay").GetComponent<Button>();

        player = 1;
        waitTime = 0f;
        enemyWait = 0f;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if ((i == 3 && j == 3) || (i == 4 && j == 4))
                {
                    mainBoard[i, j] = 2;
                }
                else if ((i == 3 && j == 4) || (i == 4 && j == 3))
                {
                    mainBoard[i, j] = 1;
                }
                else
                {
                    mainBoard[i, j] = 0;
                }
            }
        }

        StartCoroutine("ChangeCells", waitTime);
        StartCoroutine("AIMoveBlack", enemyWait);
    }

    public void Restart()
    {
        SceneManager.LoadScene("AI");
    }

    IEnumerator ChangeCells(float Count)
    {
        int[] changedButtons = new int[64];
        int buttonIndex = 0;

        int blackPoints = CountPoints(mainBoard, 1);
        int whitePoints = CountPoints(mainBoard, 2);

        string scoreBlack = blackPoints.ToString();
        string scoreWhite = whitePoints.ToString();

        pointsBlackT.text = scoreBlack;
        pointsWhiteT.text = scoreWhite;

        for (int i = 0; i < m_Buttons.Length; i++)
        {
            int col = i / 8;
            int row = i % 8;

            m_Buttons[i].GetComponent<Button>().interactable = false;

            if (mainBoard[col, row] != 0)
            {
                changedButtons[buttonIndex] = i;
                buttonIndex++;
            }
        }

        for (int i = 0; i < buttonIndex; i++)
        {
            yield return new WaitForSeconds(Count);
            int index = changedButtons[i];
            int col = index / 8;
            int row = index % 8;

            if (mainBoard[col, row] == 1)
            {
                m_Buttons[index].image.color = Color.black;
            }
            else if (mainBoard[col, row] == 2)
            {
                m_Buttons[index].image.color = Color.white;
            }

            yield return null;

            foreach (var button in m_Buttons)
            {
                button.GetComponent<Button>().interactable = true;
            }
        }
    }

    bool CheckClosed(int[,] board, int col, int row, bool trying)
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

                        if (points > 0)
                        {
                            //SaveRate(points, player);
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

    static void SaveRate(int points, int side)
    {
        string path = side == 1 ? "Assets/Resources/pointsB.txt" : "Assets/Resources/pointsW.txt";
        string pl = side == 1 ? "Black" : "White";

        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(points + "\t" + pl);
        writer.Close();
    }

    bool IsOnBoard(int x, int y)
    {
        return x >= 0 && x <= 7 && y >= 0 && y <= 7;
    }

    bool CanMove(int[,] board)
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

    void NextTurn()
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

    int CountPoints(int[,] board, int side)
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

    void End()
    {
        int blackPoints = CountPoints(mainBoard, 1);
        int whitePoints = CountPoints(mainBoard, 2);

        string scoreB = blackPoints.ToString();
        string scoreW = whitePoints.ToString();
        endText.enabled = true;
        endPanel.SetActive(true);

        if (blackPoints > whitePoints)
        {
            endText.text = "Koniec gry\nWygrał czarny\nZdobył " + scoreB + " punktów\nDzięki za grę";
            SaveResult("Black");
        }
        else if (blackPoints < whitePoints)
        {
            endText.text = "Koniec gry\nWygrał biały\nZdobył " + scoreW + " punktów\nDzięki za grę";
            SaveResult("White");
        }
        else
        {
            endText.text = "Koniec gry\nRemis\nDzięki za grę";
            SaveResult("Draw");
        }
        Restart();
    }

    private void Update()
    {
        if (player == 1)
        {
            whoPlay.image.color = Color.black;
        }
        if (player == 2)
        {
            whoPlay.image.color = Color.white;
        }
    }

    static void SaveResult(string Winner)
    {
        string path = "Assets/Resources/test.txt";
        StreamWriter writer = new StreamWriter(path, true);

        writer.WriteLine(Winner);
        writer.Close();
    }
    

    IEnumerator AIMoveBlack(float Count)
    {
        //MiniMax
        //
        //Wszystkie dostępne ruchy, dla każdego ruchu po trzy parametry [pozycjaX, pozycjaY, ocenaPola]
        yield return new WaitForSeconds(Count);
        int col = 0;
        int row = 0;
        
        int[,] allMoves = new int[60, 3];
        int movesIndex = 0;
        int min = 0;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (mainBoard[i, j] == 0 && CheckClosed(mainBoard, i, j, true))
                {
                    allMoves[movesIndex, 0] = i;
                    allMoves[movesIndex, 1] = j;
                    allMoves[movesIndex, 2] = points;

                    NextTurn();

                    for (int a = 0; a < 8; a++)
                    {
                        for (int b = 0; b < 8; b++)
                        {
                            if (mainBoard[a, b] == 0 && CheckClosed(mainBoard, a, b, true) && points > min)
                            {
                                min = points;
                            }
                        }
                    }

                    NextTurn();
                    allMoves[movesIndex, 2] -= min;
                    movesIndex++;
                }
            }
        }

        int[] max = new int[3];
        max[2] = int.MinValue;

        for (int i = 0; i < movesIndex; i++)
        {
            if (allMoves[i, 2] >= max[2])
            {
                max[0] = allMoves[i, 0];
                max[1] = allMoves[i, 1];
                max[2] = allMoves[i, 2];
            }
        }
        col = max[0];
        row = max[1];

        if (mainBoard[col, row] == 0 && CheckClosed(mainBoard, col, row, false))
        {
            mainBoard[col, row] = player;
        }

        int index = col * 8 + row;

        if (player == 1)
        {
            m_Buttons[index].image.color = Color.black;
        }
        else if (player == 2)
        {
            m_Buttons[index].image.color = Color.white;
        }

        StartCoroutine("ChangeCells", waitTime);
        NextTurn();

        if (CanMove(mainBoard) == false)
        {
            NextTurn();

            if (CanMove(mainBoard) == false)
            {
                End();
            }
            else
            {
                StartCoroutine("AIMoveBlack", enemyWait);
            }
        }
        else
        {
            StartCoroutine("AIMoveWhite", enemyWait);
        }
        yield return null;
    }

    IEnumerator AIMoveWhite(float Count)
    {
        yield return new WaitForSeconds(Count);
        int col = 0;
        int row = 0;

        //Sztywna strategia na zdobywanie krawędzi
        //
        bool strategy = false;
        //for (int i = 0; i < 8; i++)
        //{
        //    if (board[0, i] == 0 && CheckClosed(0, i, false))
        //    {
        //        col = 0;
        //        row = i;
        //        strategy = true;
        //        break;
        //    }
        //    else if (board[7, i] == 0 && CheckClosed(7, i, false))
        //    {
        //        col = 7;
        //        row = i;
        //        strategy = true;
        //        break;
        //    }
        //    else if (board[i, 0] == 0 && CheckClosed(i, 0, false))
        //    {
        //        col = i;
        //        row = 0;
        //        strategy = true;
        //        break;
        //    }
        //    else if (board[i, 7] == 0 && CheckClosed(i, 7, false))
        //    {
        //        col = i;
        //        row = 7;
        //        strategy = true;
        //        break;
        //    }
        //}

        if (!strategy)
        {
            do
            {
                col = Random.Range(0, 8);
                row = Random.Range(0, 8);
            } while (mainBoard[col, row] != 0 || !CheckClosed(mainBoard,col, row, true));
        }

        //MiniMax
        //
        //Wszystkie dostępne ruchy, dla każdego ruchu po trzy parametry [pozycjaX, pozycjaY, ocenaPola]
        //int[,] allMoves = new int[60, 3];
        //int movesIndex = 0;
        //int min = 0;

        //for (int i = 0; i < 8; i++)
        //{
        //    for (int j = 0; j < 8; j++)
        //    {
        //        if (mainBoard[i, j] == 0 && CheckClosed(mainBoard, i, j, true))
        //        {
        //            allMoves[movesIndex, 0] = i;
        //            allMoves[movesIndex, 1] = j;
        //            allMoves[movesIndex, 2] = points;

        //            NextTurn();

        //            for (int a = 0; a < 8; a++)
        //            {
        //                for (int b = 0; b < 8; b++)
        //                {
        //                    if (mainBoard[a, b] == 0 && CheckClosed(mainBoard, a, b, true) && points > min)
        //                    {
        //                        min = points;
        //                    }
        //                }
        //            }

        //            NextTurn();
        //            allMoves[movesIndex, 2] -= min;
        //            movesIndex++;
        //        }
        //    }
        //}

        //int[] max = new int[3];
        //max[2] = int.MinValue;

        //for (int i = 0; i < movesIndex; i++)
        //{
        //    if (allMoves[i, 2] >= max[2])
        //    {
        //        max[0] = allMoves[i, 0];
        //        max[1] = allMoves[i, 1];
        //        max[2] = allMoves[i, 2];
        //    }
        //}

        //col = max[0];
        //row = max[1];

        if (mainBoard[col, row] == 0 && CheckClosed(mainBoard, col, row, false))
        {
            mainBoard[col, row] = player;
        }

        int index = col * 8 + row;

        if (player == 1)
        {
            m_Buttons[index].image.color = Color.black;
        }
        else if (player == 2)
        {
            m_Buttons[index].image.color = Color.white;
        }

        StartCoroutine("ChangeCells", waitTime);
        NextTurn();

        if (CanMove(mainBoard) == false)
        {
            NextTurn();

            if (CanMove(mainBoard) == false)
            {
                End();
            }
            else
            {
                StartCoroutine("AIMoveWhite", enemyWait);
            }
        }
        else
        {
            StartCoroutine("AIMoveBlack", enemyWait);
        }
        yield return null;
    }
}
