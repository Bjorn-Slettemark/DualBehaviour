
public static class DualBehaviourUtils
{
    private static string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static string GenerateRandomString(int length)
    {
        char[] stringChars = new char[length];
        System.Random random = new System.Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = characters[random.Next(characters.Length)];
        }

        return new string(stringChars);
    }
}
