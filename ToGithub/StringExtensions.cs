using ReverseMarkdown;
using ReverseMarkdown.Converters;

namespace ToGithub
{
    internal static class StringExtensions
    {
        private static readonly Converter Converter;

        static StringExtensions()
        {
            Converter = new Converter
            {
                Config =
                {
                    GithubFlavored = true,
                    RemoveComments = true,
                    UnknownTags = Config.UnknownTagsOption.PassThrough,
                    TableWithoutHeaderRowHandling = Config.TableWithoutHeaderRowHandlingOption.EmptyRow
                }
            };
            
            Converter.Register("style", new Drop(Converter));
        }
        
        public static string ToMarkdown(this string text) => 
            Converter.Convert(text).Trim('\r', '\n', '\t', ' ');
    }
}