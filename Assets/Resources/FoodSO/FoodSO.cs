using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "FoodSO", menuName = "ScriptableObjects/FoodSO", order = 1)]
public class FoodSO : ScriptableObject
{
    [SerializeField] private string foodName;
    [SerializeField] private int cookingTime;
    [SerializeField] private int rarity;
    [SerializeField] private Sprite foodImage;

    [SerializeField] private List<IngredientsData> ingredients;

    public string FoodName => foodName;
    public int CookingTime => cookingTime;
    public int Rarity => rarity;
    public Sprite FoodImage => foodImage;
    public List<IngredientsData> Ingredients => ingredients;
}

[Serializable]
public class IngredientsData
{
    public IngredientsSO ingredient;
    public int quantity;
}