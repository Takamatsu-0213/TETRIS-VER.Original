using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エフェクトの動きを処理するクラス
/// </summary>
public class Effect_Controller : MonoBehaviour {

    public Transform Target;　　　　 //エフェクトの向かう先

    public float m_Threshold = 100f; //しきい値
    public float m_Intensity = 1f;　 //強度

    ParticleSystem Psy;
    ParticleSystem.Particle[] Particles;

    private void Awake()
    {
        Psy = this.GetComponent<ParticleSystem>();
    }

	void Update () {

        Particles = new ParticleSystem.Particle[Psy.main.maxParticles];
        int numParticlesAlive = Psy.GetParticles(Particles);
        for (int i = 0; i < numParticlesAlive; i++)
        {
            var Velocity = Psy.transform.TransformPoint(Particles[i].velocity);
            var Position = Psy.transform.TransformPoint(Particles[i].position);
            //終了点
            var Period = Particles[i].remainingLifetime * 0.9f;
            //ターゲットと自分自身の差
            var Diff = Target.TransformPoint(Target.position) - Position;
            //加速度
            Vector3 Accel = (Diff - Velocity * Period) * 2f / (Period * Period);

            //加速度が一定以上だと追尾を弱くする
            if (Accel.magnitude > m_Threshold)
            {
                Accel = Accel.normalized * m_Threshold;
            }

            // 速度の計算
            Velocity += Accel * Time.deltaTime * m_Intensity;
            Particles[i].velocity = Psy.transform.InverseTransformPoint(Velocity);
        }
        //パーティカルをセットする
        Psy.SetParticles(Particles, numParticlesAlive);
    }
}
