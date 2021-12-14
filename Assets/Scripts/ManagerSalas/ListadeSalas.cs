using UnityEngine;
using UnityEngine.UI;

public class ListadeSalas : MonoBehaviour
{
    [SerializeField] private Text _roomNameText;

    public string RoomName { get; private set;}
	public bool Updated{ get; set;}

	#region Properties
	public Text RoomNameText
	{
		get { return _roomNameText; }
	}

	#endregion

	private void Start ()
	{
		GetComponent<Button>().onClick.AddListener(() => UiManager.instance.OnClickJoinRoom(RoomNameText.text));
	}

	private void OnDestroy()
	{
		GetComponent<Button>().onClick.RemoveAllListeners ();
	}

	public void SetRoonNameText(string text)
	{
		RoomName = text;
		RoomNameText.text = text;
	}
}
