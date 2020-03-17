using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeController : MonoBehaviour
{
    [SerializeField]
    private float rayDistance = 20f;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip landClip;
    private int raycastMask;
    Vector3 direction;
    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        rb.isKinematic = true;
        raycastMask = 1 << LayerMask.NameToLayer("Player");
        startPos = transform.position;
        startPos.y -= 6f;
        direction = Quaternion.AngleAxis(-60.0f, -Vector2.up) * -Vector2.one;
    }

    // Run the raycasts in fixed update, in case they're running above 60fps, it will stop it bogging the machine down
    void FixedUpdate()
    {   RaycastHit2D hit = Physics2D.Raycast(startPos, direction, rayDistance, raycastMask);
        //Debug.DrawRay(startPos, direction, Color.red, rayDistance);

        if(hit.collider != null)
        {
            if (hit.transform.CompareTag("Player"))
            {
                rb.isKinematic = false;
                if(!audioSource.isPlaying)
                    audioSource.Play();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            audioSource.Stop();
            audioSource.clip = landClip;
            audioSource.Play();
        }
    }
}
