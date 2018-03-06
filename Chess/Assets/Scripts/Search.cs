using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Search
{
    /// <summary>
    /// 最高分支，即将死的分支
    /// </summary>
    public const int MATE_VALUE = 10000;

    /// <summary>
    /// 最大搜索深度
    /// </summary>
    public const int LIMIT_DEPTH = 32;

    /// <summary>
    /// 搜索出胜负的分值界限，超出此值就说明已经搜索出杀棋了
    /// </summary>
    public const int WIN_VALUE = MATE_VALUE - 100;

    /// <summary>
    /// 电脑走的棋
    /// </summary>
    private int mvResult;

    /// <summary>
    /// 历史表
    /// </summary>
    private int[] historyTable = new int[65536];

    /// <summary>
    /// 局面实例
    /// </summary>
    private Situation situation;

    public int MvResult
    {
        set { }
        get { return mvResult; }
    }

    public class HistoryCompare : IComparer<int>
    {
        private int[] historyTable;
        public HistoryCompare(int[] table)
        {
            historyTable = table;
        }

        public int Compare(int x, int y)
        {
            return historyTable[y] - historyTable[x];
        }
    }

    public Search(Situation s)
    {
        situation = s;
    }

    /// <summary>
    /// 超出边界(Fail-Soft)的Alpha-Beta搜索过程
    /// </summary>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public int SearchFull(int alpha, int beta, int depth)
    {
        //到达水平线，返回局面评价函数
        if(depth == 0)
        {
            return situation.Evaluate();
        }

        //初始化最佳值和最佳走法
        int bestValue = -MATE_VALUE; //是否一个走法都没走过(杀棋)
        int mvBest = 0; //是否搜索到了Beta走法或pv走法，以便保存到历史表

        //生成全部走法，并根据历史表排序
        int[] mvs = new int[ChessLogic.MAX_GENERATE_MOVES];
        int genMoves = situation.GenerateMoves(out mvs);
        Array.Sort(mvs, 0, genMoves, new HistoryCompare(historyTable));

        //遍历走法
        int pcCaptured = 0;
        int value = 0;
        for(int i = 0; i < genMoves; i++)
        {
            if(situation.MakeMove(mvs[i], out pcCaptured))
            {
                value = -SearchFull(-beta, -alpha, depth - 1);
                situation.UndoMakeMove(mvs[i], pcCaptured);

                //进行Alpha-Beta大小判断和截断
                if(value > bestValue) //找到最佳值(但不确定是Alpha、PV还是Beta走法)
                {
                    bestValue = value; //bestValue就是目前要返回的最佳值，可能超出Alpha-Beta边界

                    if (value >= beta) //找到一个Beta走法
                    {
                        mvBest = mvs[i];
                        break;
                    }

                    if(value > alpha) //找到一个PV走法
                    {
                        mvBest = mvs[i]; //PV走法要保存到历史表
                        alpha = value;
                    }
                }
            }
        }

        //所有走法都搜索完了，把最佳走法(不能是Alpha走法)保存到历史表，返回最佳值
        if(bestValue == -MATE_VALUE)
        {
            //如果是杀棋，就根据最佳走法保存到历史表
            return situation.Distance - MATE_VALUE;
        }
        if (mvBest != 0)
        {
            //如果不是Alpha走法，就将最佳走法保存到历史表中
            historyTable[mvBest] += depth * depth;
            if (situation.Distance == 0)
            {
                //搜索根节点时，总是有一个最佳走法(因为全窗口搜索不会超出边界)，将这个走法保存下来
                mvResult = mvBest;
            }
        }

        return bestValue;
    }

    /// <summary>
    /// 迭代加深搜索过程
    /// </summary>
    public void SearchMain()
    {
        //初始化历史表
        Array.Clear(historyTable, 0, 65536);
        //初始化定时器
        DateTime srcTime = DateTime.UtcNow;
        //初始化步数
        situation.Distance = 0;

        //迭代加深过程
        int value = 0;
        for(int i = 1; i <= LIMIT_DEPTH; i++)
        {
            value = SearchFull(-MATE_VALUE, MATE_VALUE, i);
            //搜索到杀棋，就停止搜索
            if (value > WIN_VALUE || value < -WIN_VALUE)
            {
                break;
            }
            //搜索大于一秒就停止搜索

            if((DateTime.UtcNow - srcTime).TotalMilliseconds > 1000)
            {
                break;
            }
        }
    }
}
