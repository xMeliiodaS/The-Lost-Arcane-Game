using UnityEngine;

public class FindPlayer : MonoBehaviour
{
    private static FindPlayer instance;
    private GameObject player;

    public static FindPlayer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<FindPlayer>();
                if (instance == null)
                {
                    Debug.LogError("FindPlayer instance not found in the scene.");
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        player = GameObject.FindWithTag("Player");
    }


    public GameObject Player
    {
        get { return player; }
        set { player = value; }
    }
}
