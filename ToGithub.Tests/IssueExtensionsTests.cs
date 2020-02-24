using System.Collections.Generic;
using System.Linq;
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
                    .Be(@"Some **bold**

and some <u>underline</u>



# Header1

## Header2

### Header3



```
public static void Main() {}
```



😊");
            }

            [Fact]
            public static void ConvertNull() =>
                new NewIssue(string.Empty)
                    .ToMarkdown()
                    .Body
                    .Should()
                    .BeNull();

            [Fact]
            public static void Styles()
            {
                var html =
                    "<style>\r\ntable {\n}\n\ntd {\npadding:0px;\ncolor:black;\nfont-size:11.0pt;\nfont-weight:400;\nfont-style:normal;\ntext-decoration:none;\nfont-family:Calibri, sans-serif;\ntext-align:general;\nvertical-align:bottom;\nborder:none;\nwhite-space:nowrap;\n}\n\n.xl65 {\ncolor:#0563C1;\ntext-decoration:underline;\n}\n</style>\n\n\n\n\n<table border=0 cellpadding=0 cellspacing=0 width=434 style=\"border-collapse:collapse;width:434pt;\">\n <colgroup><col width=434 style=\"width:434pt;\">\n </colgroup><tbody><tr height=15 style=\"height:15.0pt;\">\n\n  <td height=15 class=xl65 width=434 style=\"height:15.0pt;width:434pt;\"><a href=\"https://www.microsoft.com/en-us/learning/exam-98-375.aspx\">Exam\n  98-375/Course 40375A: HTML5 App Development Fundamentals</a></td>\n\n </tr>\n</tbody></table>";
                 new NewIssue(string.Empty) { Body = html }
                     .ToMarkdown()
                     .Body
                     .Should()
                     .Be(@"| <!----> |
| --- |
 | [Exam<br>  98-375/Course 40375A: HTML5 App Development Fundamentals](https://www.microsoft.com/en-us/learning/exam-98-375.aspx) |");
            }
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
        
        [Fact]
        public static void AddTaskListEmpty()
        {
            var issue = new NewIssue("title") { Body = "body"};
            issue.AddTaskList(Enumerable.Empty<WorkItem>()).Body
                .Should()
                .Be(@"body");
        }
    }
}