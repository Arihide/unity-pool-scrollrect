using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;

[RequireComponent(typeof(ScrollRect))]
public class PoolScrollRect : MonoBehaviour
{
    [SerializeField, Tooltip("スクロール内の要素同士の間隔")] private float m_spacing = 0;
    [SerializeField, Tooltip("アイテムプレハブ")] private PoolScrollItem m_itemPrefab = null;

    public int m_itemCount { get; private set; } = 0; // データ上のアイテム数
    private float m_sizeY = 1; // スペース含めたアイテムの高さ
    private int m_maxItemObj = 1; // ScrollRectの画面内に表示される最大オブジェクト数
    private ScrollRect m_scrollRect = null;
    private ObjectPool<PoolScrollItem> m_itemPool = null;

    private void Start()
    {
        m_scrollRect = GetComponent<ScrollRect>();
        m_sizeY = m_itemPrefab.rectTransform.sizeDelta.y + m_spacing;
        m_maxItemObj = Mathf.CeilToInt(m_scrollRect.GetComponent<RectTransform>().sizeDelta.y / m_sizeY) + 1;
        m_itemPool = new ObjectPool<PoolScrollItem>(
            () => Instantiate(m_itemPrefab, m_scrollRect.content, false),
            r => r.gameObject.SetActive(true),
            r => r.gameObject.SetActive(false)
        );
        RefreshContentSize();
    }

    public void AddItem()
    {
        m_itemCount++;

        if (m_itemCount <= m_maxItemObj)
        {
            PoolScrollItem item = m_itemPool.Get();
            item.rectTransform.SetParent(m_scrollRect.content);
            CalcItemPosition(item.rectTransform, m_itemCount - 1);
        }
        RefreshContentSize();
    }

    public void RemoveItem()
    {
        if (m_itemCount == 0) return;

        m_itemCount = Mathf.Max(m_itemCount - 1, 0);

        if (0 <= m_itemCount && m_itemCount < m_maxItemObj)
        {
            m_itemPool.Release(m_scrollRect.content.GetChild(m_itemCount).GetComponent<PoolScrollItem>());
        }
        RefreshContentSize();
    }

    public void RefreshContentSize()
    {
        m_scrollRect.content.sizeDelta = new Vector2(
            m_itemPrefab.rectTransform.sizeDelta.x,
            m_itemCount * m_sizeY
        );
    }

    private void CalcItemPosition(RectTransform item, int index)
    {
        item.anchoredPosition = new Vector2(
            m_itemPrefab.rectTransform.anchoredPosition.x,
            -index * m_sizeY
        );
        item.GetComponent<PoolScrollItem>().OnUpdate(index);
    }

    private void Update()
    {
        if (m_itemCount == 0)
            return;

        int l = Mathf.Min(m_scrollRect.content.childCount - 1, m_itemCount - 1);
        RectTransform first = m_scrollRect.content.GetChild(0).GetComponent<RectTransform>();
        RectTransform last = m_scrollRect.content.GetChild(l).GetComponent<RectTransform>();

        int index = Mathf.FloorToInt(m_scrollRect.content.anchoredPosition.y / m_sizeY);
        int firstIndex = Mathf.FloorToInt(-first.anchoredPosition.y / m_sizeY);
        int lastIndex = Mathf.FloorToInt(-last.anchoredPosition.y / m_sizeY);

        while (lastIndex < index + l && lastIndex < m_itemCount - 1) // 先頭のアイテムを最後尾に移す
        {
            CalcItemPosition(first, lastIndex + 1);
            first.SetAsLastSibling();
            firstIndex++; lastIndex++;
            last = first;
            first = m_scrollRect.content.GetChild(0).GetComponent<RectTransform>();
        }

        while (firstIndex > index && firstIndex > 0) // 最後尾のアイテムを先頭に移す
        {
            CalcItemPosition(last, firstIndex - 1);
            last.SetAsFirstSibling();
            firstIndex--; lastIndex--;
            first = last;
            last = m_scrollRect.content.GetChild(l).GetComponent<RectTransform>();
        }
    }
}
