using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Shooting _shooting;

    public GameObject PrefabMorto;
    public static PlayerHealth instance;
    public int StartingHealth = 100;
    public float sinkSpeed = 2.5f;
    public int CurrentHealth;
    public Slider HealthSlider;
    public Image DanoImage;
    public AudioClip DeadClip;
    public float FlashSpeed = 5f;
    public GameObject IsMinekk;
    public GameObject[] Huds;
    public Color FlashColor = new Color(1f, 0f, 0f, 0.1f);
    public Color Cornormal = new Color(0f, 0f, 0f, 0f);
    public Text municao;

    private Animator _anim;
    private AudioSource _playAudio;
    private PlayerMove _playerMove;
    private ParticleSystem _particulas;
    private bool _isDead, _damaged;
    private CapsuleCollider _cap;

    #region Properties
    public PlayerMove PlayerMove
    {
        get { return _playerMove; }
    }

    public Animator Anim
    {
        get { return _anim; }
    }

    public CapsuleCollider Cap
    {
        get { return _cap; }
    }
    public Shooting Shooting
    {
        get { return _shooting; }
    }

    #endregion


    private void Awake()
    {
        instance = this;
        _cap = GetComponent<CapsuleCollider>();
        _particulas = GetComponentInChildren<ParticleSystem>();
        _anim = GetComponent<Animator>();
        _playAudio = GetComponent<AudioSource>();
        _playerMove = GetComponent<PlayerMove>();
        CurrentHealth = StartingHealth;
    }

    private void Update()
    {
        if (_damaged)
        {
            DanoImage.color = FlashColor;
        }
        else
        {
            DanoImage.color = Color.Lerp(DanoImage.color, Color.clear, FlashSpeed * Time.deltaTime);
        }
        _damaged = false;
        municao.text = Shooting.Municao.ToString() + "/100";
        if (CurrentHealth > 100)
        {
            CurrentHealth = 100;
        }
        HealthSlider.value = CurrentHealth;
    }

    [PunRPC]
    public void TakeDamage(int amount, Vector3 hitPoint, int MeuID, string matador, int IDMatador)
    {
        PhotonView MeuPhotonView = GetComponent<PhotonView>();

        if (MeuID != MeuPhotonView.ViewID)
            return;

        if (_isDead)
            return;

        _damaged = true;
        CurrentHealth -= amount;
        _particulas.transform.position = hitPoint;
        _particulas.Play();

        _playAudio.Play();

        if (CurrentHealth <= 0 && !_isDead)
        {
            Death(matador, IDMatador);
        }
    }

    private void Death(string matador, int IDMatador)
    {
        Shooting.enabled = false;
        HealthSlider.value = 0;
        _playerMove.GetComponent<PhotonView>().RPC("Dead", RpcTarget.AllBuffered);
        _playerMove.GetComponent<PhotonView>().RPC("AtribuiPontos", RpcTarget.AllBuffered, matador);
        _playerMove.GetComponent<PhotonView>().RPC("Morri", RpcTarget.AllBuffered, SaveDados.Nickname);
        _playerMove.GetComponent<PhotonView>().RPC("killeds", RpcTarget.All, matador, SaveDados.Nickname);
        _isDead = true;
        _playAudio.clip = DeadClip;
        _playAudio.Play();

        Camera.main.GetComponentInChildren<CameraVisao>().transform.SetParent(UiManager.instance.Map);
        CamAuxio.instance.transform.SetParent(UiManager.instance.Map);
        
        CamAuxio.instance.enabled = false;
        Camera.main.GetComponentInChildren<CameraVisao>().enabled = false;
        UiManager.instance.Dead();
        _playerMove.enabled = false;

    }

    public void Morreu()
    {
        Instantiate(PrefabMorto, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
