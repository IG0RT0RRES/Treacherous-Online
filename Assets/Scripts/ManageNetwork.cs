using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class ManageNetwork : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _startGameBtn;
    [SerializeField] private Button _joinRandomBtn;
    [SerializeField] private InputField _inputNameRoom;
    [SerializeField] private Dropdown _dropmaxPlayer, _modoParida, _tipodeArmas, _publicOuPrivado;
    [SerializeField] private Text _infoFinalRoom;
    [SerializeField] private Sprite[] _salas = new Sprite[0];
    [SerializeField] private Transform[] _spawn = new Transform[0];
    [SerializeField] private Image _imgSalas;
    [SerializeField] private Text _salasTxt, _nameRooms;

    private int _salasEsc = 0, _modoGame = 0, _armas = 0;
    private bool _pubpriv = false;
    private byte _maxPlay = 0;
    private string _publi;
    private int _nusalas = 0;

    #region Properties

    public int SalasEsc
    {
        get { return _salasEsc; }
    }

    public int Mode
    {
        get { return _modoGame; }
    }
    public int Armas
    {
        get { return _armas; }
    }

    #endregion

    private void Update()
    {
        _infoFinalRoom.text = _inputNameRoom.text + " / " + _publi + " / " + _maxPlay.ToString();
        
        UpdateServer();
        MaxP();
        TypeWeapon();
        ModeGame();
        PubPrivado();
        NextRoom();
    }

    private void UpdateServer()
    {
        _salasTxt.text = "Roons Online : " + PhotonNetwork.CountOfRooms.ToString();

        if (!PhotonNetwork.IsConnected)
        {
            print("Connectando ao servidor..");
            PhotonNetwork.ConnectUsingSettings();
        }
        if (PhotonNetwork.IsConnected && _inputNameRoom.text != "")
        {
            _startGameBtn.interactable = true;
        }
        else
        {
            _startGameBtn.interactable = false;
        }

        if (PhotonNetwork.IsConnected)
        {
            _joinRandomBtn.interactable = true;
        }
        else
        {
            _joinRandomBtn.interactable = false;
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado ao servidor.");
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        print("Conectado ao lobby");
    }

    public void CreateRoom()
    {
        RoomOptions RO = new RoomOptions() { IsOpen = true, IsVisible = _pubpriv, MaxPlayers = _maxPlay };
        PhotonNetwork.CreateRoom(_inputNameRoom.text, RO, TypedLobby.Default);
    }

    public void Right()
    {
        if (_nusalas <= 3)
        {
            _nusalas += 1;
        }
        else if (_nusalas > 3)
        {
            _nusalas = 0;
        }
    }
    public void Left()
    {
        if (_nusalas >= 1)
        {
            _nusalas -= 1;
        }
        else if (_nusalas < 1)
        {
            _nusalas = 4;
        }
    }

    private void NextRoom()
    {
        switch (_nusalas)
        {
            case 0:
                _imgSalas.sprite = _salas[_nusalas];
                _nameRooms.text = "sala01";
                break;
            case 1:
                _imgSalas.sprite = _salas[_nusalas];
                _nameRooms.text = "sala02";
                break;
            case 2:
                _imgSalas.sprite = _salas[_nusalas];
                _nameRooms.text = "sala03";
                break;
            case 3:
                _imgSalas.sprite = _salas[_nusalas];
                _nameRooms.text = "sala04";
                break;
            case 4:
                _imgSalas.sprite = _salas[_nusalas];
                _nameRooms.text = "sala05";
                break;
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Sala Criada com sucesso.");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("VanGuard", _spawn[0].transform.position, Quaternion.identity, 0);
        UiManager.instance.HidePainel(9, false);
    }

    public void JoinRandom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    private void MaxP()
    {
        switch (_dropmaxPlayer.value)
        {
            case 0:
                _maxPlay = 4;
                break;

            case 1:
                _maxPlay = 8;
                break;

            case 2:
                _maxPlay = 12;
                break;

            case 3:
                _maxPlay = 16;
                break;

            case 4:
                _maxPlay = 20;
                break;
        }
    }

    private void ModeGame()
    {
        switch (_modoParida.value)
        {
            case 0:
                _modoGame = 0;
                break;
            case 1:
                _modoGame = 1;
                break;
            case 2:
                _modoGame = 2;
                break;
        }
    }

    private void TypeWeapon()
    {
        switch (_tipodeArmas.value)
        {
            case 0:
                _armas = 0;
                break;
            case 1:
                _armas = 1;
                break;
            case 2:
                _armas = 2;
                break;
        }
    }

    private void PubPrivado()
    {
        switch (_publicOuPrivado.value)
        {
            case 0:
                _pubpriv = true;
                _publi = "Public";
                break;
            case 1:
                _pubpriv = false;
                _publi = "Private";
                break;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
