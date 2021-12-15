using UnityEngine;
using Photon.Pun;

public class Shooting : MonoBehaviourPunCallbacks
{
    public static Shooting instance;

    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private PlayerHealth _playerHealth;
    [Space(10)]

    public int Municao = 100;
    public int DamagePerShot = 20;
    public float TimeSetShoot = 0.15f;
    public float Range = 600f;
    public bool Gatilho = false;
    public float Tempo = 0;

    private float _timer;
    private RaycastHit _shootHit;
    private int _shootableMask, _iDMatador;
    private ParticleSystem _weaponParticle;
    private LineRenderer _weaponLine;
    private AudioSource _weaponAudio;
    private Light _weaponLigth;
    private float _effectsDysplayTime = 0.2f;
    private string _myname;

    #region Properties
    public PlayerMove PlayerMove
    {
        get { return _playerMove; }
        set { _playerMove = value; }
    }
    public PlayerHealth PlayerHealth
    {
        get { return _playerHealth; }
    }
    #endregion

    private void Awake()
    {
        instance = this;
        _shootableMask = LayerMask.GetMask("shootable");
        _weaponParticle = GetComponent<ParticleSystem>();
        _weaponLigth = GetComponent<Light>();
        _weaponAudio = GetComponent<AudioSource>();
        _weaponLine = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (Input.GetButton("Fire1") && _timer >= TimeSetShoot && Municao > 0)
        {
            Shoot();
        }

        if (_timer >= TimeSetShoot * _effectsDysplayTime)
        {
            DisableEffects();
        }

        if (Municao > 100)
        {
            Municao = 100;
        }
    }

    public void DisableEffects()
    {
        _weaponLine.enabled = false;
        _weaponLigth.enabled = false;
    }

    public void Shoot()
    {
        if (!_playerMove.PhotonView.IsMine)
            return;

        _playerMove.GetComponent<PhotonView>().RPC("EfeitoTiro", RpcTarget.Others, _playerMove.GetComponent<PhotonView>().ViewID);

        _timer = 0f;
        Municao -= 1;
        _weaponAudio.Play();
        _weaponLigth.enabled = true;
        _weaponParticle.Stop();
        _weaponParticle.Play();

        _weaponLine.enabled = true;
        _weaponLine.SetPosition(0, transform.position);

        _myname = PhotonNetwork.NickName;
        _iDMatador = _playerMove.PhotonView.ViewID;

        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(camRay, out _shootHit, Range, _shootableMask))
        {
            Debug.DrawLine(transform.position, _shootHit.point);

            PlayerHealth PH = _shootHit.collider.GetComponent<PlayerHealth>();
            PhotonView PHOTO = _shootHit.collider.GetComponent<PhotonView>();

            if (PHOTO != null)
            {
                UiManager.instance.Damage = true;
                PH.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, DamagePerShot, _shootHit.point, PHOTO.ViewID, _myname, _iDMatador);
            }
            else
            {
                //Debug.Log("Nao acertou algo que tem o componente PhotonView. 'metodo Shoot Physics => PhotonView PHOTO = PH.GetComponent<PhotonView>(); '");
            }
            _weaponLine.SetPosition(1, _shootHit.point);
        }
        else
        {
            _weaponLine.SetPosition(1, transform.position + camRay.direction * Range);
        }
    }

    public void Shooth(int ID)
    {
        if (ID != _playerMove.PhotonView.ViewID)
            return;

        _weaponAudio.Play();
        _weaponLigth.enabled = true;
        _weaponParticle.Stop();
        _weaponParticle.Play();
        DisableEffects();
    }
}
