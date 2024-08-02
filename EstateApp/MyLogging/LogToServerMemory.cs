namespace EstateApp.MyLogging
{
    public class LogToServerMemory : IMyLogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Logto Server Memory");
            //write your own logic to save the logs to server memory
        }
    }
}
