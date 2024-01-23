namespace backend_pastebook_capstone.Services
{
    public class RandomGenerator
    {
        public static string GenerateRandomWord()
        {
            Random random = new Random();
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] randomWord = new char[6];

            for (int i = 0; i < 6; i++)
            {
                randomWord[i] = characters[random.Next(characters.Length)];
            }

            return new string(randomWord);
        }
    }
}
