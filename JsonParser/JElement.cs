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
        /// <summary>
        /// name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// origunal value
        /// </summary>
        public string OriginalValue { get; set; }

        /// <summary>
        /// remove quotes around, remove include single and double quotes
        /// </summary>
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

        /// <summary>
        /// return the Value attr
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// empty element constructor
        /// </summary>
        public JElement()
        {
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="json">value</param>
        public JElement(string json)
        {
            OriginalValue = json;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
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

        /// <summary>
        /// child elements
        /// </summary>
        public List<JElement> Elements
        {
            get
            {
                return SubElements;
            }
        }

        /// <summary>
        /// determine the Name and OriginalValue is null or empry
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(OriginalValue);
            }
        }

        /// <summary>
        /// determine whether this element is json array
        /// </summary>
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

        /// <summary>
        /// split json to key value pairs
        /// </summary>
        /// <param name="json">json string</param>
        /// <returns>element list</returns>
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

        /// <summary>
        /// find the first element by name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>element</returns>
        public JElement Element(string name)
        {
            return SubElements.Find(e => e.Name == name) ?? new JElement();
        }

        /// <summary>
        /// index the element
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>element</returns>
        public JElement Element(int index)
        {
            var list = SubElements;
            if (index >= 0 && index < list.Count)
            {
                return list.ElementAt(index);
            }
            return new JElement();
        }

        /// <summary>
        /// find the first element by name
        /// </summary>
        /// <param name="name">name</param>
        /// <returns>element</returns>
        public JElement this[string name]
        {
            get
            {
                return Element(name);
            }
        }

        /// <summary>
        /// index the element
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>element</returns>
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

        /// <summary>
        /// remove string quotes around 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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

        /// <summary>
        /// new JElement
        /// </summary>
        /// <param name="json">json string</param>
        /// <returns>element</returns>
        public static JElement Parse(string json)
        {
            return new JElement(json);
        }
    }
}
