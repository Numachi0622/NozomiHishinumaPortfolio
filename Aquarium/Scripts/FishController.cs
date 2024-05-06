using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FishController : MonoBehaviour
{
    private Rigidbody rb;
    private Transform tf;
    private Quaternion targetRot; // is•ûŒü‚ÉŒü‚­‚æ‚¤‚ÈQuaternion
    private float speed = 2f;
    private float moveDistance; // ˆÚ“®‚µ‚½‹——£
    private float movableRange = 7f;
    private bool isMovable => moveDistance < movableRange;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tf = GetComponent<Transform>();
    }

    private void Start()
    {
        targetRot = transform.rotation;
    }

    private void Update()
    {
        moveDistance = tf.position.magnitude;
        if (isMovable)
        {
            rb.velocity = tf.forward * speed;
        }
        else
        {
            RaycastHit hit;
            if (!Physics.Raycast(tf.position, tf.forward, out hit))
            {
                targetRot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                tf.rotation = Quaternion.RotateTowards(tf.rotation, targetRot, 1000 * Time.deltaTime);
            }
            rb.velocity = tf.forward * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //tf.LookAt(other.transform.position)
        Vector3 dir = other.transform.position - tf.position;
        targetRot = Quaternion.LookRotation(dir);
        tf.DORotateQuaternion(targetRot,0.3f);
        //tf.rotation = Quaternion.RotateTowards(tf.rotation, targetRot, 2000 * Time.deltaTime);
    }

    public void MoveStop()
    {
        speed = 0;
    }

    public void ResumeMove()
    {
        speed = 2f;
    }


}
