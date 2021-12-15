using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PainelConfig : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _startGameBtn, _buttonSpawn;
    [SerializeField] private Button _joinRandomBtn;
    [Space(10)]

    [SerializeField] private InputField _inputNameRoom;
    [SerializeField] private Dropdown _dropmaxPlayer, _publicOrPrivate, _groupSet;
    [Space(10)]

    [SerializeField] private Text _infoFinalRoom;
    [SerializeField] private Text[] _msgInfo;
    [Space(10)]

    [SerializeField] private Image _imgRoom;
    [Space(10)]

    [SerializeField] private Sprite[] _rooms = new Sprite[0];
    [Space(10)]

    public Slider SliderTimer;
    public InputField MensagensEnvio;
    public bool Exit = false;
    public Text Mens;
    public Text[] DeadSended;
    public Text Municao;
    public Image IconeWeapon;
    public Text RoomText, NameRooms;
    [Space(10)]

    [SerializeField] private int _groupA = 9;
    [SerializeField] private int _groupB = 19;

    //-----------------------------------

    private float _time = 0f;
    private float _timeTwo = 0;
    private float _timeTree = 0;
    private float _timeFour = 0;
    private int _numbRooms = 0;
    private bool _kill = false;
    private bool _playerConect = false;
    private bool _dead = false;
    private bool _spawnTimer = false;
    private byte _maxPlay = 0;
    private string _publi;

    private void Start()
    {
        PhotonNetwork.Disconnect();

        int ran = Random.Range(0, 9999);

        if (SaveDados.Nickname == "")
        {
            SaveDados.Nickname = "Visit: " + ran.ToString();
        }

        Mens.text = "Deseja realmente sair? ";
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateRoom()
    {
        RoomOptions RO = new RoomOptions() { IsOpen = true, IsVisible = _publicOrPrivate, MaxPlayers = _maxPlay };
        PhotonNetwork.CreateRoom(_inputNameRoom.text, RO, TypedLobby.Default);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connectado ao Servidor.");

        PhotonNetwork.JoinLobby(TypedLobby.Default);
        _startGameBtn.interactable = true;
        _joinRandomBtn.interactable = true;
    }

    public void SpawnInRoom()
    {
        GameManager.instance.SpawnPlayer();
    }

    public override void OnJoinedRoom()
    {
        UiManager.instance.JoinedRoomApply();
    }

    public void LeaveRoom()
    {
        UiManager.instance.LeaveRoomApply();
    }

    public void RespawnPlayer()
    {
        UiManager.instance.PainelSpawn.SetActive(true);
        GameManager.instance.RespawnPlayer();
    }

    public override void OnCreatedRoom()
    {
        print("Sala Criada com Sucesso.");
    }

    public override void OnJoinedLobby()
    {
        print("Conectado ao lobby");
    }

    private void OnPhotonCreateRoomFailed()
    {
        _msgInfo[0].text = "Falha ao Criar Sala. Algo saiu errado !";
    }

    private void OnPhotonRandomJoinFailed()
    {
        _msgInfo[0].text = "There are currently no active rooms, create one right now!";
        _joinRandomBtn.interactable = false;
    }

    private void UpdateServer()
    {
        RoomText.text = "Roons Online : " + PhotonNetwork.CountOfRooms.ToString();

        if (!PhotonNetwork.IsConnected)
        {
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

    private void Update()
    {
        _infoFinalRoom.text = _inputNameRoom.text + " / " + _publi + " / " + _maxPlay.ToString();

        UpdateServer();
        MaxP();
        Groups();

        PhotonNetwork.NickName = SaveDados.Nickname;
        _msgInfo[1].text = SaveDados.Nickname;

        if (_kill)
        {
            Conometro();
        }

        if (_playerConect)
        {
            ConometroInfoPlayerConnected();
        }

        if (_dead)
        {
            CronometroDead();
        }

        if (_spawnTimer)
        {
            CronometroSpawn();
        }
    }

    public void LeftOrRoom()
    {
        PhotonNetwork.LoadLevel(0);
    }

    public void Kills(string matador, string morto)
    {
        DeadSended[0].text = matador;
        IconeWeapon.gameObject.SetActive(true);
        DeadSended[1].text = morto;
        _kill = true;
    }

    void Conometro()
    {
        _time += Time.deltaTime;
        if (_time >= 5f)
        {
            DeadSended[0].text = "";
            IconeWeapon.gameObject.SetActive(false);
            DeadSended[1].text = "";
            _time = 0f;
            _kill = false;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SendMessage()
    {
        if (string.IsNullOrEmpty(MensagensEnvio.text))
            return;

        PlayerMove.instance.Enviar(PhotonNetwork.NickName, MensagensEnvio.text);
        MensagensEnvio.text = "";
    }

    public void Writing()
    {
        PlayerMove.instance.Esc(SaveDados.Nickname);
    }

    private void OnPhotonPlayerConnected(Player player)
    {
        _msgInfo[2].text = player + "\r\n" + "Entrou na Partida";
        _playerConect = true;
    }

    private void OnPhotonPlayerDisconnected(Player player)
    {
        _msgInfo[2].text = player + "\r\n" + "Saiu da Partida";
        _playerConect = true;
    }

    private void ConometroInfoPlayerConnected()
    {
        _timeTwo += Time.deltaTime;
        if (_timeTwo >= 5f)
        {
            _msgInfo[2].text = "";
            _timeTwo = 0f;
            _playerConect = false;
        }
    }

    public void Dead()
    {
        _dead = true;
    }

    void CronometroDead()
    {
        _timeTree += Time.deltaTime;

        if (_timeTree >= 2f)
        {
            UiManager.instance.PainelSpawn.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _dead = false;
            _spawnTimer = true;
            _timeTree = 0f;
        }
    }

    void CronometroSpawn()
    {
        _timeFour += Time.deltaTime;
        SliderTimer.value = _timeFour;
        if (_timeFour >= 10f)
        {
            SliderTimer.value = 10;
            _buttonSpawn.interactable = true;
            _spawnTimer = false;
            _timeFour = 0f;
        }
    }

    public void NextRoom(int direction)
    {
        _numbRooms += direction;

        if (_numbRooms > _rooms.Length - 1 || _numbRooms < 0)
        {
            _numbRooms = _numbRooms < 0 ? _rooms.Length - 1: 0;
        }

        _imgRoom.sprite = _rooms[_numbRooms];
        NameRooms.text = $"Room: { _numbRooms + 1 }.";
    }

    private void MaxP()
    {
        switch (_dropmaxPlayer.value)
        {
            case 0:
                _maxPlay = 2;
                break;

            case 1:
                _maxPlay = 4;
                break;

            case 2:
                _maxPlay = 8;
                break;

            case 3:
                _maxPlay = 12;
                break;

            case 4:
                _maxPlay = 16;
                break;
            case 5:
                _maxPlay = 20;
                break;
        }
    }

    private void Groups()
    {
        switch (_groupSet.value)
        {
            case 0:
                int randA = Random.Range(0, _groupA);
                GameManager.instance.Group = randA;
                break;
            case 1:
                int randB = Random.Range(10, _groupB);
                GameManager.instance.Group = randB;
                break;
        }
    }
}
