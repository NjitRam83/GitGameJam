using BackpackSurvivors.Items.ScriptableObjectSSS;
using UnityEngine;
using static BackpackSurvivors.Shared.Enums;

namespace BackpackSurvivors.Items
{
    public class ItemManager : MonoBehaviour
    {
        [SerializeField] public BackpackItemSO[] AllItems;

        private static ItemManager instance;

        public static ItemManager GetInstance()
        {
            return instance;
        }

        private void SetupSingleton()
        {
            if (instance != null)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Awake()
        {
            SetupSingleton();
            if (!gameObject.activeSelf) return;
        }

    }
}


