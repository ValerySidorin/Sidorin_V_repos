using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3 : MonoBehaviour
{
    public ArrayLayout boardLayout;

    [Header("UI Elements")]
    public RectTransform gameBoard;
    public RectTransform killedBoard;
    public RectTransform slotBoard;
    public Sprite[] pieces;
    public Sprite[] bonuses;

    [Header("Prefabs")]
    public GameObject nodePiece;
    public GameObject killedPiece;
    public GameObject[] slot;

    int width = 9;
    int height = 14;
    int[] fills;
    Node[,] board;

    List<NodePiece> update;
    List<FlippedPieces> flipped;
    List<NodePiece> dead;
    List<KilledPiece> killed;

    System.Random random;

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        fills = new int[width];
        string seed = getRandomSeed();
        random = new System.Random(seed.GetHashCode());
        update = new List<NodePiece>();
        flipped = new List<FlippedPieces>();
        dead = new List<NodePiece>();
        killed = new List<KilledPiece>();

        InitializeBoard();
        VerifyBoard();
        InstantiateBoard();
    }
    void Update()
    {
        List<NodePiece> finishedUpdating = new List<NodePiece>();

        for (int i = 0; i < update.Count; i++)
        {
            NodePiece piece = update[i];
            if (!piece.UpdatePiece())
                finishedUpdating.Add(piece);
        }

        for (int i = 0; i < finishedUpdating.Count; i++)
        {
            NodePiece piece = finishedUpdating[i];
            FlippedPieces flip = getFlipped(piece);
            NodePiece flippedPiece = null;

            int x = piece.index.x;
            fills[x] = Mathf.Clamp(fills[x] - 1, 0, width);

            List<Point> connected = isConnected(piece.index, true);
            bool wasFlipped = (flip != null);

            if (wasFlipped)
            {

                flippedPiece = flip.getOtherPiece(piece);
                AddPoints(ref connected, isConnected(flippedPiece.index, true));
            }

            if (connected.Count == 0)
            {
                if (wasFlipped)
                    FlipPieces(piece.index, flippedPiece.index, false);
            }
            else
            {
                if (connected.Count != 0 && connected.Count != 4)
                {
                    List<Point> killlist = new List<Point>();
                    List<Point> bonuslist = new List<Point>();
                    foreach (Point p in connected)
                    {
                        killlist.Add(p);
                        Node node = GetNodeAtPoint(p);
                        NodePiece nodePiece = node.GetPiece();

                        if (nodePiece.isBonus())
                        {
                            Bonus.AddBonusPoints(height, width, nodePiece.BonusValue(), p, killlist, bonuslist);
                        }
                    }

                    foreach (Point p in killlist)
                    {
                        KillPiece(p);
                        Node node = GetNodeAtPoint(p);
                        NodePiece nodePiece = node.GetPiece();


                        if (nodePiece != null)
                        {
                            nodePiece.gameObject.SetActive(false);
                            dead.Add(nodePiece);
                        }
                        node.SetPiece(null);
                    }
                }

                if (connected.Count == 4)
                {
                    Node bnode = GetNodeAtPoint(connected[0]);
                    NodePiece bpiece = bnode.GetPiece();
                    bnode.SetPiece(bpiece);
                    bpiece.InitializeBonus(Random.Range(1, 4), bonuses);

                    for (int j = 1; j < connected.Count; j++)
                    {
                        KillPiece(connected[j]);
                        Node node = GetNodeAtPoint(connected[j]);
                        NodePiece nodePiece = node.GetPiece();
                        node.SetPiece(nodePiece);

                        if (nodePiece != null)
                        {
                            nodePiece.gameObject.SetActive(false);
                            dead.Add(nodePiece);
                        }
                        node.SetPiece(null);
                    }
                }
                ApplyGravityToBoard();
            }

            flipped.Remove(flip);
            update.Remove(piece);
        }
    }

    void ApplyGravityToBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                Point p = new Point(x, y);
                Node node = GetNodeAtPoint(p);
                int val = GetValueAtPoint(p);

                if (val != 0) continue;

                for (int ny = y - 1; ny >= -1; ny--)
                {
                    Point next = new Point(x, ny);
                    int nextVal = GetValueAtPoint(next);

                    if (nextVal == 0) continue;

                    if (nextVal != -1)
                    {
                        Node got = GetNodeAtPoint(next);
                        NodePiece piece = got.GetPiece();

                        node.SetPiece(piece);
                        update.Add(piece);

                        got.SetPiece(null);
                    }
                    else
                    {
                        int newVal = fillPiece();
                        NodePiece piece;
                        Point fallPoint = new Point(x, -1 - fills[x]);

                        if (dead.Count > 0)
                        {
                            NodePiece revived = dead[0];
                            revived.ResetBonus();
                            revived.gameObject.SetActive(true);
                            piece = revived;
                            
                            dead.RemoveAt(0);

                        }
                        else
                        {
                            GameObject obj = Instantiate(nodePiece, gameBoard);
                            NodePiece n = obj.GetComponent<NodePiece>();
                            piece = n;
                        }
                        piece.Initialize(newVal, p, pieces[newVal - 1]);
                        piece.rect.anchoredPosition = getPositionFromPoint(fallPoint);

                        Node hole = GetNodeAtPoint(p);
                        hole.SetPiece(piece);
                        ResetPiece(piece);
                        fills[x]++;
                    }
                    break;
                }
            }
        }
    }

    FlippedPieces getFlipped(NodePiece p)
    {
        FlippedPieces flip = null;
        for (int i = 0; i < flipped.Count; i++)
        {
            if (flipped[i].getOtherPiece(p) != null)
            {
                flip = flipped[i];
                break;
            }
        }
        return flip;
    }
    

    void InitializeBoard() // Объявление игрового поля
    {
        board = new Node[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                board[x, y] = new Node(boardLayout.rows[y].row[x] ? -1 : fillPiece(), new Point(x, y));
            }
        }
    }

    void VerifyBoard() // Расположение кристаллов на поле
    {
        List<int> remove;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Point p = new Point(x, y);
                int val = GetValueAtPoint(p);
                if (val <= 0) continue;

                remove = new List<int>();

                while (isConnected(p, true).Count > 0)
                {
                    val = GetValueAtPoint(p);

                    if (!remove.Contains(val))
                        remove.Add(val);

                    SetValueAtPoint(p, newValue(ref remove));
                }
            }
        }
    }

    void InstantiateBoard() // Отрисовка доступного для игры поля
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (!boardLayout.rows[y].row[x])
                {
                    GameObject s = Instantiate(slot[0], slotBoard);
                    RectTransform rects = s.GetComponent<RectTransform>();
                    rects.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));

                    if (boardLayout.rows[y].row[x - 1])
                    {
                        s = Instantiate(slot[1], slotBoard);
                        rects = s.GetComponent<RectTransform>();
                        rects.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                    }

                    if (boardLayout.rows[y].row[x + 1])
                    {
                        s = Instantiate(slot[1], slotBoard);
                        rects = s.GetComponent<RectTransform>();
                        rects.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                        rects.localRotation = Quaternion.Euler(0, 0, 180);
                    }

                    if (boardLayout.rows[y + 1].row[x])
                    {
                        s = Instantiate(slot[1], slotBoard);
                        rects = s.GetComponent<RectTransform>();
                        rects.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                        rects.localRotation = Quaternion.Euler(0, 0, 90);
                    }

                    if (boardLayout.rows[y - 1].row[x])
                    {
                        s = Instantiate(slot[1], slotBoard);
                        rects = s.GetComponent<RectTransform>();
                        rects.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                        rects.localRotation = Quaternion.Euler(0, 0, 270);
                    }

                    if (boardLayout.rows[y].row[x - 1] && boardLayout.rows[y - 1].row[x])
                    {
                        s = Instantiate(slot[2], slotBoard);
                        rects = s.GetComponent<RectTransform>();
                        rects.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                    }

                    if (boardLayout.rows[y].row[x + 1] && boardLayout.rows[y - 1].row[x])
                    {
                        s = Instantiate(slot[2], slotBoard);
                        rects = s.GetComponent<RectTransform>();
                        rects.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                        rects.localRotation = Quaternion.Euler(0, 0, 270);
                    }

                    if (boardLayout.rows[y].row[x - 1] && boardLayout.rows[y + 1].row[x])
                    {
                        s = Instantiate(slot[2], slotBoard);
                        rects = s.GetComponent<RectTransform>();
                        rects.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                        rects.localRotation = Quaternion.Euler(0, 0, 90);
                    }

                    if (boardLayout.rows[y].row[x + 1] && boardLayout.rows[y + 1].row[x])
                    {
                        s = Instantiate(slot[2], slotBoard);
                        rects = s.GetComponent<RectTransform>();
                        rects.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                        rects.localRotation = Quaternion.Euler(0, 0, 180);
                    }

                    if (boardLayout.rows[y - 1].row[x] && boardLayout.rows[y + 1].row[x] && boardLayout.rows[y].row[x - 1])
                    {
                        s = Instantiate(slot[3], slotBoard);
                        rects = s.GetComponent<RectTransform>();
                        rects.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                    }

                    if (boardLayout.rows[y - 1].row[x] && boardLayout.rows[y].row[x + 1] && boardLayout.rows[y].row[x - 1])
                    {
                        s = Instantiate(slot[3], slotBoard);
                        rects = s.GetComponent<RectTransform>();
                        rects.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                        rects.localRotation = Quaternion.Euler(0, 0, 270);
                    }

                    if (boardLayout.rows[y - 1].row[x] && boardLayout.rows[y + 1].row[x] && boardLayout.rows[y].row[x + 1])
                    {
                        s = Instantiate(slot[3], slotBoard);
                        rects = s.GetComponent<RectTransform>();
                        rects.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                        rects.localRotation = Quaternion.Euler(0, 0, 180);
                    }

                    if (boardLayout.rows[y + 1].row[x] && boardLayout.rows[y].row[x - 1] && boardLayout.rows[y].row[x + 1])
                    {
                        s = Instantiate(slot[3], slotBoard);
                        rects = s.GetComponent<RectTransform>();
                        rects.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                        rects.localRotation = Quaternion.Euler(0, 0, 90);
                    }

                    if (boardLayout.rows[y + 1].row[x] && boardLayout.rows[y].row[x - 1] && boardLayout.rows[y].row[x + 1] && boardLayout.rows[y - 1].row[x])
                    {
                        s = Instantiate(slot[4], slotBoard);
                        rects = s.GetComponent<RectTransform>();
                        rects.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                    }
                }
                else continue;
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = GetNodeAtPoint(new Point(x, y));
                int val = node.value;
                if (val <= 0) continue;
                GameObject p = Instantiate(nodePiece, gameBoard);
                NodePiece piece = p.GetComponent<NodePiece>();
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                piece.Initialize(val, new Point(x, y), pieces[val - 1]);
                node.SetPiece(piece);
            }
        }
    }

    public void ResetPiece(NodePiece piece)
    {
        piece.ResetPosition();
        update.Add(piece);
    }

    void KillPiece(Point p)
    {
        List<KilledPiece> available = new List<KilledPiece>();

        for (int i = 0; i < killed.Count; i++)
            if (!killed[i].falling)
            {
                available.Add(killed[i]);
            }
                
        KilledPiece set = null;

        if (available.Count > 0)
            set = available[0];
        else
        {
            GameObject kill = GameObject.Instantiate(killedPiece, killedBoard);
            KilledPiece kPiece = kill.GetComponent<KilledPiece>();
            set = kPiece;
            killed.Add(kPiece);
        }

        int val = GetValueAtPoint(p) - 1;

        if (set != null && val >= 0 && val < pieces.Length)
        {
                set.Initialize(pieces[val], getPositionFromPoint(p));
        }
    }

    public void FlipPieces(Point one, Point two, bool main)
    {
        if (GetValueAtPoint(one) < 0) return;

        Node nodeOne = GetNodeAtPoint(one);
        NodePiece pieceOne = nodeOne.GetPiece();

        if (GetValueAtPoint(two) > 0)
        {
            Node nodeTwo = GetNodeAtPoint(two);
            NodePiece pieceTwo = nodeTwo.GetPiece();
            nodeOne.SetPiece(pieceTwo);
            nodeTwo.SetPiece(pieceOne);

            if(main)
                flipped.Add(new FlippedPieces(pieceOne, pieceTwo));

            update.Add(pieceOne);
            update.Add(pieceTwo);
        }
        else
            ResetPiece(pieceOne);

    }


    List<Point> isConnectedSquare(Point p, bool main) // Проверка на соединение квадратом
    {
        List<Point> connectedSquare = new List<Point>();
        int val = GetValueAtPoint(p);
        Point[] directions =
        {
            Point.up,
            Point.right,
            Point.down,
            Point.left
        };

        for (int i = 0; i < 4; i++)
        {
            List<Point> square = new List<Point>();

            int same = 0;
            int next = i + 1;
            if (next >= 4)
                next -= 4;

            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[next]), Point.add(p, Point.add(directions[i], directions[next])) };

            foreach (Point nextTwo in check)
            {
                if (GetValueAtPoint(nextTwo) == val)
                {
                    square.Add(nextTwo);
                    same++;
                }
            }

            if (same > 2)
                AddPoints(ref connectedSquare, square);
        }

        if (main)
        {

            for (int i = 0; i < connectedSquare.Count; i++)
                AddPoints(ref connectedSquare, isConnectedSquare(connectedSquare[i], false));
        }
        return connectedSquare;
    }

    List<Point> isConnectedFiveInLine(Point p, bool main) // Проверка на соединение 5 в ряд
    {
        List<Point> connected = new List<Point>();
        int val = GetValueAtPoint(p);
        Point[] directions =
        {
            Point.up,
            Point.right,
            Point.down,
            Point.left
        };

        for (int i = 0; i < 2; i++)
        {
            List<Point> five = new List<Point>();

            int same = 0;
            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[i + 2]), Point.add(p, Point.mult(directions[i], 2)), Point.add(p, Point.mult(directions[i + 2], 2)) };

            foreach (Point next in check)
            {
                if (GetValueAtPoint(next) == val)
                {
                    five.Add(next);
                    same++;
                }
            }

            if (same > 1)
                AddPoints(ref connected, five);

        }
        return connected;
    }
    List<Point> isConnected(Point p, bool main) // Проверка на соединение 3 в ряд
    {
        List<Point> connected = new List<Point>();
        int val = GetValueAtPoint(p);

        Point[] directions =
        {
            Point.up,
            Point.right,
            Point.down,
            Point.left
        };

        foreach(Point dir in directions)
        {
            List<Point> line = new List<Point>();

            int same = 0;
            for (int i = 1; i < 3; i++)
            {
                Point check = Point.add(p, Point.mult(dir, i));

                if(GetValueAtPoint(check) == val)
                {
                    line.Add(check);
                    same++;
                }
            }

            if (same > 1)
                AddPoints(ref connected, line);
        }

        for (int i = 0; i < 2; i++)
        {
            List<Point> line = new List<Point>();

            int same = 0;
            Point[] check = { Point.add(p, directions[i]), Point.add(p, directions[i + 2]) };

            foreach (Point next in check)
            {
                if (GetValueAtPoint(next) == val)
                {
                    line.Add(next);
                    same++;
                }
            }

            if (same > 1)
                AddPoints(ref connected, line);
        }

        if (main)
        {

            for (int i = 0; i < connected.Count; i++)
                AddPoints(ref connected, isConnected(connected[i], false));
        }

        return connected;
    }

    public static void AddPoints(ref List<Point> points, List<Point> add) // Добавление одного списка точек в другой, исключая повторы
    {
        foreach (Point p in add)
        {
            bool doAdd = true;

            for(int i = 0; i < points.Count; i++)
            {
                if (points[i].Equals(p))
                {
                    doAdd = false;
                    break;
                }
            }

            if (doAdd) points.Add(p);
        }
    }

    void SetValueAtPoint(Point p, int v)
    {
        board[p.x, p.y].value = v;
    }
    int GetValueAtPoint(Point p)
    {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return -1;
        return board[p.x, p.y].value;
    }

    Node GetNodeAtPoint(Point p)
    {
        return board[p.x, p.y];
    }


    int newValue(ref List<int> remove)
    {
        List<int> available = new List<int>();

        for (int i = 0; i < pieces.Length; i++)
            available.Add(i + 1);

        foreach (int i in remove)
            available.Remove(i);

        if (available.Count <= 0) return 0;

        return available[random.Next(0, available.Count)];
    }

    int fillPiece()
    {
        int val = 1;
        val = (random.Next(0, 100) / (100 / pieces.Length)) + 1;
        return val;
    }

    string getRandomSeed()
    {
        string seed = "";
        string acceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";

        for (int i = 0; i < 20; i++)
        {
            seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
        }

        return seed;
    }

    public Vector2 getPositionFromPoint(Point p)
    {
        return new Vector2(32 + (64 * p.x), -32 - (64 * p.y));
    }
}
