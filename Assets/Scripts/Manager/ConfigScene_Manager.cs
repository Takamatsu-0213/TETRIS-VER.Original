using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/// <summary>
/// コンフィグ(設定)シーンの機能全てを管理するクラス
/// </summary>
public class ConfigScene_Manager : MonoBehaviour {

    public static float BGMVolume = 0.5f,  //BGMのボリューム
                        SEVolume = 0.5f;   //SEのボリューム

    const float CHANGE_INTERVALS = 0.8f; //シーン移行させる前の猶予時間を表す値
    private bool m_Rotation = false;     //オブジェクトを回転させるか判断する値

    public static KeyCode ConfigKeyInput;
    public GameObject Config_Button;
    public AudioClip Sound_Button;
    public new AudioSource[] audio;
    private string m_SceneName = null;
    public Slider BGMslider, SEslider;

    AudioSource audioSource;
    private void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
        audio[0].volume = ConfigScene_Manager.BGMVolume;
        for (int i = 1; i < audio.Length; i++)
        {
            audio[i].volume = ConfigScene_Manager.SEVolume;
        }
    }
    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(Config_Button);
        BGMslider.value = BGMVolume;
        SEslider.value = SEVolume;
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

        if (m_Rotation)
            Config_Button.transform.eulerAngles += new Vector3(0, 0, 3f);

        //フォーカスのをやり直す
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            EventSystem.current.SetSelectedGameObject(Config_Button);

        if (EventSystem.current.currentSelectedGameObject == Config_Button)
            Enter_Event();
        else
            Exit_Event();
    }

    //BGMの音量のスライダー
    public void BGMSlider()
    {
        BGMVolume = BGMslider.value;

        audio[0].volume = BGMVolume;
    }
    //SEの音量のスライダー
    public void SESlider()
    {
        SEVolume = SEslider.value;

        audio[1].volume = SEVolume;
    }

    public void OnClick_GoTitle()
    {
        audioSource.PlayOneShot(Sound_Button);
        m_SceneName = "TiTle";
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
