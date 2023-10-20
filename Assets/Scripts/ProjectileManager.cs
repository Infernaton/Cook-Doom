using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField]
    private float m_Speed;

    [SerializeField]
    private LayerMask m_LayerMask;

    void Start()
    {
        //Maybe Rewrite this to call for Last Direction
        PlayerManager p = transform.parent.GetComponent<PlayerManager>();
        Vector2 mouse = Mouse.current.position.value;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, Camera.main.nearClipPlane));

        //Set the gameworld origine as player position
        Debug.Log(mousePosition - p.gameObject.transform.position);

        Vector3 force = new Vector3(m_Speed * p.lastDirection.x, 0, m_Speed * p.lastDirection.y);
        GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision c)
    {
        //Check the layer value binary if the same as the layer mask
        if (1 << c.gameObject.layer == (m_LayerMask.value & 1 << c.gameObject.layer))
            Destroy(gameObject);
    }
}
