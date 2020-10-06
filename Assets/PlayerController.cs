using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
//Photonnの名前空間参照
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Networking.Types;
using System;

public class PlayerController : MonoBehaviourPunCallbacks // Photon Realtime 用のクラスを継承する
{
    [SerializeField] float speed = 10.0f;
    [SerializeField] float rotateSpeed = 0.3f;
    Rigidbody m_rb;

    public PhotonView m_view;

    /// <summary>/// 発射する場所のオブジェクト/// </summary>
    [SerializeField] Transform nozzle;
    /// <summary>/// 弾のプレハブ/// </summary>
    [SerializeField] string canonName = "canonPrefab";
    /// <summary>/// プレイヤーの最大HP /// </summary>
    [SerializeField] int maxHp = 100;
    /// <summary>/// プレイヤーのの現在のHP/// </summary>
    int m_life;
    /// <summary>ライフをオーナーから同期する間隔</summary>
    [SerializeField] float m_syncInterval = 1f;

    float m_syncTimer;

    [SerializeField] UnityEngine.UI.Slider lifeSlider;
    [SerializeField] UnityEngine.UI.Slider tuboSlider;

    [SerializeField] int maxEne = 100;
    float m_Ene;

    [SerializeField] Transform targetPos;
    GameObject m_cannonObject;  // 砲弾のオブジェクトを参照する（連射させないため）

    [SerializeField] CinemachineVirtualCamera vcam;
    [SerializeField] GameObject can;


    // Start is called before the first frame update
    private void Start()
    {
        // 中心を向く
        transform.LookAt(new Vector3(0, this.transform.position.y, 0));

        m_rb = GetComponent<Rigidbody>();
        m_view = GetComponent<PhotonView>();

        Debug.Log(m_view.IsMine);
        if (m_view.IsMine)
        {
            can.gameObject.SetActive(true);
            m_life = maxHp;
            m_Ene = maxEne;

            lifeSlider.value = 1;
            tuboSlider.value = 1;

            //lifeSlider.gameObject.SetActive(true);
            //tuboSlider.gameObject.SetActive(true);

            CinemachineVirtualCameraBase vcam = GameObject.FindObjectOfType<CinemachineVirtualCameraBase>();
            vcam.Follow = targetPos;
            vcam.LookAt = targetPos;
        }
        else
        {
            can.gameObject.SetActive(false);
            //lifeSlider.gameObject.SetActive(false);
            //tuboSlider.gameObject.SetActive(false);
        }


    }

    private void Update()
    {
        if (!m_view.IsMine) return;

        //プレイヤー間でライフを同期(あとから参加にするにはこれがいる)
        m_syncTimer += Time.deltaTime;
        if (m_syncTimer > m_syncInterval)
        {
            m_syncTimer = 0;
            object[] parameters = new object[] { m_life };
            m_view.RPC("SyncLife", RpcTarget.Others, parameters);
        }
        //移動
        if (Input.GetKey(KeyCode.Space) && tuboSlider.value > 0)
        {
            speed = 40.0f;
            Vector3 mov = transform.forward * speed;
            m_rb.velocity = new Vector3(mov.x, mov.y, mov.z);
            m_Ene -= 0.4f;
            tuboSlider.value = m_Ene / maxEne;
        }
        else
        {
            m_Ene += 0.2f;
            tuboSlider.value = m_Ene / maxEne;
            speed = 20.0f;
            Vector3 mov = transform.forward * speed;
            m_rb.velocity = new Vector3(mov.x, mov.y, mov.z);
        }

        //方向制御
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.Rotate(-v * rotateSpeed, h * rotateSpeed, 0);

        Attack();
    }

    //private void Attack()
    //{
    //    if (Input.GetButtonDown("Fire1"))
    //    {
    //        m_cannonObject = PhotonNetwork.Instantiate(canonName, nozzle.position, nozzle.rotation);
    //    }
    //}

    private void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (m_cannonObject == null)
            {
                m_cannonObject = PhotonNetwork.Instantiate(canonName, nozzle.position, nozzle.rotation);
            }
        }
    }

    /// <summary>/// ダメージを与える。ダメージを与えた側が呼び出す。/// </summary>
    /// <param name="playerId">ダメージを与えたプレイヤーのID</param>
    /// <param name="damage">ダメージ量</param>
    public void Damage(int playerId, int damage)
    {
        m_life -= damage;
        RefreshLife();
        // ライフが減ったら、他のクライアントとライフを同期する
        object[] parameters = new object[] { m_life };
        m_view.RPC("SyncLife", RpcTarget.Others, parameters);

        Debug.LogFormat("Player {0} が Player {1} の {2} に {3} のダメージを与えた", playerId, m_view.Owner.ActorNumber, name, damage);
        Debug.LogFormat("Player {0} の {1} の残りライフは {2}", m_view.Owner.ActorNumber, gameObject.name, m_life);
        if (m_life == 0)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }

    /// <summary>/// ダメージを与えたことをクライアント間で同期する /// </summary>
    /// <param name="currentLife"></param>
    [PunRPC]
    void SyncLife(int currentLife)
    {
        m_life = currentLife;
        RefreshLife();
        Debug.LogFormat("Player {0} の {1} の残りライフは {2}", m_view.Owner.ActorNumber, gameObject.name, m_life);
    }

    /// <summary>
    /// ライフの更新
    /// </summary>
    void RefreshLife()
    {
        if (lifeSlider)
        {
            lifeSlider.value = (float)m_life / maxHp;
        }
    }

    void RefreshTubo()
    {
        if (tuboSlider)
        {
            tuboSlider.value = (float)m_Ene / maxEne;
        }
    }

   

}