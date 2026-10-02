using System.Text;
using TravelPlanner;

Console.OutputEncoding = Encoding.UTF8;

try
{
    await TravelDemo.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.WriteLine("Something went wrong.");
    Console.WriteLine(ex.Message);
    Console.WriteLine();
    Console.WriteLine("Please check:");
    Console.WriteLine("- Ollama is running");
    Console.WriteLine("- The model is downloaded");
    Console.WriteLine("- appsettings.json values are correct");
}