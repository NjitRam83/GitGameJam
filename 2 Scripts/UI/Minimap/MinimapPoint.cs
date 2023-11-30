using BackpackSurvivors.MainGame;
using System;
using UnityEngine;
namespace BackpackSurvivors.UI.Minimap
{
    public class MinimapPoint : MonoBehaviour
    {
        [SerializeField] Player _player;

        [SerializeField] MinimapType MinimapType;
        [SerializeField] Sprite MinimapSpriteOverride;

        SpriteRenderer spriteRenderer;

        internal void Disable()
        {
            gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            switch (MinimapType)
            {
                case MinimapType.Player:
                    spriteRenderer.color = Color.blue;
                    TryAndBindPlayerCamera();
                    break;
                case MinimapType.Enemy:
                    spriteRenderer.color = Color.red;
                    break;
                case MinimapType.Friendly:
                    spriteRenderer.color = Color.green;
                    break;
                case MinimapType.Loot:
                    spriteRenderer.color = Color.white;
                    break;
                case MinimapType.Interactable:
                    spriteRenderer.color = Color.yellow;
                    break;
                case MinimapType.OverrideWithIcon:
                    spriteRenderer.sprite = MinimapSpriteOverride;
                    return;
                case MinimapType.Nothing:
                    break;
                case MinimapType.Projectile:
                    spriteRenderer.color = Color.cyan;
                    break;
                default:
                    break;
            }

            spriteRenderer.enabled = true;
        }

        private void TryAndBindPlayerCamera()
        {
            if (_player != null)
            {
                Minimap minimapGo = GameObject.FindFirstObjectByType<Minimap>();
                if (minimapGo != null)
                {
                    minimapGo.SetCameraToFollow(_player.MinimapPoint.transform);
                }
            }
        }
    }


}