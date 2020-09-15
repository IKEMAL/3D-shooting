using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    /// <summary>///移動速度 /// </summary>
    [SerializeField] float speed = 10.0f;
    /// <summary>/// 回転速度/// </summary>
    [SerializeField] float rotateSpeed = 0.1f; 
    /// <summary>/// 発射する場所のオブジェクト/// </summary>
    [SerializeField] Transform nozzle;
   /// <summary>/// 弾のプレハブ/// </summary>
    [SerializeField] GameObject canon; 
    Rigidbody m_rb;

    

    // Start is called before the first frame update
    private void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        
    }

    private void Update()
    {

        this.transform.position += transform.forward * speed;//m_rb.AddForce(transform.forward * speed, ForceMode.Force);


        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.Rotate(-v * rotateSpeed, h * rotateSpeed, 0);
        

    }

    private void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (canon == null)
            {
                canon = Instantiate(canon, nozzle.position, nozzle.rotation);
            }
        }
    }
}
