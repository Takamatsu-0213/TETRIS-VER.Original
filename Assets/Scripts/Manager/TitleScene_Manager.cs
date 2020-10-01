using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// タイトルシーンの機能全てを管理するクラス
/// </summary>
public class TitleScene_Manager : MonoBehaviour
{
    public Image Fade;
    public GameObject Text, Config_Button;
    public GameObject[] Title_Button = new GameObject[2];
    public new AudioSource[] audio;
    public AudioClip Sound_Button;

    private float m_Alfa = 0.02f;        //点滅させる際に使うアルファ値

    const float LOCAL_SCALE = 0.005f,    //拡大、縮小させる際に使うスケール値
                MOVEDISTANCE = 5.0f,     //移動させる際に使う値
                RECT_POSITION = -100.0f, //既定のポジションを表す値
                ALFA_VALUE = 0.02f,      //アルファ値を既定の値に変える際に使う値
                CHANGE_INTERVALS = 0.8f; //シーン移行させる前の猶予時間を表す値

    private string m_SceneName = null;   //移動先のシーン名を入れる値

    private bool m_AppearButton = false, //ボタンを出現させるか判断する値
                 m_Rotation = false,     //オブジェクトを回転させるか判断する値
                 m_Down = false,         //アルファ値を減らすか判断する値
                 m_up = false;           //アルファ値を増やすか判断する値

    AudioSource audioSource;
    private void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
        Config_Button.SetActive(false);
        audio[0].volume = ConfigScene_Manager.BGMVolume;
        for (int i = 1; i < audio.Length; i++)
        {
            audio[i].volume = ConfigScene_Manager.SEVolume;
        }
    }

    private void Update()
    {
        //AnyButton押下後の処理
        if (Input.anyKeyDown && !m_AppearButton && m_Alfa == ALFA_VALUE)
        {
            audioSource.PlayOneShot(Sound_Button);
            m_AppearButton = true;
            m_Alfa = m_Alfa + ALFA_VALUE;
            for (int i = 0; i < Title_Button.Length; i++)
                Title_Button[i].SetActive(true);
            Config_Button.SetActive(true);
            Text.SetActive(false);
        }

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
        Text_Flashing();
        Button_Move();

        //オプションボタンにマウスを重ねた時の反応処理
        if (m_Rotation)
            Config_Button.transform.eulerAngles += new Vector3(0, 0, 3f);

        //ゲームパッド操作時のオプションボタンの反応処理
        if (EventSystem.current.currentSelectedGameObject == Config_Button)
            Enter_Event();
        else
            Exit_Event();
    }

    /// <summary>
    /// Textの点滅
    /// </summary>
    void Text_Flashing()
    {
        if (Text.transform.GetComponent<Text>().color == new Color(0, 0, 0, 1))
        {
            m_up = false;
            m_Down = true;
        }
        else if (Text.transform.GetComponent<Text>().color == new Color(0, 0, 0, 0))
        {
            m_up = true;
            m_Down = false;
        }

        if (m_Down)
        {
            Text.transform.GetComponent<Text>().color -= new Color(0, 0, 0, m_Alfa);
            Text.transform.localScale -= new Vector3(LOCAL_SCALE, LOCAL_SCALE, 0);
        }
        else if (m_up)
        {
            Text.transform.GetComponent<Text>().color += new Color(0, 0, 0, m_Alfa);
            Text.transform.localScale += new Vector3(LOCAL_SCALE, LOCAL_SCALE, 0);
        }
    }

    /// <summary>
    /// UIのボタンの移動
    /// </summary>
    void Button_Move()
    {
        if (m_AppearButton)
        {
            Title_Button[0].transform.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, MOVEDISTANCE);
            Title_Button[1].transform.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, MOVEDISTANCE);

            if (Title_Button[0].transform.GetComponent<RectTransform>().anchoredPosition.y >= RECT_POSITION)
            {
                EventSystem.current.SetSelectedGameObject(Config_Button);
                m_AppearButton = false;
            }
        }
    }

    //下記以降はUIのボタンによる、シーン移動
    public void OnClick_GoMain()
    {
        audioSource.PlayOneShot(Sound_Button);
        m_SceneName = "Main";
        Invoke("Scene_Move", CHANGE_INTERVALS);
    }
    public void OnClick_GoCreate()
    {
        audioSource.PlayOneShot(Sound_Button);
        m_SceneName = "Create";
        Invoke("Scene_Move", CHANGE_INTERVALS);
    }
    public void OnClick_GoConfig()
    {
        audioSource.PlayOneShot(Sound_Button);
        m_SceneName = "Config";
        Invoke("Scene_Move", CHANGE_INTERVALS);
    }

    void Scene_Move()
    {
        SceneManager.LoadScene(m_SceneName);
    }

    public void Enter_Event()
    {
        m_Rotation = true;
    }

    public void Exit_Event()
    {
        m_Rotation = false;
    }
}
