using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour {

    public Text roomNameText;

    public string roomName;

    public bool updated;

    void Start() {
        GameObject lobbyCanvasObj = MenuCanvasManager.Instance.lobbyCanvas.gameObject;
        if (lobbyCanvasObj == null) {
            return;
        }
        LobbyCanvas lobbyCanvas = lobbyCanvasObj.GetComponent<LobbyCanvas>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(()=>lobbyCanvas.OnClickJoinRoom(roomNameText.text));
    }

    private void OnDestroy() {
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
    }

    public void SetRoomNameText(string text) {
        roomName = text;
        roomNameText.text = roomName;
    }
}
