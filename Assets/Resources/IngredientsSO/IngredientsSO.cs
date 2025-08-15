using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "IngredientsSO", menuName = "ScriptableObjects/IngredientsSO", order = 1)]
public class IngredientsSO : ScriptableObject
{
    [SerializeField] private string ingredientName;
    [SerializeField] private Sprite ingredientImage;

    public string IngredientName => ingredientName;
    public Sprite IngredientImage => ingredientImage;
}
