using UnityEngine;
using Photon.Pun;

public class PlayerC : MonoBehaviourPunCallbacks
{
    public static PlayerC instance;

    [SerializeField] private PhotonView _photonView;
    [SerializeField] private GameObject[] Armas;
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private Shooting _shooting;

    public bool UseTransformView = true;
    public bool _equiped = false, _lowered = false;

    #region Properties

    public PlayerHealth PlayerHealth
    {
        get { return _playerHealth; }
    }

    
    public Shooting Shooting
    {
        get { return _shooting; }
    }

    #endregion

    private float _vel = 6f;
    private float _velrot = 100;
    private float _moveY;
    private float _moveX;
    private float _h = 2.0f;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private Rigidbody _rb;
    private Animator _anim;
   
    private void Awake()
    {
        instance = this;
        _photonView = GetComponent<PhotonView>();
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (_photonView.IsMine)
        {
            MoverPlayer();
        }
        else
        {
            SmothMove();
        }
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (UseTransformView)
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

    private void SmothMove()
    {
        if (UseTransformView)
            return;
        transform.position = Vector3.Lerp(transform.position, _targetPosition, 0.25f);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, 500 * Time.deltaTime);
    }

    void MoverPlayer()
    {
        float rotaçao = Input.GetAxis("Horizontal") * _velrot;
        float Mover = Input.GetAxis("Vertical") * _vel;
        float Hor = _h * Input.GetAxis("Mouse X");

        rotaçao *= Time.deltaTime;
        Mover *= Time.deltaTime;

        transform.Rotate(0, Hor, 0);

        _moveY = Input.GetAxis("Vertical");
        _moveX = Input.GetAxis("Horizontal");

        _anim.SetFloat("X", _moveX, 0.1f, Time.deltaTime);
        _anim.SetFloat("Y", _moveY, 0.1f, Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_equiped)
            {
                _photonView.RPC("DesEquipaArma", RpcTarget.AllBuffered);
                _equiped = false;
            }
            else if (!_equiped)
            {
                _photonView.RPC("EquipaArma", RpcTarget.AllBuffered);
                _equiped = true;
            }
        }

        //if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        //{
        //    _anim.SetBool("Running", true);
        //}
        //else
        //{
        //    _anim.SetBool("Running", false);
        //}

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (_lowered && !_equiped)
            {
                _anim.SetTrigger("abaixado");
                _photonView.RPC("abaixa", RpcTarget.AllBuffered);
                _lowered = false;
            }
            else if(!_lowered && _equiped)
            {
                _anim.SetTrigger("Equip");
                _photonView.RPC("Levanta", RpcTarget.AllBuffered);
                _lowered = true;
            }
        }
    }

    public void esc(string nameplayer)
    {
        photonView.RPC("Escrevendo", RpcTarget.Others,nameplayer);
    }
    public void Enviar(string name, string msg)
    {
        photonView.RPC("EnviarMSG", RpcTarget.All, name, msg);
    }

    [PunRPC]
    public void EnviarMSG(string name, string mensagem)
    {
        UiManager.instance.ManagerChat.ApplyMensagem(name, mensagem);
    }

    [PunRPC]
    public void Dead()
    {
        _playerHealth.Morreu();
    }

    [PunRPC]
    public void EquipaArma()
    {
        _anim.SetTrigger("Equip");
        Armas[1].gameObject.SetActive(true);
        Armas[0].gameObject.SetActive(false);
    }
    [PunRPC]
    public void DesEquipaArma()
    {
        _anim.SetTrigger("DesEquip");
        Armas[1].gameObject.SetActive(false);
        Armas[0].gameObject.SetActive(true);
    }

    [PunRPC]
    public void abaixa()
    {
        _anim.SetTrigger("abaixado");
    }

    [PunRPC]
    public void Levanta()
    {
        _anim.SetTrigger("Equip");
    }

    [PunRPC]
    public void killeds(string matador, string morto, int Arma)
    {
        UiManager.instance.Kills(matador, morto, Arma);
    }

    [PunRPC]
    public void EfeitoTiro(int id)
    {
        _shooting.Shooth(id);
    }
}
