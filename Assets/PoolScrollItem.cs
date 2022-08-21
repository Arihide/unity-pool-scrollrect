using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PoolScrollItem : MonoBehaviour
{
    private RectTransform _rectTransform = null;
    public RectTransform rectTransform { get => (_rectTransform ??= GetComponent<RectTransform>()); }

    [SerializeField] private TextMeshProUGUI text;

    public void OnUpdate(int itemIndex)
    {
        text.SetText("Item:{0}", itemIndex);
    }
}