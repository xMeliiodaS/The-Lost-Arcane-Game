using UnityEngine;

public class DisableUI : MonoBehaviour
{
    #region Singelton
    private static DisableUI instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one instance of Inventory found!");
            return;
        }
        instance = this;
    }
    public static DisableUI GetInstance()
    {
        return instance;
    }

    public static void SetInstance(DisableUI value)
    {
        instance = value;
    }

    #endregion

    [SerializeField] private GameObject uiGameplay;
    [SerializeField] private RectTransform parentUI;

    public GameObject GetUiGameplay()
    {
        return this.uiGameplay;
    }

    public void HideAllUIElements()
    {
        parentUI.localScale = Vector3.zero;
    }

    public void ShowAllUIElements()
    {
        parentUI.localScale = Vector3.one;
    }
}
