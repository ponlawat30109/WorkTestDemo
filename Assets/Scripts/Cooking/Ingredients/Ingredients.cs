using TMPro;
using UnityEditor.U2D.PSD;
using UnityEngine;
using UnityEngine.UI;

public class Ingredients : MonoBehaviour
{
    [SerializeField, ExposedScriptableObject] private IngredientsSO ingredientsSO;

    [SerializeField] private Sprite ingredientImage;
    [SerializeField] private Image ingredientImageSlot;
    [SerializeField] private TMP_Text ingredientNameText;

    public IngredientsSO IngredientsSO { get => ingredientsSO; set => ingredientsSO = value; }
    public Sprite IngredientImage { get => ingredientImage; set => ingredientImage = value; }
    public Image IngredientImageSlot { get => ingredientImageSlot; set => ingredientImageSlot = value; }
    public TMP_Text IngredientNameText { get => ingredientNameText; set => ingredientNameText = value; }

    public void Initialize()
    {
        IngredientImage = IngredientsSO.IngredientImage;
        IngredientImageSlot.sprite = IngredientImage;
        // IngredientNameText.text = IngredientsSO.IngredientName;
    }
}
