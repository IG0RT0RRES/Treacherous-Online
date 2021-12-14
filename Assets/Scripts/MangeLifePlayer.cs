using UnityEngine;
using UnityEngine.UI;

public class MangeLifePlayer : MonoBehaviour
{
    [SerializeField] private Text _text_NamePlayer;
    [SerializeField] private Text _text_Informativo;
    [SerializeField] private Slider _slider_Life;
    [SerializeField] private Image _danoImage;

    #region Properties
    public Text Text_NamePlayer
    {
        get { return _text_NamePlayer; }
    }

    public Text Text_Informativo
    {
        get { return _text_Informativo; }
    }
    
    public Slider Slider_Life
    {
        get { return _slider_Life; }
    }

    public Image DanoImage
    {
        get { return _danoImage; }
    }

    #endregion
}
