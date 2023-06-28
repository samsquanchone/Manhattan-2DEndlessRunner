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

        
        instrument.onValueChanged.AddListener(delegate { OnInstrumentChanged(); });
        variation.onClick.AddListener(delegate { OnRequestVariation(); });
        keyChange.onClick.AddListener(delegate { OnRequestKeyChange(); });
        keyReset.onClick.AddListener(delegate { OnRequestKeyReset(); });
      //tempo.onValueChanged.AddListener(delegate { OnTempoChanged(); });
        manhattan.Code("@Melody.instr = @Saw.instr");
    }

    public void OnInstrumentChanged() {
        switch (instrument.value) {
            case 0: // Saw Wave
                manhattan.Code("@Melody.instr = @Saw.instr");
                break;
            case 1: // Dulcimer
                manhattan.Code("@Melody.instr = @Dulcimer.instr");
                break;
            case 2: // Oboe
                manhattan.Code("@Melody.instr = @Oboe.instr");
                break;
        }
        manhattan.Run("Melody"); // (code to set register and volume)
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

    public void OnInput(string message) {
        if (message == "TubularBells") {
            if (TubularBells == null) {
                TubularBells = new Dropdown.OptionData() { text = "Tubular Bells" };
                if (!instrument.options.Contains(TubularBells))
                    instrument.options.Add(TubularBells);
            }
            instrument.value = 3;
        } else if (message == "Kalinka") {
            variation.interactable = true;
            keyChange.interactable = true;
            keyReset.interactable = true;
        }

        manhattan.Musicians[0].SetActive(instrument.value == 3);
        manhattan.Musicians[1].SetActive(instrument.value != 3);
    }
}
