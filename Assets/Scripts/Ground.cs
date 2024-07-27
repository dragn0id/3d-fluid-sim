using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public SPH sph;
    void Update()
    {
        transform.localScale = new Vector3(sph.boxSize.x, 1, sph.boxSize.z);
    }
}
