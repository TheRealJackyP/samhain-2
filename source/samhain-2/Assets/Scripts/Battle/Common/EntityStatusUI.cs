using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EntityStatusUI : MonoBehaviour
{
    [FormerlySerializedAs("Text")] public TMP_Text HealthText;
    public TMP_Text ArmorText;
    public GameObject ArmorPanel;

    public EntityHealth TargetHealth;

    public Scrollbar HealthBar;
    private Camera _camera;
    private UnityAction<GameObject> DisableArmorAction;

    private UnityAction<GameObject> EnableArmorAction;

    private void Start()
    {
        _camera = Camera.main;
        EnableArmorAction = _ => EnableArmorUI();
        DisableArmorAction = _ => DisableArmorUI();
        TargetHealth.OnArmorGain.AddListener(EnableArmorAction);
        TargetHealth.OnArmorBreak.AddListener(DisableArmorAction);
        if (TargetHealth.Armor <= 0) DisableArmorUI();
    }

    private void Update()
    {
        HealthText.text = TargetHealth.CurrentHealth + "/" + TargetHealth.BaseHealth;
        ArmorText.text = TargetHealth.Armor.ToString();
        HealthBar.size = Mathf.Clamp01((float) TargetHealth.CurrentHealth / TargetHealth.BaseHealth);
        transform.position = _camera.WorldToScreenPoint(TargetHealth.transform.position);
    }

    private void OnDestroy()
    {
        TargetHealth.OnArmorGain.RemoveListener(EnableArmorAction);
        TargetHealth.OnArmorBreak.RemoveListener(DisableArmorAction);
    }

    public void EnableArmorUI()
    {
        ArmorPanel.gameObject.SetActive(true);
    }

    public void DisableArmorUI()
    {
        ArmorPanel.gameObject.SetActive(false);
    }
}