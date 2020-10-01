using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// ミノ(テトリミノ)の動作全てを管理するクラス
/// </summary>
public class Mino_Controller : MonoBehaviour
{
    private int[,] Stage = new int[22, 12];                     //ステージの配列
    private GameObject[,] SetMinoPos = new GameObject[22, 12];　//セットしたミノ(ゲームオブジェクト)を管理する配列
    public GameObject[] ColorBlock;                             //ミノの色付きゲームオブジェクトを管理する配列でインスペクター上から入れる

    //ミノの情報を管理する構造体
    public struct MINOs_Array
    {
        public int MinoType;               //何ミノか判断する際の変数 
        public int[,] NowMino;             //現在形で使っているミノを管理する配列
        public GameObject[] UseMino;       //現在形で使っているミノの探索配列順に管理している配列
        public GameObject[] UseMinoGhost;  //現在形で使っているミノのゴーストの探索配列順に管理している配列

        /// <summary>
        /// ミノの情報を持つ構造体
        /// </summary>
        /// <param name="MinoType"></param>
        /// <param name="NowMino"></param>
        /// <param name="UseMino"></param>
        /// <param name="UseMinoGhost"></param>
        public MINOs_Array(int MinoType, int[,] NowMino, GameObject[] UseMino, GameObject[] UseMinoGhost)
        {
            this.MinoType = MinoType;
            this.NowMino = NowMino;
            this.UseMino = UseMino;
            this.UseMinoGhost = UseMinoGhost;
        }
        /// <summary>
        /// UseMino配列の親子関係設定
        /// </summary>
        /// <param name="parent"></param>
        public void UseMinoParentSet(Transform parent)
        {
            for(int i = 0; i < UseMino.Length; i++)
                UseMino[i].transform.parent = parent;
        }
        /// <summary>
        /// UseMinoGhost配列の親子関係設定
        /// </summary>
        /// <param name="parent"></param>
        public void UseMinoGhostParentSet(Transform parent)
        {
            for (int i = 0; i < UseMinoGhost.Length; i++)
                UseMinoGhost[i].transform.parent = parent;
        }
    }
    MINOs_Array Mino;

    //利用する各ゲームオブジェクトが入る
    private GameObject MINO, MINOs, GHOSTMINOs, NowColorObj;
    public GameObject SETMINOs, PerfectText, SpecialText;
    public AudioClip Sound_Set, Sound_Delete;

    const int ARRAY_MINO = 1,　　　　   //配列上の「ミノ」を示す値
              BEFOREHOLD_POS = 2,　　   //ホールド前のミノの位置調節に使う値
              POINTER = 3,　　          //Stage配列とミノ配列を合わせる時に使う値
        　　　MINO_MAXSIZE = 4, 　　　  //ミノ(ゲームオブジェクト)の最大個数
        　　　MINOSIZE = 5,             //ミノ配列の X と Y のサイズ
              MINOTYPE_I = 0,           //Iミノのタイプを意味する値
        　　　MINOTYPE_T = 6,           //Tミノのタイプを意味する値
              LINE_UPPER = 10,  　　    //ミノが一列に揃った時の数
              OPEN_DEBUGNUBER = 10,     //配列の表示をさせるデバックナンバー
              CLOSE_DEBUGNUBER = 11,    //配列の非表示をさせるデバックナンバー
              TETRIS_DEBUGNUBER = 12,   //テトリス盤面へ変えるデバックナンバー
              TSPINSIN_DEBUGNUBER = 13, //Tスピンシングル盤面へ変えるデバックナンバー
              TSPINDOU_DEBUGNUBER = 14, //Tスピンダブル盤面へ変えるデバックナンバー
              TSPINTRI_DEBUGNUBER = 15, //Tスピントリプル盤面へ変えるデバックナンバー
              LEVELUP_DEBUGNUBER = 16;  //レベルアップするデバックナンバー

    public static float MOVEDOWNTIME_INTERVALS = 0.12f, //ミノをキー入力で下げる際の配列更新までのインターバル
                        WAITING_TIME = 0.1f,            //ミノを生成後にキー入力出来るようになるまでの時間
                        DELETE_TIME = 0.4f,             //ミノを消すまでの時間
                        DOWN_ADDTIME = 0.02f;           //下降速度を上げる値

    private float m_AutoDownTime_Intervals = 0.8f, //ミノが自動で下がるまでのインターバル
                  m_AutoDownTimeCount = 0,         //自動降下の制御に使うタイムカウンタ
                  m_MoveTimeCount = 0;　　         //キー入力の制御に使うタイムカウンタ

