using System.Globalization;
using System.Transactions;

public class Turtle
{
    private float x;
    private float y;
    private int angle;
    private bool penDown;
    private string penColor;
    private List<Dot> dots;
    private List<Line> lines;
    private List<Figure> figures;
    private Figure currentFigure;
    private List<String> steps;

    public Turtle()
    {
        x = 0;
        y = 0;
        angle = 0;
        penDown = false;
        penColor = "black";
        dots = new List<Dot>();
        lines = new List<Line>();
        figures = new List<Figure>();
        currentFigure = new Figure();
        steps = new List<String>();
    }
        
    private void AddDot(Dot dot)
    {
        if (!dots.Contains(dot))
        {
            dots.Add(dot);
        }
    }

    private void AddLine(Line line)
    {
        AddDot(line.StartDot);
        AddDot(line.EndDot);

        if (!line.StartDot.Equals(line.EndDot))
        {
            if (!lines.Contains(line))
            {
                lines.Add(line);
            }
        }
    }
    public void ProcessCommand(string command)
    {
        steps.Add(command);
        string[] parts = command.Split(' ');
        string action = parts[0].ToLower();

        switch (action)
        {
            case "pu":
                PenUp();
                break;
            case "pd":
                PenDown();
                break;
            case "angle":
                if (parts.Length == 2 && int.TryParse(parts[1], out int newAngle))
                {
                    ChangeAngle(newAngle);
                }
                else
                {
                    Console.WriteLine("‼ Invalid angle command. Usage: angle N ");
                }
                break;
            case "move":
                if (parts.Length == 2 && float.TryParse(parts[1], out float distance))
                {
                    Move(distance);
                }
                else
                {
                    Console.WriteLine("‼ Invalid move command. Usage: move N ");
                }
                break;
            case "color":
                if (parts.Length == 2)
                {
                    ChangePenColor(parts[1]);
                }
                else
                {
                    Console.WriteLine("‼ Invalid color command. Usage: color {colorName} ‼");
                }
                break;
            case "list":
                if (parts.Length ==2)
                {
                    switch(parts[1])
                    {
                        case "lines":
                            DisplayLines();
                            break;
                        case "figures":
                            DisplayFigures();
                            break;
                        case "steps":
                            int i = 0;
                            String message = "";
                            Console.WriteLine("►Your steps:");
                            foreach(var step in steps)
                            {
                                if (i<5)
                                {
                                    message+=step+" ";
                                    i++;
                                } else
                                {
                                    i=0;
                                    Console.WriteLine(message);
                                    message = "";
                                }
                            }
                            break;
                        default:
                            Console.WriteLine("‼ Invalid param for list. Usage: list {lines | figures} ‼");
                            break;
                    }
                }
                break;
            default:
                Console.WriteLine("‼ Unknown command. Available commands: pu, pd, angle N, move N, color {colorName}, list {lines | steps} ‼");
                break;
        }
    }

    public void PrintMenu(){
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║               ░░░░▒▒▒▓▓▌Turtle Graphics Simulator▐▓▓▒▒▒░░░░               ║");
        Console.WriteLine("╠═══════════════════════════════════════════════════════════════════════════╣");
        Console.WriteLine("║   ╟▌ Aviable commands ▐╢                                                  ║");
        Console.WriteLine("╠─► move N: command to change turtle’s position on N steps.                 ║");
        Console.WriteLine("╠─► angle N: command to change turtle’s angle of direction to N degrees.    ║");
        Console.WriteLine("╠─► pd: command to put down the pen.                                        ║");
        Console.WriteLine("╠─► pu: command to put up the pen.                                          ║");
        Console.WriteLine("╠─► color {colorName}: command to change color of the pen to {colorName}.   ║");
        Console.WriteLine("║   Possible values: {black | green}.                                       ║");
        Console.WriteLine("╠─► list {lines | steps}: command to list lines or steps.                   ║");
        Console.WriteLine("╠─► exit: command to exit the program.                                      ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════╝\n");
    }
    private void PenUp()
    {
        penDown = false;
    }

