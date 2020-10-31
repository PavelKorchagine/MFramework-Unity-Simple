using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspPlayerDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FirstPerspPlayer perspPlayer = FindObjectOfType<FirstPerspPlayer>();

        perspPlayer.onPerspStartTranEvent.AddListener((ob) =>
        {
        });

        perspPlayer.onPerspEndTranEvent.AddListener((ob) =>
        {
        });

        perspPlayer.onPerspTraningEvent.AddListener((ob) =>
        {
            Transform[] trs = ob as Transform[];
            var tar = new Vector3(trs[1].eulerAngles.x, trs[0].eulerAngles.y, 0);
            
            // 做小地图
            //var tar = new Vector3(tr.eulerAngles.x, 0, -tr.eulerAngles.y);

            transform.localEulerAngles = tar;


        });
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_2017_1_OR_NEWER
        
#endif
    }
}
