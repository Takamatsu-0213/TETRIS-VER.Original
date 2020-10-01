using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// ミノ(テトリミノ)の生成をするクラス
/// </summary>
public class Mino_Create : MonoBehaviour
{
    #region ミノの配列
    private int[,] Mino_I = new int[5, 5]
    {
        {0,0,0,0,0},
        {1,1,1,1,0},
        {0,0,0,0,0},
        {0,0,0,0,0},
        {0,0,0,0,0}
    };
    private int[,] Mino_O = new int[5, 5]
    {
        {0,0,0,0,0},
        {0,0,1,1,0},
        {0,0,1,1,0},
        {0,0,0,0,0},
        {0,0,0,0,0}
    };
    private int[,] Mino_S = new int[5, 5]
    {
        {0,0,0,0,0},
        {0,0,1,1,0},
        {0,1,1,0,0},
        {0,0,0,0,0},
        {0,0,0,0,0}
    };
    private int[,] Mino_Z = new int[5, 5]
    {
        {0,0,0,0,0},
        {0,1,1,0,0},
        {0,0,1,1,0},
        {0,0,0,0,0},
        {0,0,0,0,0}
    };
    private int[,] Mino_J = new int[5, 5]
    {
        {0,0,0,0,0},
        {0,1,0,0,0},
        {0,1,1,1,0},
        {0,0,0,0,0},
        {0,0,0,0,0}
    };
    private int[,] Mino_L = new int[5, 5]
    {
        {0,0,0,0,0},
        {0,0,0,1,0},
        {0,1,1,1,0},
        {0,0,0,0,0},
        {0,0,0,0,0}
    };
    private int[,] Mino_T = new int[5, 5]
    {
        {0,0,0,0,0},
        {0,0,1,0,0},
        {0,1,1,1,0},
        {0,0,0,0,0},
        {0,0,0,0,0}
    };
    #endregion
    //オリジナルミノの配列
    private int[,] Mino_Original = new int[5, 5];

    const int MAX_MINO = 4,           //ミノが形になるまでの最大個数
              MINO_POSITION = 1,　　　//ミノの位置を設定する時に使う値
              ARRAY_NONE = 0,　　　　 //配列上の「何もない」を示す値
              ARRAY_MINO = 1,　　　　 //配列上の「ミノ」を示す値
              NUBER_UPPER = 7,　　　　//ミノの形を決めるListの上限
              POS_X = 5,　　　　　　　//ミノのX座標の初期位置
              POS_Y = -2,　　　　　　 //ミノのY座標の初期位置
              POS_ADD = 3;            //位置調節に使う値

    const float ALFA = 0.5f;　　　　　//フェードに使うアルファ値

    private int m_MinoNumber = 0;    //List上の取得したい値の位置を示す変数

    public List<Sprite> Mino_Sprite;                     //各ミノのSpriteリスト
    private List<int> MinosNuberList = new List<int>();   //ミノの出てくる順番を決めるリスト