    private void PenDown()
    {
        penDown = true;
    }

    private void ChangeAngle(int newAngle)
    {
        angle += newAngle;
        angle = angle % 360;
    }

    private void Move(float distance)
    {
        if (penDown)
        {
            float newX = (float)Math.Round(x + distance * (float)Math.Cos(angle * Math.PI / 180),2);
            float newY = (float)Math.Round(y + distance * (float)Math.Sin(angle * Math.PI / 180),2);

            Dot startDot = new Dot(x, y);
            Dot endDot = new Dot(newX, newY);
            Line line = new Line(startDot,endDot,penColor);

            currentFigure.AddLine(line);
            bool isIntersect = FindAllIntersections(line);
            if (isIntersect)
            {
                Figure newFigure = new Figure(currentFigure);
                figures.Add(newFigure);
                figures.Last().Display();
                Console.WriteLine("Figure created!");
                currentFigure.Clear();
                figures.Last().Display();
            }
            AddLine(line);
            
        }

        x += distance * (float)Math.Cos(angle * Math.PI / 180);
        y += distance * (float)Math.Sin(angle * Math.PI / 180);

        x = (float)Math.Round(x, 2);
        y = (float)Math.Round(y, 2);
    }

    private void ChangePenColor(string newColor)
    {
        penColor = newColor;
    }
    
    private void DisplayLines()
    {
        Console.WriteLine("► Lines:");
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }

    private void DisplayFigures()
    {
        Console.WriteLine("► Figures:");
        for(int i=0;i<figures.Count;i++)
        {
            Console.WriteLine($"•Figure {i+1}");
            figures[i].Display();
        }
    }
    
    private bool IsIntersect(Line line1, Line line2)
    {
        // if(line1.StartDot.Equals(line2.StartDot) || line1.StartDot.Equals(line2.EndDot) || line1.EndDot.Equals(line2.StartDot) || line1.EndDot.Equals(line2.EndDot))
        //     return false;

        float x1 = line1.StartDot.X;
        float y1 = line1.StartDot.Y;
        float x2 = line1.EndDot.X;
        float y2 = line1.EndDot.Y;
        float x3 = line2.StartDot.X;
        float y3 = line2.StartDot.Y;
        float x4 = line2.EndDot.X;
        float y4 = line2.EndDot.Y;

        float denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
        if (denominator == 0)
            return false;
        

        float x = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / denominator;
        float y = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / denominator;

        Dot intersection = new Dot(x, y);

        bool onLine1 = IsPointOnLine(intersection, line1.StartDot, line1.EndDot);
        bool onLine2 = IsPointOnLine(intersection, line2.StartDot, line2.EndDot);

        if (onLine1 && onLine2)
        {
            return true;
        }
        return false;
        
    }

    private bool FindAllIntersections(Line currLine)
    {
        bool foundIntersection = false;
        for(int i=0;i<lines.Count-1;i++)
        {
            foundIntersection = IsIntersect(currLine,lines[i]);
            if (foundIntersection)
                break;
        }
        return foundIntersection;
    }
    private bool IsPointOnLine(Dot point, Dot lineStart, Dot lineEnd)
    {
        float minX = Math.Min(lineStart.X, lineEnd.X);
        float maxX = Math.Max(lineStart.X, lineEnd.X);
        float minY = Math.Min(lineStart.Y, lineEnd.Y);
        float maxY = Math.Max(lineStart.Y, lineEnd.Y);

        return point.X >= minX && point.X <= maxX && point.Y >= minY && point.Y <= maxY;
    }

   public override string ToString()
    {
        string penState = penDown ? "put down" : "put up";
        return $"■ Current color: {penColor}, pen state: {penState}, location ({x}; {y}), angle: {angle} degrees. ■";
    }
}