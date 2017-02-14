using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Prototype_Assets
{
    public class MoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public SubmarineController subController;

        public void OnPointerDown(PointerEventData eventData)
        {
            subController.StartMoving();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            subController.EndMoving();
        }
    }
}
