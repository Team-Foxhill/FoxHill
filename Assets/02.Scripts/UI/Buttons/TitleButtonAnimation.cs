using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FoxHill.UI.Buttons
{
    /// <summary>
    /// TitleScene에서 사용되는 Button의 애니메이션을 관리합니다.
    /// Button에 컴포넌트로써 부착됩니다.
    /// </summary>
    [RequireComponent(typeof(Button), typeof(Image))]
    public class TitleButtonAnimation : MonoBehaviour
    {
        private Button _button;
        private Image _image;
        private TMP_Text _text;
        private readonly Color COLOR_TRANSPARENT = new Color(0f, 0f, 0f, 0f);

        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
            _text = transform.GetChild(0).GetComponent<TMP_Text>();

            OnHoverExit();
        }

        public void OnHoverEnter()
        {
            _image.color = Color.white;
            _text.color = Color.white;
        }

        public void OnHoverExit()
        {
            _image.color = COLOR_TRANSPARENT;
            _text.color = Color.black;
        }
    }
}
