namespace TeamsAppLib.Settings
{
    public static class Retry
    {
        public const int MAXRETRIES = 20;
        public const int DELAYMILLISECONDS = 50;
        public const int MAXDELAYMILLISECONDS = 2000;

        public static readonly string ERROR_MAXRETRIESATTEMPTSEXCEEDED = "Max retry attempts exceeded.";
    }
}