    public static int m_Yheight = 0,　　　    //ミノを動かした Y 位置を記録している値
                      m_XSidePos = 3,         //ミノを動かした X 位置を記録している値
        　　　      　m_SaveYheight = 0,      //m_Yheightの動かす前の値を記録している値
                      m_SaveXSidePos = 0,     //m_XSidePosの動かす前の値を記録している値
                      m_GhostYheight = 0,     //ゴーストミノの Y 位置を記録している値
                      m_DeleteLine_Value = 0, //消したラインの数を記録する値
                      m_DebugNuber = 0;       //どのデバックをするか判断する値

    private bool m_AutoMove = false,　   //ミノを自動で下げるか否か
                 m_KeyInput = false,　   //キー入力を受け付けるか否か
                 m_Drop = false,　　     //ミノを下限まで下げるか否か
                 m_Ghost = false,        //ミノのゴーストを使うか否か
                 m_Minohold = false,     //ミノのホールドを使うか否か
                 m_MinoholdOnce = false, //ミノのホールドを一度だけ使えるか否か
                 m_Rotation = false,   　//ローテーションしたか否か
                 m_Rrotation = false,    //右回転判定
                 m_Lrotation = false,    //左回転判定
                 m_Perfect = false,      //全消し判定
                 m_Tspin = false,      　//Tスピン判定
                 m_DebugMino = false;    //デバック起動でミノを特定のミノにするか否か

    //呼び出すスクリプトを宣言
    Game_Manager GM;
    Mino_ArrayAssistant MA;
    Mino_KeyInput Mino_operation;
    Mino_Create Mino_create;
    Mino_Move Mino_move;
    Mino_MoveGhost Mino_moveghost;
    Mino_Rotation Mino_rotation;
    Mino_Hold Mino_hold;
    Mino_DebugCode Dbgcode;
    AudioSource audioSource;

    private void Awake()
    {
        //コンポーネントの取得
        GM = this.GetComponent<Game_Manager>();
        MA = this.GetComponent<Mino_ArrayAssistant>();
        Mino_operation = this.GetComponent<Mino_KeyInput>();
        Mino_create = this.GetComponent<Mino_Create>();
        Mino_move = this.GetComponent<Mino_Move>();
        Mino_moveghost = this.GetComponent<Mino_MoveGhost>();
        Mino_rotation = this.GetComponent<Mino_Rotation>();
        Mino_hold = this.GetComponent<Mino_Hold>();
        Dbgcode = this.GetComponent<Mino_DebugCode>();
        audioSource = this.GetComponent<AudioSource>();
        //初期設定
        Mino = new MINOs_Array(0, new int[MINOSIZE, MINOSIZE], new GameObject[4], new GameObject[4]);
        MINO = (GameObject)Resources.Load("Prefabs/MINO");
        MINOs = Instantiate(MINO, MINO.transform.position, Quaternion.identity);
        GHOSTMINOs = Instantiate(MINO, MINO.transform.position, Quaternion.identity);
        GHOSTMINOs.name = ("GHOSTMINOs");
        NowColorObj = (GameObject)Resources.Load("Prefabs/MINO_I");

        //UseMinoGhost配列の初期設定
        for (int i = 0; i < Mino.UseMinoGhost.Length; i++)
            Mino.UseMinoGhost[i] = Instantiate(NowColorObj, MINOs.transform.position, Quaternion.identity, GHOSTMINOs.transform);
    }

    private void Update()
    {
        //デバック
        Debug();

        //デバックコマンド入力中はその他の処理をさせない
        if (GM.state == Game_Manager.State.DEBUGGING)
            return;

        //ゴーストの処理
        if (m_Ghost)
        {
            GHOSTMINOs.transform.position = MINOs.transform.position;
            m_GhostYheight = m_Yheight;
            Mino_moveghost.GhostMino(Stage, Mino.NowMino, ref m_GhostYheight, m_XSidePos, GHOSTMINOs, ref m_Ghost);
        }
        //自動降下
        if (m_AutoMove)
            Mino_AutoMove();

        //キー入力管理
        Mino_Motion();
    }

