using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CellButton : MonoBehaviour
{
    public int cellIndex;

    Button button;
    TextMeshProUGUI label;
    GameController controller;
    public float popScale = 1.2f;
    public float popSpeed = 12f;

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
    IEnumerator PopAnimation()
    {
        RectTransform rt = label.rectTransform;

        Vector3 startScale = Vector3.zero;
        Vector3 maxScale = Vector3.one * popScale;
        Vector3 endScale = Vector3.one;

        rt.localScale = startScale;

        float t = 0f;

        // Scale up
        while (t < 1f)
        {
            t += Time.deltaTime * popSpeed;
            rt.localScale = Vector3.Lerp(startScale, maxScale, t);
            yield return null;
        }

        t = 0f;

        // Scale down
        while (t < 1f)
        {
            t += Time.deltaTime * popSpeed;
            rt.localScale = Vector3.Lerp(maxScale, endScale, t);
            yield return null;
        }

        rt.localScale = Vector3.one;
    }

    public void SetValue(string value)
    {
        label.text = value;
        button.interactable = false;

        StopAllCoroutines();
        StartCoroutine(PopAnimation());
    }

    public void ResetCell()
    {
        label.text = "";
        label.rectTransform.localScale = Vector3.one;
        button.interactable = true;
    }

}
