using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ミノ(テトリミノ)の回転処理をするクラス
/// </summary>
public class Mino_Rotation : MonoBehaviour
{
    private int[,] NowArray = new int[5, 5];      //ミノ配列のクローン
    private int[,] StageClone = new int[22, 12];　//ステージ配列のクローン

    const int ARRAY_NONE = 0,　　 　　//配列上の「何もない」を示す値
              ARRAY_MINO = 1,　   　　//配列上の「ミノ」を示す値
              ARRAY_WALL = 2,　   　　//配列上の「壁」を示す値
              ARRAY_SETMINO = 3,  　　//配列上の「セットしたミノ」を示す値
              MINO_MAXSIZE = 4,   　　//ミノの最大サイズ
              MINOARRAY_MAXSIZE = 5,  //ミノ配列の最大サイズ
              MINO_I = 0,             //「Iミノ」を表す値
              MINO_O = 1,             //「Oミノ」を表す値
              ANGEL = 90,         　　//ミノ(ゲームオブジェクト)を回転させる角度
              ANGLE_A = 0,　　　　　　//SRSの判断に使う角度A
              ANGLE_B = 90,       　　//SRSの判断に使う角度B
              ANGLE_C = 180,     　　 //SRSの判断に使う角度C
              ANGLE_D = 270,      　　//SRSの判断に使う角度D
              MOVE_ONE = 1,  　　  　 //ミノ１つ分の移動量
              MOVE_TWO = 2,　      　 //ミノ２つ分の移動量
              MOVE_THREE = 3;         //ミノ３つ分の移動量

