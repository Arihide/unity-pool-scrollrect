using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PoolScrollItem : MonoBehaviour
{
    public RectTransform rectTransform { get => (_rectTransform ??= GetComponent<RectTransform>()); }

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private RectTransform _rectTransform = null;


    public void OnUpdate(int itemIndex)
    {
        text.SetText("Item:{0}", itemIndex);
    }
}