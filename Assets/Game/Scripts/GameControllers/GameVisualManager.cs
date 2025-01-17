using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour {

    [Header("Visual Settings")]
    [SerializeField] private Transform _crossPrefab;
    [SerializeField] private Transform _circlePrefab;
    [SerializeField] private Transform _lineCompleatePrefab;

    private const float GRID_SIZE = 2f;

    private List<GameObject> _visualGameObjectList;

    private void Awake()
    {
        _visualGameObjectList = new List<GameObject>();
    }

    private void Start() {

        GameManager.Instance.OnClickedOnGridPosition += GameManagerOnClickedOnGridPosition;
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnRematch += Instance_OnRematch;
    }

    private void Instance_OnRematch(object sender, EventArgs e)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        foreach (GameObject visualGameObdject in _visualGameObjectList) {
            Destroy(visualGameObdject);
        }

        _visualGameObjectList.Clear();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        float eulerZ = 0f;

        switch (e.line.Orientation)
        {
            default:
            case GameManager.Orientation.Horizontal:
                eulerZ = 0f;
                break;
            case GameManager.Orientation.Vertical:
                eulerZ = 90f;
                break;
            case GameManager.Orientation.DiagonalA:
                eulerZ = 45f;
                break;
            case GameManager.Orientation.DiagonalB:
                eulerZ = -45f;
                break;
        }

        Transform lineCompleated = Instantiate(_lineCompleatePrefab,
            GetGridWorldPosition(e.line.CenterGridPosition.x, e.line.CenterGridPosition.y),
            Quaternion.Euler(0, 0, eulerZ));
        lineCompleated.GetComponent<NetworkObject>().Spawn(true);

        _visualGameObjectList.Add(lineCompleated.gameObject);

    }

    private void GameManagerOnClickedOnGridPosition(object sender, GameManager.OnClickedOnGridPositionEventArgs eventArgs) {
        SpawnObjectRpc(eventArgs.x, eventArgs.y, eventArgs.PlayerTypes);
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(int x, int y, GameManager.PlayerType playerType)
    {
        Transform prefab;

        switch (playerType) {
            default:
            case GameManager.PlayerType.Cross:
                prefab = _crossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                prefab = _circlePrefab;
                break;
        }
        Transform spawnedCrossTransform = Instantiate(prefab, GetGridWorldPosition(x, y), Quaternion.identity);
        spawnedCrossTransform.GetComponent<NetworkObject>().Spawn(true);

        _visualGameObjectList.Add(spawnedCrossTransform.gameObject);
    }

    private Vector2 GetGridWorldPosition(int x, int y) { 
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, (-GRID_SIZE + y * GRID_SIZE) - 1.25f);
    }

}
