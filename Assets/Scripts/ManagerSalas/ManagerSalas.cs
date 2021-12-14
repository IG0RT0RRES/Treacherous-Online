using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ManagerSalas : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject prefabdeSalas;
    private GameObject PrefabdeSalas
    {
        get { return prefabdeSalas; }
    }

    private List<ListadeSalas> listadeSalasButton = new List<ListadeSalas>();
    private List<ListadeSalas> ListadeSalasButton
    {
        get { return listadeSalasButton; }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        OnReceivedRoomListUpdate(roomList);
    }

    private void OnReceivedRoomListUpdate(List<RoomInfo> roomList)
    {
        RoomInfo[] rooms = roomList.ToArray();

        foreach (RoomInfo room in rooms)
        {
            RoomReceived(room);
        }

        RemoveOldRooms();
    }

    private void RoomReceived(RoomInfo room)
    {
        int index = ListadeSalasButton.FindIndex(x => x.RoomName == room.Name);

        if (index == -1)
        {
            if (room.IsVisible && room.PlayerCount < room.MaxPlayers)
            {
                GameObject prefabdeSalaObj = Instantiate(prefabdeSalas);
                prefabdeSalaObj.transform.SetParent(transform, false);

                ListadeSalas listadeSals = prefabdeSalaObj.GetComponent<ListadeSalas>();
                ListadeSalasButton.Add(listadeSals);

                index = (ListadeSalasButton.Count - 1);

            }
        }
        if (index != -1)
        {
            ListadeSalas listadeSalas = ListadeSalasButton[index];
            listadeSalas.SetRoonNameText(room.Name);
            listadeSalas.Updated = true;
        }
    }

    private void RemoveOldRooms()
    {
        List<ListadeSalas> removeListadeSalas = new List<ListadeSalas>();
        foreach (ListadeSalas listadesalas in listadeSalasButton)
        {
            if (!listadesalas.Updated)
                removeListadeSalas.Add(listadesalas);
            else
                listadesalas.Updated = false;
        }
        foreach (ListadeSalas listadeSalas in removeListadeSalas)
        {
            GameObject listadeSalasobj = listadeSalas.gameObject;
            ListadeSalasButton.Remove(listadeSalas);
            Destroy(listadeSalasobj);
        }
    }
}
