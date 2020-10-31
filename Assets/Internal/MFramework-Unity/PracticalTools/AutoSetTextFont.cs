using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("AutoSetTextFont")]
public class AutoSetTextFont : MonoBehaviour
{
    public Font font;
    public Text[] texts;

    [ContextMenu("SetFont")]
    public void SetFont()
    {
        texts = GetComponentsInChildren<Text>(true);
        foreach (var item in texts)
        {
            item.font = font;
        }

#if UNITY_EDITOR

        UnityEditor.EditorUtility.SetDirty(this.gameObject);

#endif
    }

    public void SetFont3()
    {
        
    }


}
