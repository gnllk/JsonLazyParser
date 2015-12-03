using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonParser
{
    class Program
    {
        static void Main(string[] args)
        {
            const string json = "{'UserName':'Jackson', UserAge:22, Tel:'13800138000', Email:'Jackson@mail.com', Cars:['Audi', 'BMW', 'Ferrari']}";
            JElement ele = JElement.Parse(json);
            Say("Name:\t{0}", ele["UserName"]);
            Say("Age:\t{0}", ele["UserAge"]);
            foreach (var item in ele["Cars"].Elements)
            {
                Say("Car:\t{0}", item);
            }
            Say("The last car:\t{0}", ele["Cars"][2]);
            End();
        }
        static void Say(object msg)
        {
            Console.WriteLine(msg);
        }
        static void Say(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }
        static void End()
        {
            Say("Press any key to end.");
            Console.ReadKey();
        }
    }
}
