using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text.Json;

class Program
{
    static int WIDTH = Console.WindowWidth;
    static int HEIGHT = Console.WindowHeight - 3; // -3 because the readline at the bottom takes up some space

    static float GRAVITY = 0.2f;
    static float DRAG = 0.15f;
    static int SCORE = 3;

    static Object container = new Object();
    static Dropper dropper = new Dropper();

    static List<Object> Objects = new List<Object>();
    static List<Drop> Drops = new List<Drop>();

    static bool AABB_Point(int pointX, int pointY, int targetX, int targetY, int width, int height)
    {
        int right = targetX + width - 1;
        int left = targetX;
        int top = targetY;
        int bottom = targetY + height - 1;

        return pointX >= left && pointX <= right && pointY >= top && pointY <= bottom;
    }

    static bool AABB(int aX, int aY, int bX, int bY, int aWidth, int aHeight, int bWidth, int bHeight)
    {
        int aRight = aX + aWidth - 1;
        int aLeft = aX;
        int aTop = aY;
        int aBottom = aY + aHeight - 1;

        int bRight = bX + bWidth - 1;
        int bLeft = bX;
        int bTop = bY;
        int bBottom = bY + bHeight - 1;

        return aLeft <= bRight && aRight >= bLeft && aTop <= bBottom && aBottom >= bTop;
    }

    static void Render()
    {
        Console.Clear();

        string output = "";

        for (int y = 0; y < HEIGHT; y++)
        {
            if (y == 0)
            {
                output += "SCORE: " + SCORE + "\n";
                continue;
            }

            for (int x = 0; x < WIDTH; x++)
            {
                bool isObject = false;

                foreach (Object obj in Objects)
                {
                    if (AABB_Point(x, y, obj.x, obj.y, obj.width, obj.height))
                    {
                        isObject = true;

                        break;
                    }
                }

                if (!isObject)
                {
                    // You can add logic that checks other things, for example the player
                }

                if (x == WIDTH - 1)
                {
                    output += "|";
                }
                else if (y == HEIGHT - 1)
                {
                    output += "-";
                }
                else
                {
                    output += isObject ? "O" : " ";
                }
            }

            output += "\n";
        }

        Console.WriteLine(output);
    }

    static void Input()
    {
        var input = Console.ReadKey();

        switch (input.Key)
        {
            case ConsoleKey.Spacebar:
                Drop drop = new Drop();

                drop.x = dropper.x + Convert.ToInt32(dropper.width / 2);
                drop.y = dropper.y + dropper.height;
                drop.xvel = dropper.direction * dropper.speed;
                drop.yvel = (GRAVITY);

                Drops.Add(drop);
                Objects.Add(drop);
                
                break;
            default:
                break;
        }
    }

    static void Loop()
    {
        long lastTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

        while (true)
        {
            WIDTH = Console.WindowWidth;
            HEIGHT = Console.WindowHeight - 3; // -3 because the readline at the bottom takes up some space type shi

            container.x = Convert.ToInt32(WIDTH / 2) - Convert.ToInt32(container.width / 2);
            container.y = HEIGHT - container.height - 1;

            long unixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
            long delta = unixTimestamp - lastTimestamp;

            dropper.y = 2;
            dropper.x += Convert.ToInt32(dropper.direction * dropper.speed);

            if (Console.KeyAvailable)
            {
                Input();
            }

            if (dropper.x + dropper.width >= WIDTH)
            {
                dropper.direction = -1;
            }
            else if (dropper.x <= 0)
            {
                dropper.direction = 1;
            }

            foreach (Drop drop in Drops.ToArray())
            {
                drop.xvel -= drop.xvel * DRAG;
                drop.yvel += GRAVITY;

                drop.x += Convert.ToInt32(drop.xvel);
                drop.y += Convert.ToInt32(drop.yvel);

                if (AABB(drop.x, drop.y, container.x, container.y, drop.width, drop.height, container.width, container.height))
                {
                    Drops.Remove(drop);
                    Objects.Remove(drop);

                    SCORE++;
                }
                else if (drop.y + drop.height >= HEIGHT)
                {
                    Drops.Remove(drop);
                    Objects.Remove(drop);

                    SCORE = Math.Max(SCORE - 1, 0);

                    if (SCORE == 0)
                    {
                        Objects.Clear();

                        // You lost text:
                        Console.WriteLine(" __     __           _           _   _ \r\n \\ \\   / /          | |         | | | |\r\n  \\ \\_/ /__  _   _  | | ___  ___| |_| |\r\n   \\   / _ \\| | | | | |/ _ \\/ __| __| |\r\n    | | (_) | |_| | | | (_) \\__ \\ |_|_|\r\n    |_|\\___/ \\__,_| |_|\\___/|___/\\__(_)\r\n                                       \r\n                                       ");

                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                }
            }

            Render();

            lastTimestamp = unixTimestamp;

            Thread.Sleep(50);
        }
    }

    static void Main()
    {
        Console.WriteLine(WIDTH);
        Console.WriteLine(HEIGHT);

        container.height = 2;
        container.width = 10;

        dropper.height = 3;
        dropper.width = 3;

        Objects.Add(container);
        Objects.Add(dropper);

        Loop();
    }
}

class Object
{
    public int x = 0;
    public int y = 0;
    public int height = 2;
    public int width = 2;
}

class Dropper : Object
{
    public int direction = 1;
    public int speed = 3;
}

class Drop : Object
{
    public float xvel = 1;
    public float yvel = 3;
}
