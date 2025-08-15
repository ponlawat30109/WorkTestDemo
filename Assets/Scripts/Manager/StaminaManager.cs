using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaManager : Singleton<StaminaManager>
{
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina;
    [SerializeField, ReadOnlyInspector] private float regenAmount = 1f;
    [SerializeField, ReadOnlyInspector] private float regenInterval = 5f;

    [SerializeField] private Image staminaBar;
    [SerializeField] private TMP_Text staminaText;

    private bool isRegenerating = false;

    public float CurrentStamina { get => currentStamina; set => currentStamina = value; }

    void Start()
    {
        if (System.IO.File.Exists(System.IO.Path.Combine(Application.persistentDataPath, "save.json")))
        {
            SaveManager.Instance.LoadStamina();
        }
        else
        {
            CurrentStamina = maxStamina;
        }
    }

    void Update()
    {
        staminaBar.fillAmount = CurrentStamina / maxStamina;
        staminaText.text = $"{CurrentStamina}/{maxStamina}";
    }

    public void UseStamina(float amount)
    {
        CurrentStamina = Mathf.Max(0, CurrentStamina - amount);

        if (!isRegenerating && CurrentStamina < maxStamina)
        {
            StartCoroutine(RegenerateStaminaRoutine());
        }
    }

    private IEnumerator RegenerateStaminaRoutine()
    {
        isRegenerating = true;
        while (CurrentStamina < maxStamina)
        {
            yield return new WaitForSeconds(regenInterval);
            RegenerateStamina(regenAmount);
        }
        isRegenerating = false;
    }

    public void RegenerateStamina(float amount)
    {
        CurrentStamina = Mathf.Min(CurrentStamina + amount, maxStamina);
    }
}
