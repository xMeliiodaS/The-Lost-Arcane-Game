using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private static DamagePopup instance;
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject marker;

    private void Awake()
    {
        instance = this;
    }


    public void CreateMarker(Vector3 position, string text, Color color)
    {
        var popup = Instantiate(marker, position, Quaternion.identity);
        var temp = popup.transform.GetComponent<TextMeshPro>();
        temp.text = text;
        temp.faceColor = color;
    }

    public static DamagePopup GetInstance()
    {
        return instance;
    }

    public static void SetInstance(DamagePopup value)
    {
        instance = value;
    }


    public void CreatePopup(Vector3 position, string text, Color color)
    {
        var popup = Instantiate(prefab, position, Quaternion.identity);
        var temp = popup.transform.GetComponent<TextMeshPro>();
        temp.text = text;
        temp.faceColor = color;

        // Destroy Timer
        Destroy(popup, 1f);
    }

/*    private void Update()
    {
        *//*if(Input.GetKeyDown(KeyCode.X))
            CreatePopup(Vector3.one, Random.Range(0, 100).ToString(), Color.yellow ); *//*
        if (Input.GetKeyDown(KeyCode.X))
            CreateMarker(Vector3.one, Random.Range(0, 100).ToString(), Color.yellow);
    }
*/}
