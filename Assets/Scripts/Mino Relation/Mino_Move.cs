using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ミノ(テトリミノ)のゲームオブジェクトを動かすクラス
/// </summary>
public class Mino_Move : MonoBehaviour
{
    //ミノ(ゲームオブジェクト)の移動量
    const int MINO_MOVEPOS = 1;

    /// <summary>
    /// ミノ(ゲームオブジェクト)の位置を動かす
    /// </summary>
    /// <param name="MINOs"></param>
    /// <param name="Yheight"></param>
    /// <param name="SaveYheight"></param>
    /// <param name="XSidePos"></param>
    /// <param name="SaveXSidePos"></param>
    public void Minos_Move(GameObject MINOs, int Yheight, int XSidePos, ref int SaveYheight, ref int SaveXSidePos)
    {
        //下移動
        if (Yheight != SaveYheight)
        {
            MINOs.transform.position += new Vector3(0, -MINO_MOVEPOS, 0);
        }
        //右移動
        if (XSidePos > SaveXSidePos)
        {
            MINOs.transform.position += new Vector3(MINO_MOVEPOS, 0, 0);
        }
        //左移動
        if (XSidePos < SaveXSidePos)
        {
            MINOs.transform.position += new Vector3(-MINO_MOVEPOS, 0, 0);
        }
        //値をセーブする
        SaveYheight = Yheight;
        SaveXSidePos = XSidePos;
    }

}
