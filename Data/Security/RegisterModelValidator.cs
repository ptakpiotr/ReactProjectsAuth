using FluentValidation;
using ReactProjectsAuthApi.Models;

namespace ReactProjectsAuthApi.Data.Security
{
    public class RegisterModelValidator : AbstractValidator<RegisterModel>
    {
        private string [] emails = new [] {"gmail.com","outlook.com","ethereal.email"};

        public RegisterModelValidator()
        {
            RuleFor(r => r.Email).Must(BeWithProperProvider);
        }

        private bool BeWithProperProvider(string email)
        {
            bool f = false;

            foreach(string ending in emails)
            {
                f = email.EndsWith(ending);
                if (f) break;
            }

            return (f);
        }
    }
}
