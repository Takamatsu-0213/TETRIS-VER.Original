using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背景オブジェクトの楕円運動を制御するクラス
/// </summary>
public class BackGround_Controller : MonoBehaviour {

    float m_Degree  = 0.0f,      //角度
          m_Radian  = 0.0f,      //ラジアン
          m_Speed   = 2.0f,  　　 //スピード
          m_DegreeToRadian = Mathf.PI / 180.0f,　//角度をラジアンに変更するためのもの
          m_CenterX = 6.0f, 　　//センターX
          m_CenterY = -9.0f,　　//センターY　
          m_RadiusX = 10.0f,    //半径X
          m_RadiusY = 5.0f,　　 //半径Y
          m_Cos,
          m_Sin;

    public bool Center = false,
                Right = false,
                Left = false;

    //Sin,Cosの初期化
    private void Start()
    {
        if (Center)
        {
            m_RadiusX = 10.0f;
            m_RadiusY = 0.0f;
        }

        m_Sin = Mathf.Sin(m_Radian);
        m_Cos = Mathf.Cos(m_Radian);
    }

    //楕円運動
    private void FixedUpdate()
    {
        if (Center)
        {
            transform.position = new Vector3(m_CenterX + m_Cos * m_RadiusX, m_CenterY + m_Sin * m_RadiusY, 0);
        }
        if (Left)
        {
            transform.position = new Vector3(m_CenterX + m_Cos * m_RadiusX, m_CenterY + (m_Sin * m_RadiusY) - (m_Cos * m_RadiusX), 0);
        }
        if (Right)
        {
            transform.position = new Vector3(m_CenterX + m_Cos * m_RadiusX, m_CenterY + (m_Sin * m_RadiusY) + (m_Cos * m_RadiusX), 0);
        }

        m_Degree += m_Speed;
        m_Degree = (m_Degree % 360 + 360) % 360;

        //角度をラジアンに変更
        m_Radian = m_Degree * m_DegreeToRadian;

        m_Sin = Mathf.Sin(m_Radian);
        m_Cos = Mathf.Cos(m_Radian);
    }
}
