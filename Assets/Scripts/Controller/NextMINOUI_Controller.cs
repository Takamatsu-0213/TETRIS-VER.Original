using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 次に出てくるミノ(テトリミノ)を表示するクラス
/// </summary>
public class NextMINOUI_Controller : MonoBehaviour {

    [SerializeField] private GameObject[] MinoObjs = new GameObject[4]; //ゲーム画面に表示するオブジェクト

    const int POS_ADJUSTMENT = 2,  //位置調節に使う値
              MINO_MAXSIZE = 4,    //ミノの形になる最大個数
              ARRAY_MINO = 1; 　　 //配列上の「ミノ」を示す値

    private int m_CheckPoint = 0;  //NextMinoのUIのどれを選択しているか判断する値

    /// <summary>
    /// 次に出てくるミノをゲーム画面に表示する
    /// </summary>
    public void NextMino(int[,] ArrayMino, Sprite SpriteMino)
    {
        int CheckedMino = 0;
        GameObject SelectUI_Obj = MinoObjs[m_CheckPoint];

        for(int i = 0; i < ArrayMino.GetLength(0);i++)
        {
            for (int j = 0; j < ArrayMino.GetLength(1);j++)
            {
                if (ArrayMino[i, j] == ARRAY_MINO)
                {
                    SelectUI_Obj.transform.GetChild(CheckedMino).GetComponent<SpriteRenderer>().sprite = SpriteMino;
                    SelectUI_Obj.transform.GetChild(CheckedMino).localPosition = new Vector3(j - POS_ADJUSTMENT, -i + POS_ADJUSTMENT, 0);
                    CheckedMino++;
                }

                if (CheckedMino == MINO_MAXSIZE)
                    break;
            }
            if (CheckedMino == MINO_MAXSIZE)
                break;
        }
        m_CheckPoint++;

        //規定値分値が進んだら初期化する
        if (m_CheckPoint >= MinoObjs.Length)
            m_CheckPoint = 0;
    }
}
