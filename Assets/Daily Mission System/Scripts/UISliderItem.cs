using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Tabsil.DailyMissions
{
    [RequireComponent(typeof(Button))]
    public class UISliderItem : MonoBehaviour
    {
        [Header(" Elements ")]
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI text;
        public TextMeshProUGUI Text => text;
        
        private Button button;
        public Button Button => button; 

        public void Configure(Sprite sprite, string label)
        {
            image.sprite = sprite;
            text.text = label;

            button = GetComponent<Button>();
        }

        [NaughtyAttributes.Button]
        public void Animate()
        {
            LeanTween.cancel(image.gameObject);
            LeanTween.rotate(image.gameObject, Vector3.forward * 10, .5f).setLoopPingPong(100);
        }

        [NaughtyAttributes.Button]
        public void StopAnimation()
        {
            LeanTween.cancel(image.gameObject);
            LeanTween.rotate(image.gameObject, Vector3.zero, .5f);
        }
    }
}