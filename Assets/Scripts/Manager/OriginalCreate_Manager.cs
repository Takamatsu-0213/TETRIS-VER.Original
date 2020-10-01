using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// オリジナルのミノの動作を管理するクラス
/// </summary>
public class OriginalCreate_Manager : MonoBehaviour {

    //シーン間で受け渡す値
    public static int[,] Mino_Array = new int[5, 5];　           //オリジナルミノの配列
    public static bool Created = false;　　　　　　　　　　　　  //オリジナルミノを製作したか否か判断する値
    public static string ArrayType = null;                       //ミノのタイプが入る値
    //オリジナルミノを作るために使う配列
    private GameObject[] CreateButton_List = new GameObject[21];  //作るミノの位置を決めさせるために使うボタンの配列
    private GameObject[] MinoTypeButton_List = new GameObject[7]; //交換するミノタイプを決めさせるために使うボタンの配列
    public Sprite[] MinoColor;

    const int ARRAY_MINO = 1, 　　 //配列上の「ミノ」を示す値
              POS_ADJUSTMENT = 2,  //位置調節に使う値
              MINO_MAXSIZE = 4;    //ミノの形になる最大個数

    private int m_Mino_Count = 0; 　//ボタンを押して作ったミノのカウント

    public GameObject MINO_Obj, Back_Ground, Create_Canvas, Confirmation_Canvas;
    public Text Explanatory_text;
    public AudioClip Sound_Button;
    public new AudioSource[] audio;
    AudioSource audioSource;
    private void Awake()
    {
        audioSource = this.GetComponent<AudioSource>();
        audio[0].volume = ConfigScene_Manager.BGMVolume;
        for (int i = 1; i < audio.Length; i++)
        {
            audio[i].volume = ConfigScene_Manager.SEVolume;
        };
        //初期化
        MINO_Obj.SetActive(false);
        Back_Ground.SetActive(false);
        Confirmation_Canvas.SetActive(false);
        Array.Clear(Mino_Array, 0, Mino_Array.Length);
        Created = false;
        ArrayType = null;
        //作るミノの位置を決めさせるために使うボタンを配列に格納
        for (int i = 0; i < CreateButton_List.Length; i++)
            CreateButton_List[i] = Create_Canvas.transform.GetChild(i).gameObject;
        //交換するミノタイプを決めさせるために使うボタンを配列に格納
        for (int i = 0; i < MinoTypeButton_List.Length; i++)
            MinoTypeButton_List[i] = Confirmation_Canvas.transform.GetChild(i).gameObject;
    }

    private void Start()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(CreateButton_List[0]);
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

        Mino_Created();

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            Return_Focus();
    }

    /// <summary>
    /// オリジナルミノを作る処理
    /// </summary>
    private void Mino_Created()
    {
        //ボタンを4つ選択されていなければこの先の処理へはいけない
        if (m_Mino_Count != MINO_MAXSIZE || Created)
            return;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        Created = true;
        m_Mino_Count = 0;
        //作ったミノ配列に沿って、ミノ(ゲームオブジェクト)を配置する処理
        for (int i = 0; i < Mino_Array.GetLength(0); i++)
        {
            for (int j = 0; j < Mino_Array.GetLength(1); j++)
            {
                if (Mino_Array[i, j] == ARRAY_MINO)
                {
                    MINO_Obj.transform.GetChild(m_Mino_Count).position = new Vector3(j - POS_ADJUSTMENT, -i + POS_ADJUSTMENT, 0);
                    m_Mino_Count++;
                }
                if (m_Mino_Count == MINO_MAXSIZE)
                    break;
            }
        }
        Create_Canvas.SetActive(false);
        Back_Ground.SetActive(true);
        MINO_Obj.SetActive(true);
        Confirmation_Canvas.SetActive(true);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(MinoTypeButton_List[0]);
    }

    /// <summary>
    /// フォーカスをし直す
    /// </summary>
    void Return_Focus()
    {
        if (!Created)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(CreateButton_List[CreateButton_List.Length -1]);
            return;
        }
        else
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(MinoTypeButton_List[MinoTypeButton_List.Length - 1]);
    }

    /// <summary>
    /// ミノ位置を選択させ、ミノを作るボタン
    /// </summary>
    /// <param name="number"></param>
    public void OnClick(int number)
    {
        if (Created)
            return;

        switch (number)
        {
            case 0:
                Mino_Array[1, 0] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 1:
                Mino_Array[1, 1] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 2:
                Mino_Array[1, 2] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 3:
                Mino_Array[1, 3] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 4:
                Mino_Array[1, 4] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 5:
                Mino_Array[2, 0] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 6:
                Mino_Array[2, 1] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 7:
                Mino_Array[2, 2] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 8:
                Mino_Array[2, 3] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 9:
                Mino_Array[2, 4] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 10:
                Mino_Array[3, 0] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 11:
                Mino_Array[3, 1] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 12:
                Mino_Array[3, 2] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 13:
                Mino_Array[3, 3] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 14:
                Mino_Array[3, 4] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 15:
                Mino_Array[4, 0] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 16:
                Mino_Array[4, 1] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 17:
                Mino_Array[4, 2] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 18:
                Mino_Array[4, 3] = ARRAY_MINO;
                m_Mino_Count++;
                break;
            case 19:
                Mino_Array[4, 4] = ARRAY_MINO;
                m_Mino_Count++;
                break;

            default:
                break;
        }
        audioSource.PlayOneShot(Sound_Button);
        CreateButton_List[number].GetComponent<Button>().interactable = false;
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(CreateButton_List[number+1]);
    }

    /// <summary>
    /// 交換するミノのタイプを決めるボタン
    /// </summary>
    /// <param name="number"></param>
    public void Change_Type(int number)
    {
        switch (number)
        {
            case 0:
                ArrayType = "MINO_I";
                break;
            case 1:
                ArrayType = "MINO_O";
                break;
            case 2:
                ArrayType = "MINO_S";
                break;
            case 3:
                ArrayType = "MINO_Z";
                break;
            case 4:
                ArrayType = "MINO_J";
                break;
            case 5:
                ArrayType = "MINO_L";
                break;
            case 6:
                ArrayType = "MINO_T";
                break;
            default:
                break;
        }
        for(int i = 0; i < MINO_MAXSIZE; i++)
        {
            MINO_Obj.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = MinoColor[number];
        }
        Explanatory_text.text = ArrayType + "と変更してゲームを始めますか？";
        audioSource.PlayOneShot(Sound_Button);
    }

    //「Main」シーンへ移行、その際に配列を渡す
    public void OnClick_GoMain()
    {
        if (ArrayType == null)
        {
            Explanatory_text.text = "変更するミノを選択して下さい";
            return;
        }
        audioSource.PlayOneShot(Sound_Button);
        SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// 作ったミノ配列を渡す
    /// </summary>
    /// <returns></returns>
    public static int[,] getCreateArray()
    {
        return Mino_Array;
    }
    /// <summary>
    /// オリジナルミノを製作したか否かををbool型で渡す
    /// </summary>
    /// <returns></returns>
    public static bool getCreateBool()
    {
        return Created;
    }
    /// <summary>
    /// どのミノと交換するか判断する値を渡す
    /// </summary>
    /// <returns></returns>
    public static string getArrayType()
    {
        return ArrayType;
    }
}
