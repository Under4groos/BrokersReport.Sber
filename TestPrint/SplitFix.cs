namespace TestPrint
{
    public static class SplitFix
    {
        public static List<string> Fix(this List<string> lines, string end)
        {
            int maxLength = lines.Select(l => l.Length).Max();

            for (int i = 0; i < lines.Count; i++)
            {
                // Выравниваем строки по правому краю
                lines[i] = lines[i].PadRight(maxLength) + end;
            }

            return lines;

        }
    }
}
