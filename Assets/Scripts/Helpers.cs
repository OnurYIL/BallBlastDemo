using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helpers{

    public static int RandomWeighted(int weight1, int weight2, int weight3)
    {
        int rand = Random.Range(0, (weight1 + weight2 + weight3));

        if (rand <= weight1)
            return 0;
        else if (rand > weight1 && rand <= weight2)
            return 1;
        else return 2;
    }
}
