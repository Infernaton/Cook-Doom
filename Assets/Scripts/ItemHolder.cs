using Entity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utils;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] Material m_MaterialHeal;
    [SerializeField] Material m_MaterialTier1;
    [SerializeField] Material m_MaterialTier2;
    [SerializeField] Material m_MaterialTier3;
    [SerializeField] Modifier[] m_BuyableItem;

    [SerializeField] TextMeshPro m_Title;
    [SerializeField] MeshRenderer m_Renderer;

    private Modifier _currentItem;
    private bool _isTriggerActive;
    private int _cost;

    private RawImage parentTitle;

    private GameManager gm;

    Material SelectColor(int rarity)
    {
        if (_currentItem is InstantHeal) return m_MaterialHeal;
        switch (rarity)
        {
            case 1: return m_MaterialTier1;
            case 2: return m_MaterialTier2;
            case 3: return m_MaterialTier3;
            default: return m_Renderer.material;
        }
    }

    Modifier GenHoldingItem(int iteration = 1)
    {
        int r = Random.Range(0, m_BuyableItem.Length - 1);
        Modifier mod = m_BuyableItem[r];
        return mod.Rarity - iteration <= 0 ? mod : GenHoldingItem(iteration+1);
    }

    private void Awake()
    {
        parentTitle = m_Title.transform.parent.GetComponent<RawImage>();
        parentTitle.gameObject.SetActive(false);
    }

    void Start()
    {
        gm = GameManager.Instance;
        _currentItem = GenHoldingItem();
        _cost = DMath.Square(_currentItem.Rarity) * DMath.Square((gm.Player().GetTotalItem() / 10) + 1);

        m_Renderer.material = SelectColor(_currentItem.Rarity);

        m_Title.text = _currentItem.name + " Cost: " + _cost + "VGs";
        m_Title.color = SelectColor(_currentItem.Rarity).color;
    }

    void Update()
    {
        if (_isTriggerActive && Keyboard.current.fKey.wasPressedThisFrame && _cost <= gm.Score)
        {
            gm.IncrementScore(-_cost);
            if (_currentItem is InstantHeal heal) gm.Player().InstantHeal(heal.Healing);
            else gm.Player().AddModifier(_currentItem);
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTriggerActive = true;
            StartCoroutine(Anim.FadeIn(0.2f, parentTitle));
            UIManager.Instance.DisplayDetails(_currentItem.ToString());
            //UIManager.Instance.MakeTips("Press 'F' to Buy");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTriggerActive = false;
            StartCoroutine(Anim.FadeOut(0.1f, parentTitle));
            UIManager.Instance.HideDetails();
        }
    }

    public void OnDestroy()
    {
        UIManager.Instance.HideDetails();
    }
}
