﻿namespace GithubActors
{
    /// <summary>
    /// Begin processing a new Github repository for analysis
    /// </summary>
    public class ProcessRepo
    {
        public ProcessRepo(string repoUri)
        {
            RepoUri = repoUri;
        }

        public string RepoUri { get; }
    }

    public class RepoKey
    {
        public RepoKey(string owner, string repo)
        {
            Repo = repo;
            Owner = owner;
        }

        public string Owner { get; }

        public string Repo { get; }
    }


    public class RetryableQuery
    {
        public RetryableQuery(object query, int allowableTries) : this(query, allowableTries, 0)
        {
        }

        private RetryableQuery(object query, int allowableTries, int currentAttempt)
        {
            AllowableTries = allowableTries;
            Query = query;
            CurrentAttempt = currentAttempt;
        }


        public object Query { get; }

        public int AllowableTries { get; }

        public int CurrentAttempt { get; }

        public bool CanRetry { get { return RemainingTries > 0; } }
        public int RemainingTries { get { return AllowableTries - CurrentAttempt; } }

        public RetryableQuery NextTry()
        {
            return new RetryableQuery(Query, AllowableTries, CurrentAttempt+1);
        }
    }
}
