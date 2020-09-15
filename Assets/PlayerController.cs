using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    PhotonView m_view;

   
    // Start is called before the first frame update
    private void Start()
    {
       m_rb = GetComponent<Rigidbody>();
        m_view = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!m_view.IsMine) return;

        //移動
        m_rb.AddForce(transform.forward * speed, ForceMode.Force);

        //方向制御
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vartical");
        transform.Rotate(-v * rotateSpeed, h * rotateSpeed, 0);
    }

    private void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (true)
            {

            }
        }
    }

    
}