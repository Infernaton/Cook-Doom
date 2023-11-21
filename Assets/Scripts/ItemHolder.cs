using Entity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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

    private GameManager gm;

    Material SelectColor(int rarity)
    {
        if (_currentItem is InstantHeal) return m_MaterialHeal;
        return rarity switch
        {
            1 => m_MaterialTier1,
            2 => m_MaterialTier2,
            3 => m_MaterialTier3,
            _ => m_Renderer.material,
        };
    }

    Modifier GenHoldingItem(int iteration = 1)
    {
        int r = Random.Range(0, m_BuyableItem.Length - 1);
        Modifier mod = m_BuyableItem[r];
        print("iterate: " + iteration + " | Random: " + r + " | Reset ? " + !(mod.Rarity - iteration <= 0));
        return mod.Rarity - iteration <= 0 ? mod : GenHoldingItem(iteration+1);
    }

    private void Awake()
    {
        m_Title.enabled = false;
    }

    void Start()
    {
        gm = GameManager.Instance;
        _currentItem = GenHoldingItem();
        _cost = _currentItem.Rarity * _currentItem.Rarity;

        m_Renderer.material = SelectColor(_currentItem.Rarity);

        m_Title.text = _currentItem.name + " Cost: " + _cost + "VGs";
        m_Title.material = SelectColor(_currentItem.Rarity);
    }

    void Update()
    {
        if (_isTriggerActive && Keyboard.current.fKey.wasPressedThisFrame && _cost <= gm.Score)
        {
            gm.IncrementScore(-_cost);
            if (_currentItem is InstantHeal heal) gm.Player().InstantHeal(heal.Healing);
            else gm.Player().AddModifier(_currentItem);
            UIManager.Instance.HideTips();
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTriggerActive = true;
            StartCoroutine(Anim.FadeIn(0.5f, m_Title));
            UIManager.Instance.MakeTips("Press 'F' to Buy");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTriggerActive = false;
            StartCoroutine(Anim.FadeOut(0.5f, m_Title));
            UIManager.Instance.HideTips();
        }
    }
}
