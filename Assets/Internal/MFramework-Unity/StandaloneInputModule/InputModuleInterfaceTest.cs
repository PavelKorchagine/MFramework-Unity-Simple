using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFramework_Unity.InputSystem;

public class InputModuleInterfaceTest : MonoBehaviour, IPointClickHandler
    , IPointDownHandler
    , IPointUpHandler
{
    public void OnPointClick(object par)
    {
        Debug.Log(999999);
    }

    public IPointDownHandler OnPointDown(object par)
    {
        return gameObject.GetComponent<IPointDownHandler>();
    }

    public IPointUpHandler OnPointUp(object par)
    {
        return gameObject.GetComponent<IPointUpHandler>();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
