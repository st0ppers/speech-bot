using System.Runtime.CompilerServices;
using CSharpFunctionalExtensions;

namespace SpeechDiscordBot.Extensions;

public static class ResultExtensions
{
    public static Maybe<string> ToMaybe(this string value) =>
        string.IsNullOrWhiteSpace(value) ? Maybe.None : Maybe.From(value);

    // public static Maybe<T> Tap<T>(this Maybe<T> maybe, Action<T> action)
    // {
    //     if (maybe.HasValue)
    //     {
    //         action(maybe.Value);
    //         return maybe.Value;
    //     }
    //
    //     return Maybe<T>.None;
    // }

    public static async Task<Maybe<T>> Check<T>(this Maybe<T> maybe, Func<T, bool> predicate, Func<T, Task> func)
    {
        if (!maybe.HasValue)
        {
            return Maybe<T>.None;
        }

        if (!predicate(maybe.Value))
        {
            return maybe;
        }

        await func(maybe.Value);
        return maybe;
    }

    public static async Task<Maybe<T>> Tap<T>(this Maybe<T> maybe, Func<T, Task> action)
    {
        if (maybe.HasValue)
        {
            await action(maybe.Value);
            return maybe.Value;
        }

        return Maybe<T>.None;
    }

    public static async Task<Maybe<T>> Tap<T>(this Task<Maybe<T>> maybeTask, Func<T, Task> action)
    {
        var maybe = await maybeTask.DefaultAwait();
        if (maybe.HasValue)
        {
            await action(maybe.Value);
            return maybe.Value;
        }

        return Maybe<T>.None;
    }


    // public static async Task<Result<T, TE>> ToResult<T, TE>(this Task<Maybe<T>> maybeTask, TE exception) where TE : Exception
    // {
    //     var maybe = await maybeTask.DefaultAwait();
    //     if (maybe.HasValue)
    //     {
    //         return Result.Success<T, TE>(maybe.Value);
    //     }
    //
    //     return Result.Failure<T, TE>(exception);
    // }

    private static ConfiguredTaskAwaitable<T> DefaultAwait<T>(this Task<T> task) =>
        task.ConfigureAwait(Result.Configuration.DefaultConfigureAwait);
}