using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゴーストミノ(ゴーストテトリミノ)の動きの処理をするクラス
/// </summary>
public class Mino_MoveGhost : MonoBehaviour {

    const int ARRAY_NONE = 0,　　  //配列上の「何もない」を示す値
              ARRAY_MINO = 1,　    //配列上の「ミノ」を示す値
              ARRAY_WALL = 2,　    //配列上の「壁」を示す値
              ARRAY_SETMINO = 3,   //配列上の「セットしたミノ」を示す値
              MAX_MINO = 4;        //各種ミノが形になるの最大個数

    /// <summary>
    /// ゴーストミノを常にミノの入れる場所まで下げて表示するもの
    /// </summary>
    public void GhostMino(int[,] Stage, int[,] NowMino, ref int GhostYheight, int XSidePos, GameObject GHOSTMINOs, ref bool Ghost)
    {
        //Stage配列内にセットせずに一番下まで進める
        for (int i = 0; i < Stage.GetLength(0); i++)
        {
            if (GhostYheight < Stage.GetLength(0))
            {
                GhostYheight++;
                GhostMino_Update(Stage, NowMino, ref GhostYheight, XSidePos, ref Ghost);

                if (!Ghost)
                    return;

                GHOSTMINOs.transform.position += new Vector3(0, -1, 0);
            }
        }
    }

    /// <summary>
    /// Stage配列内でゴーストを進めて予測ゴーストを作る
    /// </summary>
    public void GhostMino_Update(int[,] Stage, int[,] NowMino, ref int GhostYheight, int XSidePos, ref bool Ghost)
    {
        int ArrayY = 0;
        int ArrayX = 0;
        int CheckedMino = 0;

        if(GhostYheight < 0)
        {
            Ghost = false;
            return;
        }

        //Stage配列でゴーストミノを下げて予測ゴーストを作る
        for (int i = GhostYheight; i < Stage.GetLength(0); i++)
        {
            for (int j = XSidePos; j < NowMino.GetLength(0) + XSidePos; j++)
            {
                //ゴーストミノを全てチェックできたなら、処理を終了する
                if (CheckedMino == MAX_MINO)
                    return;

                if (NowMino[ArrayY, ArrayX] == ARRAY_MINO)
                {
                    //ゴーストミノが何かと被ったら処理を終了する
                    if (Stage[i, j] == ARRAY_WALL || Stage[i, j] == ARRAY_SETMINO)
                    {
                        //GhostYheightの値を１つ戻す
                        GhostYheight--;
                        Ghost = false;
                        return;
                    }
                    CheckedMino++;
                }
                ArrayX++;
                //Stage配列とNowMino配列の位置を合わせる処理
                if (ArrayX >= NowMino.GetLength(1))
                {
                    ArrayY++;
                    ArrayX = 0;

                    //NowMino配列の上限
                    if (ArrayY >= NowMino.GetLength(0))
                        return;

                    break;
                }
            }
        }
    }
}
