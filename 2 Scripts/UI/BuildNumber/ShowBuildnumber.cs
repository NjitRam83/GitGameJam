using TMPro;
using UnityEngine;

namespace BackpackSurvivors.UI.BuildNumber
{
    public class ShowBuildnumber : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _buildnumberText;
        [SerializeField] string _buildnumberPrefix = "Buildnumber:";
        [SerializeField] BuildnumberSO _buildnumberSO;

        private void Start()
        {
            _buildnumberText.text = $"{_buildnumberPrefix} {_buildnumberSO.Buildnumber}";
        }
    }
}
