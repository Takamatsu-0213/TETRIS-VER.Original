using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ボタンを押下後の処理を管理するクラス
/// </summary>
public class Button_Controller : MonoBehaviour {

    public AudioClip Sound_Button;
    AudioSource audioSource;

    const float CHANGE_INTERVALS = 0.4f; //シーン移行させる前の猶予時間を表す値

    string m_SceneName = null;　　　　　 //移動先のシーン名を入れる値
    private void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    /// <summary>
    /// MainのSceneにシーン移動をする
    /// </summary>
    public void OnClick_GoMain()
    {
        audioSource.PlayOneShot(Sound_Button);
        m_SceneName = "Main";
        Invoke("Scene_Move",CHANGE_INTERVALS);
    }

    /// <summary>
    /// TitleのSceneにシーン移動をする
    /// </summary>
    public void OnClick_GoTitle()
    {
        OriginalCreate_Manager.Created = false;
        Time.timeScale = 1;
        audioSource.PlayOneShot(Sound_Button);
        m_SceneName = "Title";
        Invoke("Scene_Move", CHANGE_INTERVALS);
    }

    /// <summary>
    /// CreateのSceneにシーン移動をする
    /// </summary>
    public void OnClick_GoCreate()
    {
        audioSource.PlayOneShot(Sound_Button);
        m_SceneName = "Create";
        Invoke("Scene_Move", CHANGE_INTERVALS);
    }

    /// <summary>
    /// シーン移動を実行する
    /// </summary>
    void Scene_Move()
    {
        SceneManager.LoadScene(m_SceneName);
    }
}
