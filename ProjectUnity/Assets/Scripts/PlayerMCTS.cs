using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;


public class PlayerMCTS : MonoBehaviour
{
    Othello gameState;
    TreeNode tn = null;

    int simulationCount = 1000;

    private float bwaitTime = 0f;
    public Button[] m_Buttons;
    private Text endText;
    private GameObject endPanel;
    private Text pointsBlackT;
    private Text pointsWhiteT;
    public Button whoPlay;

    private GameObject startPanel;
    private Text startText;
    private InputField simulations;

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

        simulations = GameObject.Find("InputField").GetComponent<InputField>();
        startPanel = GameObject.Find("StartPanel");

        gameState = new Othello();

        foreach (var button in m_Buttons)
        {
            button.onClick.AddListener(() => Move(button.name));
        }
        
    }

    public void Play()
    {
        simulationCount = int.Parse(simulations.text);
        startPanel.SetActive(false);

        drawBoard(gameState);
        if (UnityEngine.Random.Range(0, 2) == 1)
        {
            StartCoroutine("moveBlack", bwaitTime);
        }
    }

    private void Update()
    {
        if(gameState.player == 1)
        {
            whoPlay.image.color = Color.black;
        }
        if (gameState.player == 2)
        {
            whoPlay.image.color = Color.white;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("PlayerMCTS");
    }

    public void Move(string buttonName)
    {
        int index = 0;
        for (int i = 0; i < m_Buttons.Length; i++)
        {
            if (m_Buttons[i].name.Equals(buttonName))
            {
                index = i;
                break;
            }
        }

        int col = index / 8;
        int row = index % 8;
        if (gameState.Board[col, row] == 0 && gameState.CheckClosed(gameState.Board, col, row, false))
        {
            gameState.Board[col, row] = gameState.player;
            if (gameState.player == 1)
            {
                m_Buttons[index].image.color = Color.black;
            }
            else if (gameState.player == 2)
            {
                m_Buttons[index].image.color = Color.white;
            }
            //StartCoroutine("ChangeCells", waitTime);
            drawBoard(gameState);
            gameState.NextTurn();
            if (gameState.CanMove(gameState.Board) == true)
            {
                StartCoroutine("moveBlack", bwaitTime);
            }
            else
            {
                gameState.NextTurn();
                if (gameState.CanMove(gameState.Board) == false)
                {
                    End();
                }
            }
        }
    }

    void End()
    {
        int blackPoints = gameState.CountPoints(gameState.Board, 1);
        int whitePoints = gameState.CountPoints(gameState.Board, 2);

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
        //Restart();
    }

    static void SaveResult(string Winner)
    {
        string path = "Assets/Resources/test.txt";
        StreamWriter writer = new StreamWriter(path, true);

        writer.WriteLine(Winner);
        writer.Close();
    }

    IEnumerator moveBlack(float Count)
    {
        yield return new WaitForSeconds(Count);

        Othello ttt = new Othello(gameState);
        tn = new TreeNode(ttt);

        test();

        TreeNode BC = tn.bestChild();
        int index = BC.lastMove;
        int col = index / 8;
        int row = index % 8;    

        if (gameState.Board[col, row] == 0 && gameState.CheckClosed(gameState.Board, col, row, false))
        {
            gameState.Board[col, row] = gameState.whoseMove();
        }

        if (gameState.whoseMove() == 1)
        {
            m_Buttons[index].image.color = Color.black;
        }
        else if (gameState.whoseMove() == 2)
        {
            m_Buttons[index].image.color = Color.white;
        }

        drawBoard(gameState);
        gameState.NextTurn();
        
        if (gameState.CanMove(gameState.Board) == false)
        {
            gameState.NextTurn();

            if (gameState.CanMove(gameState.Board) == false)
            {                
                End();
            }
            else
            {
                StartCoroutine("moveBlack", bwaitTime);
            }
        }
        yield return null;
    }
    
    void test()
    {
        for (int i = 0; i < simulationCount; i++)
        {
            tn.selectAction();
        }
    }

    public void drawBoard(Othello gs)
    {
        int[] changedButtons = new int[64];
        int buttonIndex = 0;

        int blackPoints = gameState.CountPoints(gameState.Board, 1);
        int whitePoints = gameState.CountPoints(gameState.Board, 2);

        string scoreBlack = blackPoints.ToString();
        string scoreWhite = whitePoints.ToString();

        pointsBlackT.text = scoreBlack;
        pointsWhiteT.text = scoreWhite;

        for (int i = 0; i < m_Buttons.Length; i++)
        {
            int col = i / 8;
            int row = i % 8;

            m_Buttons[i].GetComponent<Button>().interactable = false;

            if (gs.Board[col, row] != 0)
            {
                changedButtons[buttonIndex] = i;
                buttonIndex++;
            }
        }        

        for (int i = 0; i < buttonIndex; i++)
        {
            int index = changedButtons[i];
            int col = index / 8;
            int row = index % 8;            

            if (gs.Board[col, row] == 1)
            {
                m_Buttons[index].image.color = Color.black;
            }
            else if (gs.Board[col, row] == 2)
            {
                m_Buttons[index].image.color = Color.white;
            }            
        }

        foreach (var button in m_Buttons)
        {
            button.GetComponent<Button>().interactable = true;
        }
    }
}