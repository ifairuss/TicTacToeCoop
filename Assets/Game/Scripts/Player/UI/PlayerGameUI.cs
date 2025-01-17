using System;
using UnityEngine;

public class PlayerGameUI : MonoBehaviour {

    [Header("Game preferences")]
    [SerializeField] private GameObject _arrowGameObject;

    [Header("Player information preferences")]
    [SerializeField] private GameObject _player_1NameGameObject;
    [SerializeField] private GameObject _player_2NameGameObject;
    [SerializeField] private GameObject _player_1AvatarGameObject;
    [SerializeField] private GameObject _player_2AvatarGameObject;
    [SerializeField] private GameObject _player_1ScoreGameObject;
    [SerializeField] private GameObject _player_2ScoreGameObject;

    private void Start()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.OnCurrentPlayebleTypeChanged += GameManager_OnCurrentPlayebleTypeChanged;
    }

    private void GameManager_OnCurrentPlayebleTypeChanged(object sender, EventArgs e)
    {
        UpdateCurrentArrow();
    }

    private void GameManager_OnGameStarted(object sender, EventArgs e)
    {
        if (GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Cross)
        {
            _arrowGameObject.transform.rotation = Quaternion.Euler(0, 0, -90f);
        } else
        {
            _arrowGameObject.transform.rotation = Quaternion.Euler(0, 0, 90f);
        }

        UpdateCurrentArrow();
    }

    private void UpdateCurrentArrow()
    {
        if (GameManager.Instance.GetCurrentPlayeblePlayerType() == GameManager.PlayerType.Cross)
        {
            _arrowGameObject.transform.rotation = Quaternion.Euler(0, 0, -90f);
        }
        else
        {
            _arrowGameObject.transform.rotation = Quaternion.Euler(0, 0, 90f);
        }
    }
}
