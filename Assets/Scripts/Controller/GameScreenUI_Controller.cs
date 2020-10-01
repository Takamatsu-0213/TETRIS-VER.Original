using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲーム画面のUIを管理するクラス
/// </summary>
public class GameScreenUI_Controller : MonoBehaviour {

    public GameObject[] back; //背景で動くゲームオブジェクトを入れる配列

    [SerializeField]
    private Text Time_Text = null,
                 Level_Text = null,
                 Delete_text = null,
                 Score_Text = null,
                 GameOverScore_Text = null;

    [SerializeField] private GameObject Pause_Text, Title_Button, BackGround;

    const float INTERVALS = 1.0f;　　　　 //背景オブジェクトを動かすためのインターバル

    private int m_DeleteLine_Count = 0,   //ゲームが始まってから消したラインの数をカウント
                m_Level = 1,              //現在のレベル
                m_NowScore = 0;　　　　   //現在のスコア

    private float m_TimeCount_m = 00,　 　//分を意味する値
       　　　　　 m_TimeCount_s = 00, 　　//秒を意味する値
       　　　　　 m_BackObjTimeCount = 0; //背景の動くオブジェクトを動かすために使うタイムカウント

    private bool m_GroundOn = true;       //背景の動くオブジェクトを動かすか判断する値

    const int TIME_UPPERLIMIT = 60,       //秒数の上限
              TIME_LOWERLIMIT = 0,        //秒数の下限
              LINE_LOWERLIMIT = 10,       //レベルアップさせる際に使う、消した列の最低値
              ONE_LINE = 40,              //１ライン消した時にスコアに加算する値
              TWO_LINE = 100,             //２ライン消した時にスコアに加算する値
              THREE_LINE = 300,           //３ライン消した時にスコアに加算する値
              TETRIS_LINE = 1200,         //４ライン(テトリス)消した時にスコアに加算する値
              TSPIN_ADD = 3,              //Tスピンで消した時にスコアに乗算する値
              PERFECT = 3000;             //パーフェクト消しが出来たときにスコアに加算する値

    //スクリプトの宣言
    [SerializeField] Game_Manager GM;
    private void Update()
    {
        if (GM.state == Game_Manager.State.DEBUGGING)
            return;

        //ポーズ画面
        if (GM.m_PlayON && Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            if (Time.timeScale == 1)
            {
                Pause_Text.SetActive(true);
                Title_Button.SetActive(true);
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(Title_Button);
                BackGround.SetActive(false);
                Time.timeScale = 0;
            }
            else
            {
                Pause_Text.SetActive(false);
                Title_Button.SetActive(false);
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                BackGround.SetActive(true);
                Time.timeScale = 1;
            }
        }

        //背景のオブジェクトを起動させる
        if (m_GroundOn)
        {
            int Checked_Obj = 0;
            m_BackObjTimeCount += Time.deltaTime;

            if (m_BackObjTimeCount > INTERVALS)
            {
                back[Checked_Obj].SetActive(true);
                m_BackObjTimeCount = 0f;
                Checked_Obj++;
            }

            if (Checked_Obj > back.Length - 1)
            {
                m_GroundOn = false;
            }
        }
    }

    private void FixedUpdate()
    {
        //ゲームが開始されたら時間を数え始める
        if (GM.m_PlayON && GM.state != Game_Manager.State.GAMEOVER)
            Time_Count();

    }

    /// <summary>
    /// プレイ時間を計算
    /// </summary>
    void Time_Count()
    {
        m_TimeCount_s += Time.deltaTime;

        if (m_TimeCount_s > TIME_UPPERLIMIT)
        {
            m_TimeCount_m++;
            m_TimeCount_s = 00;
        }

        Time_Text.text = m_TimeCount_m.ToString("00") + ":" + m_TimeCount_s.ToString("f1");
    }

    /// <summary>
    /// プレイ時間をカウントダウンして行き、独自ルールで1秒で1ポイントをスコアに加算させる処理
    /// </summary>
    public void ScoreAdd_TimeCount()
    {
        int AddScore = 0;
        while(m_TimeCount_m > 0 || m_TimeCount_s > 0)
        {
            AddScore += (int)m_TimeCount_s;
            m_TimeCount_s = m_TimeCount_s - m_TimeCount_s;

            if (m_TimeCount_s <= TIME_LOWERLIMIT && m_TimeCount_m > 0)
            {
                m_TimeCount_m--;
                m_TimeCount_s = TIME_UPPERLIMIT;
            }
        }
        m_NowScore += AddScore;
        GameOverScore_Text.text = m_NowScore.ToString("000000000");
    }

    /// <summary>
    /// 消したラインの数とレベルチェック
    /// </summary>
    /// <param name="DeleteLine_Value"></param>
    /// <param name="LevelUp"></param>
    public void DeleteLine_LevelCheak(int DeleteLine_Value, ref bool LevelUp)
    {
        m_DeleteLine_Count += DeleteLine_Value;

        if (m_DeleteLine_Count == 0)
            return;
        //消したラインの数を表示させる
        Delete_text.text = m_DeleteLine_Count.ToString("00");

        //現在消したラインの数によってレベルを上げる
        if (m_DeleteLine_Count >= LINE_LOWERLIMIT * m_Level)
        {
            m_Level++;
            LevelUp = true;
            Level_Text.text = m_Level.ToString("00");
        }
    }

    /// <summary>
    /// スコアチェック
    /// </summary>
    /// <param name="DeleteLine_Value"></param>
    /// <param name="Perfect"></param>
    /// <param name="Tspin"></param>
    public void ScoreCheak(int DeleteLine_Value, bool Perfect, bool Tspin)
    {
        //何ライン消したかによってスコアを変動させる処理
        switch (DeleteLine_Value)
        {
            case 1:
                if (Tspin)
                {
                    m_NowScore = m_NowScore + ONE_LINE * m_Level * TSPIN_ADD;
                    break;
                }
                m_NowScore = m_NowScore + ONE_LINE * m_Level;
                break;
            case 2:
                if (Tspin)
                {
                    m_NowScore = m_NowScore + TWO_LINE * m_Level * TSPIN_ADD;
                    break;
                }
                m_NowScore = m_NowScore + TWO_LINE * m_Level;
                break;
            case 3:
                if (Tspin)
                {
                    m_NowScore = m_NowScore + THREE_LINE * m_Level * TSPIN_ADD;
                    break;
                }
                m_NowScore = m_NowScore + THREE_LINE * m_Level;
                break;
            case 4:
                m_NowScore = m_NowScore + TETRIS_LINE * m_Level;
                break;
            default:
                break;
        }
        //全消し出来た際のスコアの加算
        if(Perfect)
            m_NowScore = m_NowScore + PERFECT * m_Level;

        Score_Text.text = m_NowScore.ToString("000000000");
    }
}
