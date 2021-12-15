using UnityEngine;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
    [SerializeField] private Image _containerSprite;
    [SerializeField] private Text _nameText;
    [SerializeField] private Sprite[] _sprites;
    [Space(10)]

    [SerializeField] private int _indexSprite = 0;

    #region Properties
    public string Name
    {
        get { return _nameText.text; }
    }

    #endregion

    public void Right()
    {
        if (_indexSprite <= 3)
        {
            _indexSprite += 1;
        }
        else if (_indexSprite > 3)
        {
            _indexSprite = 0;
        }
    }
    public void Left()
    {
        if (_indexSprite >= 1)
        {
            _indexSprite -= 1;
        }
        else if (_indexSprite < 1)
        {
            _indexSprite = 4;
        }
    }

    private void Update()
    {
        Choice();
    }

    private void Choice()
    {
        switch (_indexSprite)
        {
            case 0:
                _nameText.text = "Captain";
                _containerSprite.sprite = _sprites[0];
                SetPlayer();
                break;
            case 1:
                _nameText.text = "Lieutenant";
                _containerSprite.sprite = _sprites[1];
                SetPlayer();
                break;
            case 2:
                _nameText.text = "Major";
                _containerSprite.sprite = _sprites[2];
                SetPlayer();
                break;
            case 3:
                _nameText.text = "Recruit";
                _containerSprite.sprite = _sprites[3];
                SetPlayer();
                break;
            case 4:
                _nameText.text = "Ybot";
                _containerSprite.sprite = _sprites[4];
                SetPlayer();
                break;
        }
    }

    private void SetPlayer()
    {
        UiManager.instance.ApplayPersonagem();
    }
}
