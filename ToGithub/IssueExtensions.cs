using Octokit;
using ReverseMarkdown;

namespace ToGithub
{
    public static class IssueExtensions
    {
        public static NewIssue ToMarkdown(this NewIssue issue)
        {
            issue.Body =  issue.Body != null
                ? new Converter
                {
                    Config =
                    {
                        GithubFlavored = true,
                        RemoveComments = true,
                        UnknownTags = Config.UnknownTagsOption.PassThrough
                    }
                }.Convert(issue.Body)
                : null;

            return issue;
        }
    }
}