namespace Anet.Core.Akka.Util;

public interface IResult<T> {
    public sealed class Success(T value) : IResult<T>
    {
        public T Value { get; } = value;
    }

    public sealed class Failure(string message) : IResult<T>
    {
        public string Message { get; } = message;
    }
 }