using BackpackSurvivors.Backpack;
using BackpackSurvivors.MainGame;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.UI
{
    public class UiController : MonoBehaviour
    {
        [SerializeField] GameObject _numberPopupPrefab;


        public void InstantiateNumberPopup(Vector2 position, string text, Color color)
        {
            var numberPopupGameobject = Instantiate(_numberPopupPrefab, position, Quaternion.identity);
            var numberPopup = numberPopupGameobject.GetComponent<NumberPopup>();
            numberPopup.Init(text, color);
        }
    }
}
