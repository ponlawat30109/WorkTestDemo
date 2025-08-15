using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class MenuSaveData
{
    public List<string> currentMenuFoodIDs = new();
    public float cookingTime = 0f;
    public List<IngredientSaveData> ingredientData = new();
    public float stamina = 0f;
}

[System.Serializable]
public class IngredientSaveData
{
    public string ingredient;
    public int quantity;
}

public class SaveManager : Singleton<SaveManager>
{
    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

#if UNITY_EDITOR
    [ContextMenu("Open Save JSON Location")]
    private void OpenJsonFile()
    {
        UnityEditor.EditorUtility.RevealInFinder(SavePath);
    }
#endif

    public void SaveCooking()
    {
        var data = new MenuSaveData();

        foreach (var food in GameManager.Instance.CurrentFoodData)
        {
            data.currentMenuFoodIDs.Add(food.name);
        }
        data.cookingTime = GameManager.Instance.CookingTimer;

        foreach (var ingredient in GameManager.Instance.CurrentIngredientData)
        {
            data.ingredientData.Add(new IngredientSaveData
            {
                ingredient = ingredient.ingredient.name,
                quantity = ingredient.quantity
            });
        }

        data.stamina = StaminaManager.Instance != null ? StaminaManager.Instance.CurrentStamina : 0f;

        var json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    // public void LoadCooking()
    // {
    //     if (!File.Exists(SavePath))
    //         return;

    //     var json = File.ReadAllText(SavePath);
    //     var data = JsonUtility.FromJson<MenuSaveData>(json);

    //     var allFoods = GameManager.Instance.AllFoodData;
    //     var loadedMenu = new List<FoodSO>();
    //     foreach (var name in data.currentMenuFoodIDs)
    //     {
    //         var food = allFoods.Find(f => f.name == name);
    //         if (food != null)
    //             loadedMenu.Add(food);
    //     }
    //     GameManager.Instance.CurrentFoodData = loadedMenu;
    //     GameManager.Instance.CookingTimer = data.cookingTime;

    //     var allIngredients = GameManager.Instance.AllIngredientsData;
    //     var loadedIngredients = new List<IngredientsData>();
    //     foreach (var ing in data.ingredientData)
    //     {
    //         var ingredientSO = allIngredients.Find(i => i.name == ing.ingredient);
    //         if (ingredientSO != null)
    //         {
    //             loadedIngredients.Add(new IngredientsData
    //             {
    //                 ingredient = ingredientSO,
    //                 quantity = ing.quantity
    //             });
    //         }
    //     }
    //     GameManager.Instance.CurrentIngredientData = loadedIngredients;

    //     if (StaminaManager.Instance != null)
    //         StaminaManager.Instance.CurrentStamina = data.stamina;

    //     GameManager.Instance.InitializeMenuContent();
    // }

    public void LoadIngredientsData()
    {
        if (!File.Exists(SavePath))
            return;

        var json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<MenuSaveData>(json);

        var allIngredients = GameManager.Instance.AllIngredientsData;
        var loadedIngredients = new List<IngredientsData>();
        foreach (var ing in data.ingredientData)
        {
            var ingredientSO = allIngredients.Find(i => i.name == ing.ingredient);
            if (ingredientSO != null)
            {
                loadedIngredients.Add(new IngredientsData
                {
                    ingredient = ingredientSO,
                    quantity = ing.quantity
                });
            }
        }

        GameManager.Instance.CurrentIngredientData = loadedIngredients;
    }

    public void LoadStamina()
    {
        if (!File.Exists(SavePath))
            return;

        var json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<MenuSaveData>(json);

        if (StaminaManager.Instance != null)
            StaminaManager.Instance.CurrentStamina = data.stamina;
    }
}