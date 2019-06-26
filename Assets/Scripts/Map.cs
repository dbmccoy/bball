using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{

    public GameObject hexPrefab;

    int width = 22;
    int height = 11;

    float xOffset = .442f * 2;
    float offRowYOffset = .762f;

    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {

                float xval = x * xOffset;

                if(y % 2 != 0) {
                    xval += (xOffset / 2);
                }

                var go = Instantiate(hexPrefab, new Vector3(xval,0,y * offRowYOffset), Quaternion.identity);
                go.transform.name = "Hex_" + x + "_" + y;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
