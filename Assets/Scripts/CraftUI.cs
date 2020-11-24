using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CraftUI : MonoBehaviour
{
    public GameObject player;
    private GameObject shipyard; //TODO what if there are multiple shipyards?
    private bool isPlayerCrafter = true; //TODO ugly

    public AudioSource audioSource;
    public AudioClip craftSound;

    public GameObject craftUI;
    public GameObject optionsPanel;
    public GameObject craftOption;
    public Button craftButton;
    public Color canCraftColor;

    private Crafter crafter; // The crafter which is using this menu (e.g. player or workbench)

    void Start()
    {
        craftButton.onClick.AddListener(craftSelected);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("c")) toggleUI();
    }

    void toggleUI()
    {
        if (craftUI.activeInHierarchy)
        {
            craftUI.SetActive(false);
        }
        else
        {
            setCrafter();
            craftUI.SetActive(true);
            detailPanel.SetActive(false);
            refreshUI();
        }
    }

    // If we ever want to craft using workbenches or other special stations, this is where you should set them
    void setCrafter()
    {
        if (shipyard && Vector3.Distance(player.transform.position, shipyard.transform.position) < 7)
        {
            crafter = shipyard.GetComponent<Crafter>();
            isPlayerCrafter = false;
        }
        else
        {
            crafter = player.GetComponent<Crafter>();
            isPlayerCrafter = true;
        }
    }

    public void setShipyard(GameObject newShipyard)
    {
        shipyard = newShipyard;
    }

    // Fetches all the possible recipes again
    // We *could* make this trigger every x ms to get an automatically updating menu, but I'm not sure if it is
    // a smart idea regarding performance - there are many expensive calls (especially getPossibleRecipes)
    private GameObject optionsList;

    void refreshUI()
    {
        if (optionsList != null) Destroy(optionsList); // Destroy any options we created earlier
        optionsList = new GameObject("OptionsList"); // Empty GameObject to easily contain all recipe options
        optionsList.transform.SetParent(optionsPanel.transform, false);

        var possibleRecipes = crafter.getPossibleRecipes();

        var xOffset = 0;
        foreach (var recipe in crafter.knownRecipes)
        {
            var newButton = Instantiate(craftOption, optionsList.transform, false);
            newButton.transform.localPosition += new Vector3(xOffset % 360, -(xOffset / 360) * 120, 0);
            xOffset += 120; // TODO make actual craft UI
            newButton.GetComponentInChildren<Text>().text = recipe.name;
            newButton.GetComponent<Button>().onClick.AddListener(delegate { selectRecipe(recipe); });
            if (!possibleRecipes.Contains(recipe)) continue; // We can't craft this recipe
            newButton.GetComponent<Image>().color = canCraftColor;
        }
    }

    public GameObject detailPanel;
    public Text detailTitle;
    public Text detailDescription;
    public Text detailIngredients;
    private Recipe currentlySelected;

    void selectRecipe(Recipe recipe)
    {
        if (!detailPanel.activeInHierarchy) detailPanel.SetActive(true);
        detailTitle.text = recipe.name;
        detailDescription.text = recipe.description;
        currentlySelected = recipe;
        var neededItems = crafter.getNeededItems(recipe);
        // TODO replace string display with icons
        detailIngredients.text = neededItems.Keys.Aggregate("", (current, item) =>
            current + (item.name + ": " + neededItems[item].Item1 + "/" + neededItems[item].Item2 + "\n"));
    }

    void craftSelected()
    {
        if (isPlayerCrafter) crafter.craftPlayerRecipe(currentlySelected);
        else
        {
            crafter.craftShipyardRecipe(currentlySelected, shipyard);
            craftUI.SetActive(false);
        }

        audioSource.PlayOneShot(craftSound); // todo - currently always plays regardless of success
        StartCoroutine(nameof(delayedUpdate));
    }

    // Calls refresh after a short bit, to allow unity to delete the crafting ingredients of the previous recipe
    // This is not a very pretty way so feel free to improve it
    IEnumerator delayedUpdate()
    {
        yield return new WaitForSeconds(0.05f);
        refreshUI();
        selectRecipe(currentlySelected); // Despite already selected, we need to reselect it to update the ingredients
    }
}