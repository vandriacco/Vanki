namespace Vanki.Client.Services
{
    public readonly record struct Result(bool Ok, string? Error = null)
    {
        public static Result Success() => new(true, null);
        public static Result Fail(string? error) => new(false, error);
    }
}
