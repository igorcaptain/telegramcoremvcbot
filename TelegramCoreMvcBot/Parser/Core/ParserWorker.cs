using System;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using System.Collections.Generic;

namespace Parser.Core
{
    class ParserWorker<T> where T : class
    {
        IParser<T> parser;
        IParserSettings parserSettings;
        HtmlLoader loader;
        bool isActive;
        #region Properties
        public event Action<object, T> OnNewData;
        public event Action<object> OnCompleted;
        public IParser<T> Parser
        {
            get
            {
                return parser;
            }
            set
            {
                parser = value;
            }
        }
        public IParserSettings Settings
        {
            get
            {
                return parserSettings;
            }
            set
            {
                parserSettings = value;
                loader = new HtmlLoader(value);
            }
        }
        public bool IsActive
        {
            get
            {
                return isActive;
            }
        }
        #endregion
        public ParserWorker(IParser<T> parser)
        {
            this.parser = parser;
        }
        public ParserWorker(IParser<T> parser, IParserSettings parserSetings) : this(parser)
        {
            //this.parserSettings = parserSetings;
            Settings = parserSetings;
        }
        public void Start()
        {
            isActive = true;
            Worker();
        }
        public void Abort()
        {
            isActive = false;
        }
        private async void Worker()
        {
            if(!isActive)
            {
                OnCompleted?.Invoke(this);
                return;
            }
            var source = await loader.GetSourceOfPage();
            var domParser = new HtmlParser();
            var document = await domParser.ParseDocumentAsync(source);

            var result = parser.Parser(document);
            

            OnNewData?.Invoke(this, result);

            OnCompleted?.Invoke(this);
            isActive = false;
        }
        public async Task<T> GetGroups()
        {
            var source = await loader.GetSourceOfPage();
            var domParser = new HtmlParser();
            var document = await domParser.ParseDocumentAsync(source);
            var result = parser.Parser(document);

            return result;
        }

        public async Task<string> GetDate()
        {
            dynamic result = await GetGroups();
            return result[0].Date;
        }
    }
}
