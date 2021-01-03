using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Fire : MonoBehaviour {
    public float fireSize; // The current size of the fire, also used for initial value
    public float maximalSizeFire;
    public float speedDecreasing;
    public float warningSize; // Threshold after which a warning plays when fire level is below this size
    public float speedDecreasingWarning; // Speed at which fire decreases after warning (for helping player)
    public int densityFire;
    public int damageColliderSize = 100;
    public int fireColliderSize = 150;

    public float damageFire;
    public Light lightFire;
    public float lightRange;
    public GameObject damageCol;
    public GameObject col;
    public PlayerMovement player;

    public Slider slider;
    public GameObject warningSymbol;
    public AudioClip warningSound;

    private ParticleSystem part;
    private ParticleSystem.ShapeModule sh;
    private ParticleSystem.EmissionModule em;

    private AudioSource fireSfxSource;

    void Start() {
        fireSfxSource = GetComponents<AudioSource>()[1];
        part = GetComponent<ParticleSystem>();
        sh = part.shape;
        em = part.emission;
        slider.maxValue = maximalSizeFire;
        updateParts();
    }

    void Update() {
        if (part.isStopped) return;
        if (sh.scale.z < 0) {
            part.Stop();
            lightFire.intensity = 0;
            GetComponent<NavMeshObstacle>().enabled = false;
            Game.EndGame(false, "The fire died out...");
        } else {
            fireSize -= (fireSize > warningSize ? speedDecreasing : speedDecreasingWarning) * Time.deltaTime;
            updateParts();
        }
    }

    // Updates the particle system and the colliders
    void updateParts() {
        damageCol.transform.localScale = Vector3.one * (fireSize * damageColliderSize);
        col.transform.localScale = Vector3.one * (Math.Max(fireSize, 0.5f) * fireColliderSize);
        lightFire.range = Math.Min(fireSize, maximalSizeFire) * lightRange;
        sh.scale = Vector3.one * fireSize;
        em.rateOverTime = (ParticleSystem.MinMaxCurve)(Math.Pow(sh.scale.magnitude, 3) * densityFire);
        updateUI();
    }

    public void OnChildTriggerStay(string type, Collider other) {
        switch (type) {
            case "damage":
                if (other.bounds.size.x > 10) return; //TODO this is a stupid hack to not trigger this function with thedamageCollider that is being used for automatic crafting ui
                if (other.gameObject.CompareTag("Player") && !part.isStopped) {
                    other.gameObject.GetComponent<PlayerMovement>().TakeDamage(damageFire * Time.deltaTime);
                }
                break;

            default:
                if (other.gameObject.CompareTag("Item")) {
                    var item = other.gameObject.GetComponent<ItemAssociation>().item;
                    if (item.fuelSize > 0) {
                        fireSfxSource.Play();
                        fireSize = Math.Min(fireSize + item.fuelSize, maximalSizeFire);
                        player.removeObject(other.gameObject);
                        updateParts();
                        Destroy(other.gameObject);
                    } else if (other.gameObject.GetComponent<IFireInteraction>() != null) {
                        var interactableItem = other.gameObject.GetComponent<IFireInteraction>();
                        interactableItem.onFireInteraction();
                    }
                }
                break;

        }
    }

    // Updates the display indicating how much fuel is left in the fire
    private void updateUI() {
        if (fireSize < warningSize && !warningSymbol.activeInHierarchy) {
            warningSymbol.SetActive(true);
            player.playSound(warningSound);
            Hints.displayHint("My fire is almost out, \n I should add fuel soon !");
        }
        else if (warningSymbol.activeInHierarchy && fireSize > warningSize) warningSymbol.SetActive(false);
        slider.value = fireSize;
    }
}
