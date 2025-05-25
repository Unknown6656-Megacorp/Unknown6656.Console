using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Drawing;
using System.Text;
using System.Linq;
using System;

using Unknown6656.Terminal.Markdown;
using Unknown6656.Generics.Text;
using Unknown6656.Generics;
using Unknown6656.Runtime;

namespace Unknown6656.Terminal;


public static unsafe partial class Console
{
    [SupportedOSPlatform(OS.LNX)]
    [SupportedOSPlatform(OS.MAC)]
    [SupportedOSPlatform(OS.MACC)]
    public static (ConsoleColor Foreground, ConsoleColor Background) WindowFrameColors
    {
        set => SetWindowFrameColor(value.Foreground, value.Background);
    }

    public static ConsoleWindowProgressbar? WindowProgressbar
    {
        set => SetWindowProgressbar(value);
    }

    public static bool IsWindowFramed
    {
        // get maybe via private DEC mode?
        set => SetVT520Bit(111, value);
    }

    public static ConsoleColor WindowFrameBackgroundColor
    {
        set => SetWindowFrameColor(value);
    }


#pragma warning disable CA1416 // Validate platform compatibility
    /// <summary>
    /// Sets the foreground color of the window frame.
    /// </summary>
    /// <param name="background">The terminal window frame background color. The color must be a system color (i.e., one of <see cref="ConsoleColor"/>'s static members, or an instance of <see cref="sysconsolecolor"/>).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if a <paramref name="background"/> is not a system color.</exception>
    public static void SetWindowFrameColor(ConsoleColor background) => SetWindowFrameColor(ConsoleColor.Black, background);
#pragma warning restore CA1416

    /// <summary>
    /// Sets the foreground and background colors of the window frame.
    /// This method is currently only supported on Linux and macOS.
    /// </summary>
    /// <param name="foreground">The terminal window frame foreground color. The color must be a system color (i.e., one of <see cref="ConsoleColor"/>'s static members, or an instance of <see cref="sysconsolecolor"/>).</param>
    /// <param name="background">The terminal window frame background color. The color must be a system color (i.e., one of <see cref="ConsoleColor"/>'s static members, or an instance of <see cref="sysconsolecolor"/>).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if a <paramref name="foreground"/> and/or <paramref name="background"/> is not a system color.</exception>
    [SupportedOSPlatform(OS.LNX)]
    [SupportedOSPlatform(OS.MAC)]
    [SupportedOSPlatform(OS.MACC)]
    public static void SetWindowFrameColor(ConsoleColor foreground, ConsoleColor background)
    {
        if (background.ToSystemColor() is not sysconsolecolor bg)
            throw new ArgumentOutOfRangeException(nameof(background), $"The specified background color '{background}' is not supported.");
        else if (foreground.ToSystemColor() is not sysconsolecolor fg)
            throw new ArgumentOutOfRangeException(nameof(foreground), $"The specified foreground color '{foreground}' is not supported.");
        else
            Write($"{_CSI}2;{(int)fg};{(int)bg},|");
    }

    public static void SetWindowProgressbar(ConsoleWindowProgressbar? progressbar)
    {
        progressbar ??= ConsoleWindowProgressbar.Hidden;

        Write($"{_OSC}9;4;{progressbar._state};{progressbar._progress}{_BEL}");
    }
}

public sealed class ConsoleWindowProgressbar
{
    public static ConsoleWindowProgressbar Hidden { get; } = new(0,0);
    public static ConsoleWindowProgressbar Indeterminate { get; } = new(3, 0);
    public static ReadOnlyIndexer<double, ConsoleWindowProgressbar> Progress { get; } = new(x => new(1, (int)(double.Clamp(x, 0, 1) * 100)));
    public static ReadOnlyIndexer<double, ConsoleWindowProgressbar> Warning { get; } = new(x => new(4, (int)(double.Clamp(x, 0, 1) * 100)));
    public static ReadOnlyIndexer<double, ConsoleWindowProgressbar> Error { get; } = new(x => new(2, (int)(double.Clamp(x, 0, 1) * 100)));

    internal readonly int _state;
    internal readonly int _progress;


    private ConsoleWindowProgressbar(int state, int progress)
    {
        _state = state;
        _progress = progress;
    }
}



