using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tabsil.ModernUISystem
{
    public class PopupManager : MonoBehaviour
    {
        public static PopupManager instance;

        [Header(" Elements ")]
        [SerializeField] private Canvas popupCanvas;

        [Header(" Data ")]
        private List<UIPopupBase> activePopupList = new List<UIPopupBase>();

        [Header(" Actions ")]
        public static Action popupOpened;
        public static Action allPopupsClosed;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            UIPopupBase.closed += OnPopupClosed;
        }

        private void OnDestroy()
        {
            UIPopupBase.closed -= OnPopupClosed;
        }

        private void OnPopupClosed(UIPopupBase popup)
        {
            instance.activePopupList.Remove(popup);

            if (instance.activePopupList.Count > 0)
                return;

            allPopupsClosed?.Invoke();
        }

        public static void Show(UIPopupBase popup)
        {
            // Instantiate the popup
            UIPopupBase popupInstance = Instantiate(popup, instance.popupCanvas.transform);

            instance.activePopupList.Add(popupInstance);

            popupOpened?.Invoke();
        }

        public static T Show<T>(T popup) where T : UIPopupBase
        {
            T popupInstance = Instantiate(popup, instance.popupCanvas.transform);

            instance.activePopupList.Add(popupInstance);

            popupOpened?.Invoke();

            return popupInstance;
        }
    }
}