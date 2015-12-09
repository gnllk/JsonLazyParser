using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonParser
{
    /// <summary>
    /// Json lazy parser
    /// </summary>
    public class JElement
    {
        public string Name { get; set; }

        public string OriginalValue { get; set; }

        public string Value
        {
            get
            {
                return RemoveAroundQuotes(OriginalValue);
            }
            set
            {
                OriginalValue = value;
            }
        }

        public override string ToString()
        {
            return Value;
        }

        public JElement()
        {
        }

        public JElement(string json)
        {
            OriginalValue = json;
        }

        public JElement(string name, string value)
        {
            Name = name;
            OriginalValue = value;
        }

        protected List<JElement> m_SubElements = null;

        protected List<JElement> SubElements
        {
            get
            {
                if (null == m_SubElements)
                    m_SubElements = SplitJson(OriginalValue);
                return m_SubElements;
            }
        }

        public List<JElement> Elements
        {
            get
            {
                return SubElements;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(OriginalValue);
            }
        }

        public bool IsArray
        {
            get
            {
                string val = Value;
                if (!string.IsNullOrWhiteSpace(val) && val.Length > 1)
                {
                    return val.StartsWith("[");
                }
                return false;
            }
        }

        protected virtual List<JElement> SplitJson(string json)
        {
            List<JElement> list = new List<JElement>();
            if (!IsJsonSimpleValue(json))
            {
                json = json.Trim().Substring(1, json.Length - 2);
                string name = string.Empty;
                string value = string.Empty;
                int i = 0;
                int index = 0;
                int leftTagCount = 0;
                int rightTagCount = 0;
                string keyValuePair = null;
                foreach (char item in json)
                {
                    if (item == '[' || item == '{')
                        leftTagCount++;
                    else if (item == ']' || item == '}')
                        rightTagCount++;
                    else if (item == ',' && leftTagCount == rightTagCount)
                    {
                        keyValuePair = json.Substring(index, i - index);
                        index = i + 1;
                        var element = CreateLazyElement(keyValuePair);
                        if (null != element) list.Add(element);
                    }
                    i++;
                }
                //the last one
                keyValuePair = json.Substring(index);
                var lastElement = CreateLazyElement(keyValuePair);
                if (null != lastElement) list.Add(lastElement);
            }
            return list;
        }

        public JElement Element(string name)
        {
            return SubElements.Find(e => e.Name == name) ?? new JElement();
        }

        public JElement Element(int index)
        {
            var list = SubElements;
            if (index >= 0 && index < list.Count)
            {
                return list.ElementAt(index);
            }
            return new JElement();
        }

        public JElement this[string name]
        {
            get
            {
                return Element(name);
            }
        }

        public JElement this[int index]
        {
            get
            {
                return Element(index);
            }
        }

        protected bool IsJonObject(string json)
        {
            if (string.IsNullOrEmpty(json))
                return false;
            json = json.Trim();
            if (json.Length < 2)
                return false;
            return (json.First() == '{' && json.Last() == '}');
        }

        protected bool IsJsonArray(string json)
        {
            if (string.IsNullOrEmpty(json))
                return false;
            json = json.Trim();
            if (json.Length < 2)
                return false;
            return (json.First() == '[' && json.Last() == ']');
        }

        protected bool IsJsonSimpleValue(string json)
        {
            if (string.IsNullOrEmpty(json))//null value
                return true;
            json = json.Trim();
            if (json.Length < 2)
                return true;
            return (json.First() != '{' && json.First() != '[');
        }

        protected JElement CreateLazyElement(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            json = json.Trim();
            if (IsJsonSimpleValue(json))
            {
                int tagIndex = json.IndexOf(':');
                if (tagIndex > 1 && json.IndexOf('[', 0, tagIndex) == -1 && json.IndexOf('{', 0, tagIndex) == -1)
                    return new JElement() { Name = RemoveAroundQuotes(json.Substring(0, tagIndex)), OriginalValue = json.Substring(tagIndex + 1) };
                else
                    return new JElement() { OriginalValue = json };
            }
            else
                return new JElement() { OriginalValue = json };
        }

        public virtual string RemoveAroundQuotes(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            if (text.StartsWith(" ") || text.EndsWith(" "))
                text = text.Trim();
            if (text.First() == '"' || text.First() == '\'')
                text = text.Substring(1);
            if (text.Length > 0 && (text.Last() == '"' || text.Last() == '\''))
                text = text.Substring(0, text.Length - 1);
            return text;
        }

        public static JElement Parse(string json)
        {
            return new JElement(json);
        }
    }
}
