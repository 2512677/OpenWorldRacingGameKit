using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sensor : MonoBehaviour
{
    public List<Collider> collidersInRange = new List<Collider>();
    public List<int> layers = new List<int>();


    public void AddLayer(int layer)
    {
        layers.Add(layer);
    }


	void OnTriggerEnter (Collider col)
    {
	    if(!collidersInRange.Contains(col))
        {
            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i] == col.gameObject.layer)
                {
                    collidersInRange.Add(col);
                }
            }
        }
	}


    void OnTriggerStay(Collider col)
    {
        if (!collidersInRange.Contains(col))
        {
            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i] == col.gameObject.layer)
                {
                    collidersInRange.Add(col);
                }
            }
        }
    }


    void OnTriggerExit(Collider col)
    {
        if (collidersInRange.Contains(col))
        {
            collidersInRange.Remove(col);
        }
    }
}
