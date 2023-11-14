using Entity;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class ItemHolder : MonoBehaviour
{
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
        switch (rarity)
        {
            case 1: return m_MaterialTier1;
            case 2: return m_MaterialTier2;
            case 3: return m_MaterialTier3;
            default: return m_Renderer.material;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        //TODO Generate the object it has
        _currentItem = m_BuyableItem[0];
        _cost = 1;

        print(_currentItem.Rarity);
        m_Renderer.material = SelectColor(_currentItem.Rarity);

        m_Title.text = _currentItem.name + " Cost: " + _cost + "VGs";
        m_Title.material = SelectColor(_currentItem.Rarity);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isTriggerActive && Keyboard.current.fKey.wasPressedThisFrame && _cost <= gm.Score)
        {
            gm.Player().AddModifier(_currentItem);
            gm.IncrementScore(-_cost);
            UIManager.Instance.HideTips();
            Destroy(gameObject);
        }
        if (!gm.IsWaitingWave) Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTriggerActive = true;
            UIManager.Instance.MakeTips("Press 'F' to Buy");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isTriggerActive = false;
            UIManager.Instance.HideTips();
        }
    }
}
