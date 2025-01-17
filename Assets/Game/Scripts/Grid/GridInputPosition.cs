using UnityEngine;

public class GridInputPosition : MonoBehaviour
{

    [Header("Field Setting")]
    [SerializeField] private int x;
    [SerializeField] private int y;

    private void OnMouseDown()
    {
        GameManager.Instance.ClickedOnGridPosition(x, y);
    }
}