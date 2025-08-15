using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
    [SerializeField, ExposedScriptableObject] private FoodSO foodSO;
    [SerializeField] private List<GameObject> rarityIcons;
    [SerializeField] private Sprite foodImage;
    [SerializeField] private Image foodImageSlot;
    [SerializeField] private TMP_Text foodNameText;

    public FoodSO FoodSO { get => foodSO; set => foodSO = value; }
    public Sprite FoodImage { get => foodImage; set => foodImage = value; }
    public Image FoodImageSlot { get => foodImageSlot; set => foodImageSlot = value; }
    public TMP_Text FoodNameText { get => foodNameText; set => foodNameText = value; }

    private Button foodButton;

    public void Initialize()
    {
        FoodNameText.text = FoodSO.FoodName;
        FoodImage = FoodSO.FoodImage;
        FoodImageSlot.sprite = FoodImage;

        foreach (GameObject icon in rarityIcons)
        {
            icon.SetActive(false);
        }

        switch (FoodSO.Rarity)
        {
            case 1:
                rarityIcons[0].SetActive(true);
                break;
            case 2:
                rarityIcons[1].SetActive(true);
                break;
            case 3:
                rarityIcons[2].SetActive(true);
                break;
        }

        foodButton = GetComponent<Button>();
        foodButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance.IsCooking)
                return;

            GameManager.Instance.SetCurrentFood(FoodSO);
            GetComponent<Outline>().enabled = true;
        });
    }

    private void OnDestroy()
    {
        if (foodButton != null)
        {
            foodButton.onClick.RemoveAllListeners();
        }
    }
}
