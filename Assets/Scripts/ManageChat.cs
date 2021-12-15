using UnityEngine;
using UnityEngine.UI;

public class ManageChat : MonoBehaviour
{
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private RectTransform _layoutMsg;
    [SerializeField] private GameObject _mensageChatPrefab;
    [SerializeField] private InputField _mensagemTxt;

    #region Properties
    public GameObject MensageChatPrefab
    {
        get { return _mensageChatPrefab; }
    }

    public string Mensagem
    {
        get { return _mensagemTxt.text; }
        set { if(!string.IsNullOrEmpty(value)) _mensagemTxt.text = value; }
    }

    #endregion

    private void Update()
    {
        UpdateChat(); // atualiza o método .
    }
    // controla a quantidade de mensagens existentes para nao acumular alem de 15 mensagens.
    private void UpdateChat()
    {
        int Nmsg = _layoutMsg.transform.childCount; // verifica a quantidade existente de mensagem.

        if(Nmsg > 15)
        {
            Destroy(_layoutMsg.transform.GetChild(0).gameObject); // destroi o primeiro gameobject da mensagem enviada.
        }
    }

    // seta a mensagem escrita no prefab e o instancia no chat.
    public void ApplyMensagem(string Name, string msg)
    {
        _mensageChatPrefab.GetComponent<MsgPrefab>().EnviarMensagem(Name, msg); // set a mensagem.
        GameObject _prefabMsg = Instantiate(_mensageChatPrefab); // instancia a mensagem .
        _prefabMsg.transform.SetParent(_layoutMsg, false); // seta como parente do containe de mensagens.
        //UiManager.instance.HidePainel(4,true);
        _scrollbar.value = 0; // desce o scrol da mensagem pra visualizar as novas mensagens recebidas.
    }

    public void Focus()
    {
        _mensagemTxt.Select();
    }
}
