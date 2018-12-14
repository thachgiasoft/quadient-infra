﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.WebFramework.Security
{
    /// <summary>
    /// Validates Html only letting a predefined set of Elemtnts/Attributes remain.
    /// </summary>
    public class HtmlSanitizer
    {
        // Original list courtesy of Robert Beal :
        // http://www.robertbeal.com/37/sanitising-html

        private static readonly Dictionary<string, string[]> ValidHtmlTags =
            new Dictionary<string, string[]>
        {
            {"p", new[]          {"style", "class", "align"}},
            {"div", new[]        {"style", "class", "align"}},
            {"span", new[]       {"style", "class"}},
            {"br", new[]         {"style", "class"}},
            {"hr", new[]         {"style", "class"}},
            {"label", new[]      {"style", "class"}},
 
            {"h1", new[]         {"style", "class"}},
            {"h2", new[]         {"style", "class"}},
            {"h3", new[]         {"style", "class"}},
            {"h4", new[]         {"style", "class"}},
            {"h5", new[]         {"style", "class"}},
            {"h6", new[]         {"style", "class"}},
 
            {"font", new[]       {"style", "class", "color", "face", "size"}},
            {"strong", new[]     {"style", "class"}},
            {"b", new[]          {"style", "class"}},
            {"em", new[]         {"style", "class"}},
            {"i", new[]          {"style", "class"}},
            {"u", new[]          {"style", "class"}},
            {"strike", new[]     {"style", "class"}},
            {"ol", new[]         {"style", "class"}},
            {"ul", new[]         {"style", "class"}},
            {"li", new[]         {"style", "class"}},
            {"blockquote", new[] {"style", "class"}},
            {"code", new[]       {"style", "class"}},
 
            {"a", new[]          {"style", "class", "href", "title"}},
            {"img", new[]        {"style", "class", "src", "height", "width",
                "alt", "title", "hspace", "vspace", "border"}},
 
            {"table", new[]      {"style", "class"}},
            {"thead", new[]      {"style", "class"}},
            {"tbody", new[]      {"style", "class"}},
            {"tfoot", new[]      {"style", "class"}},
            {"th", new[]         {"style", "class", "scope"}},
            {"tr", new[]         {"style", "class"}},
            {"td", new[]         {"style", "class", "colspan","align"}},
 
            {"q", new[]          {"style", "class", "cite"}},
            {"cite", new[]       {"style", "class"}},
            {"abbr", new[]       {"style", "class"}},
            {"acronym", new[]    {"style", "class"}},
            {"del", new[]        {"style", "class"}},
            {"ins", new[]        {"style", "class"}}
        };

        /// <summary>
        /// Takes raw HTML input and cleans against a whitelist
        /// </summary>
        /// <param name="source">Html source</param>
        /// <returns>Clean output</returns>
        public static string SanitizeHtml(string source)
        {
            HtmlDocument html = GetHtml(source);
            if (html == null) return String.Empty;

            // All the nodes
            HtmlNode allNodes = html.DocumentNode;

            // Select whitelist tag names
            string[] whitelist = (from kv in ValidHtmlTags
                                  select kv.Key).ToArray();

            // Scrub tags not in whitelist
            CleanNodes(allNodes, whitelist);

            // Filter the attributes of the remaining
            foreach (KeyValuePair<string, string[]> tag in ValidHtmlTags)
            {
                var tag1 = tag;
                var nodes = (from n in allNodes.DescendantsAndSelf()
                             where n.Name == tag1.Key
                             select n);

                if (nodes == null) continue;

                foreach (HtmlAttribute a in from n in nodes where n.HasAttributes select n.Attributes.ToArray() into attr from a in attr select a)
                {
                    if (!tag.Value.Contains(a.Name))
                    {
                        a.Remove(); // Wasn't in the list
                    }
                    else
                    {
                        // AntiXss
                        a.Value = System.Web.HttpUtility.HtmlEncode(a.Value);
                    }
                }
            }

            return allNodes.InnerHtml;
        }

        /// <summary>
        /// Takes a raw source and removes all HTML tags
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string StripHtml(string source)
        {
            source = SanitizeHtml(source);

            // No need to continue if we have no clean Html
            if (String.IsNullOrEmpty(source))
                return String.Empty;

            var html = GetHtml(source);
            var result = new StringBuilder();

            // For each node, extract only the innerText
            foreach (HtmlNode node in html.DocumentNode.ChildNodes)
                result.Append(node.InnerText);

            return result.ToString();
        }

        /// <summary>
        /// Recursively delete nodes not in the whitelist
        /// </summary>
        private static void CleanNodes(HtmlNode node, string[] whitelist)
        {
            if (node.NodeType == HtmlNodeType.Element)
            {
                if (!whitelist.Contains(node.Name))
                {
                    node.ParentNode.RemoveChild(node);
                    return; // We're done
                }
            }

            if (node.HasChildNodes)
                CleanChildren(node, whitelist);
        }

        /// <summary>
        /// Apply CleanNodes to each of the child nodes
        /// </summary>
        private static void CleanChildren(HtmlNode parent, string[] whitelist)
        {
            for (int i = parent.ChildNodes.Count - 1; i >= 0; i--)
                CleanNodes(parent.ChildNodes[i], whitelist);
        }

        /// <summary>
        /// Helper function that returns an HTML document from text
        /// </summary>
        private static HtmlDocument GetHtml(string source)
        {
            var html = new HtmlDocument
            {
                OptionFixNestedTags = true,
                OptionAutoCloseOnEnd = true,
                OptionDefaultStreamEncoding = Encoding.UTF8
            };

            html.LoadHtml(source);

            return html;
        }
    }
}
