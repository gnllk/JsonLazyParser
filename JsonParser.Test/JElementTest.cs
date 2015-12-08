using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace JsonParser.Test
{
    [TestClass]
    public class JElementTest
    {
        [TestMethod]
        public void TestJElement()
        {
            const string json = "{'UserName':'Jackson', UserAge:22, Tel:'13800138000', Email:'Jackson@mail.com', Cars:['Audi', 'BMW', 'Ferrari']}";
            JElement ele = JElement.Parse(json);
            ele.Should().NotBeNull();
            "Jackson".Should().Equals(ele["UserName"].Value);
            "22".Should().Equals(ele["UserAge"].Value);
            "13800138000".Should().Equals(ele["Tel"].Value);
            "Jackson@mail.com".Should().Equals(ele["Email"].Value);
            "Audi".Should().Equals(ele["Cars"][0].Value);
            "Ferrari".Should().Equals(ele["Cars"][2].Value);
            ele["Cars"].Value.Length.Should().Equals(3);
            ele["Cars"][100].Value.Should().BeNull();
            ele["novalue"].Value.Should().BeNull();
            ele["novalue"]["novalue"]["novalue"].Value.Should().BeNull();
        }
    }
}