    /// <summary>
    /// ミノの回転処理
    /// </summary>
    public void Minos_Rotate(ref int[,] Stage, ref int Yheight, ref int XSidePos, Mino_Controller.MINOs_Array Mino, GameObject MINOs, GameObject GHOSTMINOs, 
                             ref bool Lrotation, ref bool Rrotation)
    {
        if (Mino.MinoType == MINO_O && !OriginalCreate_Manager.getCreateBool())
            return;

        int Checked = 0;
        int MINOsAngles = Mathf.FloorToInt(MINOs.transform.localEulerAngles.z);
        //ミノの角度の調整
        if (Rrotation)
        {
            switch (MINOsAngles)
            {
                case 0:
                    MINOsAngles = ANGLE_B;
                    break;
                case 90:
                    MINOsAngles = ANGLE_A;
                    break;
                case 180:
                    MINOsAngles = ANGLE_D;
                    break;
                case 270:
                    MINOsAngles = ANGLE_C;
                    break;
            }
        }         
        if (Lrotation)
        {
            switch (MINOsAngles)
            {
                case 0:
                    MINOsAngles = ANGLE_D;
                    break;
                case 90:
                    MINOsAngles =ANGLE_C;
                    break;
                case 180:
                    MINOsAngles = ANGLE_B;
                    break;
                case 270:
                    MINOsAngles = ANGLE_A;
                    break;
            }
        }           

        //配列のクローン配列を作成
        Array.Copy(Stage, 0, StageClone, 0, Stage.Length);
        var Objclone = new GameObject[MINOARRAY_MAXSIZE,MINOARRAY_MAXSIZE];
        var GhostObjclone = new GameObject[MINOARRAY_MAXSIZE, MINOARRAY_MAXSIZE];

        //右回転
        if (Rrotation)
        {
            //NowArray配列(NowMinoのクローン配列)に右回転させたNowMino配列を入れる
            for (int i = 0; i < Mino.NowMino.GetLength(0); i++)
            {
                for (int j = 0; j < Mino.NowMino.GetLength(1); j++)
                {
                    NowArray[j, Mino.NowMino.GetLength(1) - 1 - i] = Mino.NowMino[i, j];
                }
            }
            //Iミノの位置調節
            if(Mino.MinoType == MINO_I && !OriginalCreate_Manager.getCreateBool())
            {
                if (MINOsAngles == ANGLE_B)
                {
                    XSidePos--;
                }
                if (MINOsAngles == ANGLE_D)
                {
                    XSidePos++;
                }
            }

            Checked = 0;
            //ゲームオブジェクトの位置調節
            for (int i = 0; i < NowArray.GetLength(0); i++)
            {
                for (int j = 0; j < NowArray.GetLength(1); j++)
                {
                    if(NowArray[i,j] == ARRAY_MINO)
                    {
                        Objclone[i, j] = Mino.UseMino[Checked];
                        GhostObjclone[i, j] = Mino.UseMinoGhost[Checked];
                        Checked++;
                    }
                }
            }
            //回転後に壁やミノに埋まってないかチェックする
            Check_Rotation(Yheight, XSidePos, ref Rrotation);
            if (!Rrotation && !OriginalCreate_Manager.getCreateBool())
            {
                Rrotation = true;
                SuperRotationSystemCheck(MINOs, Mino.MinoType, ref Yheight, ref XSidePos, ref Lrotation, ref Rrotation);
            }

            //チェック後回転が出来なかったら終わり
            if (!Rrotation)
                return;

            //配列コピー
            Array.Copy(StageClone, 0, Stage, 0, StageClone.Length);
            Array.Copy(NowArray, 0, Mino.NowMino, 0, Mino.NowMino.Length);

            //ミノのゲームオブジェクト右回転
            MINOs.transform.eulerAngles += new Vector3(0, 0, -ANGEL);
            GHOSTMINOs.transform.eulerAngles += new Vector3(0, 0, -ANGEL);
            Rrotation = false;
            //回転後のミノ(ゲームオブジェクト)の位置と角度の調節
            Placement_Adjustment(Stage, MINOs, GHOSTMINOs, Mino);
        }
        //左回転
        if (Lrotation)
        {
            //NowArray配列(NowMinoのクローン配列)に左回転させたNowMino配列を入れる
            for (int i = 0; i < Mino.NowMino.GetLength(0); i++)
            {
                for (int j = 0; j < Mino.NowMino.GetLength(1); j++)
                {
                    NowArray[j, i] = Mino.NowMino[i, Mino.NowMino.GetLength(1) - 1 - j];
                }
            }
            //Iミノの位置調節
            if (Mino.MinoType == MINO_I && !OriginalCreate_Manager.getCreateBool())
            {
                if (MINOsAngles == ANGLE_A)
                {
                    XSidePos++;
                }
                if (MINOsAngles == ANGLE_C)
                {
                    XSidePos--;
                }
            }

            Checked = 0;
            //ゲームオブジェクトの位置調節
            for (int i = 0; i < NowArray.GetLength(0); i++)
            {
                for (int j = 0; j < NowArray.GetLength(1); j++)
                {
                    if (NowArray[i, j] == ARRAY_MINO)
                    {
                        Objclone[i, j] = Mino.UseMino[Checked];
                        GhostObjclone[i, j] = Mino.UseMinoGhost[Checked];
                        Checked++;
                    }
                }
            }

            //回転後に壁やミノに埋まってないかチェックする
            Check_Rotation(Yheight, XSidePos, ref Lrotation);
            if (!Lrotation && !OriginalCreate_Manager.getCreateBool())
            {
                Lrotation = true;
                SuperRotationSystemCheck(MINOs, Mino.MinoType, ref Yheight, ref XSidePos, ref Lrotation, ref Rrotation);
            }

            //チェック後回転が出来なかったら終わり
            if (!Lrotation)
                return;

            //配列コピー
            Array.Copy(StageClone, 0, Stage, 0, StageClone.Length);
            Array.Copy(NowArray, 0, Mino.NowMino, 0, Mino.NowMino.Length);

            //ミノのゲームオブジェクト左回転
            MINOs.transform.eulerAngles += new Vector3(0, 0, ANGEL);
            GHOSTMINOs.transform.eulerAngles += new Vector3(0, 0, ANGEL);
            Lrotation = false;
            //回転後のミノ(ゲームオブジェクト)の位置と角度の調節
            Placement_Adjustment(Stage, MINOs, GHOSTMINOs, Mino);
        }
    }

