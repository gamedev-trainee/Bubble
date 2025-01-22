using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bubble
{
    public class ButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Button button = null;

        public System.Action onButtonPressed = null;
        public System.Action onButtonReleased = null;

        public void OnPointerDown(PointerEventData eventData)
        {
            onButtonPressed?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onButtonReleased?.Invoke();
        }
    }
}