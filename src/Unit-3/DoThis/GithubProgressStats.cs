using System;
using Octokit;

namespace GithubActors
{
    /// <summary>
    /// Used to report on incremental progress.
    /// Immutable.
    /// </summary>
    public class GithubProgressStats
    {
        public int ExpectedUsers { get; }
        public int UsersThusFar { get; }
        public int QueryFailures { get; }
        public DateTime StartTime { get; }
        public DateTime? EndTime { get; }

        public TimeSpan Elapsed => (EndTime ?? DateTime.UtcNow) -StartTime;

        public bool IsFinished => ExpectedUsers == UsersThusFar + QueryFailures;

        public GithubProgressStats()
        {
            StartTime = DateTime.UtcNow;
        }

        private GithubProgressStats(DateTime startTime, int expectedUsers, int usersThusFar, int queryFailures, DateTime? endTime)
        {
            EndTime = endTime;
            QueryFailures = queryFailures;
            UsersThusFar = usersThusFar;
            ExpectedUsers = expectedUsers;
            StartTime = startTime;
        }

        /// <summary>
        /// Add <see cref="delta"/> users to the running total of <see cref="UsersThusFar"/>
        /// </summary>
        public GithubProgressStats UserQueriesFinished(int delta = 1)
        {
            return Copy(usersThusFar: UsersThusFar + delta);
        }

        /// <summary>
        /// Set the <see cref="ExpectedUsers"/> total
        /// </summary>
        public GithubProgressStats SetExpectedUserCount(int totalExpectedUsers)
        {
            return Copy(expectedUsers: totalExpectedUsers);
        }

        /// <summary>
        /// Add <see cref="delta"/> to the running <see cref="QueryFailures"/> total
        /// </summary>
        public GithubProgressStats IncrementFailures(int delta = 1)
        {
            return Copy(queryFailures: QueryFailures + delta);
        }

        /// <summary>
        /// Query is finished! Set's the <see cref="EndTime"/>
        /// </summary>
        public GithubProgressStats Finish()
        {
            return Copy(endTime: DateTime.UtcNow);
        }

        /// <summary>
        /// Creates a deep copy of the <see cref="GithubProgressStats"/> class
        /// </summary>
        public GithubProgressStats Copy(int? expectedUsers = null, int? usersThusFar = null, int? queryFailures = null,
            DateTime? startTime = null, DateTime? endTime = null)
        {
            return new GithubProgressStats(startTime ?? StartTime, expectedUsers ?? ExpectedUsers, usersThusFar ?? UsersThusFar,
                queryFailures ?? QueryFailures, endTime ?? EndTime);
        }
    }

    public class SimilarRepo : IComparable<SimilarRepo>
    {
        public SimilarRepo(Repository repo)
        {
            Repo = repo;
        }

        public Repository Repo { get; }

        public int SharedStarrers { get; set; }

        public int CompareTo(SimilarRepo other)
        {
            return SharedStarrers.CompareTo(other.SharedStarrers);
        }
    }
}