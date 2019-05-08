using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangeonGenerator : MonoBehaviour
{

    int dangeonSize = 50;
    public GameObject floor;
    public GameObject wall;

    public GameObject[][] MapObjects;

    // Start is called before the first frame update
    void Start()
    {
        GenerateFloor();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateFloor()
    {
        for (int i = 0; i < dangeonSize; ++i)
        {
            for (int j = 0; j < dangeonSize; ++j)
            {
                if (i == 0 || j == 0 || i == dangeonSize -1 || j == dangeonSize -1)
                {
                    Vector3 position = new Vector3(i, 0, j);
                    Instantiate(wall, position, Quaternion.identity);
                }
                else
                {
                    Vector3 position = new Vector3(i, 0, j);
                    Instantiate(floor, position, Quaternion.identity);
                }
            }
        }
    }
}
