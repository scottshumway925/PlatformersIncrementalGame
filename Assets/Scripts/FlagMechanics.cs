using UnityEngine;

public class FlagMechanics : MonoBehaviour
{
    public GameObject playerCharacter;
    public GameObject basicLilGuy;
    public int playerCash;
    public int lilGuyCash;
    public Vector3 spawnLocation;
    private MoneyManager money;

    void Start()
    {
        money = GameObject.FindGameObjectWithTag("Money").GetComponent<MoneyManager>();
    }

    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered in");
            money.money += playerCash;
            Destroy(other.gameObject);
            Instantiate(playerCharacter, spawnLocation, Quaternion.identity);
        }
    }
}
