using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Photonの名前空間参照
using Photon.Pun;

public class CanonCon : MonoBehaviourPunCallbacks // Photon Realtime 用のクラスを継承する
{
    [SerializeField] float power = 1.0f;
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

    }
}
