using UnityEngine;

public class LilGuy : MonoBehaviour
{
    private LilGuyFSM currentState;

    public Rigidbody2D rb;
    public Transform groundLevel;



    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        currentState = new Running(gameObject, rb, groundLevel);
    }

    void Update()
    {
        currentState = currentState.Process();
    }
}
