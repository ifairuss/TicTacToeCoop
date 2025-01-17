using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [Header("Network Manager UI Preference")]
    [SerializeField] private Button _hostBtn;
    [SerializeField] private Button _clientBtn;

    private void Awake()
    {
        _hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            Hide();
        });
        _clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            Hide();
        });
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
