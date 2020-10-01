using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// キー入力を管理するクラス
/// </summary>
public class Mino_KeyInput : MonoBehaviour
{
    const int OPEN_DEBUGNUBER = 10,   //配列の表示をさせる値
              CLOSE_DEBUGNUBER = 11,  //配列の非表示をさせる値
              TETRIS_DEBUGNUBER = 12, //テトリスができる盤面にさせる値
              TSPINS_DEBUGNUBER = 13, //Tスピンシングルができる盤面にさせる値
              TSPIND_DEBUGNUBER = 14, //Tスピンダブルができる盤面にさせる値
              TSPINT_DEBUGNUBER = 15, //Tスピントリプルができる盤面にさせる値
              LEVELUP_DEBUGNUBER = 16; //Tスピントリプルができる盤面にさせる値

    const float OFF_TIME = 1.5f;

    private float m_MoveY = 0,  //ミノ配列のY軸の値
                  m_MoveX = 0;  //ミノ配列のX軸の値

    public InputField Inputfield;
    public Text Error_Text;

    Game_Manager GM;

    private void Start()
    {
        GM = this.GetComponent<Game_Manager>();
    }

    /// <summary>
    /// ミノのキー入力処理
    /// </summary>
    /// <param name="Yheight"></param>
    /// <param name="XSidePos"></param>
    /// <param name="Minohold"></param>
    /// <param name="Drop"></param>
    /// <param name="Ghost"></param>
    /// <param name="Lrotation"></param>
    /// <param name="Rrotation"></param>
    public void KeyInput(ref int Yheight, ref int XSidePos, ref bool Minohold, ref bool Drop, ref bool Ghost, ref bool Lrotation, ref bool Rrotation)
    {
        if (GM.state == Game_Manager.State.ARRAYSET_MINO)
            return;

        m_MoveX = (int)Input.GetAxisRaw("Horizontal") | (int)Input.GetAxisRaw("Hori");
        m_MoveY = (int)Input.GetAxisRaw("Vertical") | (int)Input.GetAxisRaw("Ver");

        //ホールド
        Minohold = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1);
        //ローテーション
        Lrotation = Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Joystick1Button2);

        Rrotation = Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Joystick1Button0);

        //ハードドロップ　　　　　　　　
        Drop = m_MoveY > 0;
        if (Drop)
            return;

        GM.state = Game_Manager.State.PLAY_NOKEYINPUT;

        //手動で左右どちらかに進ませる
        if (m_MoveX != 0 && m_MoveY == 0)
        {
            XSidePos += (int)m_MoveX;
            Ghost = true;
            GM.state = Game_Manager.State.PLAY_KEYINPUT;
        }
        //手動で下に進ませる
        if (m_MoveY < 0 && m_MoveX == 0)
        {
            Yheight += -(int)m_MoveY;
            GM.state = Game_Manager.State.PLAY_KEYINPUT;
        }
    }

    /// <summary>
    /// デバックキー入力処理
    /// </summary>
    /// <param name="DebugNuber"></param>
    public int Dbg_KeyInput()
    {
        //何のデバックをするか判断する数値
        int DebugNuber = 0;

        //スラッシュキー入力でコマンド入力が可能になる
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Slash))
        {
            GM.state = Game_Manager.State.DEBUGGING;
            Inputfield.gameObject.SetActive(true);
            Inputfield.ActivateInputField();
            Time.timeScale = 0;
        }
        //Enterキー入力でコマンド入力が確定する
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //改行などのエスケープ文字の削除
            Inputfield.text = Inputfield.text.Replace("\r", "").Replace("\n", "");
            GM.state = Game_Manager.State.PLAY_NOKEYINPUT;
            //入力されたコマンドによって、分岐処理をさせる
            switch (Inputfield.text)
            {
                case "OPEN":
                    DebugNuber = OPEN_DEBUGNUBER;
                    break;
                case "CLOSE":
                    DebugNuber = CLOSE_DEBUGNUBER;
                    break;
                case "TETRIS":
                    DebugNuber = TETRIS_DEBUGNUBER;
                    break;
                case "TSPIN SINGLE":
                    DebugNuber = TSPINS_DEBUGNUBER;
                    break;
                case "TSPIN DOUBLE":
                    DebugNuber = TSPIND_DEBUGNUBER;
                    break;
                case "TSPIN TRIPLE":
                    DebugNuber = TSPINT_DEBUGNUBER;
                    break;
                case "LEVEL UP":
                    DebugNuber = LEVELUP_DEBUGNUBER;
                    break;
                default:
                    Error_Text.gameObject.SetActive(true);
                    Error_Text.text = "「" + Inputfield.text + "」というコマンドは存在しません";
                    Invoke("Error_Off", OFF_TIME);
                    break;
            }
            Inputfield.text = "";
            Time.timeScale = 1;
            Inputfield.gameObject.SetActive(false);
        }
        //ショートカットキー入力
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.T) && GM.state != Game_Manager.State.DEBUGGING)
        {
            DebugNuber = TETRIS_DEBUGNUBER;
        }
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.L) && GM.state != Game_Manager.State.DEBUGGING)
        {
            DebugNuber = LEVELUP_DEBUGNUBER;
        }
        return DebugNuber;
    }

    void Error_Off()
    {
        Error_Text.text = "";
        Error_Text.gameObject.SetActive(false);
    }
}