    /// <summary>
    /// 回転後の配列チェック
    /// </summary>
    /// <param name="Yheight"></param>
    /// <param name="XSidePos"></param>
    /// <param name="LorRrotation"></param>
    void Check_Rotation(int Yheight, int XSidePos, ref bool LorRrotation)
    {
        int ArrayY = 0;
        int ArrayX = 0;
        int CheckedMino = 0;
        int[,] StageClones = StageClone.Clone() as int[,];

        //Stage配列Clone内の回転前のミノ位置を初期化
        for (int i = 0; i < StageClones.GetLength(0); i++)
        {
            for (int j = 0; j < StageClones.GetLength(1); j++)
            {
                if (StageClones[i, j] == ARRAY_MINO)
                {
                    StageClones[i, j] = ARRAY_NONE;
                }
            }
        }

        //Stage配列Clone内で回転後のミノ配列の比較検証し、正常なら保存
        for (int i = Yheight; i < StageClones.GetLength(0); i++)
        {
            for (int j = XSidePos; j < NowArray.GetLength(1) + XSidePos; j++)
            {
                if (NowArray[ArrayY, ArrayX] == ARRAY_MINO)
                {
                    //Stage配列の「X軸」の要素数外まで調べようとした場合、即刻処理を終了する
                    if (j >= StageClones.GetLength(1) || j <= -1 || i >= StageClones.GetLength(0) || i <= -1)
                    {
                        LorRrotation = false; return;
                    }

                    //何らかのものと被っていたので処理を終了する
                    if (StageClones[i, j] != ARRAY_NONE)
                    {
                        LorRrotation = false; return;
                    }
                    StageClones[i, j] = ARRAY_MINO;

                    CheckedMino++;

                    //正常終了
                    if (CheckedMino == MINO_MAXSIZE)
                    {
                        Array.Copy(StageClones, 0, StageClone, 0, StageClone.Length);
                        return;
                    }
                }
                ArrayX++;
                if (ArrayX >= NowArray.GetLength(1))
                {
                    ArrayY++;
                    ArrayX = 0;

                    //NowArray配列の上限
                    if (ArrayY >= NowArray.GetLength(0))
                    {
                        LorRrotation = false; return;
                    }
                    break;
                }
            }
        }
    }

    /// <summary>
    /// ミノの回転を戻すために使うもの
    /// </summary>
    /// <param name="Mino"></param>
    /// <param name="MINOs"></param>
    /// <param name="Lrotation"></param>
    /// <param name="Rrotation"></param>
    public void Rotation_Return(ref Mino_Controller.MINOs_Array Mino, GameObject MINOs, ref bool Lrotation, ref bool Rrotation)
    {
        Array.Clear(NowArray, 0, NowArray.Length);

        if (Rrotation && Mino.MinoType != MINO_O)
        {
            //NowArray配列(NowMinoのクローン配列)に右回転させたNowMino配列を入れる
            for (int i = 0; i < Mino.NowMino.GetLength(0); i++)
            {
                for (int j = 0; j < Mino.NowMino.GetLength(1); j++)
                {
                    NowArray[j, Mino.NowMino.GetLength(1) - 1 - i] = Mino.NowMino[i, j];
                }
            }
            //配列コピー
            Array.Copy(NowArray, 0, Mino.NowMino, 0, Mino.NowMino.Length);

            //ミノのゲームオブジェクト右回転
            MINOs.transform.eulerAngles += new Vector3(0, 0, -ANGEL);
            Rrotation = false;
        }
        if(Lrotation && Mino.MinoType != MINO_O)
        {
            //NowArray配列(NowMinoのクローン配列)に左回転させたNowMino配列を入れる
            for (int i = 0; i < Mino.NowMino.GetLength(0); i++)
            {
                for (int j = 0; j < Mino.NowMino.GetLength(1); j++)
                {
                    NowArray[j, i] = Mino.NowMino[i, Mino.NowMino.GetLength(1) - 1 - j];
                }
            }
            //配列コピー
            Array.Copy(NowArray, 0, Mino.NowMino, 0, Mino.NowMino.Length);

            //ミノのゲームオブジェクト右回転
            MINOs.transform.eulerAngles += new Vector3(0, 0, ANGEL);
            Lrotation = false;
        }
    }

