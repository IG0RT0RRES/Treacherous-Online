using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField] private Text _textInfo;
    [SerializeField] private InputField _nameInput;
    [SerializeField] private InputField _passwordInput;

    #region Properties
    public string Info
    {
        get { return _textInfo.text; }
        set { _textInfo.text = value; }
    }

    public string Name
    {
        get { return _nameInput.text; }
    }

    public string Password
    {
        get { return _passwordInput.text; }
    }

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            if (_nameInput.isFocused)
            {
                _passwordInput.Select();
            }
            else if(_passwordInput.isFocused)
            {
                _nameInput.Select();
            }
        }
    }


    public void Clear()
    {
        _nameInput.text = "";
        _passwordInput.text = "";
    }
}
