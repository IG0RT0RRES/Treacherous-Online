using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private Transform[] _spawnGrupo;
    [SerializeField] private int _group;

    #region Properties

    public int Group
    {
        get { return _group; }
        set { _group = value; }
    }

    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void SpawnPlayer()
    {
        //-----------------------------------------------------------

        GameObject player = (PhotonNetwork.Instantiate("Player_" + UiManager.instance._personagemSeted, _spawnGrupo[_group].transform.position, Quaternion.identity, 0));
        PlayerHealth pp = player.GetComponent<PlayerHealth>();
        pp.HealthSlider = UiManager.instance.Life;
        pp.DanoImage = UiManager.instance.DamageSprite;
        pp.municao = UiManager.instance.Ammunition;
        UiManager.instance.CameraUi.enabled = false;

        //-----------------------------------------------------------

        PlayerMove pm = player.GetComponent<PlayerMove>();
        CameraVisao cameraVisao = Camera.main.GetComponentInChildren<CameraVisao>();
        cameraVisao.Head[0] = pm.Referencias[1].gameObject;
        cameraVisao.Head[1] = pm.Referencias[2].gameObject;
        cameraVisao.Pos[0] = pm.Referencias[3].gameObject;
        cameraVisao.Pos[1] = pm.Referencias[4].gameObject;

        CamAuxio.instance.visaoT = pm.Referencias[0].gameObject;
        CamAuxio.instance.CabecaM = pm.Referencias[5].gameObject;
        CamAuxio.instance.enabled = true;

        Camera.main.GetComponent<Camera>().enabled = true;
        cameraVisao.enabled = true;
        
        UiManager.instance.PainelCharacter.SetActive(false);
        UiManager.instance.PainelExit.SetActive(false);
    }

    public void RespawnPlayer()
    {
        //--------------------------------------------------------
        
        GameObject player = (PhotonNetwork.Instantiate(UiManager.instance._personagemSeted, _spawnGrupo[_group].transform.position, Quaternion.identity, 0));
        PlayerHealth pp = player.GetComponent<PlayerHealth>();
        pp.HealthSlider = UiManager.instance.Life;
        pp.DanoImage = UiManager.instance.DamageSprite;
        pp.municao = UiManager.instance.Ammunition;
        UiManager.instance.CameraUi.enabled = false;

        //--------------------------------------------------------

        PlayerMove pm = player.GetComponent<PlayerMove>();
        CameraVisao cameraVisao = Camera.main.GetComponentInChildren<CameraVisao>();

        cameraVisao.Head[0] = pm.Referencias[1].gameObject;
        cameraVisao.Head[1] = pm.Referencias[2].gameObject;
        cameraVisao.Pos[0] = pm.Referencias[3].gameObject;
        cameraVisao.Pos[1] = pm.Referencias[4].gameObject;
        
        CamAuxio.instance.visaoT = pm.Referencias[0].gameObject;
        CamAuxio.instance.CabecaM = pm.Referencias[5].gameObject;
        CamAuxio.instance.enabled = true;
        cameraVisao.enabled = true;

        UiManager.instance.PainelSpawn.GetComponentInChildren<Button>().interactable = false;
        UiManager.instance.PainelSpawn.SetActive(false);
        UiManager.instance.PainelExit.SetActive(false);
    }
}
