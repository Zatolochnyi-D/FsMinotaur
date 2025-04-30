namespace ConsoleFacade;

public class ConsoleFacade
{
    public const ConsoleColor defaultBackgroundColor = ConsoleColor.Black;
    public const ConsoleColor defaultForehroundColor = ConsoleColor.White;

    private ConsoleKey consoleKey = ConsoleKey.None;

    public (int, int) ConsoleSize => (Console.WindowWidth, Console.WindowHeight);

    public ConsoleFacade()
    {
        Console.CursorVisible = false;
        Console.BackgroundColor = defaultBackgroundColor;
        Console.ForegroundColor = defaultForehroundColor;
        StartKeyReader();
    }

    private async void StartKeyReader()
    {
        await Task.Run(() => { while (true) consoleKey = Console.ReadKey(true).Key; });
    }

    public void Clear()
    {
        Console.Clear();
    }

    public void WriteChar(int x, int y, char character)
    {
        Console.SetCursorPosition(x, y);
        Console.Write(character);
    }

    public void SetColor(ConsoleColor backgroundColor, ConsoleColor foregroundColor)
    {
        if (Console.BackgroundColor != backgroundColor)
            Console.BackgroundColor = backgroundColor;
        if (Console.ForegroundColor != foregroundColor)
            Console.ForegroundColor = foregroundColor;
    }

    public ConsoleKey ReadKey()
    {
        var key = consoleKey;
        consoleKey = ConsoleKey.None;
        return key;
    }
}
