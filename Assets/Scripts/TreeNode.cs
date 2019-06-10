using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TreeNode
{
    static System.Random r = new System.Random();
    static double epsilon = 1e-6;
    static double Cp = 2 * (1 / Math.Sqrt(2));

    public TreeNode[] children;
    public double nVisits, totValue;
    public Othello gameState;
    public int lastMove = 0;

    public TreeNode(Othello ttt)
    {
        gameState = new Othello(ttt);
    }

    public void selectAction()
    {
        LinkedList<TreeNode> visited = new LinkedList<TreeNode>();
        TreeNode cur = this;
        visited.AddLast(this);
        while (!cur.isLeaf())
        {
            cur = cur.select();
            visited.AddLast(cur);
        }
        if (cur.gameState.checkWin() == -1)
        {
            cur.expand();
            TreeNode newNode = cur.select();
            if (newNode != null)
            {
                visited.AddLast(newNode);
                double value = rollOut(newNode);
                foreach (TreeNode node in visited)
                {
                    node.updateStats(value);
                }
            }
            else Debug.LogError("select action selected a null node");
        }
        else if (cur.gameState.checkWin() == -2)
        {
            cur.gameState.NextTurn();
            cur.expand();
            TreeNode newNode = cur.select();
            if (newNode != null)
            {
                visited.AddLast(newNode);
                double value = rollOut(newNode);
                foreach (TreeNode node in visited)
                {
                    node.updateStats(value);
                }
            }
            else Debug.LogError("select action selected a null node");
            cur.gameState.NextTurn();
        }
        else
        {
            double value = 0;
            switch (cur.gameState.checkWin())
            {
                case 1:
                    value = 1.0;
                    break;
                case 2:
                case 0:
                    value = 0.0;
                    break;
                default:
                    break;
            }
            foreach (TreeNode node in visited)
            {
                node.updateStats(value);
            }
        }
    }

    public void expand()
    {
        ArrayList am;
        
        am = this.gameState.availableMoves();
        if (am.Count > 0)
        {
            children = new TreeNode[am.Count];
            for (int i = 0; i < am.Count; i++)
            {
                children[i] = new TreeNode(gameState);
                children[i].gameState.makeMove((int)am[i]);
                children[i].lastMove = (int)am[i];
            }
        }
    }

    private TreeNode select()
    {
        TreeNode selected = null;
        double bestValue = Double.MinValue;
        foreach (TreeNode c in children)
        {
            double uctValue =
                c.totValue / (c.nVisits + epsilon) +
                    Cp * Math.Sqrt(2 * Math.Log(nVisits + 1) / (c.nVisits + epsilon)) +
                    r.NextDouble() * epsilon;

            if (uctValue > bestValue)
            {
                selected = c;
                bestValue = uctValue;
            }
        }
        return selected;
    }

    public bool isLeaf()
    {
        return children == null;
    }

    public double rollOut(TreeNode tn)
    {
        Othello rollGS = new Othello(tn.gameState);
        bool stillPlaying = true;
        double rc = 0;
        int moveIndex;
        ArrayList am = rollGS.availableMoves();
        
        while (am.Count > 0 && stillPlaying)
        {
            moveIndex = r.Next(0, am.Count);
            
            int move = (int)am[moveIndex];
            rollGS.makeMove(move);

            rollGS.NextTurn();

            if (rollGS.CanMove(rollGS.Board) == false)
            {
                rollGS.NextTurn();

                if (rollGS.CanMove(rollGS.Board) == false)
                {
                    stillPlaying = false;
                    int blackPoints = rollGS.CountPoints(rollGS.Board, 1);
                    int whitePoints = rollGS.CountPoints(rollGS.Board, 2);

                    if (blackPoints > whitePoints)
                    {
                        rc = 1.0;
                    }
                    else if (blackPoints <= whitePoints)
                    {
                        rc = 0.0;
                    }
                }
            }
            am = rollGS.availableMoves();
        }
        
        return rc;
    }

    public void updateStats(double value)
    {
        nVisits++;
        totValue += value;
    }

    public int arity()
    {
        return children == null ? 0 : children.Length;
    }

    public TreeNode bestChild()
    {
        TreeNode bestChild = null;

        for (int i = 0; i < children.Length; i++)
        {
            if (bestChild == null)
            {
                bestChild = children[i];
            }
            else if (children[i].totValue > bestChild.totValue)
            {
                bestChild = children[i];
            }
        }
        return bestChild;
    }
}