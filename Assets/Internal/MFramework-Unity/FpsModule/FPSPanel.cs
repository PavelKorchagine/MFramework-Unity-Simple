using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSPanel : MonoBehaviour
{
    public Text FpsText;
    public bool showFPS = true;
    public bool testMaxFrameRate = false;
    public bool isTargetFrameRate = true;

    private float updateInterval = 0.5f;
    private float accum = 0.0f; // FPS accumulated over the interval
    private float frames = 0f; // Frames drawn over the interval
    private float timeleft; // Left time for current interval

    // Start is called before the first frame update
    private void Start()
    {
        if (!showFPS)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!testMaxFrameRate && isTargetFrameRate)
        {
            Application.targetFrameRate = 60;
        }

        if (FpsText == null)
        {
            try
            {
                FpsText = transform.Find(nameof(FpsText)).GetComponent<Text>();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("警告: " + e);
            }
        }

        if (FpsText == null)
        {
            showFPS = false;
            this.enabled = false;
            return;
        }

        InvokeRepeating("SetType", 0.1f, 0.5f);
    }
    private void LateUpdate()
    {
        // CALCULATE FPS
        if (showFPS)
        {
            timeleft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            ++frames;


            // Interval ended - update GUI text and start new interval
            if (timeleft <= 0.0f)
            {
                // display two fractional digits (f2 format)
                timeleft = updateInterval;
                accum = 0.0f;
                frames = 0f;
            }
        }
        else
        {
            if (FpsText != null)
            {
                FpsText.text = "";

            }
        }
    }

    private void SetType()
    {
        if (FpsText != null && accum > 0f && frames > 0f)
        {
            FpsText.text = "FPS: " + (accum / frames).ToString("f0");
        }
    }
}