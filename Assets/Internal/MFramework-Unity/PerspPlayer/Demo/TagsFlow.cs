using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagsFlow : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
            return;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;

        var rt = transform.position;
        rt.x = target.position.x;
        rt.y = target.position.y;
        transform.position = rt;
    }
}
