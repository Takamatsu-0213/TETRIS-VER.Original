using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームの進行を管理するクラス
/// </summary>
public class Game_Manager : MonoBehaviour
{
    //ミノの状態
    public enum State
    {
        /// <summary>
        /// ステージ配列の初期化
        /// </summary>
        GAMESTART,
        /// <summary>
        /// ミノを生成
        /// </summary>
        CREATE_MINO,
        /// <summary>
        /// 無操作時
        /// </summary>
        PLAY_NOKEYINPUT,
        /// <summary>
        /// 操作中
        /// </summary>
        PLAY_KEYINPUT,
        /// <summary>
        /// ミノ移動
        /// </summary>
        MINO_MOVE,
        /// <summary>
        /// ミノを配列に固定
        /// </summary>
        ARRAYSET_MINO,
        /// <summary>
        /// ゲームオーバー
        /// </summary>
        GAMEOVER,
        /// <summary>
        /// デバック中
        /// </summary>
        DEBUGGING,
    }
    //ステートの初期
    public State state = State.GAMESTART;

    const int DISPLAY_INTERVALS = 1, //スコア表示のインターバル
              MINO_ARRAYSIZE = 5,    //ミノ配列(NowMino配列)のサイズ
              MINO_TYPESIZE = 6,     //ミノのタイプの最大値
              SEARCH_SIZE = 4,       //NEXTミノの表示数
              INITIAL_VALUE = 2;     //初期値

    const float m_Alfa = 0.01f;      //フェードアウトに使うアルファ値

    private int m_TypeNumber = 0;    //ミノのタイプを判断する値

    public bool m_PlayON = false;    //ゲームの開始判定

    //使用するゲームオブジェクトの宣言
    [SerializeField] GameObject MainCamera, ScoreDisplay_Canvas;
    [SerializeField] Image Fade;
    public new AudioSource[] audio;

    //スクリプトの宣言
    Mino_Controller Mino_Con;
    Mino_Create Mino_create;
    [SerializeField] GameScreenUI_Controller Gamescreen_UI;
    [SerializeField] NextMINOUI_Controller Nextmino_UI;
    private void Awake()
    {
        Mino_Con = this.GetComponent<Mino_Controller>();
        Mino_create = this.GetComponent<Mino_Create>();
        audio[0].volume = ConfigScene_Manager.BGMVolume;
        for(int i = 1; i < audio.Length; i++)
        {
            audio[i].volume = ConfigScene_Manager.SEVolume;
        }
        ScoreDisplay_Canvas.SetActive(false);
    }
    private void Update()
    {
        //ESCキーを押すことによるゲーム強制終了
        if (Input.GetKey(KeyCode.Escape))
        {
          #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
          #elif UNITY_STANDALONE
           Application.Quit();
          #endif
        }
    }

    private void FixedUpdate()
    {
        //現在の状態の確認
        if (m_PlayON)
            NowState();

        //フェードアウトが完了するまでゲーム開始を待つ
        if (!m_PlayON && state == State.GAMESTART)
            FadeOUT();
    }

    //フェードアウト
    void FadeOUT()
    {
        if (Fade.color != new Color(0, 0, 0, 0))
            Fade.color -= new Color(0, 0, 0, m_Alfa);

        //フェードアウト完了後に処理を終了
        if (Fade.color == new Color(0, 0, 0, 0))
            m_PlayON = true;
    }

    /// <summary>
    /// 現在の状態に沿った処理をする
    /// </summary>
    void NowState()
    {
        switch (state)
        {
            case State.GAMESTART:
                Gamescreen_UI.enabled = true;
                Mino_Con.Stage_Creating();
                state = State.CREATE_MINO;
                break;
            case State.CREATE_MINO:
                Mino_Con.Minos_Creating();
                NextMino_UIdisplay();
                ScoreCheck();
                state = State.PLAY_NOKEYINPUT;
                break;
            case State.GAMEOVER:
                MainCamera.GetComponent<AudioSource>().enabled = false;
                Mino_Con.enabled = false;
                Gamescreen_UI.ScoreAdd_TimeCount();
                StartCoroutine(Score_Display());
                m_PlayON = false;
                break;
        }
    }

    /// <summary>
    /// スコアチェック
    /// </summary>
    void ScoreCheck()
    {
        int DeleteLine_Value = 0;
        bool Perfect = false;
        bool Tspin = false;
        bool LevelUp = false;
        Mino_Con.Ask_DeleteCount(ref DeleteLine_Value, ref Perfect, ref Tspin);
        Gamescreen_UI.ScoreCheak(DeleteLine_Value, Perfect, Tspin);
        Gamescreen_UI.DeleteLine_LevelCheak(DeleteLine_Value, ref LevelUp);
        if (LevelUp)
            Mino_Con.LevelUp();
    }

    /// <summary>
    /// 画面に次に出てくるミノを表示する
    /// </summary>
    void NextMino_UIdisplay()
    {
        m_TypeNumber++;

        if (m_TypeNumber > MINO_TYPESIZE)
            m_TypeNumber = 0;

        int tmp_StartNumber = m_TypeNumber;
        //４つ分のミノを調べる
        for(int i = 0; i < SEARCH_SIZE; i++)
        {
            int[,] Array = new int[MINO_ARRAYSIZE, MINO_ARRAYSIZE];
            Sprite Mino_ObjColor = null;
            int tmp_Nuber = m_TypeNumber;
            Mino_create.NowMino_Sprite(ref Mino_ObjColor, ref m_TypeNumber);
            Mino_create.Mino_ArrayCopy(ref Array, m_TypeNumber);
            Nextmino_UI.NextMino(Array, Mino_ObjColor);
            m_TypeNumber = tmp_Nuber;
            m_TypeNumber++;

            if (m_TypeNumber > MINO_TYPESIZE)
                m_TypeNumber = 0;
        }
        m_TypeNumber = tmp_StartNumber;
    }

    /// <summary>
    /// ゲームオーバーした後にスコアを表示する処理
    /// </summary>
    /// <returns></returns>
    IEnumerator Score_Display()
    {
        int ChildCount = INITIAL_VALUE;
        ScoreDisplay_Canvas.SetActive(true);
        yield return new WaitForSeconds(DISPLAY_INTERVALS);
        ScoreDisplay_Canvas.transform.GetChild(ChildCount).gameObject.SetActive(true);
        ChildCount++;
        yield return new WaitForSeconds(DISPLAY_INTERVALS);
        ScoreDisplay_Canvas.transform.GetChild(ChildCount).gameObject.SetActive(true);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(ScoreDisplay_Canvas.transform.GetChild(ChildCount).gameObject);
        ChildCount++;
        ScoreDisplay_Canvas.transform.GetChild(ChildCount).gameObject.SetActive(true);
    }
}
