using UnityEngine;
using TMPro;

[ExecuteAlways]
public class ItemCountText : MonoBehaviour
{
    public TextMeshProUGUI m_text;

    public PoolScrollRect m_pool;

    private void Update()
    {
        m_text.SetText("ItemCount: {0}", m_pool.m_itemCount);
    }
}