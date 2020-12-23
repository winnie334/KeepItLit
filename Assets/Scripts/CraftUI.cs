using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CraftUI : MonoBehaviour {
    public Transform playerBrain;
    private GameObject shipyard; //TODO what if there are multiple shipyards?
    
    public AudioSource audioSource;
    public AudioClip craftSound;

    public GameObject tutorial;
    public GameObject craftUI;
    public GameObject recipiesPanel;
    public GameObject recipe;
    public Button craftButton;
    public Color canCraftColor;

    private Crafter crafter; // The crafter which is using this menu (e.g. player or workbench)

    void Start() {
        setCrafter(null);
        craftButton.onClick.AddListener(craftSelected);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("c")) toggleUI();
    }

    void toggleUI() {
        if (craftUI.activeInHierarchy) {
            craftUI.SetActive(false);
            tutorial.SetActive(true);
            crafter.resetAvailableItems();
        } else {
            craftUI.SetActive(true);
            tutorial.SetActive(false);
            detailPanel.SetActive(false);
            refreshUI();
        }
    }

    // If we ever want to craft using workbenches or other special stations, this is where you should set them
    public void setCrafter(Crafter newCrafter) {
        crafter = newCrafter ? newCrafter : playerBrain.GetComponent<Crafter>();
        if (craftUI.activeInHierarchy) {
            refreshUI();
        }
    }

    // Fetches all the possible recipes again
    // We *could* make this trigger every x ms to get an automatically updating menu, but I'm not sure if it is
    // a smart idea regarding performance - there are many expensive calls (especially getPossibleRecipes)
    private GameObject optionsList;

    public void refreshUI() {
        if (!craftUI.activeInHierarchy) return;
        if (optionsList != null) Destroy(optionsList); // Destroy any options we created earlier
        optionsList = new GameObject("OptionsList"); // Empty GameObject to easily contain all recipe options

        var possibleRecipes = crafter.getPossibleRecipes();

        foreach (Transform child in recipiesPanel.transform) {
            Destroy(child.gameObject);
        }

        foreach (var i in crafter.knownRecipes) {
            var rcp = Instantiate(recipe, recipiesPanel.transform, true);
            rcp.SetActive(true);
            rcp.GetComponent<Button>().onClick.AddListener(delegate { selectRecipe(i); });
            
            var item = i.resultingItem.GetComponent<ItemAssociation>().item;
            rcp.GetComponentsInChildren<Image>()[1].sprite = i.resultingItem.GetComponent<ItemAssociation>().item.icon;
            if (!possibleRecipes.Contains(i)) continue; // We can't craft this recipe
            rcp.GetComponent<Image>().color = canCraftColor;
        }

        if (currentlySelected) selectRecipe(currentlySelected);
    }

    public GameObject detailPanel;
    public RawImage icon;
    public Text detailTitle;
    public Text detailDescription;
    public GameObject ingredients;
    public GameObject ingredient;
    private Recipe currentlySelected;

    void selectRecipe(Recipe recipe) {
        if (!detailPanel.activeInHierarchy) detailPanel.SetActive(true);
        detailTitle.text = recipe.resultingItem.GetComponent<ItemAssociation>().item.title;
        detailDescription.text = recipe.description;
        currentlySelected = recipe;
        var neededItems = crafter.getNeededItems(recipe);

        foreach (Transform child in ingredients.transform) {
            Destroy(child.gameObject);
        }

        foreach (var item in neededItems) {
            var ingr = Instantiate(ingredient);
            ingr.SetActive(true);
            ingr.transform.SetParent(ingredients.transform);
            ingr.GetComponentInChildren<Image>().sprite = item.Key.icon;
            ingr.GetComponentInChildren<Text>().text = item.Value.Item1 + "/" + item.Value.Item2;

        }
    }

    void craftSelected() {
        if (!crafter.canMakeRecipe(currentlySelected)) return;
        crafter.craftRecipe(currentlySelected);
        audioSource.PlayOneShot(craftSound);
        StartCoroutine(nameof(delayedUpdate));
    }

    // Calls refresh after a short bit, to allow unity to delete the crafting ingredients of the previous recipe
    // This is not a very pretty way so feel free to improve it
    IEnumerator delayedUpdate() {
        yield return new WaitForSeconds(0.05f);
        refreshUI();
        selectRecipe(currentlySelected); // Despite already selected, we need to reselect it to update the ingredients
    }
}