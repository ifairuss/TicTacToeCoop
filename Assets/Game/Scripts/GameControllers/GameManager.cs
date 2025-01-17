using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static GameManager;

public class GameManager : NetworkBehaviour {

    public static GameManager Instance { get; private set; }

    public event EventHandler<OnClickedOnGridPositionEventArgs> OnClickedOnGridPosition;
    public class OnClickedOnGridPositionEventArgs : EventArgs {
        public int x;
        public int y;
        public PlayerType PlayerTypes;
    }

    public class OnGameWinEventArgs : EventArgs
    {
        public Line line;
        public PlayerType winPlayerType;
    }

    public event EventHandler OnGameStarted;
    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public event EventHandler OnCurrentPlayebleTypeChanged;
    public event EventHandler OnRematch;

    public enum PlayerType {
        None,
        Cross,
        Circle
    }

    public enum Orientation
    {
        Horizontal,
        Vertical,
        DiagonalA,
        DiagonalB
    }

    public struct Line
    {
        public List<Vector2Int> GridVector2IntList;
        public Vector2Int CenterGridPosition;
        public Orientation Orientation;
    }

    private PlayerType _localPlayerType;
    private NetworkVariable<PlayerType> _currentPlayablePlayerType = new NetworkVariable<PlayerType>(PlayerType.None,
                                                                                                     NetworkVariableReadPermission.Everyone,
                                                                                                     NetworkVariableWritePermission.Server);
    private PlayerType[,] playerTypeArray;
    private List<Line> _lineList;

    private void Awake()
    {
        Instance = this;

        playerTypeArray = new PlayerType[3,3];

        _lineList = new List<Line>
        {
            //Horizontal
            new Line
            {
                GridVector2IntList = new List<Vector2Int>{ new Vector2Int(0,0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                CenterGridPosition = new Vector2Int(1,0),
                Orientation = Orientation.Horizontal,
            },
            new Line
            {
                GridVector2IntList = new List<Vector2Int>{ new Vector2Int(0,1), new Vector2Int(1, 1), new Vector2Int(2, 1) },
                CenterGridPosition = new Vector2Int(1,1),
                Orientation = Orientation.Horizontal,

            },
            new Line
            {
                GridVector2IntList = new List<Vector2Int>{ new Vector2Int(0,2), new Vector2Int(1, 2), new Vector2Int(2, 2) },
                CenterGridPosition = new Vector2Int(1,2),
                Orientation = Orientation.Horizontal,

            },
            //Vertical
            new Line
            {
                GridVector2IntList = new List<Vector2Int>{ new Vector2Int(0,0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
                CenterGridPosition = new Vector2Int(0,1),
                Orientation = Orientation.Vertical,

            },
            new Line
            {
                GridVector2IntList = new List<Vector2Int>{ new Vector2Int(1,0), new Vector2Int(1, 1), new Vector2Int(1, 2) },
                CenterGridPosition = new Vector2Int(1,1),
                Orientation = Orientation.Vertical,
            },
            new Line
            {
                GridVector2IntList = new List<Vector2Int>{ new Vector2Int(2,0), new Vector2Int(2, 1), new Vector2Int(2, 2) },
                CenterGridPosition = new Vector2Int(2,1),
                Orientation = Orientation.Vertical,
            },
            //Diagonals
            new Line
            {
                GridVector2IntList = new List<Vector2Int>{ new Vector2Int(0,0), new Vector2Int(1, 1), new Vector2Int(2, 2) },
                CenterGridPosition = new Vector2Int(1,1),
                Orientation = Orientation.DiagonalA,
            },
            new Line
            {
                GridVector2IntList = new List<Vector2Int>{ new Vector2Int(0,2), new Vector2Int(1, 1), new Vector2Int(2, 0) },
                CenterGridPosition = new Vector2Int(1,1),
                Orientation = Orientation.DiagonalB,
            }
        };
    }

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.LocalClientId == 0)
            _localPlayerType = PlayerType.Cross;
        else
            _localPlayerType = PlayerType.Circle;

        if (IsServer) { 
            _currentPlayablePlayerType.Value = PlayerType.Cross;

            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        }

        _currentPlayablePlayerType.OnValueChanged += (PlayerType oldPlayerType, PlayerType newPlayerType) => {
            OnCurrentPlayebleTypeChanged?.Invoke(this, EventArgs.Empty);
        };
    }

    private void NetworkManager_OnClientConnectedCallback(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2) {
            _currentPlayablePlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int xCord, int yCord, PlayerType playerType) {

        if (playerType != _currentPlayablePlayerType.Value) {
            return;
        }

        if (playerTypeArray[xCord, yCord] != PlayerType.None) { return; }

        playerTypeArray[xCord, yCord] = playerType;

        OnClickedOnGridPosition?.Invoke(this, new OnClickedOnGridPositionEventArgs {
            x = xCord,
            y = yCord,
            PlayerTypes = playerType
        });

        switch (_currentPlayablePlayerType.Value) {
            default:
            case PlayerType.Cross  :
                _currentPlayablePlayerType.Value = PlayerType.Circle;
                break;
            case PlayerType.Circle:
                _currentPlayablePlayerType.Value = PlayerType.Cross;
                break;
        }

        TemporaryAlgorithmWinner();
    }

    private bool TemporaryAlgorithmLine(Line line)
    {
        return TemporaryAlgorithmLine(
                playerTypeArray[line.GridVector2IntList[0].x, line.GridVector2IntList[0].y],
                playerTypeArray[line.GridVector2IntList[1].x, line.GridVector2IntList[1].y],
                playerTypeArray[line.GridVector2IntList[2].x, line.GridVector2IntList[2].y]
            );
    }

    private bool TemporaryAlgorithmLine(PlayerType playerTypeOne, PlayerType playerTypeTwo, PlayerType playerTypeThree)
    {
        return
            playerTypeOne != PlayerType.None &&
            playerTypeOne == playerTypeTwo &&
            playerTypeTwo == playerTypeThree;
    }

    private void TemporaryAlgorithmWinner() {

        for (int i = 0; i < _lineList.Count; i++)
        {
            Line line = _lineList[i];

            if (TemporaryAlgorithmLine(line))
            {
                _currentPlayablePlayerType.Value = PlayerType.None;
                TriggerOnGameWinRpc(i, playerTypeArray[line.CenterGridPosition.x, line.CenterGridPosition.y]);
                break;
            }
        }


    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWinRpc(int lineIndex, PlayerType winPlayerType)
    {
        Line line = _lineList[lineIndex];

        OnGameWin?.Invoke(this, new OnGameWinEventArgs
        {
            line = line,
            winPlayerType = playerTypeArray[line.CenterGridPosition.x, line.CenterGridPosition.y]
        });
    }

    [Rpc(SendTo.Server)]
    public void RematchRpc()
    {
        for (int x = 0; x < playerTypeArray.GetLength(0); x++)
        {
            for (int y = 0; y < playerTypeArray.GetLength(1); y++)
            {
                playerTypeArray[x, y] = PlayerType.None;
            }
        }

        _currentPlayablePlayerType.Value = PlayerType.Cross;
        TriggerOnRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnRematchRpc()
    {
        OnRematch?.Invoke(this, EventArgs.Empty);
    }

    public PlayerType GetLocalPlayerType() {
        return _localPlayerType;
    }

    public PlayerType GetCurrentPlayeblePlayerType()
    {
        return _currentPlayablePlayerType.Value;
    }

}
