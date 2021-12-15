using UnityEngine;

public class CameraVisao : MonoBehaviour
{
    [SerializeField] private GameObject[] _head;
    [SerializeField] private GameObject[] _pos;
    [SerializeField] private Camera[] _cam;
    [SerializeField] private int _iD = 0;

    private bool Cmenabled = false;
    private float _v = -2.0f;
    private float _h = 2.0f;
    private RaycastHit _hit;

    #region Properties

    public GameObject[] Head
    {
        get { return _head; }
    }

    public GameObject[] Pos
    {
        get { return _pos; }
    }

    public Camera[] Cam
    {
        get { return _cam; }
    }

    #endregion

    private void LateUpdate()
    {
        Follow();
        Rotation();

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (_iD < 1)
            {
                _iD++;
            }
            else if (_iD > 0)
            {
                _iD = 0;
            }
        }
    }

    void Follow()
    {
      
       transform.LookAt(_head[_iD].transform);

       if (!Physics.Linecast(_head[_iD].transform.position, _pos[_iD].transform.position))
       {
            transform.position = _pos[_iD].transform.position;
            transform.SetParent(_pos[_iD].transform);
              
       }
       else if (Physics.Linecast(_head[_iD].transform.position, _pos[_iD].transform.position, out _hit))
       {
            transform.position = _hit.point;
       }


        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!Cmenabled)
            {
                _cam[0].enabled = false;
                _pos[2].gameObject.SetActive(false);
                _cam[1].enabled = true;
                Cmenabled = true;
            }
            else
            {
                _pos[2].gameObject.SetActive(true);
                _cam[1].enabled = false;
                _cam[0].enabled = true;
                Cmenabled = false;
            }
            
        }
    }

    private void Rotation()
    {
        float Hor = _h * Input.GetAxis("Mouse X");
        float Ver = _v * Input.GetAxis("Mouse Y");

        _head[_iD].transform.Rotate(Ver, 0, 0);
    }
}
