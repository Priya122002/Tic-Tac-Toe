using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellButton : MonoBehaviour
{
    public int cellIndex;

    Button button;
    TextMeshProUGUI label;
    GameController controller;

    void Awake()
    {
        button = GetComponent<Button>();
        label = GetComponentInChildren<TextMeshProUGUI>();
        button.onClick.AddListener(OnClick);
    }

    public void SetController(GameController gameController)
    {
        controller = gameController;
    }

    void OnClick()
    {
        if (controller == null)
            return;

        controller.OnCellSelected(this);
    }

    public void SetValue(string value)
    {
        label.text = value;
        button.interactable = false;
    }

    public void ResetCell()
    {
        label.text = "";
        button.interactable = true;
    }
}