    private void Awake()
    {
        //Listの初期設定
        for (int i = 0; i < NUBER_UPPER; i++)
            MinosNuberList.Insert(i, i);
        //Listのランダム化
        MinosNuberList = MinosNuberList.OrderBy(i => Guid.NewGuid()).ToList();

        //オリジナルミノを作ってないならこの後の処理へはいけない設定
        if (!OriginalCreate_Manager.getCreateBool())
            return;
        //オリジナルミノの配列を受け取る
        Mino_Original = OriginalCreate_Manager.getCreateArray();
        string MINO_OriginalType = OriginalCreate_Manager.getArrayType();
        //MINO_OriginalTypeの値に従って、ミノの配列を置き換える
        switch (MINO_OriginalType)
        {
            case "MINO_I":
                Array.Copy(Mino_Original, 0, Mino_I, 0, Mino_Original.Length);
                break;
            case "MINO_O":
                Array.Copy(Mino_Original, 0, Mino_O, 0, Mino_Original.Length);
                break;
            case "MINO_S":
                Array.Copy(Mino_Original, 0, Mino_S, 0, Mino_Original.Length);
                break;
            case "MINO_Z":
                Array.Copy(Mino_Original, 0, Mino_Z, 0, Mino_Original.Length);
                break;
            case "MINO_J":
                Array.Copy(Mino_Original, 0, Mino_J, 0, Mino_Original.Length);
                break;
            case "MINO_L":
                Array.Copy(Mino_Original, 0, Mino_L, 0, Mino_Original.Length);
                break;
            case "MINO_T":
                Array.Copy(Mino_Original, 0, Mino_T, 0, Mino_Original.Length);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 生成するミノを選択する処理
    /// </summary>
    /// <param name="Mino"></param>
    /// <param name="ColorBlock"></param>
    /// <param name="NowColorObj"></param>
    /// <param name="DebugMino"></param>
    public Mino_Controller.MINOs_Array MinoSelect(Mino_Controller.MINOs_Array Mino, ref GameObject[] ColorBlock,ref GameObject NowColorObj, bool DebugMino)
    {
        ////ポインタが配列上７に来たら配列ポインタを先頭に戻す
        if (m_MinoNumber == NUBER_UPPER)
            m_MinoNumber = 0;

        //加算
        if (m_MinoNumber != NUBER_UPPER)
        {
            if (!DebugMino)
            {
                Mino.MinoType = MinosNuberList[m_MinoNumber];
            }
            m_MinoNumber++;
        }
        //ミノの色に合わせたゲームオブジェクトを入れる
        NowColorObj = ColorBlock[Mino.MinoType];
        //配列のコピー
        Mino_ArrayCopy(ref Mino.NowMino, Mino.MinoType);
        return Mino;
    }

    /// <summary>
    /// ミノのスプライトを受け取る処理
    /// </summary>
    /// <param name="Sprite"></param>
    /// <param name="MinoType"></param>
    public void NowMino_Sprite(ref Sprite Sprite, ref int MinoType)
    {
        //MinosNuberListからミノのタイプをもらう
        MinoType = MinosNuberList[MinoType];
        //ミノのタイプにあったスプライトを渡す
        Sprite = Mino_Sprite[MinoType];
        return;
    }

    /// <summary>
    /// ミノの配列をコピー
    /// </summary>
    /// <param name="Array"></param>
    /// <param name="MinoType"></param>
    public void Mino_ArrayCopy(ref int[,] Array, int MinoType)
    {
        //MinoTypeの値に従て、使うミノの配列をコピーして渡された配列に格納する
        switch (MinoType)
        {
            case 0:
                System.Array.Copy(Mino_I, 0, Array, 0, Array.Length);
                break;
            case 1:
                System.Array.Copy(Mino_O, 0, Array, 0, Array.Length);
                break;
            case 2:
                System.Array.Copy(Mino_S, 0, Array, 0, Array.Length);
                break;
            case 3:
                System.Array.Copy(Mino_Z, 0, Array, 0, Array.Length);
                break;
            case 4:
                System.Array.Copy(Mino_J, 0, Array, 0, Array.Length);
                break;
            case 5:
                System.Array.Copy(Mino_L, 0, Array, 0, Array.Length);
                break;
            case 6:
                System.Array.Copy(Mino_T, 0, Array, 0, Array.Length);
                break;
        }
    }

    /// <summary>
    /// ミノを生成する処理
    /// </summary>
    /// <param name="Mino"></param>
    /// <param name="MINOs"></param>
    /// <param name="GHOSTMINOs"></param>
    /// <param name="NowColorObj"></param>
    /// <returns></returns>
    public Mino_Controller.MINOs_Array MinoCreate(Mino_Controller.MINOs_Array Mino, GameObject MINOs, GameObject GHOSTMINOs,GameObject NowColorObj)
    {
        //親子関係の初期化
        MINOs.transform.DetachChildren();
        GHOSTMINOs.transform.DetachChildren();
        //ミノとゴーストを初期位置に設定
        MINOs.transform.position = new Vector3(POS_X, POS_Y, 0);
        GHOSTMINOs.transform.position = new Vector3(POS_X, POS_Y, 0);
        MINOs.transform.rotation = Quaternion.identity;
        GHOSTMINOs.transform.rotation = Quaternion.identity;

        int CheckedMino = 0;
        //ミノとゴーストの形を作る
        for (int i = 0; i < Mino.NowMino.GetLength(0); i++)
        {
            for (int j = 0; j < Mino.NowMino.GetLength(1); j++)
            {
                if (Mino.NowMino[i, j] == ARRAY_MINO)
                {
                    //ミノ(ゲームオブジェクト)のゴーストの設定
                    Mino.UseMinoGhost[CheckedMino].SetActive(true);
                    Mino.UseMinoGhost[CheckedMino].transform.position = new Vector3(MINO_POSITION * (j + POS_ADD), -MINO_POSITION * i, 0);
                    Mino.UseMinoGhost[CheckedMino].transform.parent = GHOSTMINOs.transform;
                    Mino.UseMinoGhost[CheckedMino].name = NowColorObj.name;
                    Mino.UseMinoGhost[CheckedMino].GetComponent<SpriteRenderer>().sprite = NowColorObj.GetComponent<SpriteRenderer>().sprite;
                    Mino.UseMinoGhost[CheckedMino].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, ALFA);

                    //ミノ(ゲームオブジェクト)の設定
                    Mino.UseMino[CheckedMino] = Instantiate(NowColorObj, transform.position, Quaternion.identity, MINOs.transform);
                    Mino.UseMino[CheckedMino].name = NowColorObj.name;
                    Mino.UseMino[CheckedMino].transform.position = new Vector3(MINO_POSITION * (j + POS_ADD), -MINO_POSITION * i, 0);

                    CheckedMino++;
                }
                if (CheckedMino == MAX_MINO)
                    break;
            }
            if (CheckedMino == MAX_MINO)
                break;
        }
        return Mino;
    }
}