    /// <summary>
    /// デバック関連
    /// </summary>
    public void Debug()
    {
        //特定のステートの時はデバックができないように設定
        if (GM.state != Game_Manager.State.CREATE_MINO && GM.state != Game_Manager.State.ARRAYSET_MINO && GM.state != Game_Manager.State.GAMESTART)
        {
            //デバックキー入力値の受け取り
            m_DebugNuber = Mino_operation.Dbg_KeyInput();

            if (m_DebugNuber == 0)
                return;

            //デバックキー入力値による処理の分岐
            Dbgcode.Switching(ref Stage, m_DebugNuber, SetMinoPos);
            //デバックキー入力値による、Mino_Controller側の設定をするかどうか
            if (m_DebugNuber < TETRIS_DEBUGNUBER)
                return;
            //MINOsの子になっているゲームオブジェクトを切り離す
            for (int i = MINO_MAXSIZE - 1; i >= 0; i--)
            {
                MINOs.transform.GetChild(i).gameObject.SetActive(false);
                MINOs.transform.GetChild(i).transform.position = new Vector3(0, 0, 0);
                MINOs.transform.GetChild(i).transform.parent = SETMINOs.transform;
            }
            //Mino_Controller側でもm_DebugNuberの値に従った処理をする
            if (m_DebugNuber == TETRIS_DEBUGNUBER)
            {
                //次に出すミノを決める
                Mino.MinoType = MINOTYPE_I;
                m_DebugMino = true;
            }
            if (m_DebugNuber >= TSPINSIN_DEBUGNUBER && m_DebugNuber != LEVELUP_DEBUGNUBER)
            {
                Mino.MinoType = MINOTYPE_T;
                m_DebugMino = true;
            }
            if (m_DebugNuber == LEVELUP_DEBUGNUBER)
            {
                LevelUp();
            }
            GM.state = Game_Manager.State.CREATE_MINO;
            m_DebugNuber = 0;
        }
        //ゲームオーバー時に配列のデバックを非表示にする
        if (GM.state == Game_Manager.State.GAMEOVER)
        {
            m_DebugNuber = CLOSE_DEBUGNUBER;
            Dbgcode.Switching(ref Stage, m_DebugNuber, SetMinoPos);
            return;
        }
    }

    /// <summary>
    /// キー入力を受け取り、キー入力によってミノの各種動作をさせる
    /// </summary>
    void Mino_Motion()
    {
        //キー入力が可能かどうか判断
        if (!m_KeyInput || GM.state == Game_Manager.State.CREATE_MINO || GM.state == Game_Manager.State.GAMEOVER)
            return;

        m_MoveTimeCount += Time.deltaTime;
        GM.state = Game_Manager.State.PLAY_NOKEYINPUT;

        if (m_MoveTimeCount < MOVEDOWNTIME_INTERVALS)
            return;
        //キー入力値を受け取る
        Mino_operation.KeyInput(ref m_Yheight, ref m_XSidePos, ref m_Minohold, ref m_Drop, ref m_Ghost, ref m_Lrotation, ref m_Rrotation);

        //ハードドロップ
        if (m_Drop)
        {
            GM.state = Game_Manager.State.PLAY_KEYINPUT;
            Mino_Drop();
            return;
        }
        //ホールド
        if (m_Minohold)
        {
            m_Minohold = false;
            //ホールドは一度だけ
            if (!m_MinoholdOnce)
                return;

            m_AutoMove = false;
            m_KeyInput = false;

            //ホールド処理をする
            Mino_Holding();        

            //ミノを作り直す
            if (GM.state == Game_Manager.State.CREATE_MINO)
                return;
            //ミノの操作前の初期設定をする
            Invoke("Play_Start", WAITING_TIME);
            return;
        }
         //ローテーション
        if ((m_Rrotation || m_Lrotation) && m_XSidePos == m_SaveXSidePos)
        {
            Mino_rotation.Minos_Rotate(ref Stage, ref m_Yheight, ref m_XSidePos, Mino, MINOs, GHOSTMINOs, ref m_Lrotation, ref m_Rrotation);
            m_SaveYheight = m_Yheight;
            m_SaveXSidePos = m_XSidePos;
            m_Ghost = true;
            m_Rotation = true;
            return;
        }
        //ミノの移動
        if (m_Yheight != m_SaveYheight || m_XSidePos != m_SaveXSidePos)
        {
            Mino_MoveingCheck();
            m_MoveTimeCount = 0; //カウントのリセット
        }
    }

    /// <summary>
    /// Stage配列の作成指示処理
    /// </summary>
    public void Stage_Creating()
    {
        MA.Stage_Create(ref Stage);
        MA.DisturbingMino_ArraySet(SETMINOs);
    }

