namespace GithubRepositoryAnalyzer.Config;

public class Enforce
{
    public static void NotNull([ValidatedNotNull] object argument, string argumentName)
    {
        if (argument == null)
        {
            throw new ArgumentNullException(argumentName);
        }
    }
}