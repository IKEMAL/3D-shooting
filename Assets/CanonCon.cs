using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Photonの名前空間参照
using Photon.Pun;

public class CanonCon : MonoBehaviourPunCallbacks // Photon Realtime 用のクラスを継承する
{
    /// <summary>/// 弾を飛ばす力/// </summary>
    [SerializeField] float power = 1.0f;
    /// <summary>消滅するまでの秒数</summary>
    [SerializeField] float m_lifeTime = 1f;
    /// <summary>弾が与えるダメージ量</summary>
    [SerializeField] int m_attackPower = 5;
   

    float m_timer;

    Rigidbody m_rb;
    PhotonView m_view;


    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        m_view = GetComponent<PhotonView>();

        if (m_view.IsMine)
        {
            m_rb.AddForce(transform.forward * power, ForceMode.Impulse);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_view.IsMine) return;

        m_timer += Time.deltaTime;
        if (m_timer > m_lifeTime)
        {
            PhotonNetwork.Destroy(gameObject); // ネットワークオブジェクトとして Destroy する（他のクライアントからも消える）//this省略
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //自分が発射した弾が当たったら
        if (!m_view.IsMine)
        {
            //相手がPlayerだったらダメージ
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player)
            {
                player.Damage(PhotonNetwork.LocalPlayer.ActorNumber, m_attackPower);
                PhotonNetwork.Destroy(gameObject);//this省略
            }
        }
    }
}