    /// <summary>
    /// ミノ(ゲームオブジェクト)とミノ配列(NowMino配列)の作成指示処理
    /// </summary>
    public void Minos_Creating()
    {
        //初期化
        m_Yheight = 0;
        m_XSidePos = POINTER;
        m_MinoholdOnce = true;
        m_AutoMove = false;
        m_KeyInput = false;
        //ミノの作成
        Array.Clear(Mino.NowMino, 0, Mino.NowMino.Length);
        Mino = Mino_create.MinoSelect(Mino, ref ColorBlock, ref NowColorObj, m_DebugMino);
        Mino = Mino_create.MinoCreate(Mino, MINOs, GHOSTMINOs, NowColorObj);
        //ミノが生成時に被っていないかチェックし、異常がない場合配列に格納
        MA.Array_Comparison(Mino.NowMino, ref Stage, ref m_Yheight, ref m_XSidePos, m_SaveYheight, m_SaveXSidePos);
        //デバックフラグの初期化
        m_DebugMino = false;
        //初期化処理
        Invoke("Play_Start", WAITING_TIME);
    }

    /// <summary>
    /// ミノを操作する前の初期化処理
    /// </summary>
    void Play_Start()
    {
        //初期化
        m_MoveTimeCount = 0;
        m_AutoDownTimeCount = 0;
        m_GhostYheight = 0;
        m_Yheight = 0;
        m_XSidePos = POINTER;
        m_SaveYheight = m_Yheight;
        m_SaveXSidePos = m_XSidePos;

        GHOSTMINOs.SetActive(true);
        m_KeyInput = true;
        m_AutoMove = true;
        m_Ghost = true;
        m_Rrotation = false;
        m_Lrotation = false;
        m_Tspin = false;
    }

    /// <summary>
    /// ミノの左右下移動処理 & ミノ列が揃っているかチェック
    /// </summary>
    void Mino_MoveingCheck()
    {
        //ミノの左右下移動が可能か判断する処理
        MA.Array_Comparison(Mino.NowMino, ref Stage, ref m_Yheight, ref m_XSidePos, m_SaveYheight, m_SaveXSidePos);

        //ミノをセットする状態ではないならこの処理をする
        if(GM.state != Game_Manager.State.ARRAYSET_MINO)
        {
            //ミノのゲームオブジェクトを移動
            Mino_move.Minos_Move(MINOs, m_Yheight, m_XSidePos, ref m_SaveYheight, ref m_SaveXSidePos);
        }
        //ミノを盤面にセットする処理
        if (MA.SetCheak(ref Stage, Mino, m_Yheight, m_XSidePos, SetMinoPos))
        {
            audioSource.PlayOneShot(Sound_Set);
            //セットする直前にTミノを回転させていたらTスピン判定をする
            if (Mino.MinoType == MINOTYPE_T && m_Rotation)
                m_Tspin = true;

            //親子関係の設定
            MINOs.transform.DetachChildren();
            Mino.UseMinoParentSet(SETMINOs.transform);
            //移動制限
            m_KeyInput = false;
            m_AutoMove = false;
            GHOSTMINOs.SetActive(false);
            //残存ミノの処理
            StartCoroutine(RemainMino_Check());
        }
        m_Rotation = false;
    }

    /// <summary>
    /// 盤面に残っているミノの処理＆UI表示設定
    /// </summary>
    /// <returns></returns>
    IEnumerator RemainMino_Check()
    {
        //パーフェクト判定の初期化
        m_Perfect = false;
        //
        MA.StageMino_Check(Stage, SetMinoPos);
        //消したライン数
        m_DeleteLine_Value = MA.Line_Check(DELETE_TIME, Stage, m_DeleteLine_Value, SetMinoPos);
        //ミノ列が揃っていなかったら処理を終了させる
        if (m_DeleteLine_Value <= 0)
        {
            GM.state = Game_Manager.State.CREATE_MINO;
            yield break;
        }
        audioSource.PlayOneShot(Sound_Delete);
        yield return new WaitForSeconds(DELETE_TIME);
        //ミノを下げる処理
        MA.Mino_Down(Stage, ref m_Perfect, ref SetMinoPos);
        //何もない消し方をした時の処理
        if (!m_Perfect && !m_Tspin && m_DeleteLine_Value != MINO_MAXSIZE)
        {
            GM.state = Game_Manager.State.CREATE_MINO;
            yield break;
        }
        //テトリスができた時のUI表示
        if (m_DeleteLine_Value == MINO_MAXSIZE)
        {
            SpecialText.SetActive(true);
            SpecialText.transform.GetComponent<Text>().text = "TETRIS";
        }
        //Tspinができた時のUI表示
        if (m_Tspin)
        {
            SpecialText.SetActive(true);
            string Line_Value = "";
            switch (m_DeleteLine_Value)
            {
                case 1:
                    Line_Value = "Single";
                    break;
                case 2:
                    Line_Value = "Double";
                    break;
                case 3:
                    Line_Value = "Triple";
                    break;
                default:
                    break;
            }
            SpecialText.transform.GetComponent<Text>().text = "Tspin\n " + Line_Value;
        }
        //全消し後の処理
        if (m_Perfect)
        {
            PerfectText.SetActive(true);
        }
        yield return new WaitForSeconds(DELETE_TIME * 2);
        PerfectText.SetActive(false);
        SpecialText.SetActive(false);
        GM.state = Game_Manager.State.CREATE_MINO;
    }

