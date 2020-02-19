using System.Collections.Generic;
using FluentAssertions;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Octokit;
using Xunit;

namespace ToGithub.Tests
{
    public static class IssueExtensionsTests
    {
        public static class Markdown
        {
            [Fact]
            public static void Convert()
            {
                const string html =
                    @"<div>Some <b>bold</b></div><div>and some <u>underline</u></div><div><i style="">cursive?</i></div><div><i style=""><br></i></div>
<h1><span style=""font-weight: normal;\"">Header1</span></h1><h2><span style=""font-weight: normal;"">Header2</span></h2><h3><span style=""font-weight: normal;"">Header3</span></h3><div><span style=""font-weight: normal;""><br></span></div>
<pre><code><div><span style=""font-weight: normal;"">public static void Main() {}</span></div></code></pre><div><span style=""font-weight: normal;""><br></span></div><div>
<span style=""font-weight: normal;""><br><span>😊</span></span></div><div><span style=""font-weight: normal;""><br></span></div><div><span style=""font-weight: normal;""><br></span></div>";

                new NewIssue(string.Empty)
                    {
                        Body = html
                    }
                    .ToMarkdown()
                    .Body
                    .Should()
                    .Be(@"
Some **bold**

and some <u>underline</u>



# Header1

## Header2

### Header3



```
public static void Main() {}
```



😊




");
            }

            [Fact]
            public static void ConvertNull() =>
                new NewIssue(string.Empty)
                    .ToMarkdown()
                    .Body
                    .Should()
                    .BeNull();
        }

        [Fact]
        public static void AddTaskList()
        {
            var issue = new NewIssue("title") { Body = "body"};
            issue.AddTaskList(new[]
            {
                new WorkItem
                {
                    Fields = new Dictionary<string, object>
                    {
                        ["System.Title"] = "title",
                        ["System.State"] = "Done"
                    }
                }
            }).Body
                .Should()
                .Be(@"body

- [x] title");
            
        }
    }
}