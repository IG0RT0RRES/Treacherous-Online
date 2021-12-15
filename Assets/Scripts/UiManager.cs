using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    [SerializeField] private MangeLifePlayer _mangeLife;
    [SerializeField] private ManageChat _manageChat;
    [SerializeField] private ManageKills _manageKills;
    [SerializeField] private ManageMunicao _manageMunicao;
    [SerializeField] private LayoutPlayers _layoutPlayers;
    [SerializeField] private PlayerSelection _playerSelection;
    [SerializeField] private PainelConfig _painelConfig;
    [Space(10)]

    [SerializeField] private GameObject _painelCharacterChoice;
    [SerializeField] private GameObject _painelRoomChoice;
    [SerializeField] private GameObject _painelSpawn;
    [SerializeField] private GameObject _painelExit;
    [SerializeField] private Transform _map;
    [Space(10)]

    [SerializeField] private bool _chat = false;
    [SerializeField] private bool _exit = false;
    [SerializeField] private bool _locked = false;
    [SerializeField] private bool _damage = false;
    [Space(10)]

    [SerializeField] private Color _flashColor = new Color(1f, 0f, 0f, 0.3f);
    [SerializeField] private float _flashSpeed = 5f;
    [Space(10)]

    [SerializeField] private Image _aimImage;
    [SerializeField] private Image _damageImg;
    [SerializeField] private Slider _lifeSlider;
    [SerializeField] private Text _ammunitionTxt;
    [SerializeField] private Camera _camUiConfig;
    [Space(10)]

    [SerializeField] private GameObject _positionPainel;
    [SerializeField] private GameObject _painelScore;
    [Space(10)]

    public string _personagemSeted;

    private int _limite = 0;
    private float _tempoMsg = 0f;
    private bool _canSend = true;

    #region Properties

    public GameObject PainelExit
    {
        get { return _painelExit; }
    }

    public Transform Map
    {
        get { return _map; }
    }

    public GameObject PainelSpawn
    {
        get { return _painelSpawn; }
    }

    public GameObject PainelCharacter
    {
        get { return _painelCharacterChoice; }
    }

    public GameObject PainelRoom
    {
        get { return _painelRoomChoice; }
    }

    public Camera CameraUi
    {
        get { return _camUiConfig; }
    }

    public Text Ammunition
    {
        get { return _ammunitionTxt; }
    }

    public Slider Life
    {
        get { return _lifeSlider; }
    }

    public Image DamageSprite
    {
        get { return _damageImg; }
    }

    public PainelConfig PainelConfigure
    {
        get { return _painelConfig; }
    }

    public bool Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }
    public MangeLifePlayer MangeLifePlayer
    {
        get { return _mangeLife; }
    }

    public ManageChat ManagerChat
    {
        get { return _manageChat; }
    }

    public ManageKills ManageKills
    {
        get { return _manageKills; }
    }

    public ManageMunicao ManageMunicao
    {
        get { return _manageMunicao; }
    }

    public PlayerSelection PlayerSelection
    {
        get { return _playerSelection; }
    }

    public LayoutPlayers LayoutPlayers
    {
        get { return _layoutPlayers; }
    }

    #endregion

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            SetReferences();
        }
        SceneManager.sceneLoaded += Loading;
    }

    void Loading(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            SetReferences();
        }
    }

    private void SetReferences()
    {
        _mangeLife = FindObjectOfType(typeof(MangeLifePlayer)) as MangeLifePlayer;
        _painelConfig = FindObjectOfType(typeof(PainelConfig)) as PainelConfig;

        _manageChat = GameObject.Find("PainelChat").GetComponent<ManageChat>();
       
        _manageKills = FindObjectOfType(typeof(ManageKills)) as ManageKills;
        _manageMunicao = FindObjectOfType(typeof(ManageMunicao)) as ManageMunicao;
        _layoutPlayers = FindObjectOfType(typeof(LayoutPlayers)) as LayoutPlayers;
        _playerSelection = FindObjectOfType(typeof(PlayerSelection)) as PlayerSelection;

        _painelSpawn = GameObject.Find("PainelSpawn");
        _damageImg = GameObject.Find("DamageImage").GetComponent<Image>();
        _aimImage = GameObject.Find("HitEnemyImage").GetComponent<Image>();
        _lifeSlider = GameObject.Find("LifePlayerSlider").GetComponent<Slider>();
        _ammunitionTxt = GameObject.Find("AmmunitionText").GetComponent<Text>();
        _camUiConfig = GameObject.Find("CamUiConfig").GetComponent<Camera>();
        _painelExit = GameObject.Find("PainelExit");
        _painelRoomChoice = GameObject.Find("PainelRoomChoice");
        _painelCharacterChoice  = GameObject.Find("PainelCharacterChoice");

        _map = GameObject.Find("Map_01").transform;

        _positionPainel = GameObject.Find("PositionPainel");
        _painelScore = GameObject.Find("PainelScore");
    }

    private void Start()
    {
        _manageChat.gameObject.SetActive(false); // esconde o chat.
        _chat = false;
        _painelSpawn.SetActive(false);
        _painelExit.SetActive(false);
    }

    public void JoinedRoomApply()
    {
        _painelRoomChoice.SetActive(false);
        _painelCharacterChoice.SetActive(true);
    }

    public void LeaveRoomApply()
    {
        _painelRoomChoice.SetActive(true);
        _painelCharacterChoice.SetActive(false);

        Camera.main.GetComponentInChildren<CameraVisao>().enabled = false;
        Camera.main.transform.SetParent(_map.transform, false);
        CamAuxio.instance.enabled = false;
        CamAuxio.instance.transform.SetParent(_map.transform, false);
        CamAuxio.instance.gameObject.SetActive(true);
        _exit = false;
        PhotonNetwork.LeaveRoom();
    }

    public void OnClickJoinRoom(string roomName)
    {
        if (PhotonNetwork.JoinRoom(roomName))
        {
            Debug.Log("Entrou na sala com sucesso.");
        }
        else
        {
            Debug.Log("Join room failed.");
        }
    }

    private void Update()
    {
        _tempoMsg += Time.deltaTime;
        
        Comandos(); // atualiza o médoto .

        if (_locked)
        {
            Lockmouse();
        }

        if (_damage)
        {
            _aimImage.color = _flashColor;
        }
        else
        {
            _aimImage.color = Color.Lerp(_aimImage.color, Color.clear, _flashSpeed * Time.deltaTime);
        }

        _damage = false;
    }

    public void EnviarMensagens()
    {
        if (_tempoMsg <= 5 && _canSend)
        {
            // envia a mensagem que foi escrita no input atravez de um rpc para todos na sala inclusive a si mesmo.
            PlayerC.instance.Enviar(SaveDados.Nickname, _manageChat.Mensagem);
            _limite += 1;
            if (_tempoMsg <= 10 && _limite >= 3)
            {
                _canSend = false;
                _tempoMsg = 0f;
            }
        }
        else if (_tempoMsg >= 60)
        {
            _canSend = true;
            _limite = 0;
            _tempoMsg = 0f;
        }

        if (_tempoMsg >= 5 && _canSend)
        {
            _tempoMsg = 0f;
            _limite = 0;
        }
    }

    public void Kill(string Name, int Pontos)
    {
        _layoutPlayers.SetPoints(Name, Pontos);
    }

    public void PlayerDead(string Name)
    {
        _layoutPlayers.SetDeadView(Name);
    }

    // seta na tela quem matou e quem foi morto. exibe tbm a arma usada.
    public void Kills(string matador, string morto, int arma)
    {
        ManageKills.ApplyKills(matador, morto, arma); // chama o método passando os parametros recebidos.
    }

    private void Comandos() // comandos para controlar a tela de Uis.
    {
        if (Input.GetKeyDown(KeyCode.T)) // exibe o chat na tela .
        {
            if (!_chat)
            {
                Cursor.lockState = CursorLockMode.None;
                _manageChat.gameObject.SetActive(true);
                _manageChat.Focus();
                _chat = true;
            }
            else
            {
                _locked = true;
                _manageChat.gameObject.SetActive(false); // esconde o chat.
                _chat = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !_chat)
        {
            if (!_exit)
            {
                Cursor.lockState = CursorLockMode.None;
                _painelExit.SetActive(true);
                _exit = true;
            }
            else
            {
                _locked = true;
                _painelExit.SetActive(false);
                _exit = false;
            }
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            _painelScore.transform.position = _aimImage.transform.position;
        }
        else
        {
            _painelScore.transform.position = _positionPainel.transform.position;
        }
    }

    public void Lockmouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _locked = false;
    }

    public void ApplayPersonagem()
    {
        _personagemSeted = _playerSelection.Name;
    }

    public void Dead()
    {
        _painelCharacterChoice.SetActive(true);
    }
}