    //スーパーローテーション
    void SuperRotationSystemCheck(GameObject MINOs, int MinoType, ref int Yheight, ref int XSidePos, ref bool Lrotation, ref bool Rrotation)
    {
        int Step = MOVE_ONE;
        int tmp_Y = Yheight;
        int tmp_X = XSidePos;
        int MINOsAngles = Mathf.FloorToInt(MINOs.transform.localEulerAngles.z);
        //ミノの角度の調整
        if (Rrotation)
        {
            switch (MINOsAngles)
            {
                case 0:
                    MINOsAngles = ANGLE_B;
                    break;
                case 90:
                    MINOsAngles = ANGLE_A;
                    break;
                case 180:
                    MINOsAngles = ANGLE_D;
                    break;
                case 270:
                    MINOsAngles = ANGLE_C;
                    break;
            }
        }
        if (Lrotation)
        {
            switch (MINOsAngles)
            {
                case 0:
                    MINOsAngles = ANGLE_D;
                    break;
                case 90:
                    MINOsAngles = ANGLE_C;
                    break;
                case 180:
                    MINOsAngles = ANGLE_B;
                    break;
                case 270:
                    MINOsAngles = ANGLE_A;
                    break;
            }
        }

        if (MinoType != 0) //Iミノ以外の回転
        {
            while (Step <= MINO_MAXSIZE)
            {
                if (MINOsAngles == ANGLE_A)
                {
                    //R:D→A && L:B→A
                    switch (Step)
                    {
                        case 1:
                            if (Rrotation)
                                XSidePos--;
                            if (Lrotation)
                                XSidePos++;
                            Step++;
                            break;
                        case 2:
                            if (Rrotation)
                                Yheight++;
                            if (Lrotation)
                                Yheight++;
                            Step++;
                            break;
                        case 3:
                            if (Rrotation)
                            {
                                Yheight -= MOVE_THREE;
                                XSidePos++;
                            }
                            if (Lrotation)
                            {
                                Yheight -= MOVE_THREE;
                                XSidePos--;
                            }
                            Step++;
                            break;
                        case 4:
                            if (Rrotation)
                                XSidePos--;
                            if (Lrotation)
                                XSidePos++;
                            Step++;
                            break;
                    }
                }
                if (MINOsAngles == ANGLE_B)
                {
                    //R:A→B && L:C→B
                    switch (Step)
                    {
                        case 1:
                            XSidePos--;
                            Step++;
                            break;
                        case 2:
                            Yheight--;
                            Step++;
                            break;
                        case 3:
                            Yheight += MOVE_THREE;
                            XSidePos++;
                            Step++;
                            break;
                        case 4:
                            XSidePos--;
                            Step++;
                            break;
                    }
                }
                if (MINOsAngles == ANGLE_C)
                {
                    //R:B→C && L:D→C
                    switch (Step)
                    {
                        case 1:
                            if (Rrotation)
                                XSidePos++;
                            if (Lrotation)
                                XSidePos--;
                            Step++;
                            break;
                        case 2:
                            if (Rrotation)
                                Yheight++;
                            if (Lrotation)
                                Yheight++;
                            Step++;
                            break;
                        case 3:
                            if (Rrotation)
                            {
                                Yheight += MOVE_THREE;
                                XSidePos--;
                            }
                            if (Lrotation)
                            {
                                Yheight -= MOVE_THREE;
                                XSidePos++;
                            }
                            Step++;
                            break;
                        case 4:
                            if (Rrotation)
                                XSidePos++;
                            if (Lrotation)
                                XSidePos--;
                            Step++;
                            break;
                    }
                }
                if (MINOsAngles == ANGLE_D)
                {
                    //C→D && A→D
                    switch (Step)
                    {
                        case 1:
                            XSidePos++;
                            Step++;
                            break;
                        case 2:
                            Yheight--;
                            Step++;
                            break;
                        case 3:
                            Yheight += MOVE_THREE;
                            XSidePos--;
                            Step++;
                            break;
                        case 4:
                            XSidePos++;
                            Step++;
                            break;
                    }
                }
                //SRSで動いた値でステージ配列とミノ配列を比較する
                if (Rrotation)
                {
                    Check_Rotation(Yheight, XSidePos, ref Rrotation);
                    if (Rrotation)
                        return;
                    if (Step <= MINO_MAXSIZE)
                        Rrotation = true;
                }
                if (Lrotation)
                {
                    Check_Rotation(Yheight, XSidePos, ref Lrotation);
                    if (Lrotation)
                        return;
                    if (Step <= MINO_MAXSIZE)
                        Lrotation = true;
                }
                if(Step == MOVE_ONE)
                {
                    Rrotation = false;Lrotation = false;
                    return;
                }
            }
        }
        else　//Iミノの回転
        {
            while (Step <= MINO_MAXSIZE)
            {
                if (MINOsAngles == ANGLE_A)
                {
                    //R:D→A && L:B→A
                    switch (Step)
                    {
                        case 1:
                            if (Rrotation)
                                XSidePos -= MOVE_TWO;
                            if (Lrotation)
                                XSidePos += MOVE_TWO;
                            Step++;
                            break;
                        case 2:
                            if (Rrotation)
                                XSidePos += MOVE_THREE;
                            if (Lrotation)
                                XSidePos -= MOVE_THREE;
                            Step++;
                            break;
                        case 3:
                            if (Rrotation)
                                Yheight += MOVE_TWO;
                            if (Lrotation)
                            {
                                Yheight--;
                                XSidePos += MOVE_THREE;
                            }
                            Step++;
                            break;
                        case 4:
                            if (Rrotation)
                            {
                                Yheight -= MOVE_THREE;
                                XSidePos -= MOVE_THREE;
                            }
                            if (Lrotation)
                            {
                                Yheight += MOVE_THREE;
                                XSidePos -= MOVE_THREE;
                            }
                            Step++;
                            break;
                    }
                }
                if (MINOsAngles == ANGLE_B)
                {
                    //R:A→B && L:C→B
                    switch (Step)
                    {
                        case 1:
                            if (Rrotation)
                                XSidePos -= MOVE_TWO;
                            if (Lrotation)
                                XSidePos++;
                            Step++;
                            break;
                        case 2:
                            if (Rrotation)
                                XSidePos += MOVE_THREE;
                            if (Lrotation)
                                XSidePos -= MOVE_THREE;
                            Step++;
                            break;
                        case 3:
                            if (Rrotation)
                            {
                                Yheight++;
                                XSidePos -= MOVE_THREE;
                            }
                            if (Lrotation)
                            {
                                Yheight += MOVE_TWO;
                                XSidePos += MOVE_THREE;
                            }
                            Step++;
                            break;
                        case 4:
                            if (Rrotation)
                            {
                                Yheight += MOVE_THREE;
                                XSidePos += MOVE_THREE;
                            }
                            if (Lrotation)
                            {
                                Yheight -= MOVE_THREE;
                                XSidePos -= MOVE_THREE;
                            }
                            Step++;
                            break;
                    }
                }
                if (MINOsAngles == ANGLE_C)
                {
                    //R:B→C && L:D→C
                    switch (Step)
                    {
                        case 1:
                            if (Rrotation)
                                XSidePos--;
                            if (Lrotation)
                                XSidePos++;
                            Step++;
                            break;
                        case 2:
                            if (Rrotation)
                                XSidePos += MOVE_THREE;
                            if (Lrotation)
                                XSidePos -= MOVE_THREE;
                            Step++;
                            break;
                        case 3:
                            if (Rrotation)
                            {
                                Yheight -= MOVE_TWO;
                                XSidePos -= MOVE_THREE;
                            }
                            if (Lrotation)
                                Yheight++;
                            Step++;
                            break;
                        case 4:
                            if (Rrotation)
                            {
                                Yheight += MOVE_THREE;
                                XSidePos += MOVE_THREE;
                            }
                            if (Lrotation)
                            {
                                Yheight -= MOVE_THREE;
                                XSidePos += MOVE_THREE;
                            }
                            Step++;
                            break;
                    }
                }
                if (MINOsAngles == ANGLE_D)
                {
                    //R:C→D && L:A→D
                    switch (Step)
                    {
                        case 1:
                            if (Rrotation)
                                XSidePos += MOVE_TWO;
                            if (Lrotation)
                                XSidePos--;
                            Step++;
                            break;
                        case 2:
                            if (Rrotation)
                                XSidePos -= MOVE_THREE;
                            if (Lrotation)
                                XSidePos += MOVE_THREE;
                            Step++;
                            break;
                        case 3:
                            if (Rrotation)
                            {
                                Yheight--;
                                XSidePos += MOVE_THREE;
                            }
                            if (Lrotation)
                            {
                                Yheight -= MOVE_TWO;
                                XSidePos -= MOVE_THREE;
                            }
                            Step++;
                            break;
                        case 4:
                            if (Rrotation)
                            {
                                Yheight += MOVE_THREE;
                                XSidePos -= MOVE_THREE;
                            }
                            if (Lrotation)
                            {
                                Yheight += MOVE_THREE;
                                XSidePos += MOVE_THREE;
                            }
                            Step++;
                            break;
                    }
                }
                //SRSで動いた値でステージ配列とミノ配列を比較する
                if (Rrotation)
                {
                    Check_Rotation(Yheight, XSidePos, ref Rrotation);
                    if (Rrotation)
                        return;
                    if (Step <= MINO_MAXSIZE)
                        Rrotation = true;
                }
                if (Lrotation)
                {
                    Check_Rotation(Yheight, XSidePos, ref Lrotation);
                    if (Lrotation)
                        return;
                    if (Step <= MINO_MAXSIZE)
                        Lrotation = true;
                }
                if (Step == MOVE_ONE)
                {
                    Rrotation = false; Lrotation = false;
                    return;
                }
            }
        }
        Yheight = tmp_Y;
        XSidePos = tmp_X;
    }

