using TMPro;
using UnityEngine;

public class PassivePrefab : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _text;
    internal void SetText(string value)
    {
        _text.SetText(value);
    }
}

