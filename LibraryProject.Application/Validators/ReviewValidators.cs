using FluentValidation;
using LibraryProject.Application.DTOs;

namespace LibraryProject.Application.Validators
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewValidator()
        {
            RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("Kitap belirtilmelidir.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Puan 1 ile 5 arasında olmalıdır.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Yorum boş olamaz.")
                .MaximumLength(1000).WithMessage("Yorum en fazla 1000 karakter olabilir.");
        }
    }

    public class UpdateReviewValidator : AbstractValidator<UpdateReviewDto>
    {
        public UpdateReviewValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Puan 1 ile 5 arasında olmalıdır.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Yorum boş olamaz.")
                .MaximumLength(1000).WithMessage("Yorum en fazla 1000 karakter olabilir.");
        }
    }
}