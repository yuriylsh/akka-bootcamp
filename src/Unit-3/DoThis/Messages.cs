namespace GithubActors
{
    public class ProcessRepo
    {
        public string RepoUri { get; }

        public ProcessRepo(string repoUri) => RepoUri = repoUri;
    }

    public class RepoKey
    {
        public string Owner { get; }

        public string Repo { get; }

        public RepoKey(string owner, string repo)
        {
            Repo = repo;
            Owner = owner;
        }
    }


    public class RetryableQuery
    {
        public object Query { get; }

        public int AllowableTries { get; }

        public int CurrentAttempt { get; }

        public bool CanRetry => RemainingTries > 0;

        public int RemainingTries => AllowableTries - CurrentAttempt;

        public RetryableQuery(object query, int allowableTries) : this(query, allowableTries, 0)
        {
        }

        private RetryableQuery(object query, int allowableTries, int currentAttempt)
        {
            AllowableTries = allowableTries;
            Query = query;
            CurrentAttempt = currentAttempt;
        }

        public RetryableQuery NextTry() => new RetryableQuery(Query, AllowableTries, CurrentAttempt+1);
    }
}
