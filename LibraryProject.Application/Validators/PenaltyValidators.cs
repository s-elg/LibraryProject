using FluentValidation;
using LibraryProject.Application.DTOs;

namespace LibraryProject.Application.Validators
{
    public class CreatePenaltyValidator : AbstractValidator<CreatePenaltyDto>
    {
        public CreatePenaltyValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Kullanıcı belirtilmelidir.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Ceza sebebi boş olamaz.")
                .MinimumLength(5).WithMessage("Ceza sebebi en az 5 karakter olmalı.")
                .MaximumLength(500).WithMessage("Ceza sebebi en fazla 500 karakter olabilir.");
        }
    }
}