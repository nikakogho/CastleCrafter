using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildUIButton : MonoBehaviour
{
    [Header("Wiring")]
    public PartData part;
    public Image iconImage;
    public TMP_Text hotkeyLabel;

    private static TMP_Text tooltipText;

    void Awake()
    {
        iconImage.sprite = part.icon;
    }

    private void Start()
    {
        tooltipText = BuildManager.Instance.tooltip.GetComponentInChildren<TMP_Text>();
    }

    void OnEnable() => GetComponent<Button>().onClick.AddListener(OnClick);
    void OnDisable() => GetComponent<Button>().onClick.RemoveListener(OnClick);
    void OnClick() => BuildManager.Instance.SelectPart(part);

    /* ---------- Tooltip ---------- */
    public void ShowTip()
    {
        tooltipText.text = part.name;
        BuildManager.Instance.tooltip.SetActive(true);
    }

    public void HideTip() => BuildManager.Instance.tooltip.SetActive(false);

    public void SelectPart()
    {
        BuildManager.Instance.SelectPart(part);
    }
}
