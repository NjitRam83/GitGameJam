using BackpackSurvivors.Combat;
using BackpackSurvivors.Level.ScriptableObjects;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace BackpackSurvivors.UI
{
    public class YesNoPopupController : MonoBehaviour
    {

        [SerializeField] TextMeshProUGUI _headerText;
        [SerializeField] TextMeshProUGUI _messageText;
        [SerializeField] Animator _animator;
        public bool IsOpen;

        public void Show(string message, string title)
        {
            IsOpen = true;
            _messageText.SetText(message);
            _headerText.SetText(title);
            gameObject.SetActive(true);
            _animator.SetBool("Showing", true);
        }
        public void Hide()
        {
            IsOpen = false;
            _animator.SetBool("Showing", false);

            StartCoroutine(DoHide());

           
        }
        private IEnumerator DoHide()
        {
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
        }

        public void YesPressed()
        {
            if (OnYesPressed != null)
            {
                OnYesPressed(this, new YesPressedEventArgs(this));
            }
        }
        public void NoPressed()
        {
            if (OnNoPressed != null)
            {
                OnNoPressed(this, new NoPressedEventArgs(this));
            }
        }

        internal void ClearEvents()
        {
            OnYesPressed = null;
            OnNoPressed = null;
        }

        public delegate void YesPressedHandler(object sender, YesPressedEventArgs e);
        public event YesPressedHandler OnYesPressed;

        public delegate void NoPressedHandler(object sender, NoPressedEventArgs e);
        public event NoPressedHandler OnNoPressed;
    }

    public class YesPressedEventArgs : EventArgs
    {

        public YesPressedEventArgs(YesNoPopupController yesNoPopupController)
        {
            YesNoPopupController = yesNoPopupController;
        }

        public YesNoPopupController YesNoPopupController { get; }
    }

    public class NoPressedEventArgs : EventArgs
    {

        public NoPressedEventArgs(YesNoPopupController yesNoPopupController)
        {
            YesNoPopupController = yesNoPopupController;
        }

        public YesNoPopupController YesNoPopupController { get; }
    }
}