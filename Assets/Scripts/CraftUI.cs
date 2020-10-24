using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftUI : MonoBehaviour {

    public GameObject player;
    
    public GameObject craftUI;
    public GameObject optionsPanel;
    public GameObject craftOption;

    public Button craftButton;
    public Color canCraftColor;

    void Start() {
        craftButton.onClick.AddListener(craftSelected);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("c")) toggleUI();
    }

    void toggleUI() {
        if (craftUI.activeInHierarchy) {
            craftUI.SetActive(false);
        } else {
            craftUI.SetActive(true);
            detailPanel.SetActive(false);
            refreshUI();
        }
    }

    // Fetches all the possible recipes again
    // We *could* make this trigger every x ms to get an automatically updating menu, but I'm not sure if it is
    // a smart idea regarding performance - there are many expensive calls (especially getPossibleRecipes)
    private GameObject optionsList;
    void refreshUI() {
        if (optionsList != null) Destroy(optionsList); // Destroy any options we created earlier
        optionsList = new GameObject("OptionsList"); // Empty GameObject to easily contain all recipe options
        optionsList.transform.SetParent(optionsPanel.transform, false);

        var crafter = player.GetComponent<Crafter>(); // If we have workbenches at some point, we should check for 
                                                      // their presence here
        var possibleRecipes = crafter.getPossibleRecipes();

        var xOffset = 0;
        foreach (var recipe in crafter.knownRecipes) {
            var newButton = Instantiate(craftOption, optionsList.transform, false);
            newButton.transform.localPosition += new Vector3(xOffset, 0, 0); xOffset += 120; // TODO make actual craft UI
            newButton.GetComponentInChildren<Text>().text = recipe.name;
            newButton.GetComponent<Button>().onClick.AddListener(delegate { selectRecipe(recipe); });
            if (!possibleRecipes.Contains(recipe)) continue; // We can't craft this recipe
            newButton.GetComponent<Image>().color = canCraftColor;
        }
    }

    public GameObject detailPanel;
    public Text detailTitle;
    public Text detailDescription;
    public GameObject detailIngredients; // TODO: displaying which ingredients you have and still need is kinda a hassle
                                         // TODO: I think the best option is to add a function to crafter which tells you
                                         // TODO: for one specific recipe. Only issue is that you might have moved since opening UI 
    private Recipe currentlySelected;
    void selectRecipe(Recipe recipe) {
        if (!detailPanel.activeInHierarchy) detailPanel.SetActive(true);
        detailTitle.text = recipe.name;
        detailDescription.text = recipe.description;
        currentlySelected = recipe;
    }

    void craftSelected() {
        player.GetComponent<Crafter>().craftRecipe(currentlySelected);
        StartCoroutine(nameof(delayedRefresh));
    }
    
    // Calls refresh after a short bit, to allow unity to delete the crafting ingredients of the previous recipe
    // This is not a very pretty way so feel free to improve it
    IEnumerator delayedRefresh() {
        yield return new WaitForSeconds(0.05f);
        refreshUI();
    }
}
