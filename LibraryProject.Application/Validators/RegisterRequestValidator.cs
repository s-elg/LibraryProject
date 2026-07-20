using FluentValidation;
using LibraryProject.Application.DTOs;

namespace LibraryProject.Application.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Ad Soyad boş olamaz.")
                .MinimumLength(3).WithMessage("Ad Soyad en az 3 karakter olmalı.")
                .MaximumLength(100).WithMessage("Ad Soyad en fazla 100 karakter olabilir.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir email adresi girin.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalı.")
                .Matches("[A-Z]").WithMessage("Şifre en az bir büyük harf içermeli.")
                .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermeli.");
        }
    }

    public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email boş olamaz.")
                .EmailAddress().WithMessage("Geçerli bir email adresi girin.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.");
        }
    }

    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequestDto>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.AccessToken)
                .NotEmpty().WithMessage("Access token boş olamaz.");

            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token boş olamaz.");
        }
    }
}