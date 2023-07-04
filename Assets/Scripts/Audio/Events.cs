using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Events : MonoBehaviour, Manhattan.Listener {



    public static Events Instance => m_instance;
    private static Events m_instance;
    public Manhattan manhattan;
    public Dropdown instrument;
    public Button variation;
    public Button keyChange, keyReset;
    public Slider tempo;

    
    

    // Start is called before the first frame update
    void Start()
    {
        m_instance = this;

        
       
        manhattan.Code("@Melody.instr = @Saw.instr");
    }

    public void OnRequestVariation() {
        manhattan.Set("Variation", 1);
        variation.interactable = false;
    }

    public void OnRequestKeyChange() {
        manhattan.Set("Transpose", 1);
        keyChange.interactable = false;
    }

    public void OnRequestKeyReset() {
        manhattan.Set("Transpose", 0);
        manhattan.Code("@Transpose.pitch = E-3");
        keyChange.interactable = false;
        keyReset.interactable = false;
    }

    public void OnTriggerStinger(PickUpTypes pickUpTypes)
    {
        switch(pickUpTypes)
        {
            case PickUpTypes.SPEED:
                Debug.Log("Triggering: " + pickUpTypes.ToString() + "  Stinger");
                break;

            case PickUpTypes.HEALTH:
                Debug.Log("Triggering: " + pickUpTypes.ToString() + "  Stinger");
                break;

            case PickUpTypes.WORMHOLE:
                Debug.Log("Triggering: " + pickUpTypes.ToString() + "  Stinger");
                break;

            case PickUpTypes.SHIELD:
                Debug.Log("Triggering: " + pickUpTypes.ToString() + "  Stinger");
                break;

        }
    }

    public void Hello()
    {
        
    }
    public void OnTempoChanged(int value)
    {
        manhattan.Set(".tempo", value);
        manhattan.Run("Loop"); // (code to resync drum loop)
    }

    public void ResetTempo()
    {
        manhattan.Set(".tempo", 80);
        manhattan.Run("Loop"); // (code to resync drum loop)
    }

    Dropdown.OptionData TubularBells = null;    // 'hidden' Tubular Bells instrument entry

    
    public void OnInput(string message)
    {

        
    }
    
}
