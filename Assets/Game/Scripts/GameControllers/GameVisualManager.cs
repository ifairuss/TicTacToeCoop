using Unity.Netcode;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour {

    [Header("Visual Settings")]
    [SerializeField] private Transform _crossPrefab;
    [SerializeField] private Transform _circlePrefab;

    private const float GRID_SIZE = 2f;

    private void Start() {

        GameManager.Instance.OnClickedOnGridPosition += GameManagerOnClickedOnGridPosition;
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
    }

    private Vector2 GetGridWorldPosition(int x, int y) { 
        return new Vector2(-GRID_SIZE + x * GRID_SIZE, (-GRID_SIZE + y * GRID_SIZE) - 1.25f);
    }

}
