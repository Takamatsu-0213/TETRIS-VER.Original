using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 配列を使っての処理するクラス
/// </summary>
public class Mino_ArrayAssistant : MonoBehaviour {

    /// <summary>
    /// 消すラインのY座標を保存する配列
    /// </summary>
    private int[] DeleteLine = new int[4];

    [SerializeField] private Sprite White_Sprite;
    [SerializeField] private GameObject[] Particle = null;
    private GameObject[] SetMino_Obj = new GameObject[180];
    private GameObject[] ObstacleMino_Obj = new GameObject[180];
    public List<GameObject> DeleteMino;

    const int STAGE_XWALLSIZE = 11,     //Stage配列(ゼロオリジン)のXサイズ
              STAGE_YWALLSIZE = 21,     //Stage配列(ゼロオリジン)のYサイズ
              STAGE_WALLPOS = 0,        //壁の左端の位置
              ARRAY_NONE = 0,　　       //配列上の「何もない」を示す値
              ARRAY_MINO = 1,　　　　   //配列上の「ミノ」を示す値
              ARRAY_WALL = 2,　  　     //配列上の「壁」を示す値
              ARRAY_SETMINO = 3,        //配列上の「セットしたミノ」を示す値
              MINO_MAXSIZE = 4,         //各種ミノが形になるの最大個数
              LINE_LIMIT = 10,          //ミノが一列に揃った時の数
              DOWNCOUNT_LIMIT = 20;     //ミノが一列に揃った時の数

    const float MULTI_TIME = 5;

    private int m_ObstacleMinoCount = 0;　//お邪魔ミノの出現に使うカウント

    Game_Manager GM;
    private void Awake()
    {
        GM = this.GetComponent<Game_Manager>();
    }

    /// <summary>
    /// お邪魔ブロック(ゲームオブジェクト)を配列に格納する
    /// </summary>
    /// <param name="SETMINOs"></param>
    public void DisturbingMino_ArraySet(GameObject SETMINOs)
    {
        int CheckedMino = 0;
        //初期格納
        for (int i = 0; i < ObstacleMino_Obj.Length; i++)
        {
            ObstacleMino_Obj[i] = SETMINOs.transform.GetChild(CheckedMino).gameObject;
            CheckedMino++;
        }
    }

    /// <summary>
    /// Stage配列の外枠を作成
    /// </summary>
    public void Stage_Create(ref int[,] Stage)
    {
        //ゼロクリア
        Array.Clear(Stage, 0, Stage.Length);

        //Stage配列の外枠を作成
        for (int i = 0; i < Stage.GetLength(0); i++)
        {
            for (int j = 0; j < Stage.GetLength(1); j++)
            {
                if (i == STAGE_WALLPOS || i == STAGE_YWALLSIZE) //上下壁
                {
                    Stage[i, j] = ARRAY_WALL;
                }
                else if (j == STAGE_WALLPOS || j == STAGE_XWALLSIZE)　//横壁
                {
                    Stage[i, j] = ARRAY_WALL;
                }
            }
        }
    }

