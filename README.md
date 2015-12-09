# JsonLazyParser
A lazy Json parser using C# language

example:

const string json = "{'UserName':'Jackson', UserAge:22, Tel:'13800138000', Email:'gnllk@mail.com', Cars:['Audi', 'BMW', 'Ferrari']}";

JElement ele = JElement.Parse(json);
Console.WriteLine("Name:\t{0}", ele["UserName"]);
Console.WriteLine("Age:\t{0}", ele["UserAge"]);

foreach (var item in ele["Cars"].Elements)
{
    Console.WriteLine("Car:\t{0}", item);
}

Console.WriteLine("The last car:\t{0}", ele["Cars"][2]);

