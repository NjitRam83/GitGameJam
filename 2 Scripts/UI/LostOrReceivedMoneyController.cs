using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LostOrReceivedMoneyController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _addedOrRemovedMoneyText;
    [SerializeField] Animator _addedOrRemovedAnimator;


    public void Init(int changedAmount)
    {
        if (isActiveAndEnabled)
        {
            if (changedAmount > 0)
            {
                _addedOrRemovedMoneyText.SetText($"+{changedAmount}");
                _addedOrRemovedAnimator.SetTrigger("Received");
            }
            else if (changedAmount < 0)
            {
                _addedOrRemovedMoneyText.SetText($"-{changedAmount}");
                _addedOrRemovedAnimator.SetTrigger("Lost");
            }
        }


        if (isActiveAndEnabled)
        {
            StartCoroutine(DoDestroy());
        }
        else
        {
            gameObject.SetActive(true);
            Destroy(gameObject);
        }
        
    }

    private IEnumerator DoDestroy()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(true);
        Destroy(gameObject);
    }
}
