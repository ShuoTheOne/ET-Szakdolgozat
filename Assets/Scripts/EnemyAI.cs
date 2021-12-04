using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings

	public bool floating;
	public float begin;
	public float dist = 4;
	public float speed = 4;
	public int dir;

	public Animator animator;
	public ParticleSystem dust;

		// Start is called before the first frame update
		void Start()
    {
        begin = transform.position.x;
        dir = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Move();
	}
    private void Move()
    {
        if (transform.position.x > begin + dist) { dir = -1; CreateDust(); Flip(); }
        else { if (transform.position.x < begin - dist) { dir = 1; CreateDust(); Flip(); } }

        transform.position = new Vector3(transform.position.x + Time.deltaTime * speed * dir,
                                          transform.position.y,
                                          transform.position.z);
    }

    private void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void CreateDust()
    {
        dust.Play();
    }
}
