using SwiftlyS2.Shared.Menus;
using System.Threading;

namespace SwiftlyS2.Core.Menus.OptionsBase.Helpers;

internal sealed class DynamicTextUpdater : IDisposable
{
    private readonly TextStyleProcessor processor;
    private readonly Func<string> getSourceText;
    private readonly Func<MenuOptionTextStyle> getTextStyle;
    private readonly Func<float> getMaxWidth;
    private readonly Action<string> setDynamicText;
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly SemaphoreSlim pauseSemaphore;

    private volatile bool disposed;

    public DynamicTextUpdater(
        Func<string> getSourceText,
        Func<MenuOptionTextStyle> getTextStyle,
        Func<float> getMaxWidth,
        Action<string> setDynamicText,
        int updateIntervalMs = 120,
        int pauseIntervalMs = 1000 )
    {
        disposed = false;

        this.getSourceText = getSourceText;
        this.getTextStyle = getTextStyle;
        this.getMaxWidth = getMaxWidth;
        this.setDynamicText = setDynamicText;

        processor = new();
        cancellationTokenSource = new();
        pauseSemaphore = new(0); // Initially paused (0 count), need manual Resume() to start

        _ = Task.Run(() => UpdateLoopAsync(updateIntervalMs, pauseIntervalMs, cancellationTokenSource.Token), cancellationTokenSource.Token);
    }

    ~DynamicTextUpdater()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        // Console.WriteLine($"{GetType().Name} has been disposed.");
        _ = pauseSemaphore.Release(); // Ensure any waiting thread can exit

        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();

        pauseSemaphore.Dispose();
        processor.Dispose();

        disposed = true;
        GC.SuppressFinalize(this);
    }

    public void Pause()
    {
        if (!disposed && pauseSemaphore.CurrentCount > 0)
        {
            pauseSemaphore.Wait();
        }
    }

    public void Resume()
    {
        if (!disposed && pauseSemaphore.CurrentCount == 0)
        {
            _ = pauseSemaphore.Release();
        }
    }

    private async Task UpdateLoopAsync( int intervalMs, int pauseIntervalMs, CancellationToken token )
    {
        while (!token.IsCancellationRequested && !disposed)
        {
            try
            {
                // Wait if paused
                await pauseSemaphore.WaitAsync(token);
                _ = pauseSemaphore.Release();

                await Task.Delay(intervalMs, token);
                var sourceText = getSourceText();
                var textStyle = getTextStyle();
                var maxWidth = getMaxWidth();
                var (styledText, offset) = processor.ApplyHorizontalStyle(sourceText, textStyle, maxWidth);
                setDynamicText(styledText);
                // Console.WriteLine($"sourceText: {sourceText}, textStyle: {textStyle}, maxWidth: {maxWidth}, styledText: {styledText}, offset: {offset}");

                if (offset == 0)
                {
                    await Task.Delay(pauseIntervalMs, token);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
            }
        }
    }
}