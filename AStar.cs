using System;
using System.Collections.Generic;

public class Node
{
    public int x;
    public int y;
    public double g;
    public double h;
    public double f;
    public Node parent;

    public Node(int _x, int _y, double _g, double _h, Node _parent = null)
    {
        x = _x;
        y = _y;
        g = _g;
        h = _h;
        f = _g + _h;
        parent = _parent;
    }
}

public class Program
{
    static double CalculateHeuristic(int currentX, int currentY, int targetX, int targetY)
    {
        return Math.Sqrt(Math.Pow(targetX - currentX, 2) + Math.Pow(targetY - currentY, 2));
    }

    static List<Node> FindPath(int startX, int startY, int targetX, int targetY, List<List<int>> map)
    {
        int mapWidth = map.Count;
        int mapHeight = map[0].Count;

        bool[,] visited = new bool[mapWidth, mapHeight];
        var openSet = new PriorityQueue<Node>((a, b) => a.f.CompareTo(b.f));

        openSet.Enqueue(new Node(startX, startY, 0, CalculateHeuristic(startX, startY, targetX, targetY)));

        while (openSet.Count > 0)
        {
            Node current = openSet.Dequeue();

            visited[current.x, current.y] = true;

            if (current.x == targetX && current.y == targetY)
            {
                List<Node> path = new List<Node>();
                while (current != null)
                {
                    path.Add(current);
                    current = current.parent;
                }
                path.Reverse();
                return path;
            }

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nextX = current.x + dx;
                    int nextY = current.y + dy;

                    if (nextX >= 0 && nextX < mapWidth && nextY >= 0 && nextY < mapHeight &&
                        !visited[nextX, nextY] && map[nextX][nextY] == 0)
                    {
                        double nextG = current.g + CalculateHeuristic(current.x, current.y, nextX, nextY);
                        double nextH = CalculateHeuristic(nextX, nextY, targetX, targetY);
                        double nextF = nextG + nextH;
                        openSet.Enqueue(new Node(nextX, nextY, nextG, nextH, current));
                    }
                }
            }
        }

        return new List<Node>(); // Path not found
    }

    public static void Main(string[] args)
    {
        List<List<int>> map = new List<List<int>> {
            new List<int> { 0, 0, 0, 0, 0 },
            new List<int> { 0, 1, 1, 0, 0 },
            new List<int> { 0, 0, 1, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0 },
            new List<int> { 0, 0, 0, 0, 0 }
        };

        int startX = 0;
        int startY = 0;
        int targetX = 4;
        int targetY = 4;

        List<Node> path = FindPath(startX, startY, targetX, targetY, map);

        if (path.Count == 0)
        {
            Console.WriteLine("Path not found!");
        }
        else
        {
            Console.WriteLine("Path found!");
            foreach (Node node in path)
            {
                Console.Write($"({node.x}, {node.y}) ");
            }
            Console.WriteLine();
        }

        // Cleanup
        foreach (Node node in path)
        {
            node.parent = null;
        }

        Console.ReadLine();
    }
}

// Custom priority queue implementation
public class PriorityQueue<T>
{
    private readonly List<T> elements;
    private readonly Comparison<T> comparison;

    public PriorityQueue(Comparison<T> comparison)
    {
        elements = new List<T>();
        this.comparison = comparison;
    }

    public int Count => elements.Count;

    public void Enqueue(T item)
    {
        elements.Add(item);
        int i = elements.Count - 1;
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (comparison(elements[parent], item) <= 0)
                break;
            elements[i] = elements[parent];
            i = parent;
        }
        elements[i] = item;
    }

    public T Dequeue()
    {
        T frontItem = elements[0];
        int lastIndex = elements.Count - 1;
        T lastItem = elements[lastIndex];
        elements.RemoveAt(lastIndex);

        if (elements.Count > 0)
        {
            int i = 0;
            while (true)
            {
                int childIndex = 2 * i + 1;
                if (childIndex > lastIndex)
                    break;
                int rightChildIndex = childIndex + 1;
                if (rightChildIndex <= lastIndex && comparison(elements[childIndex], elements[rightChildIndex]) > 0)
                    childIndex = rightChildIndex;
                if (comparison(lastItem, elements[childIndex]) <= 0)
                    break;
                elements[i] = elements[childIndex];
                i = childIndex;
            }
            elements[i] = lastItem;
        }

        return frontItem;
    }
}
