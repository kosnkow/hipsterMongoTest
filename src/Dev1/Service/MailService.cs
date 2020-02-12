using System.Threading.Tasks;
using JHipsterNet.Config;
using MyCompany.Models;
using Microsoft.Extensions.Options;

namespace MyCompany.Service {
    public interface IMailService {
        Task SendPasswordResetMail(User user);
        Task SendActivationEmail(User user);
        Task SendCreationEmail(User user);
    }

    public class MailService : IMailService {
        private readonly JHipsterSettings _jhipsterSettings;

        public MailService(IOptions<JHipsterSettings> jhipsterSettings)
        {
            _jhipsterSettings = jhipsterSettings.Value;
        }

        public Task SendPasswordResetMail(User user)
        {
            //TODO send reset Email
            return Task.FromResult(Task.CompletedTask);
        }

        public Task SendActivationEmail(User user)
        {
            //TODO Activation Email
            return Task.FromResult(Task.CompletedTask);
        }

        public Task SendCreationEmail(User user)
        {
            //TODO Creation Email
            return Task.FromResult(Task.CompletedTask);
        }
    }
}
