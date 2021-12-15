using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour, IPunObservable
{
    public static PlayerMove instance;

    [SerializeField] private PhotonView _photonView;
    [SerializeField] private bool _useTransformView = true;
    [SerializeField] private Rigidbody _rb;
    [Space(10)]

    [SerializeField] private float _force = 65;
    [SerializeField] private float _timerJump = 0f;
    [SerializeField] private bool _canJump = true;
    [SerializeField] private float _speed = 6f;
    [SerializeField] private bool _equip = false;
    [SerializeField] private bool _unEquip = true;
    [SerializeField] private Shooting _shooting;
    [SerializeField] private PlayerHealth _playerHealth;
    [Space(10)]

    [SerializeField] private GameObject[] _posicoes;
    [SerializeField] private GameObject[] _referencias;

    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private Animator _anim;
    private int _floorMask;

    #region Properties

    public GameObject[] Referencias
    {
        get { return _referencias; }
    }
    public PlayerHealth PlayerHealth
    {
        get { return _playerHealth; }
    }

    public Shooting Shooting
    {
        get { return _shooting; }
    }

    public PhotonView PhotonView
    {
        get { return _photonView; }
    }

    #endregion

    private void Awake()
    {
        instance = this;
        _rb = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();
        _floorMask = LayerMask.GetMask("shootable");
        _anim = GetComponent<Animator>();
        _shooting.PlayerMove = this;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        UiManager.instance.PainelSpawn.SetActive(false);
    }

    private void SmothMove()
    {
        if (_useTransformView)
            return;
        transform.position = Vector3.Lerp(transform.position, _targetPosition, 0.25f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, 500 * Time.deltaTime);
    }

    void Update()
    {
        if (PhotonView.IsMine)
        {
            MoverPlayer();
            Jump();
        }
        else
        {
            SmothMove();
        }

    }

    void MoverPlayer()
    {
        _timerJump += Time.deltaTime;

        float _horizontal = Input.GetAxis("Horizontal") * _speed;
        float _vertical = Input.GetAxis("Vertical") * _speed;

        transform.Rotate(0, Input.GetAxis("Mouse X"), 0);
        
        _rb.transform.position += new Vector3(_horizontal, 0, _vertical);

        //_anim.SetFloat("X", Input.GetAxis("Horizontal"));
        //_anim.SetFloat("Y", Input.GetAxis("Vertical"));
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_equip)
            {
                _photonView.RPC("DesEquipaArma", RpcTarget.OthersBuffered);
                //_anim.SetTrigger("DesEquip");
                _posicoes[1].gameObject.SetActive(false);
                _posicoes[0].gameObject.SetActive(true);
                _equip = false;
                _unEquip = true;

            }
            else if (_unEquip)
            {
                _photonView.RPC("EquipaArma", RpcTarget.OthersBuffered);
                //_anim.SetTrigger("Equip");
                _posicoes[1].gameObject.SetActive(true);
                _posicoes[0].gameObject.SetActive(false);
                _unEquip = false;
                _equip = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && _canJump && _timerJump > 1f)
        {
            _rb.AddForce(Vector3.up * _force, ForceMode.Impulse);
            _timerJump = 0f;
            _canJump = false;
        }
    }

    void Jump()
    {
        if (Physics.Linecast(transform.position, transform.position - Vector3.up, _floorMask))
        {
            _canJump = true;
            Debug.DrawLine(transform.position, transform.position - Vector3.up, Color.green);
        }
        else if (!Physics.Linecast(transform.position, transform.position - Vector3.up, _floorMask))
        {
            _canJump = false;
            Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.up) * 0.1f, Color.green);
        }
    }

    public void Enviar(string name, string msg)
    {
        _photonView.RPC("EnviarMSG", RpcTarget.All, name, msg);
    }

    [PunRPC]
    public void Dead(string Name)
    {
        UiManager.instance.PlayerDead(Name);
    }

    [PunRPC]
    public void AtribuiPontos(string matador)
    {
        UiManager.instance.Kill(matador, 1);
    }

    [PunRPC]
    public void EnviarMSG(string name, string mensagem)
    {
        UiManager.instance.ManagerChat.ApplyMensagem(name, mensagem);
    }

    public void Esc(string nameplayer)
    {
        _photonView.RPC("Escrevendo", RpcTarget.Others, nameplayer);
    }

    [PunRPC]
    public void Dead()
    {
        _playerHealth.Morreu();
    }

    [PunRPC]
    public void EquipaArma()
    {
        //_anim.SetTrigger("Equip");
        _posicoes[1].gameObject.SetActive(true);
        _posicoes[0].gameObject.SetActive(false);
    }
    [PunRPC]
    public void DesEquipaArma()
    {
        //_anim.SetTrigger("DesEquip");
        _posicoes[1].gameObject.SetActive(false);
        _posicoes[0].gameObject.SetActive(true);
    }

    [PunRPC]
    public void killeds(string matador, string morto)
    {
        UiManager.instance.PainelConfigure.Kills(matador, morto);
    }

    [PunRPC]
    public void EfeitoTiro(int id)
    {
        _shooting.Shooth(id);
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (_useTransformView)
            return;

        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            _targetPosition = (Vector3)stream.ReceiveNext();
            _targetRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
