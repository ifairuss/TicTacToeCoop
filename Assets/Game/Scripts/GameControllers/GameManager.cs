using System;
using Unity.Netcode;

public class GameManager : NetworkBehaviour {

    public static GameManager Instance { get; private set; }

    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;
    public class OnClickedOnGridPositionEventArgs : EventArgs {
        public int x;
        public int y;
        public PlayerType PlayerTypes;
    }

    public enum PlayerType {
        None,
        Cross,
        Circle
    }

    private PlayerType _localPlayerType;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.LocalClientId == 0)
            _localPlayerType = PlayerType.Cross;
        else
            _localPlayerType = PlayerType.Circle;
    }

    public void ClickedOnGridPosition(int xCord, int yCord) {
        print($"{xCord},{yCord}");
        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs {
            x = xCord,
            y = yCord,
            PlayerTypes = GetLocalPlayerType()
        });
    }

    public PlayerType GetLocalPlayerType() {
        return _localPlayerType;
    }

}