    /// <summary>
    /// Stage配列とミノの配列(NowMino配列)を比較してミノ同士が被っていないかチェックする
    /// </summary>
    public void Array_Comparison(int[,] NowMino, ref int[,] Stage,ref int Yheight,ref int XSidePos,int SaveYheight,int SaveXSidePos)
    {
        //特定のステートの場合は処理を中断する
        if (GM.state == Game_Manager.State.ARRAYSET_MINO || GM.state == Game_Manager.State.MINO_MOVE || GM.state == Game_Manager.State.GAMEOVER)
            return;
        //チェックしたミノ数
        int CheckedMino = 0;
        //Stage配列のクローンを作成
        int[,] StageClone = Stage.Clone() as int[,];

        //Stage配列Clone内のミノ位置を初期化
        for (int i = 0; i < Stage.GetLength(0); i++)
        {
            for (int j = 0; j < Stage.GetLength(1); j++)
            {
                if (StageClone[i, j] == ARRAY_MINO)
                    StageClone[i, j] = ARRAY_NONE;
            }
        }

        //Stage配列Cloneとミノの配列の比較
        for (int i = 0; i + Yheight < Stage.GetLength(0) && i < NowMino.GetLength(1); i++)
        {
            for (int j = XSidePos; j < NowMino.GetLength(0) + XSidePos; j++)
            {
                //ミノ配列上のミノがある位置に一致した場合
                if (NowMino[i, j - XSidePos] != ARRAY_MINO)
                    continue;

                //Stage配列上の何かと被ったときは処理を中断
                if (StageClone[i + Yheight, j] != ARRAY_NONE)
                {
                    //ミノ生成時にミノ同士が被ったのでゲームオーバー判定をする
                    if (GM.state == Game_Manager.State.CREATE_MINO)
                    {
                        GM.state = Game_Manager.State.GAMEOVER;
                        return;
                    }
                    if (XSidePos != SaveXSidePos) //左右の移動量を元に戻す
                    {
                        XSidePos = SaveXSidePos;
                        return;
                    }
                    else if (Yheight != SaveYheight) //下の移動量を元に戻す
                    {
                        Yheight = SaveYheight;
                        GM.state = Game_Manager.State.ARRAYSET_MINO;
                        return;
                    }
                }

                StageClone[i + Yheight, j] = ARRAY_MINO;  //Stage配列Cloneにミノの位置を保存
                CheckedMino++;                    //異常がないチェック済みのミノ数を加算

                //全てのミノをチェック出来た場合
                if (CheckedMino != MINO_MAXSIZE)
                    continue;

                //ミノの生成に異常はなかったので処理を終了する
                if (GM.state == Game_Manager.State.CREATE_MINO)
                    return;

                //ミノの移動量を配列にコピーして処理を終了する
                if (GM.state != Game_Manager.State.ARRAYSET_MINO)
                {
                    Array.Copy(StageClone, 0, Stage, 0, Stage.Length);
                    GM.state = Game_Manager.State.MINO_MOVE;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// ミノを盤面にセットし、処理が滞りなく終わったかどうかをbool型の値を返す
    /// </summary>
    public bool SetCheak(ref int[,] Stage, Mino_Controller.MINOs_Array Mino, int Yheight, int XSidePos, GameObject[,] SetMinoPos)
    {
        if (GM.state != Game_Manager.State.ARRAYSET_MINO)
            return false;

        int ArrayY = 0;
        int ArrayX = 0;
        int CheckedMino = 0;

        //Stage配列内のミノ位置を初期化
        for (int i = 0; i < Stage.GetLength(0); i++)
        {
            for (int j = 0; j < Stage.GetLength(1); j++)
            {
                if (Stage[i, j] == ARRAY_MINO)
                    Stage[i, j] = ARRAY_NONE;
            }
        }
        //Stage配列内にセット
        for (int i = Yheight; i < Stage.GetLength(0); i++)
        {
            for (int j = XSidePos; j < Mino.NowMino.GetLength(0) + XSidePos; j++)
            {
                //ミノを見つけたらStage配列にセットする
                if (Mino.NowMino[ArrayY, ArrayX] == ARRAY_MINO)
                {
                    if (Stage[i, j] != ARRAY_NONE)
                    {
                        GM.state = Game_Manager.State.GAMEOVER;
                        return false;
                    }

                    Stage[i, j] = ARRAY_SETMINO;
                    SetMinoPos[i, j] = Mino.UseMino[CheckedMino];

                    CheckedMino++;
                }
                //異常がなかったら処理を終了させる
                if (CheckedMino == MINO_MAXSIZE)
                    return true;

                ArrayX++;
                //Stage配列とNowMino配列の位置を合わせる処理
                if (ArrayX >= Mino.NowMino.GetLength(1))
                {
                    ArrayY++;
                    ArrayX = 0;

                    //NowMino配列の上限
                    if (ArrayY >= Mino.NowMino.GetLength(0))
                        return false;
                    break;
                }
            }
        }
        return false;
    }

    //盤面にミノが配列通りに置かれているかチェックする
    public void StageMino_Check(int[,] Stage, GameObject[,] SetMinoPos)
    {
        for (int i = 0; i < Stage.GetLength(0); i++)
        {
            for (int j = 0; j < Stage.GetLength(1); j++)
            {
                if (Stage[i, j] != ARRAY_SETMINO)
                    return;
                SetMinoPos[i, j].transform.position = new Vector3(j, -i, 0);
            }
        }
    }

    /// <summary>
    /// ミノが横に揃っているか確認する。揃っていた場合ミノを消し、その消したラインの数を渡す。
    /// </summary>
    public int Line_Check(float DeleteTime, int[,] Stage, int DeleteLine_Value, GameObject[,] SetMinoPos)
    {
        //消すLineの数を数えるために使う変数の初期化
        int CheckedMinos = 0;
        DeleteLine_Value = 0;

        //Stage配列からLineが揃っているところを探す
        for (int i = 0; i < Stage.GetLength(0); i++)
        {
            CheckedMinos = 0;
            for (int j = 0; j < Stage.GetLength(1); j++)
            {
                if (Stage[i, j] == ARRAY_SETMINO)　//セットしてあるミノがあったら、CheckedMinosに１を加算
                    CheckedMinos++;

                if (CheckedMinos == LINE_LIMIT)　//CheckedMinosが規定数に達したら、DeleteLine配列にその行の高さの値を入れる
                {
                    DeleteLine[DeleteLine_Value] = i;

                    DeleteLine_Value++;
                    CheckedMinos = 0;
                }
            }
        }

        //消すLineがないなら処理を終了する
        if (DeleteLine_Value < 0)
            return 0;
        int tmp_Line = DeleteLine_Value;
        DeleteLine_Value--;
        //ミノを消す処理
        while (DeleteLine_Value >= 0)
        {
            int i = DeleteLine[DeleteLine_Value];

            for (int j = 0; j < Stage.GetLength(1); j++)
            {
                //消すオブジェクト(ミノ)はあらかじめ配列に入れて管理している内から消していく
                if (Stage[i, j] == ARRAY_SETMINO)
                {
                    Stage[i, j] = ARRAY_NONE;
                    Sprite tmp_Sprite = SetMinoPos[i, j].transform.GetComponent<SpriteRenderer>().sprite;
                    SetMinoPos[i, j].transform.GetComponent<SpriteRenderer>().sprite = White_Sprite;
                    StartCoroutine(Flash(SetMinoPos[i, j].transform.GetComponent<SpriteRenderer>(), DeleteTime));
                    SetMinoPos[i, j].transform.GetComponent<SpriteRenderer>().sprite = tmp_Sprite;
                    DeleteMino.Add(SetMinoPos[i, j]);
                    SetMinoPos[i, j] = null;
                }
            }
            DeleteLine_Value--;
        }
        DeleteLine_Value = tmp_Line;
        //ミノの消える際の演出
        Invoke("DeleteLineMinos", DeleteTime);
        Appearance_Effect(DeleteLine_Value);
        return tmp_Line;
    }
    /// <summary>
    /// 点滅エフェクトをさせる処理
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator Flash(SpriteRenderer obj, float time)
    {
        float CountTime = 0;
        Color tempColor = obj.color;
        while (CountTime < time)
        {
            CountTime += Time.deltaTime;
            obj.color = new Color(255, 255, 255, Mathf.Abs(Mathf.Cos(CountTime * MULTI_TIME)));
            yield return null;
        }
        obj.color = tempColor;
    }
    /// <summary>
    /// ミノ(ゲームオブジェクト)の不可視化
    /// </summary>
    void DeleteLineMinos()
    {
        for (int i = 0; i < DeleteMino.Count; i++)
            DeleteMino[i].SetActive(false);

        DeleteMino.Clear();
    }

    /// <summary>
    /// エフェクトの出現させる
    /// </summary>
    /// <param name="DeleteLine_Value"></param>
    void Appearance_Effect(int DeleteLine_Value)
    {
        for (int i = 0; i < DeleteLine_Value; i++)
        {
            Vector3 pos = Particle[i].transform.position;
            pos.y = -DeleteLine[i];
            Particle[i].transform.position = pos;
            Particle[i].GetComponent<ParticleSystem>().Play();
        }
    }

    /// <summary>
    /// 残ったミノを下げる処理
    /// </summary>
    public void Mino_Down(int[,] Stage,ref bool Perfect, ref GameObject[,] SetMinoPos)
    {
        int DownCount = 0;
        int CheckedMino = 0;
        bool DownMino = false;
        //Stage配列にセットしてあるミノを探す
        for (int i = Stage.GetLength(0) - 1; i > 0; i--)
        {
            CheckedMino = 0;
            DownMino = false;
            for (int j = 1; j < Stage.GetLength(1) - 1; j++)
            {
                //セットしてあったミノを見つけた時
                if (Stage[i, j] == ARRAY_SETMINO)
                {
                    //DownCountが０(ゼロ)だった場合は処理を飛ばす
                    if (DownCount == 0)
                        break;

                    //Stage配列上のセットしてあるミノの位置の盤面を何もない状態にする
                    Stage[i, j] = ARRAY_NONE;
                    //Stage配列上のセットしてあったミノを DownCount分下げて、セットする
                    Stage[i + DownCount, j] = ARRAY_SETMINO;
                    //SetMinoPos配列も上記と同じように処理する
                    SetMinoPos[i + DownCount, j] = SetMinoPos[i, j];
                    SetMinoPos[i, j] = null;

                    //SetMinoPos配列に入っているゲームオブジェクトのPositionを DownCount分下げておく
                    SetMinoPos[i + DownCount , j].transform.position += new Vector3(0, -DownCount, 0);
                    DownMino = true;
                    continue;
                }
                //セットしてあるミノがない
                if (Stage[i,j] == ARRAY_NONE)
                {
                    CheckedMino++;
                    //セットしてあるミノが横一列でなかったら、DownCountをプラスする
                    if (CheckedMino == LINE_LIMIT)
                        DownCount++;
                }

            }
            //横一列でミノを一つでも見つけたら、DownCountをリセット
            if (CheckedMino != LINE_LIMIT && !DownMino)
                DownCount = 0;
        }
        //Stage配列内全てをチェックしてミノが１つも残っていなかったら、パーフェクト判定をする
        if (DownCount == DOWNCOUNT_LIMIT)
        {
            Perfect = true;
            Particle[MINO_MAXSIZE].GetComponent<ParticleSystem>().Play();
        }
    }

    /// <summary>
    /// レベルアップ処理
    /// </summary>
    /// <param name="Stage"></param>
    /// <param name="SetMinoPos"></param>
    /// <param name="SETMINOs"></param>
    public void LevelUp_Stage(ref int[,] Stage, GameObject[,] SetMinoPos, GameObject SETMINOs)
    {
        int n = 0;
        int CheckedMino = 0; 
        int UpSize = UnityEngine.Random.Range(1, 4);
        int HoleNuber = UnityEngine.Random.Range(1, 11);
        int tmp_UpSize = UpSize;

        //残っているミノをSetMino_Obj配列へ格納
        for (int i = 0; i < Stage.GetLength(0); i++)
        {
            for (int j = 0; j < Stage.GetLength(1); j++)
            {
                if (Stage[i, j] != ARRAY_SETMINO)
                    continue;

                //セットしてあるミノを見つけたら
                if (i - UpSize < 0)
                {
                    j = 0;
                    UpSize--;
                    continue;
                }
                //配列内の値を調整
                Stage[i, j] = ARRAY_NONE;
                Stage[i - UpSize, j] = ARRAY_SETMINO;
                //SetMino_Obj配列に残っているミノを格納
                SetMino_Obj[CheckedMino] = SetMinoPos[i, j];
                CheckedMino++;
            }
            UpSize = tmp_UpSize;
        }
        //初期化
        CheckedMino = 0;
        //残っているミノのせり上げ
        for (int i = 0; i < Stage.GetLength(0); i++)
        {
            for (int j = 0; j < Stage.GetLength(1); j++)
            {
                if (Stage[i, j] == ARRAY_SETMINO)
                {
                    SetMino_Obj[CheckedMino].transform.position = new Vector3(j, -i, 0);
                    SetMinoPos[i, j] = SetMino_Obj[CheckedMino];
                    CheckedMino++;
                }
            }
        }
        //お邪魔ブロックの出現
        for (int i = 1; i < UpSize + 2; i++)
        {
            n = Stage.GetLength(0) - i;
            for (int j = 1; j < Stage.GetLength(1) - 1; j++)
            {
                //穴をあける位置を飛ばす
                if (j == HoleNuber)
                    continue;

                //お邪魔ミノをセットできる位置を見つけたら
                if (Stage[n, j] == ARRAY_NONE)
                {
                    //お邪魔ミノをセットする処理
                    Stage[n, j] = ARRAY_SETMINO;
                    ObstacleMino_Obj[m_ObstacleMinoCount].SetActive(true);
                    ObstacleMino_Obj[m_ObstacleMinoCount].transform.position = new Vector3(j, -n, 0);
                    SetMinoPos[n, j] = ObstacleMino_Obj[m_ObstacleMinoCount];
                    m_ObstacleMinoCount++;
                }
                //規定値分値が進んだら、値を初期化する
                if (m_ObstacleMinoCount == ObstacleMino_Obj.Length)
                    m_ObstacleMinoCount = 0;
            }
        }
    }

    /// <summary>
    /// デバック用のミノを配置する
    /// </summary>
    /// <param name="Stage"></param>
    /// <param name="SetMinoPos"></param>
    public void DebugMino_Placement(int[,] Stage, ref GameObject[,] SetMinoPos)
    {
        for (int i = 0; i < Stage.GetLength(0); i++)
        {
            for (int j = 0; j < Stage.GetLength(1); j++)
            {
                if (SetMinoPos[i, j] != null)
                    SetMinoPos[i, j].SetActive(false);

                //デバック用のミノをセットする位置を見つけたら
                if (Stage[i, j] == ARRAY_SETMINO)
                {
                    //オブジェクトをセットする
                    ObstacleMino_Obj[m_ObstacleMinoCount].SetActive(true);
                    ObstacleMino_Obj[m_ObstacleMinoCount].transform.position = new Vector3(j, -i, 0);
                    SetMinoPos[i, j] = ObstacleMino_Obj[m_ObstacleMinoCount];
                    m_ObstacleMinoCount++;
                }
                //規定値分値が進んだら、値を初期化する
                if (m_ObstacleMinoCount == ObstacleMino_Obj.Length)
                    m_ObstacleMinoCount = 0;
            }
        }
    }
}
