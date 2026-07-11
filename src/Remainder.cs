namespace DeviceOfHermes;

/// <summary>A remainder of counting</summary>
public class Remainder(int defaultStack)
{
    /// <summary>Is remains</summary>
    public bool Remains => _current != 0;

    /// <summary>Lose one stack</summary>
    public void Lose()
    {
        if (_current > 0)
        {
            _current -= 1;
        }
    }

    /// <summary>Reset counter</summary>
    public void Reset()
    {
        _current = _default;
    }

    private int _current = defaultStack;

    private readonly int _default = defaultStack;
}
