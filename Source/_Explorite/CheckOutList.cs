/********************
 * 嵌套式检测清单。
 * 
 * --siiftun1857
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Explorite
{
    ///<summary>为<see cref = "CheckOutList&#60;T&#62;" />设置匹配模式。</summary>
    public enum CheckOutListMode : byte
    {
        ///<summary>全部符合。</summary>
        All = 0,
        ///<summary>任意符合。</summary>
        Any = 1,
        ///<summary>任意不符合。</summary>
        Neither = 2,
        ///<summary>全部不符合。</summary>
        None = 3,
    }
    ///<summary>检测清单。</summary>
    public class CheckOutList<T>
    {
        ///<summary>检测项目列表。</summary>
        public List<T> objects = new List<T>();
        ///<summary>检测项目子列表。</summary>
        public List<CheckOutList<T>> sublists = new List<CheckOutList<T>>();
        ///<summary>检测模式。</summary>
        public CheckOutListMode mode = CheckOutListMode.All;

        /**
         * <summary>
         * 检测列表并获取匹配性结果。
         * </summary>
         * <param name="checkList">需要被检查的列表。</param>
         * <returns>匹配结果。</returns>
         */
        public bool CheckOut(IEnumerable<T> checkList)
        {
            if (checkList == null)
                checkList = Enumerable.Empty<T>();
            foreach (T obj in objects)
            {
                bool? testResult = Test(checkList.Contains(obj));
                if (testResult.HasValue)
                    return testResult.Value;
            }
            foreach (CheckOutList<T> sublist in sublists)
            {
                bool? testResult = Test(sublist.CheckOut(checkList));
                if (testResult.HasValue)
                    return testResult.Value;
            }
            return true;
        }

        private bool? Test(bool input)
        {
            switch (mode)
            {
                case CheckOutListMode.All:
                    if (!input)
                        return false;
                    break;
                case CheckOutListMode.Neither:
                    if (!input)
                        return true;
                    break;
                case CheckOutListMode.Any:
                    if (input)
                        return true;
                    break;
                case CheckOutListMode.None:
                    if (input)
                        return false;
                    break;
                default:
                    throw new InvalidOperationException("CheckOutListMode mode is invalid.");

            }
            return null;
        }

        ///<summary>为<see cref = "CheckOutList&#60;T&#62;" />提供清单参数。</summary>
        public struct CheckOutListStringRule
        {
            ///<summary>全部符合模式时的前缀。</summary>
            public string modeAll;
            ///<summary>任意符合模式时的前缀。</summary>
            public string modeAny;
            ///<summary>任意不符合模式时的前缀。</summary>
            public string modeNeither;
            ///<summary>全部不符合模式时的前缀。</summary>
            public string modeNone;
            ///<summary>一组的前缀。</summary>
            public string clusterStart;
            ///<summary>一组的后缀。</summary>
            public string clusterEnd;
            ///<summary>连接符起始字符。</summary>
            public string join;
            ///<summary>缩进字符。</summary>
            public string indentation;
            ///<summary>将数据转换为字符串的选择器。</summary>
            public Func<T, string> selector;
            public CheckOutListStringRule(CheckOutListStringRule presetStringRule)
            {
                modeAll = presetStringRule.modeAll;
                modeAny = presetStringRule.modeAny;
                modeNeither = presetStringRule.modeNeither;
                modeNone = presetStringRule.modeNone;
                clusterStart = presetStringRule.clusterStart;
                clusterEnd = presetStringRule.clusterEnd;
                join = presetStringRule.join;
                indentation = presetStringRule.indentation;
                selector = presetStringRule.selector;
            }
        }

        public static CheckOutListStringRule presetStringRule = new CheckOutListStringRule()
        {
            modeAll = "全部",
            modeAny = "任意",
            modeNeither = "没有",
            modeNone = "全无",
            clusterStart = "(",
            clusterEnd = ")",
            join = ", ",
            indentation = "",
            selector = obj => obj?.ToString(),
        };
        public static CheckOutListStringRule presetStringRule2 = new CheckOutListStringRule(presetStringRule)
        {
            clusterStart = ":\n",
            clusterEnd = "",
            join = "\n",
            indentation = "  ",
        };


        /**
         * <summary>
         * 以文本形式打印清单数据。
         * </summary>
         * <param name="rule">打印参数。</param>
         * <returns>以文本形式打印的清单数据。</returns>
         */
        public string ReportContact(CheckOutListStringRule? rule = null)
        {
            CheckOutListStringRule rulev = rule ?? presetStringRule;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(rulev.clusterStart);
            List<string> strs = objects.Select(rulev.selector).ToList();
            strs.AddRange(sublists.Select(sublist => sublist.ReportContact(rulev)));
            stringBuilder.Append(string.Join(rulev.join, strs));
            stringBuilder.Append(rulev.clusterEnd);
            return mode switch
            {
                CheckOutListMode.All => rulev.modeAll,
                CheckOutListMode.Any => rulev.modeAny,
                CheckOutListMode.Neither => rulev.modeNeither,
                CheckOutListMode.None => rulev.modeNone,
                _ => throw new InvalidOperationException("CheckOutListMode mode is invalid.")
            } + string.Join("\n", stringBuilder.ToString().Split('\n').Select((str, i) => ( i == 0 ? "" : rulev.indentation) + str));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051", Justification = "<挂起>")]
        private void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            bool modeAttrEncountered = false;
            foreach (XmlAttribute attribute in xmlRoot.Attributes)
            {
                if (attribute.Name == "Mode")
                {
                    switch (attribute.Value.ToLower())
                    {
                        case "all":
                        case "and":
                            mode = CheckOutListMode.All;
                            break;
                        case "any":
                        case "or":
                            mode = CheckOutListMode.Any;
                            break;
                        case "neither":
                        case "nor":
                            mode = CheckOutListMode.Neither;
                            break;
                        case "none":
                        case "nand":
                            mode = CheckOutListMode.None;
                            break;
                        default:
                            Verse.Log.Error($"XML format error: Misconfigured mode {attribute.Value}.");
                            break;
                    };
                    if (modeAttrEncountered)
                    {
                        Verse.Log.Error($"XML format error: Encountered attribute \"{attribute.Name}\" multiple times.");
                    }
                    else
                    {
                        modeAttrEncountered = true;
                    }
                }
            }
            foreach (XmlNode node in xmlRoot.ChildNodes)
            {
                if (node is XmlComment)
                {
                }
                else if (node is XmlText)
                {
                    Verse.Log.Error("XML format error: Raw text found inside a list element. " + node.OuterXml);
                }
                else
                {
                    switch (node.Name)
                    {
                        case "li":
                            XmlAttribute xmlAttribute = node.Attributes["MayRequire"];
                            Verse.DirectXmlCrossRefLoader.RegisterListWantsCrossRef<T>(objects, node.InnerText, xmlRoot.Name, (xmlAttribute != null) ? xmlAttribute.Value : null);
                            break;

                        case "list":
                            sublists.Add(Verse.DirectXmlToObject.ObjectFromXml<CheckOutList<T>>(node, true));
                            break;

                        default:
                            Verse.Log.Error($"XML format error: Misconfigured node named \"{node.Name}\"." + node.OuterXml);
                            break;
                    }
                }
            }
        }
    }
}
