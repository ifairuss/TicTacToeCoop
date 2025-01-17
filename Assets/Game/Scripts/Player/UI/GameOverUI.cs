using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("GameOver Preference")]
    [SerializeField] private Color _loseColor;
    [SerializeField] private Color _winColor;
    [SerializeField] private TextMeshProUGUI _resultTextMesh;
    [SerializeField] private TextMeshProUGUI _addPointTextMesh;
    [SerializeField] private Button _rematchButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _x2PointButton;

    private int _addPoints;

    private void Awake()
    {
        _rematchButton.onClick.AddListener(() => { 
            GameManager.Instance.RematchRpc();
        });
        _menuButton.onClick.AddListener(() => {
            print("Menu");
        });
        _x2PointButton.onClick.AddListener(() => {
            _addPoints += _addPoints;
        });
    }

    private void Start()
    {
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnRematch += GameManager_OnRematch;
        Hide();
    }

    private void GameManager_OnRematch(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnGameWin(object sender, GameManager.OnGameWinEventArgs e)
    {
        if (e.winPlayerType == GameManager.Instance.GetLocalPlayerType())
        {
            _resultTextMesh.text = "You Win";
            _resultTextMesh.color = _winColor;
            _addPoints = 23;
            int addPoint = _addPoints;
;
            _addPointTextMesh.text = $"+{addPoint}";
        }
        else
        {
            _resultTextMesh.text = "You Lose";
            _resultTextMesh.color = _loseColor;
            _addPoints = 13;
            int addPoint = _addPoints;
;
            _addPointTextMesh.text = $"-{addPoint}";
        }
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
