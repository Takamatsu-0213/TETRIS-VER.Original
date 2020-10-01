using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// デバックの処理を管理するクラス
/// </summary>
public class Mino_DebugCode : MonoBehaviour {

    #region Debug用の配列
    //テトリスになるステージの配列
    public int[,] TetrisStage = new int[22, 12]
    {
        {2,2,2,2,2,2,2,2,2,2,2,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,3,3,3,3,0,3,3,3,3,3,2},
        {2,3,3,3,3,0,3,3,3,3,3,2},
        {2,3,3,3,3,0,3,3,3,3,3,2},
        {2,3,3,3,3,0,3,3,3,3,3,2},
        {2,2,2,2,2,2,2,2,2,2,2,2},
    };
    //TspinSingle＆Doubleが出来るステージの配列
    private int[,] TspinSDStage = new int[22, 12]
    {
        {2,2,2,2,2,2,2,2,2,2,2,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,3,3,3,3,0,0,0,0,0,0,2},
        {2,3,3,3,0,0,0,3,3,3,3,2},
        {2,3,3,3,3,0,3,3,3,3,3,2},
        {2,3,3,3,3,0,3,3,3,3,3,2},
        {2,2,2,2,2,2,2,2,2,2,2,2},
    };
    //TspinTripleが出来るステージの配列
    private int[,] TspinTripleStage = new int[22, 12]
    {
        {2,2,2,2,2,2,2,2,2,2,2,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,0,0,0,0,0,0,0,0,2},
        {2,0,0,3,0,0,3,0,0,0,0,2},
        {2,3,3,3,0,3,3,3,3,0,0,2},
        {2,3,3,3,3,3,3,3,0,0,0,2},
        {2,3,3,3,3,3,3,3,0,3,3,2},
        {2,3,3,3,3,3,3,3,0,0,3,2},
        {2,3,3,3,3,3,3,3,0,3,3,2},
        {2,2,2,2,2,2,2,2,2,2,2,2},
    };
    #endregion

    private const int ARRAY_NONE = 0,           //配列上の「何もない」を示す値
                      ARRAY_MINO = 1,           //配列上の「ミノ」を示す値
                      ARRAY_WALL = 2,           //配列上の「壁」を示す値
                      ARRAY_SETMINO = 3,        //配列上の「セットしたミノ」を示す値
                      OPEN_DEBUGNUBER = 10,     //配列の表示をさせる値
                      CLOSE_DEBUGNUBER = 11,    //配列の非表示をさせる値
                      TETRIS_DEBUGNUBER = 12,   //テトリスができる盤面にさせる値
                      TSPINSIN_DEBUGNUBER = 13, //Tスピンシングルができる盤面にさせる値
                      TSPINDOU_DEBUGNUBER = 14, //Tスピンダブルができる盤面にさせる値
                      TSPINTRI_DEBUGNUBER = 15; //Tスピントリプルができる盤面にさせる値

    private bool m_ArrayDbg = false;　//配列のデバックをするか否か判定

    [SerializeField]
    private GameObject ArrayText;

    Mino_ArrayAssistant MA;

    private void Awake()
    {
        MA = this.GetComponent<Mino_ArrayAssistant>();
    }

    /// <summary>
    /// デバックキー入力値に沿った、処理の切り替え
    /// </summary>
    /// <param name="Stage"></param>
    /// <param name="DebugNuber"></param>
    /// <param name="SetMinoPos"></param>
    public void Switching(ref int[,] Stage, int DebugNuber, GameObject[,] SetMinoPos)
    {
        //DebugNuberの値に従った処理をする
        switch (DebugNuber)
        {
            case OPEN_DEBUGNUBER:
                ArrayText.SetActive(true);
                m_ArrayDbg = true;
                break;
            case CLOSE_DEBUGNUBER:
                ArrayText.SetActive(false);
                m_ArrayDbg = false;
                break;
            case TETRIS_DEBUGNUBER:
                Array.Copy(TetrisStage, 0, Stage, 0, TetrisStage.Length);
                break;
            case TSPINSIN_DEBUGNUBER:
                Array.Copy(TspinSDStage, 0, Stage, 0, TspinSDStage.Length);
                break;
            case TSPINDOU_DEBUGNUBER:
                Array.Copy(TspinSDStage, 0, Stage, 0, TspinSDStage.Length);
                break;
            case TSPINTRI_DEBUGNUBER:
                Array.Copy(TspinTripleStage, 0, Stage, 0, TspinTripleStage.Length);
                break;
            default:
                break;
        }
        //特定の値以上は盤面にミノをセットさせる
        if(DebugNuber >= TETRIS_DEBUGNUBER)
        {
            MA.DebugMino_Placement(Stage, ref SetMinoPos);
        }
        //配列を画面上に表示させるか否か
        if (m_ArrayDbg)
            Stage_Dbg(Stage);
    }

    /// <summary>
    /// 配列の数字をTextにセット
    /// </summary>
    /// <param name="Stage"></param>
    private void Stage_Dbg(int[,] Stage)
    {
        string print_array = "";
        for (int i = 0; i < Stage.GetLength(0); i++)
        {
            for (int j = 0; j < Stage.GetLength(1); j++)
            {
                if (Stage[i, j] == ARRAY_NONE)
                    print_array += " 0 ";
                if (Stage[i, j] == ARRAY_MINO)
                    print_array += " 1 ";
                if (Stage[i, j] == ARRAY_WALL)
                    print_array += " 2 ";
                if (Stage[i, j] == ARRAY_SETMINO)
                    print_array += " 3 ";
            }
            print_array += "\n";
        }
        ArrayText.GetComponent<Text>().text = print_array;
    }
}
