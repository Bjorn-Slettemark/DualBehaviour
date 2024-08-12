using System;
using System.Threading.Tasks;
using Firebase.Database;

public static class FirebaseHelper
{
    public static async Task<DataSnapshot> WithTimeout(Task<DataSnapshot> task, int timeoutMilliseconds = 5000)
    {
        var timeoutTask = Task.Delay(timeoutMilliseconds);
        var completedTask = await Task.WhenAny(task, timeoutTask);
        if (completedTask == timeoutTask)
        {
            throw new TimeoutException($"Firebase operation timed out after {timeoutMilliseconds}ms");
        }
        return await task;
    }

    public static async Task WithTimeout(Task task, int timeoutMilliseconds = 5000)
    {
        var timeoutTask = Task.Delay(timeoutMilliseconds);
        var completedTask = await Task.WhenAny(task, timeoutTask);
        if (completedTask == timeoutTask)
        {
            throw new TimeoutException($"Firebase operation timed out after {timeoutMilliseconds}ms");
        }
        await task;
    }
}