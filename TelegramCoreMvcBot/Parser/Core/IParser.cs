using System;
using AngleSharp.Html.Dom;

namespace Parser.Core
{
    interface IParser<T> where T : class
    {
        T Parser(IHtmlDocument document);
    }
}
