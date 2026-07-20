using FluentValidation;
using LibraryProject.Application.DTOs;

namespace LibraryProject.Application.Validators
{
    public class BorrowBookRequestValidator : AbstractValidator<BorrowBookRequestDto>
    {
        public BorrowBookRequestValidator()
        {
            RuleFor(x => x.BookId)
                .NotEmpty().WithMessage("Ödünç alınacak kitap belirtilmelidir.");
        }
    }
}