    /// <summary>
    /// 位置と角度の調整
    /// </summary>
    /// <param name="Stage"></param>
    /// <param name="UseMino"></param>
    /// <param name="UseMinoGhost"></param>
    void Placement_Adjustment(int[,] Stage, GameObject MINOs, GameObject GHOSTMINOs , Mino_Controller.MINOs_Array Mino)
    {
        //親子関係を切る
        MINOs.transform.DetachChildren();
        GHOSTMINOs.transform.DetachChildren();
        //ゴーストをミノの位置と合わせる
        GHOSTMINOs.transform.position = MINOs.transform.position;
        int Checked = 0;

        //回転したミノとゴーストの位置＆角度を調整
        for (int i = 0; i < Stage.GetLength(0); i++)
        {
            for (int j = 0; j < Stage.GetLength(1); j++)
            {
                if (Stage[i, j] == ARRAY_MINO)
                {
                    Mino.UseMino[Checked].transform.position = new Vector3(j, -i, 0);
                    Mino.UseMino[Checked].transform.rotation = Quaternion.Euler(0, 0, 0);
                    Mino.UseMino[Checked].transform.parent = MINOs.transform;
                    Mino.UseMinoGhost[Checked].transform.position = new Vector3(j, -i, 0);
                    Mino.UseMinoGhost[Checked].transform.rotation = Quaternion.Euler(0, 0, 0);
                    Mino.UseMinoGhost[Checked].transform.parent = GHOSTMINOs.transform;
                    Checked++;
                }
            }
        }
    }
}
