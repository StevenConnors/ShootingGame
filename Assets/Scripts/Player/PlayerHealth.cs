using UnityEngine;
using UnityEngine.UI; //need this to use the UI components
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100; //dictates initial health
    public int currentHealth;
    public Slider healthSlider; //reference to the slider UI element that we created
    public Image damageImage;  //reference to damageimage we created
    public AudioClip deathClip;
    public float flashSpeed = 5f; //how quickly the damageImage flashes 
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f); //color of damageImage 
    
    
    Animator anim;
    AudioSource playerAudio;
    PlayerMovement playerMovement;
    PlayerShooting playerShooting;
    bool isDead;
    bool damaged;
    
	public float restartDelay = 15f;
	float restartTimer;   
    
    void Awake ()
    {
        anim = GetComponent <Animator> ();
        playerAudio = GetComponent <AudioSource> ();
        playerMovement = GetComponent <PlayerMovement> (); //use the name of the script 
        playerShooting = GetComponentInChildren <PlayerShooting> ();
        currentHealth = startingHealth;
    }
    
    
    void Update ()
    {
        if(damaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
    }
    
    
    public void TakeDamage (int amount)
    {
        damaged = true;
        
        currentHealth -= amount;
        
        healthSlider.value = currentHealth;
        
        playerAudio.Play ();
        
        if(currentHealth <= 0 && !isDead)
        {
            Death ();
        }
    }
    
    
    void Death ()
    {
        isDead = true;
        
        playerShooting.DisableEffects ();
        
        anim.SetTrigger ("Die");
        
        playerAudio.clip = deathClip;
        playerAudio.Play ();
        
        playerMovement.enabled = false;
        playerShooting.enabled = false;
    }
    
    
    public void RestartLevel ()
    {
	    // .. increment a timer to count up to restarting.
		restartTimer += Time.deltaTime;
		
		// .. if it reaches the restart delay...
		if(restartTimer >= restartDelay)
		{
			// .. then reload the currently loaded level.
			Application.LoadLevel(Application.loadedLevel);
		}
    }
}