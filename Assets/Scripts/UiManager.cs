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
    [SerializeField] private SelecaoDePlayers _selecaoDePlayers;
    [SerializeField] private PainelConfig _painelConfig;
    [Space(10)]

    [SerializeField] private GameObject _painelSpawn;
    [Space(10)]

    [SerializeField] private bool _chat = false;
    [SerializeField] private bool _sair = false;
    [SerializeField] private bool _locked = false;
    [SerializeField] private bool _damaged = false;
    [Space(10)]

    [SerializeField] private Color _flashColor = new Color(1f, 0f, 0f, 0.3f);
    [SerializeField] private float _flashSpeed = 5f;
    [SerializeField] private Image _miraImage;
    [Space(10)]

    [SerializeField] private GameObject _positionPainel;
    [SerializeField] private GameObject _painelScore;
    [Space(10)]

    public string _personagemSeted;

    private int _limite = 0;
    private float _tempoMsg = 0f;
    private bool _canSend = true;

    #region Properties

    public PainelConfig PainelConfigure
    {
        get { return _painelConfig; }
    }

    public bool Damaged
    {
        get { return _damaged; }
        set { _damaged = value; }
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

    public SelecaoDePlayers SelecaoDePlayers
    {
        get { return _selecaoDePlayers; }
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
        _selecaoDePlayers = FindObjectOfType(typeof(SelecaoDePlayers)) as SelecaoDePlayers;

        _painelSpawn = GameObject.Find("PainelSpawn");

        _miraImage = GameObject.Find("Hit_Enemy").GetComponent<Image>();
        _positionPainel = GameObject.Find("PositionPainel");
        _painelScore = GameObject.Find("PainelScore");
    }

    private void Start()
    {
        _manageChat.gameObject.SetActive(false); // esconde o chat.
        _chat = false;
        _painelSpawn.SetActive(false);
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

    public void HidePainel(int index, bool isEnable)
    {
        _painelConfig.Painels[index].SetActive(isEnable);
    }

    public Transform GetPainel(int index)
    {
        return _painelConfig.Painels[index].transform;
    }

    private void Update()
    {
        _tempoMsg += Time.deltaTime;
        
        Comandos(); // atualiza o médoto .

        if (_locked)
        {
            Lockmouse();
        }

        if (_damaged)
        {
            _miraImage.color = _flashColor;
        }
        else
        {
            _miraImage.color = Color.Lerp(_miraImage.color, Color.clear, _flashSpeed * Time.deltaTime);
        }

        _damaged = false;
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
            if (!_sair)
            {
                Cursor.lockState = CursorLockMode.None;
                _painelConfig.Painels[3].gameObject.SetActive(true);
                _sair = true;
            }
            else
            {
                _locked = true;
                _painelConfig.Painels[3].gameObject.SetActive(false);
                _sair = false;
            }
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            _painelScore.transform.position = _miraImage.transform.position;
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
        _personagemSeted = _selecaoDePlayers.txt_NamePerso.text;
    }

    public void Dead()
    {
        HidePainel(9, true);
    }
}
