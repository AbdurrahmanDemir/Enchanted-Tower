using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAnalyticsSDK;

public class GameAnalyticsManager : MonoBehaviour
{
    [Header("Optional")]
    [SerializeField] private bool sendTestEventOnStart = true;

    private static bool _initialized;

    private void Awake()
    {
        if (_initialized)
        {
            Destroy(gameObject);
            return;
        }

        _initialized = true;
        DontDestroyOnLoad(gameObject);

        // Ýstersen user id'yi initialize'dan önce set edebilirsin (opsiyonel)
        // GameAnalytics.SetCustomId("myCustomUserId"); // Initialize'dan önce olmalý. :contentReference[oaicite:4]{index=4}

        GameAnalytics.Initialize(); // SDK init :contentReference[oaicite:5]{index=5}
    }

    private void Start()
    {
        // "Waiting for first event..." yazýsýný kaldýran en pratik þey:
        // Ýlk frame'de 1 adet event yollamak.
        if (sendTestEventOnStart)
        {
            GameAnalytics.NewDesignEvent("game:start"); // Design event örneði :contentReference[oaicite:6]{index=6}
        }
    }

    // Ýstediðin yerden çaðýr diye kýsa helper’lar:
    public static void TrackDesign(string eventId)
    {
        GameAnalytics.NewDesignEvent(eventId); // :contentReference[oaicite:7]{index=7}
    }

    public static void TrackDesign(string eventId, float value)
    {
        GameAnalytics.NewDesignEvent(eventId, value); // :contentReference[oaicite:8]{index=8}
    }
}
