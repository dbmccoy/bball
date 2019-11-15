using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Map : MonoBehaviour
{

    public GameObject hexPrefab;

    int width = 25;
    int height = 25;

    float xOffset = .442f * 2;
    float offRowYOffset = .762f;

    public Dictionary<string, Hex> HexDict = new Dictionary<string, Hex>();

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

                HexDict.Add(x+","+y, go.GetComponent<Hex>());
            }
        }

        GameController.Instance.MapGenFinished();
    }

    public Hex GetHex(int x, int y) {
        Debug.Log(HexDict.ContainsKey(x + "," + y));
        return HexDict[x+","+y];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
