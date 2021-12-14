using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LayoutPlayers : MonoBehaviour
{
    [SerializeField] private GameObject _playerListingPrefab;
    private List<ListPlayers> _listPlayers = new List<ListPlayers>();

    #region Properties
    public GameObject PlayerListingPrefab
    {
        get { return _playerListingPrefab; }
    }
    public List<ListPlayers> ListPlayers
    {
        get { return _listPlayers; }
    }

    #endregion

    private void OnJoinedRoom()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Player[] photonPlayer = PhotonNetwork.PlayerList;
        for (int i = 0; i < photonPlayer.Length; i++)
        {
            PlayerJoinedRoom(photonPlayer[i]);
        }
    }

    private void OnPhotonPlayerConnected(Player photonPlayer)
    {
        PlayerJoinedRoom(photonPlayer);
    }

    private void OnPhotonPlayerDisconnected(Player photonPlayer)
    {
        PlayerLeftRoom(photonPlayer);
    }

    private void PlayerJoinedRoom(Player photonPlayer)
    {
        if (photonPlayer == null)
            return;
        PlayerLeftRoom(photonPlayer);

        GameObject listPlayersObj = Instantiate(PlayerListingPrefab);
        listPlayersObj.transform.SetParent(transform, false);

        ListPlayers listPlayers = listPlayersObj.GetComponent<ListPlayers>();
        listPlayers.ApplyPhotonPlayer(photonPlayer);

        ListPlayers.Add(listPlayers);
    }

    private void PlayerLeftRoom(Player photonPlayer)
    {
        int index = ListPlayers.FindIndex(x => x.PhotonPayer == photonPlayer);
        if (index != -1)
        {
            Destroy(ListPlayers[index].gameObject);
            ListPlayers.RemoveAt(index);
        }
    }

    public void SetPoints(string Name,int Ponto)
    {
        Transform OndeVaiPontos = GameObject.Find(Name).transform;
        OndeVaiPontos.GetComponent<ListPlayers>().SetaPontosNoPrefab(Ponto);
    }

    public void SetDeadView(string Name)
    {
        Transform Morte = GameObject.Find(Name).transform;
        Morte.GetComponent<ListPlayers>().SetaMorte();
    }
}
