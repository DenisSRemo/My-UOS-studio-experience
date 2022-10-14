using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.IO;

public class Options : MonoBehaviour
{
    public static float OptionsValue_DragSensitivity = 1f;
    public static bool OptionsValue_PushControls = false, OptionsValue_ParentalControls = false;

    public AudioMixer MasterMixer;

    public Slider Slider_Drag, Slider_EffectsVolume, Slider_MusicVolume;

    public Toggle Toggle_PushControls, Toggle_ParentalControl;

    public Image Image_PushControls, Image_ParentalControl;
    public Sprite 
        Sprite_ToggleOn,Sprite_ToggleOnHighLight,
        Sprite_ToggleOff, Sprite_ToggleOffHighLight;

    public Button ShopButton;

    private static string OptionsFolder = "OptionsData", OptionsFileName = "Data";
    private string FilePath;

    private void Start()
    {
        FilePath = Application.dataPath + "/Resources/" + OptionsFolder + "/" + OptionsFileName + ".txt";

        LoadOptions();
    }


    private void LoadOptions()
    {
        CheckDirectory();

        if (!File.Exists(FilePath)) { return; }

        StreamReader sr = File.OpenText(FilePath);

        Set_Drag(float.Parse(sr.ReadLine()));
        Set_EffectsVolume(float.Parse(sr.ReadLine()));
        Set_MusicVolume(float.Parse(sr.ReadLine()));
        Set_PushControls(bool.Parse(sr.ReadLine()));
        Set_ParentalControls(bool.Parse(sr.ReadLine()));

        sr.Close();
        Debug.Log("Loaded options");
    }

    public void saveOptions()
    {
        CheckDirectory();

        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }

        StreamWriter SW = File.CreateText(FilePath);

        SW.WriteLine(OptionsValue_DragSensitivity);
        SW.WriteLine(Slider_EffectsVolume.value);
        SW.WriteLine(Slider_MusicVolume.value);
        SW.WriteLine(OptionsValue_PushControls);
        SW.WriteLine(OptionsValue_ParentalControls);

        SW.Close();
        Debug.Log("Saved options");
    }

    private void CheckDirectory()
    {
        if (!Directory.Exists(Application.dataPath + "/Resources/" + OptionsFolder))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources/" + OptionsFolder);
            Debug.Log("Created options data directory.");
        }
    }

    public void Change_PushControls()
    {
        Set_PushControls(Toggle_PushControls.isOn);
    }

    public void Change_ParentalControls()
    {
        Set_ParentalControls(Toggle_ParentalControl.isOn);
    }

    public void Change_Drag()
    {
        Set_Drag(Slider_Drag.value);
    }

    public void Change_Effects()
    {
        Set_EffectsVolume(Slider_EffectsVolume.value);
    }

    public void Change_Music()
    {
        Set_MusicVolume(Slider_MusicVolume.value);
    }

    private void Set_Drag(float newValue)
    {
        Slider_Drag.value = newValue;
        OptionsValue_DragSensitivity = newValue;
    }
    private void Set_EffectsVolume(float newValue)
    {
        Slider_EffectsVolume.value = newValue;
        //Debug.Log("Set Effects volume to: " + ConvertValueToSound(newValue));
        MasterMixer.SetFloat("Effects", ConvertValueToSound(newValue));
    }
    private void Set_MusicVolume(float newValue)
    {
        Slider_MusicVolume.value = newValue;
        //Debug.Log("Set Music volume to: " + ConvertValueToSound(newValue));
        MasterMixer.SetFloat("Music", ConvertValueToSound(newValue));
    }
    private float ConvertValueToSound(float value)
    {
        return Mathf.Clamp((Mathf.Log(value) * 20f), -80f, 0f);
    }

    private void Set_PushControls(bool newValue)
    {
        OptionsValue_PushControls = newValue;

        SpriteState SS = new SpriteState();

        if (OptionsValue_PushControls)
        {
            Image_PushControls.sprite = Sprite_ToggleOn;
           
            SS.highlightedSprite = Sprite_ToggleOnHighLight;
            SS.pressedSprite = Sprite_ToggleOff;
            SS.selectedSprite = Sprite_ToggleOnHighLight;
            SS.disabledSprite = Sprite_ToggleOn;
            
        }
        else
        {
            Image_PushControls.sprite = Sprite_ToggleOff;

            SS.highlightedSprite = Sprite_ToggleOffHighLight;
            SS.pressedSprite = Sprite_ToggleOff;
            SS.selectedSprite = Sprite_ToggleOffHighLight;
            SS.disabledSprite = Sprite_ToggleOff;
        }
        Toggle_PushControls.spriteState = SS;
    }

    private void Set_ParentalControls(bool newValue)
    {
        OptionsValue_ParentalControls = newValue;

        SpriteState SS = new SpriteState();

        if (OptionsValue_ParentalControls)
        {
            Image_ParentalControl.sprite = Sprite_ToggleOn;
            
            SS.highlightedSprite = Sprite_ToggleOnHighLight;
            SS.pressedSprite = Sprite_ToggleOff;
            SS.selectedSprite = Sprite_ToggleOnHighLight;
            SS.disabledSprite = Sprite_ToggleOn;            
        }
        else
        {
            Image_ParentalControl.sprite = Sprite_ToggleOff;

            SS.highlightedSprite = Sprite_ToggleOffHighLight;
            SS.pressedSprite = Sprite_ToggleOff;
            SS.selectedSprite = Sprite_ToggleOffHighLight;
            SS.disabledSprite = Sprite_ToggleOff;            
        }

        ShopButton.interactable = !OptionsValue_ParentalControls;
        Toggle_ParentalControl.spriteState = SS;
    }
}
