using System.Threading.Tasks;

namespace PDF_EMAIL.Services
{
    public interface IEnviarEmail
    {
        Task<int> SendEmailAsync(string filename);
    }
}