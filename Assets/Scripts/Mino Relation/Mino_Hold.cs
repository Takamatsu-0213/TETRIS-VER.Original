using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ミノ(テトリミノ)をホールドするクラス
/// </summary>
public class Mino_Hold : MonoBehaviour
{
    //ホールドミノの保存領域
    private int[,] HoldMino_Array = new int[5, 5];
    private GameObject[] HoldMino_Obj = new GameObject[4];

    const int LINE_UPPER = 10,     //ミノが一列に揃った時の数
              POS_X = 5,           //ゲーム内のミノのX座標を設定する値
              POS_Y = -2,          //ゲーム内のミノのY座標を設定する値
              POS_ADJUSTMENT = 2,  //位置調節に使う値
              MINO_MAX = 4,        //ミノが形になるまでの最大個数
              ARRAY_NONE = 0,      //配列上の「何もない」を示す値
              ARRAY_MINO = 1,      //配列上の「ミノ」を示す値
              MAXIMUM_SCALE = 1;   //HoldMinosのScaleの最大値

    const float MINIMUM_SCALE = 0.5f; //HoldMinosのScaleの最小値

    [SerializeField] private GameObject HoldMinos;　   //ホールドしたミノを入れるもの(インスペクター上から入れる)

    private bool m_Holdkeep = false;  //ホールドしているか否か

    private int Mino_BeforeType = 0;  //ホールドしているミノタイプを保持

    private Sprite HoldMino_Sprite;

    Game_Manager GM;

    private void Awake()
    {
        GM = this.GetComponent<Game_Manager>();
    }

    public void Mino_hold(ref Mino_Controller.MINOs_Array Mino, GameObject MINOs, GameObject GHOSTMINOs, ref bool MinoholdOnce)
    {
        //HoldMinosを最大サイズへ
        HoldMinos.transform.localScale = new Vector3(MAXIMUM_SCALE, MAXIMUM_SCALE, 0);
        //MINOsをHoldMinoの位置まで持ってくる
        MINOs.transform.position = HoldMinos.transform.position;
        HoldMinos.transform.DetachChildren();
        MINOs.transform.DetachChildren();

        //初回ホールド時の処理
        if (m_Holdkeep == false)
        {
            //ミノのタイプを保存
            Mino_BeforeType = Mino.MinoType;
            //配列コピー
            Array.Copy(Mino.NowMino, 0, HoldMino_Array, 0, Mino.NowMino.Length);
            HoldMino_Sprite = GHOSTMINOs.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;

            //ホールドしたミノを持つ、MINOsからゲームオブジェクトをもらってくる
            for (int i = 0; i < HoldMino_Obj.Length; i++)
            {
                Mino.UseMino[i].transform.parent = HoldMinos.transform;
                HoldMino_Obj[i] = HoldMinos.transform.GetChild(i).gameObject;
                GHOSTMINOs.transform.GetChild(i).gameObject.SetActive(false);
            }

            //HoldMinosを縮小させる
            HoldMinos.transform.localScale = new Vector3(MINIMUM_SCALE, MINIMUM_SCALE, 0);
            m_Holdkeep = true;
            GM.state = Game_Manager.State.CREATE_MINO;
            return;
        }
        else if (m_Holdkeep == true && MinoholdOnce == true) //二回目以上のホールド処理
        {
            MinoholdOnce = false;

            //ミノのタイプを入れ替える
            int tmp = Mino_BeforeType;
            Mino_BeforeType = Mino.MinoType;
            Mino.MinoType = tmp;

            int[,] tmp_Array = new int[Mino.NowMino.GetLength(0), Mino.NowMino.GetLength(1)];
            Array.Copy(HoldMino_Array, 0, tmp_Array, 0, HoldMino_Array.Length);
            //ホールドしたミノを保存領域の配列にコピー
            Array.Copy(Mino.NowMino, 0, HoldMino_Array, 0, Mino.NowMino.Length);
            //ホールドしてあったミノを NowMino配列へコピー
            Array.Copy(tmp_Array, 0, Mino.NowMino, 0, tmp_Array.Length);

            //ホールドしたミノ(GameObject)をMINOsからもらってくる
            for (int i = 0; i < HoldMino_Obj.Length; i++)
            {
                Mino.UseMino[i].transform.parent = HoldMinos.transform;
            }
            //ホールドしてあったミノ(GameObject)をMINOsへ渡す
            for (int i = 0; i < HoldMino_Obj.Length; i++)
            {
                Mino.UseMino[i] = HoldMino_Obj[i];
                Mino.UseMino[i].transform.parent = MINOs.transform;
                HoldMino_Obj[i] = HoldMinos.transform.GetChild(i).gameObject;
            }
            HoldMino_Sprite = MINOs.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            int CheckedMino = 0;
            //位置調整
            for (int i = 0; i < Mino.NowMino.GetLength(0); i++)
            {
                for (int j = 0; j < Mino.NowMino.GetLength(1); j++)
                {
                    if (Mino.NowMino[i, j] == ARRAY_MINO)
                    {
                        //GHOSTMINOsのスプライトをホールドしてあったミノのスプライトに変える
                        GHOSTMINOs.transform.GetChild(CheckedMino).GetComponent<SpriteRenderer>().sprite = HoldMino_Sprite;
                        GHOSTMINOs.transform.GetChild(CheckedMino).name = HoldMino_Sprite.name;
                        //GHOSTMINOsの子のゲームオブジェクトをホールドしてあったミノの形に変形させる
                        GHOSTMINOs.transform.GetChild(CheckedMino).localPosition = new Vector3(j - POS_ADJUSTMENT, -i + POS_ADJUSTMENT, 0);

                        CheckedMino++;
                    }
                    if (CheckedMino == MINO_MAX)
                        break;
                }
                if (CheckedMino == MINO_MAX)
                    break;
            }
            //HoldMinosを縮小させる
            HoldMinos.transform.localScale = new Vector3(MINIMUM_SCALE, MINIMUM_SCALE, 0);
            //初期位置にセット
            MINOs.transform.position = new Vector3(POS_X, POS_Y, 0);
            GHOSTMINOs.transform.position = new Vector3(POS_X, POS_Y, 0);
        }
    }
}
