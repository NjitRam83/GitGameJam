using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace BackpackSurvivors.UI.Tooltip
{
    public class Tooltip : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _headerObject;
        [SerializeField] TextMeshProUGUI _contentObject;
        [SerializeField] TextMeshProUGUI _priceObject;
        [SerializeField] LayoutElement _layoutElement;
        [SerializeField] int _characterWrapLimit;

        [SerializeField] RectTransform _rect;

        // Start is called before the first frame update
        void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Application.isEditor)
            {
                int headerLength = _headerObject.text.Length;
                int contentLength = _contentObject.text.Length;

                _layoutElement.enabled = (headerLength > _characterWrapLimit || contentLength > _characterWrapLimit) ? true : false;
            }

            RepositionTooltip();

        }

        public void RepositionTooltip()
        {
            Vector2 position = Input.mousePosition;
            float pivotX = position.x / Screen.width;
            float pivotY = position.y / Screen.height;
            float finalPivotX = 0f;
            float finalPivotY = 0f;
            if (pivotX < 0.5) //If mouse on left of screen move tooltip to right of cursor and vice vera
            {
                finalPivotX = -0.3f;
            }

            else
            {
                finalPivotX = 1.03f;
            }



            if (pivotY < 0.5) //If mouse on lower half of screen move tooltip above cursor and vice versa
            {
                finalPivotY = 0;
            }

            else
            {
                finalPivotY = 1;
            }


            _rect.pivot = new Vector2(finalPivotX, finalPivotY);


            transform.position = position;
        }

        public void SetText(string content, string header = "", string price = "")
        {
            _headerObject.gameObject.SetActive(!string.IsNullOrEmpty(header));
            _contentObject.text = content;
            _headerObject.text = header;
            if (_priceObject != null)
            {
                _priceObject.gameObject.SetActive(!string.IsNullOrEmpty(price));
                _priceObject.text = price;
            }
            RepositionTooltip();
        }
    }
}