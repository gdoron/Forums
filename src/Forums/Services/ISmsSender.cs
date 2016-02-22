using System.Threading.Tasks;

namespace Forums.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
