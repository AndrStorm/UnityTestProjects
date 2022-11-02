using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxEplosion : MonoBehaviour
{
    public GameObject box;
    public GameObject fracturedBox;
    public GameObject vfx;
    public float explosionForceMin;
    public float explosionForceMax;
    public float explosionForceRad;

    bool exploded = false;


    private void Update()
    {
        if (Input.GetKey(KeyCode.E) && !exploded)
        {
            Explode(box,fracturedBox,vfx,explosionForceMin,explosionForceMax,explosionForceRad);
            exploded = true;
        }
        if (Input.GetKey(KeyCode.R) && exploded)
        {
            Reset(box,fracturedBox);
            exploded = false;
        }
    }

    void Explode(GameObject obj, GameObject fractObject, GameObject vfx, float min, float max, float rad)
    {
        obj.SetActive(false);
        fractObject.SetActive(true);

        GameObject fractObj = Instantiate(fractObject) as GameObject;
        fractObj.transform.position = obj.transform.position;
        fractObj.transform.SetParent(gameObject.transform);

        foreach (Transform t in fractObj.transform)
        {
            var rb = t.gameObject.GetComponent<Rigidbody>();

            if (rb != null)
            rb.AddExplosionForce(Random.Range(min, max), t.position, rad);

            StartCoroutine(Shrink(t,2));
               
        }

        if (vfx != null)
        { 
            GameObject explosionVFX = Instantiate(vfx) as GameObject;
            explosionVFX.transform.position = obj.transform.position;
            Destroy(explosionVFX, 3);
        }
            

        Destroy(fractObj, 10);
        
    }

    private void Reset(GameObject obj, GameObject fractObject)
    {
        obj.SetActive(true);
        fractObject.SetActive(false);
    }
    
    IEnumerator Shrink(Transform t, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 scale = t.localScale;

        while (t.localScale.x >= 0)
        {
            scale -= Vector3.one*1f;
            t.localScale = scale;
            yield return new WaitForSeconds(0.05f);
        }

    }




}
