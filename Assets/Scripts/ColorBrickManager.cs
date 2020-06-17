using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBrickManager : MonoBehaviour {

	public List<Color> colors;
    public List<int> CountsBrickColor;

    public static ColorBrickManager Singlton
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (Singlton == null)
            Singlton = this;
    }


    public Color GetColorBrick(int count)
    {
        Color color = colors[colors.Count-1];

        for (int i = 0; i < CountsBrickColor.Count; i++)
        {
            if (CountsBrickColor[i] > count && count > 4)
            {
                color = colors[i - 1];
                break;
            }
        }

        return color;
    }
}
