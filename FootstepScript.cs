using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepScript : MonoBehaviour 
{
    [Header("References")]
    public AudioClip[] footsteps;

    [Header("Attributes")]
    public float stepInterval = 0.6f; // Interval in seconds.
    [Range(0,1f)] public float stepWidth = 0.3f; //Affects the panning amount between steps.

    private AudioSource audioSource;
    private bool isWalking;
    private int storedStepIndex = -1; // Initialize with an invalid value.
    private bool waitingForStep;
    private bool rightStep = false;

    void Start()
    {
        //Use the audioSource attached to this gameObject for footstep sounds.
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        // Check for continuous input (e.g., holding down W).
        isWalking = Input.GetButton("Vertical") || Input.GetButton("Horizontal");

        if (isWalking && !waitingForStep)
        {
            // Only start the coroutine if not already walking and not waiting for a step.
            StartCoroutine(WaitForStep());
        }
        else if (!isWalking)
        {
            // Stop the audio if not walking.
            waitingForStep = false;
            StopAllCoroutines();
        }
    }

    void TakeStep()
    {
        int newStepIndex;
        if (footsteps.Length > 1)
        {
            //If there are multiple footstep sounds to select from,
            //find a step sound that was different from the last one.
            do
            {
                newStepIndex = Random.Range(0, footsteps.Length);
            } while (newStepIndex == storedStepIndex);
    
            storedStepIndex = newStepIndex;
        } else {
            //If there is only one footstep sound, choose that one to play.
            newStepIndex = 0;
        }

        //Prepare audio source to play left or right panned audio.
        rightStep = !rightStep;
        if(rightStep)
        {
            audioSource.panStereo = stepWidth;
        } else {
            audioSource.panStereo = -stepWidth;
        }

        //Set and play the audio.
        audioSource.clip = footsteps[newStepIndex];
        audioSource.Play();
    }

    IEnumerator WaitForStep()
    {
        while (isWalking)
        {
            //Take a step then wait for the next step while directional input is being held.
            TakeStep();
            waitingForStep = true;
            yield return new WaitForSeconds(stepInterval);
            waitingForStep = false;
        }
    }
}
