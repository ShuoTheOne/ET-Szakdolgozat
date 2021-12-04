using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float begin;
    public float dist = 4;
    public float speed = 4;
    public int dir;
    public GameObject Adam;

    // Start is called before the first frame update
    void Start()
    {
        begin = transform.position.x;
        dir = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > begin + dist) { dir = -1; }
        else { if (transform.position.x < begin - dist) { dir = 1; } }

        transform.position = new Vector3(transform.position.x + Time.deltaTime * speed * dir,
                                          transform.position.y,
                                          transform.position.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Adam.transform.parent = transform;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Adam.transform.parent = null;
    }

}
