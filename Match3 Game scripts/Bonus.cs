using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Bonus
{
    
    public static void InitializeBonus(this NodePiece piece, int bv, Sprite[] bonus)
    {
        piece.img = piece.GetComponent<Image>();
        piece.rect = piece.GetComponent<RectTransform>();

        piece.bonusval = bv;
        piece.img.sprite = bonus[piece.value - 1];
    }

    public static bool isBonus(this NodePiece piece)
    {
        return piece.bonusval != 0;
    }

    public static void ResetBonus(this NodePiece piece)
    {
        piece.bonusval = 0;
    }

    public static int BonusValue(this NodePiece piece)
    {
        return piece.bonusval;
    }

    public static void AddBonusPoints(int height, int width, int i, Point p, List<Point> kill, List<Point> bonus)
    {
        switch (i)
        {
            case 1:
                for (int j = 1; j < height - 1; j++)
                {
                    bonus.Add(new Point(p.x, j));
                }
                Match3.AddPoints(ref kill, bonus);
                break;

            case 2:
                for (int k = 1; k < width - 1; k++)
                {
                    bonus.Add(new Point(k, p.y));
                }
                Match3.AddPoints(ref kill, bonus);
                break;

            case 3:
                for (int j = 1; j < height - 1; j++)
                {
                    bonus.Add(new Point(p.x, j));
                }
                Match3.AddPoints(ref kill, bonus);

                for (int k = 1; k < width - 1; k++)
                {
                    bonus.Add(new Point(k, p.y));
                }
                Match3.AddPoints(ref kill, bonus);
                break;
        }
    }
}
