using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TsemachPod
{
    class Pod
    {
        private string _title;
        private string _description;
        private string _pubDate;
        private string _enclosure;

        private string _titleMain;
        private string _link;
        private string _descriptionMain;
        private string _language;
        private string _copyright;
        private string _lastBuildDate;
        private string _generator;
        private string _webMaster;

       

        public string TitleMain
        {
            get
            {
                return _titleMain;
            }
            set
            {
                _titleMain = value;
            }
        }
        public string Link
        {
            get
            {
                return _link;
            }
            set
            {
                _link = value;
            }
        }

        public string DescriptionMain
        {
            get
            {
                return _descriptionMain;
            }
            set
            {
                _descriptionMain = value;
            }
        }

        public string Language
        {
            get
            {
                return _language;
            }
            set
            {
                _language = value;
            }
        }

        public string CopyRight
        {
            get
            {
                return _copyright;
            }
            set
            {
                _copyright = value;
            }
        }

        public string LastBuildDate
        {
            get
            {
                return _lastBuildDate;
            }
            set
            {
                _lastBuildDate = value;
            }
        }

        public string Generator
        {
            get
            {
                return _generator;
            }
            set
            {
                _generator = value;
            }
        }

        public string WebMaster
        {
            get
            {
                return _webMaster;
            }
            set
            {
                _webMaster = value;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }


        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public string PubDate
        {
            get
            {
                return _pubDate;
            }
            set
            {
                _pubDate = value;
            }
        }


        public string Enclosure
        {
            get
            {
                return _enclosure;
            }
            set
            {
                _enclosure = value;
            }
        }

    }
}
