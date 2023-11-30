using BackpackSurvivors.Backpack;
using BackpackSurvivors.Items.ScriptableObjectSSS;
using UnityEngine;
using UnityEngine.UI;

public class ShopDebugButton : MonoBehaviour
{

    [Header("Base")]
    [SerializeField] public Button button;
    [SerializeField] public TMPro.TextMeshProUGUI text;
    public BackpackItemSO BackpackItemSO;
    public BackPackController BackpackGridManager;

    private void Start()
    {
        button.onClick.AddListener(this.ButtonClicked);
    }

    public void ButtonClicked()
    {
        Debug.Log("CLICK");
        BackpackGridManager.CreateItem(BackpackItemSO);
    }
}