    /// <summary>
    /// ミノを一定間隔で降下させる処理　
    /// </summary>
    void Mino_AutoMove()
    {
        m_AutoDownTimeCount += Time.deltaTime;

        if (m_AutoDownTimeCount >= m_AutoDownTime_Intervals && GM.state != Game_Manager.State.ARRAYSET_MINO && GM.state != Game_Manager.State.MINO_MOVE)
        {
            m_SaveYheight = m_Yheight;
            m_Yheight++;
            Mino_MoveingCheck();
            m_AutoDownTimeCount = 0f;
        }
    }

    /// <summary>
    /// ハードドロップ処理
    /// </summary>
    void Mino_Drop()
    {
        m_KeyInput = false;
        m_AutoMove = false;
        m_Drop = false;

        //ゴーストまで移動させて盤面にセットする
        MINOs.transform.position = GHOSTMINOs.transform.position;
        m_Yheight = m_GhostYheight;
        m_SaveYheight = m_Yheight;
        m_Yheight++;
        Mino_MoveingCheck();
    }

    /// <summary>
    /// ミノを初期の状態に戻し、ホールドさせる処理
    /// </summary>
    void Mino_Holding()
    {
        //現在扱っているミノの初期配列をコピー
        Mino_create.Mino_ArrayCopy(ref Mino.NowMino, Mino.MinoType);

        MINOs.transform.DetachChildren();
        GHOSTMINOs.transform.DetachChildren();
        MINOs.transform.position = new Vector3(BEFOREHOLD_POS, -BEFOREHOLD_POS, 0);
        MINOs.transform.localEulerAngles = Vector3.zero;
        GHOSTMINOs.transform.localEulerAngles = Vector3.zero;

        int CheckedMino = 0;
        for (int i = 0; i < Mino.NowMino.GetLength(0); i++)
        {
            for(int j = 0; j < Mino.NowMino.GetLength(1); j++)
            {
                if (Mino.NowMino[i, j] != ARRAY_MINO)
                    continue;

                Mino.UseMino[CheckedMino].transform.localEulerAngles = Vector3.zero;
                Mino.UseMino[CheckedMino].transform.position = new Vector3(j, -i, 0);
                Mino.UseMinoGhost[CheckedMino].transform.localEulerAngles = Vector3.zero;
                Mino.UseMinoGhost[CheckedMino].transform.position = new Vector3(j, -i, 0);
                CheckedMino++;
            }
        }
        Mino.UseMinoParentSet(MINOs.transform);
        Mino.UseMinoGhostParentSet(GHOSTMINOs.transform);
        //ホールド処理
        Mino_hold.Mino_hold(ref Mino, MINOs, GHOSTMINOs, ref m_MinoholdOnce);
    }

    /// <summary>
    /// 消したラインの数を渡す
    /// </summary>
    /// <param name="DeleteLine_Value"></param>
    public void Ask_DeleteCount(ref int DeleteLine_Value , ref bool Perfect, ref bool Tspin)
    {
        Tspin = m_Tspin;
        Perfect = m_Perfect;
        DeleteLine_Value = m_DeleteLine_Value;
        m_DeleteLine_Value = 0;
    }

    /// <summary>
    /// レベルアップ処理
    /// </summary>
    public void LevelUp()
    {
        //下降速度の上昇
        m_AutoDownTime_Intervals -= DOWN_ADDTIME;
        //お邪魔ブロックの出現
        MA.LevelUp_Stage(ref Stage, SetMinoPos, SETMINOs);
    }
}
