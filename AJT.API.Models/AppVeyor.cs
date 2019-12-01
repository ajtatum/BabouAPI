using System;
using Newtonsoft.Json;

namespace AJT.API.Models
{
    public class AppVeyor
    {
        [JsonProperty("eventName")]
        public string EventName { get; set; }

        [JsonProperty("eventData")]
        public EventData EventData { get; set; }
    }

    public class EventData
    {
        [JsonProperty("passed")]
        public bool Passed { get; set; }

        [JsonProperty("failed")]
        public bool Failed { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("started")]
        public string Started { get; set; }

        [JsonProperty("finished")]
        public string Finished { get; set; }

        [JsonProperty("duration")]
        public DateTimeOffset Duration { get; set; }

        [JsonProperty("projectId")]
        public long ProjectId { get; set; }

        [JsonProperty("projectName")]
        public string ProjectName { get; set; }

        [JsonProperty("buildId")]
        public long BuildId { get; set; }

        [JsonProperty("buildNumber")]
        public long BuildNumber { get; set; }

        [JsonProperty("buildVersion")]
        public string BuildVersion { get; set; }

        [JsonProperty("repositoryProvider")]
        public string RepositoryProvider { get; set; }

        [JsonProperty("repositoryScm")]
        public string RepositoryScm { get; set; }

        [JsonProperty("repositoryName")]
        public string RepositoryName { get; set; }

        [JsonProperty("branch")]
        public string Branch { get; set; }

        [JsonProperty("commitId")]
        public string CommitId { get; set; }

        [JsonProperty("commitAuthor")]
        public string CommitAuthor { get; set; }

        [JsonProperty("commitAuthorEmail")]
        public string CommitAuthorEmail { get; set; }

        [JsonProperty("commitDate")]
        public string CommitDate { get; set; }

        [JsonProperty("commitMessage")]
        public string CommitMessage { get; set; }

        [JsonProperty("commitMessageExtended")]
        public string CommitMessageExtended { get; set; }

        [JsonProperty("committerName")]
        public string CommitterName { get; set; }

        [JsonProperty("committerEmail")]
        public string CommitterEmail { get; set; }

        [JsonProperty("isPullRequest")]
        public bool IsPullRequest { get; set; }

        [JsonProperty("pullRequestId")]
        public long PullRequestId { get; set; }

        [JsonProperty("buildUrl")]
        public Uri BuildUrl { get; set; }

        [JsonProperty("notificationSettingsUrl")]
        public Uri NotificationSettingsUrl { get; set; }

        [JsonProperty("messages")]
        public object[] Messages { get; set; }

        [JsonProperty("jobs")]
        public Job[] Jobs { get; set; }
    }

    public class Job
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("passed")]
        public bool Passed { get; set; }

        [JsonProperty("failed")]
        public bool Failed { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("started")]
        public string Started { get; set; }

        [JsonProperty("finished")]
        public string Finished { get; set; }

        [JsonProperty("duration")]
        public DateTimeOffset Duration { get; set; }

        [JsonProperty("messages")]
        public object[] Messages { get; set; }

        [JsonProperty("compilationMessages")]
        public CompilationMessage[] CompilationMessages { get; set; }

        [JsonProperty("artifacts")]
        public Artifact[] Artifacts { get; set; }
    }

    public class Artifact
    {
        [JsonProperty("permalink")]
        public Uri Permalink { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public class CompilationMessage
    {
        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("line")]
        public long Line { get; set; }

        [JsonProperty("column")]
        public long Column { get; set; }

        [JsonProperty("projectName")]
        public string ProjectName { get; set; }

        [JsonProperty("projectFileName")]
        public string ProjectFileName { get; set; }

        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }
    }
}
