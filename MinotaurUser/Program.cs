using static Minotaur.Window.Window;
using static Minotaur.Window.Page;
using static Minotaur.GUI.TextBox;
using static Minotaur.GUI.Rect;
using static Minotaur.GUI.Button;
using static Minotaur.Colors;
using Microsoft.FSharp.Core;
using Minotaur;

namespace MinotaurUser;

class Program
{
    private static Window window = new Window(30);

    static void Main(string[] args)
    {
        var title = CreateCenteredText(white, black, 0, 3, "+== Title ==+");

        var switchText = CreateCenteredText(white, black, 0, 10, """
                                                                 ┌────────────┐
                                                                 │ To page 2  │
                                                                 └────────────┘
                                                                 """);
        Converter<Unit, Unit> switchFunction = (_) => { window.SetPageIndex(1); return null!; };
        var switchButton = new Button(switchText, switchFunction);

        var exitText = CreateCenteredText(white, black, 0, 16, """
                                                               ┌────────────┐
                                                               │    Exit    │
                                                               └────────────┘
                                                               """);
        Converter<Unit, Unit> exitFunction = (_) => { Environment.Exit(0); return null; };
        var exitButton = new Button(exitText, exitFunction);

        var page = new Page();
        page.AddStaticElement(title);
        page.AddSelectableElement(switchButton);
        page.AddSelectableElement(exitButton);

        var secondPage = Examples.getExamplePage(window);

        window.AddPage(page);
        window.AddPage(secondPage);

        window.MainLoop();
    }

    private static TextBox CreateCenteredText(Color foreground, Color background, int x, int y, string content)
    {
        return textBox(Anchor.Center, Anchor.TopCenter, TextAlignment.Middle, FSharpOption<Rect>.Some(window.Rect), foreground, background, x, y, content);
    }
}
