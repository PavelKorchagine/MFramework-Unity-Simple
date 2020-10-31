using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Displays one of the color values of aColorPicker
/// </summary>
[RequireComponent(typeof(Slider), typeof(EventTrigger))]
public class ColorSlider : MonoBehaviour
{
    public ColorPicker hsvpicker;

    /// <summary>
    /// Which value this slider can edit.
    /// </summary>
    public ColorValues type;

    private Slider slider;
    private EventTrigger eventTrigger;

    private bool listen = true;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }
        hsvpicker.onValueChanged.AddListener(ColorChanged);
        hsvpicker.onHSVChanged.AddListener(HSVChanged);
        slider.onValueChanged.AddListener(SliderChanged);

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.BeginDrag;
        entryEnter.callback.AddListener((ba) => {
            hsvpicker.onValueBeginChanged.Invoke(hsvpicker.CurrentColor);
        });
        eventTrigger.triggers.Add(entryEnter);

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.EndDrag;
        entry.callback.AddListener((ba)=> {
            hsvpicker.onValueEndChanged.Invoke(hsvpicker.CurrentColor);
        });
        eventTrigger.triggers.Add(entry);
    }

    private void OnDestroy()
    {
        hsvpicker.onValueChanged.RemoveListener(ColorChanged);
        hsvpicker.onHSVChanged.RemoveListener(HSVChanged);
        slider.onValueChanged.RemoveListener(SliderChanged);

        eventTrigger.triggers.Clear();
    }

    private void ColorChanged(Color newColor)
    {
        listen = false;
        switch (type)
        {
            case ColorValues.R:
                slider.normalizedValue = newColor.r;
                break;
            case ColorValues.G:
                slider.normalizedValue = newColor.g;
                break;
            case ColorValues.B:
                slider.normalizedValue = newColor.b;
                break;
            case ColorValues.A:
                slider.normalizedValue = newColor.a;
                break;
            default:
                break;
        }
    }

    private void HSVChanged(float hue, float saturation, float value)
    {
        listen = false;
        switch (type)
        {
            case ColorValues.Hue:
                slider.normalizedValue = hue; //1 - hue;
                break;
            case ColorValues.Saturation:
                slider.normalizedValue = saturation;
                break;
            case ColorValues.Value:
                slider.normalizedValue = value;
                break;
            default:
                break;
        }
    }

    private void SliderChanged(float newValue)
    {
        if (listen)
        {
            newValue = slider.normalizedValue;
            //if (type == ColorValues.Hue)
            //    newValue = 1 - newValue;

            hsvpicker.AssignColor(type, newValue);
        }
        listen = true;
    }
}
