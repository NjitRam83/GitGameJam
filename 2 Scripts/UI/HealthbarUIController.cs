using BackpackSurvivors.MainGame;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarUIController : MonoBehaviour
{

    [SerializeField] Image HealthImage;
    [SerializeField] TextMeshProUGUI HealthText;
    Player _player;
    Health _health;

    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<Player>();

        if (_player == null) return;        
        _player.OnCharacterDied += _player_OnCharacterDied; 
        _health = _player.GetComponent<Health>();
        
       
    }

    private void _player_OnCharacterDied(object sender, CharacterDiedEventArgs e)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_health == null) return;
        UpdateVisuals(_health.CurrentHealth, _health.MaximumHealth);
    }

    public void UpdateVisuals(float currentHP, float maxHP)
    {
        HealthImage.fillAmount = currentHP / maxHP;
        HealthText.SetText(string.Format("{0}/{1}", (int)currentHP, (int)maxHP));
    }
}
